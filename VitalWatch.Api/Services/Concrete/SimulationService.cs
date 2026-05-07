using Microsoft.AspNetCore.SignalR;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Enums;
using VitalWatch.Api.Hubs;

namespace VitalWatch.Api.Services.Concrete
{
    public class SimulationService
    {
        private readonly IHubContext<VitalHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dictionary<int, CancellationTokenSource> _runningSimulations = new();

        public SimulationService(IHubContext<VitalHub> hubContext, IServiceScopeFactory scopeFactory)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
        }

        public bool IsRunning(int patientId) => _runningSimulations.ContainsKey(patientId);

        public void Start(int patientId)
        {
            if (_runningSimulations.ContainsKey(patientId)) return;

            var cts = new CancellationTokenSource();
            _runningSimulations[patientId] = cts;

            Task.Run(() => RunSimulation(patientId, cts.Token));
        }

        public void Stop(int patientId)
        {
            if (_runningSimulations.TryGetValue(patientId, out var cts))
            {
                cts.Cancel();
                _runningSimulations.Remove(patientId);
            }
        }

        private async Task RunSimulation(int patientId, CancellationToken ct)
        {
            var rng = new Random();
            double pulse = 75, spo2 = 97, respiration = 16;

            while (!ct.IsCancellationRequested)
            {
                // Gerçekçi dalgalanma
                pulse       = Math.Clamp(pulse       + rng.NextDouble() * 4 - 2,  50, 140);
                spo2        = Math.Clamp(spo2        + rng.NextDouble() * 1 - 0.5, 85, 100);
                respiration = Math.Clamp(respiration + rng.NextDouble() * 2 - 1,   10,  30);

                var now = DateTime.UtcNow;

                // DB'ye yaz
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VitalWatchDbContext>();
                db.SensorMeasurements.AddRange(
                    new SensorMeasurement { PatientId = patientId, MeasurementType = MeasurementType.HeartRate,  Value = Math.Round(pulse, 1),       Unit = "bpm", Timestamp = now, DeviceType = DeviceType.SmartWatch },
                    new SensorMeasurement { PatientId = patientId, MeasurementType = MeasurementType.SpO2,        Value = Math.Round(spo2, 1),        Unit = "%",   Timestamp = now, DeviceType = DeviceType.SmartWatch },
                    new SensorMeasurement { PatientId = patientId, MeasurementType = MeasurementType.Respiration, Value = Math.Round(respiration, 1), Unit = "rpm", Timestamp = now, DeviceType = DeviceType.SmartWatch }
                );
                await db.SaveChangesAsync(ct);

                // SignalR ile clientlara gönder
                var payload = new
                {
                    patientId,
                    pulse        = Math.Round(pulse, 1),
                    spO2         = Math.Round(spo2, 1),
                    respiration  = Math.Round(respiration, 1),
                    timestamp    = now
                };

                await _hubContext.Clients
                    .Group($"patient_{patientId}")
                    .SendAsync("VitalUpdate", payload, ct);

                await Task.Delay(2000, ct); // Her 2 saniyede bir
            }
        }
    }
}
