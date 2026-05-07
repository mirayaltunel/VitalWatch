using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Hubs;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class SimulationService
    {
        private readonly IHubContext<VitalHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dictionary<int, CancellationTokenSource> _running = new();

        public SimulationService(IHubContext<VitalHub> hubContext, IServiceScopeFactory scopeFactory)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        public bool IsRunning(int patientId) => _running.ContainsKey(patientId);

        public void Start(int patientId)
        {
            if (_running.ContainsKey(patientId)) return;
            var cts = new CancellationTokenSource();
            _running[patientId] = cts;
            Task.Run(async () =>
            {
                await ClearPatientLogs(patientId);
                await RunSimulation(patientId, cts.Token);
            });
        }

        private async Task ClearPatientLogs(int patientId)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VitalWatchDbContext>();

            db.SensorMeasurements.RemoveRange(
                db.SensorMeasurements.Where(m => m.PatientId == patientId));
            db.Alerts.RemoveRange(
                db.Alerts.Where(a => a.PatientId == patientId));
            db.HealthEvents.RemoveRange(
                db.HealthEvents.Where(e => e.PatientId == patientId));

            await db.SaveChangesAsync();
        }

        public void Stop(int patientId)
        {
            if (_running.TryGetValue(patientId, out var cts))
            {
                cts.Cancel();
                _running.Remove(patientId);
            }
        }

        private async Task RunSimulation(int patientId, CancellationToken ct)
        {
            var rng = new Random();
            double pulse = 75, spo2 = 97, respiration = 16;
            int tick = 0;

            // Hasta için bir cihaz garantile
            int deviceId;
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<VitalWatchDbContext>();
                var device = await db.Devices.FirstOrDefaultAsync(d => d.PatientId == patientId, ct);
                if (device == null)
                {
                    device = new Device
                    {
                        PatientId = patientId,
                        DeviceName = "Simülasyon Bilekliği",
                        DeviceTypeId = SeedConstants.DeviceTypes.SmartWatch,
                        DeviceStatusId = SeedConstants.DeviceStatuses.Active,
                        BatteryLevel = 95,
                        LastSeenAt = DateTime.UtcNow
                    };
                    db.Devices.Add(device);
                    await db.SaveChangesAsync(ct);
                }
                deviceId = device.Id;
            }

            while (!ct.IsCancellationRequested)
            {
                tick++;

                // Demo modu: her ~6 cycle (≈12 saniye) sonra ya da %15 olasılıkla nöbet
                bool seizureBurst = (tick % 6 == 0) || rng.NextDouble() < 0.15;

                if (seizureBurst)
                {
                    // Nöbet sırasında: nabız fırlar, SpO2 düşer, solunum bozulur
                    pulse       = 145 + rng.NextDouble() * 20;   // 145-165
                    spo2        = 80 + rng.NextDouble() * 5;     // 80-85
                    respiration = 28 + rng.NextDouble() * 6;     // 28-34
                }
                else
                {
                    pulse       = Math.Clamp(pulse       + rng.NextDouble() * 4 - 2,    50, 140);
                    spo2        = Math.Clamp(spo2        + rng.NextDouble() * 1 - 0.5,  85, 100);
                    respiration = Math.Clamp(respiration + rng.NextDouble() * 2 - 1,    10,  30);
                }

                var now = DateTime.UtcNow;

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VitalWatchDbContext>();
                var alertSvc = scope.ServiceProvider.GetRequiredService<IAlertService>();

                // Nöbet sırasında accelerometer da kuvvetli sallanma versin (seizure detection için)
                var accel = seizureBurst
                    ? new { x = 18 + rng.NextDouble() * 6, y = 17 + rng.NextDouble() * 6, z = 16 + rng.NextDouble() * 6 }
                    : new { x = rng.NextDouble() * 1.5, y = rng.NextDouble() * 1.5, z = 9.8 + rng.NextDouble() * 0.5 };

                var measurements = new[]
                {
                    new SensorMeasurement { PatientId = patientId, DeviceId = deviceId,
                        MeasurementTypeId = SeedConstants.MeasurementTypes.HeartRate,
                        Value = Math.Round(pulse, 1), Timestamp = now },
                    new SensorMeasurement { PatientId = patientId, DeviceId = deviceId,
                        MeasurementTypeId = SeedConstants.MeasurementTypes.SpO2,
                        Value = Math.Round(spo2, 1), Timestamp = now },
                    new SensorMeasurement { PatientId = patientId, DeviceId = deviceId,
                        MeasurementTypeId = SeedConstants.MeasurementTypes.Respiration,
                        Value = Math.Round(respiration, 1), Timestamp = now },
                    new SensorMeasurement { PatientId = patientId, DeviceId = deviceId,
                        MeasurementTypeId = SeedConstants.MeasurementTypes.AccelerometerX,
                        Value = Math.Round(accel.x, 2),
                        ValueX = accel.x, ValueY = accel.y, ValueZ = accel.z,
                        Timestamp = now },
                };
                db.SensorMeasurements.AddRange(measurements);
                await db.SaveChangesAsync(ct);

                // SignalR canlı yayın
                await _hubContext.Clients.Group($"patient_{patientId}").SendAsync("VitalUpdate", new
                {
                    patientId,
                    pulse = Math.Round(pulse, 1),
                    spO2 = Math.Round(spo2, 1),
                    respiration = Math.Round(respiration, 1),
                    timestamp = now
                }, ct);

                // Threshold + nöbet kontrolü
                foreach (var m in measurements)
                {
                    await alertSvc.EvaluateMeasurement(m.PatientId, m.DeviceId, m.MeasurementTypeId,
                        m.Value, m.ValueX, m.ValueY, m.ValueZ, m.Timestamp);
                }

                await Task.Delay(2000, ct);
            }
        }
    }
}
