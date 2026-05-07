using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Hubs;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class AlertService : IAlertService
    {
        private readonly VitalWatchDbContext _db;
        private readonly IHubContext<VitalHub> _hub;

        public AlertService(VitalWatchDbContext db, IHubContext<VitalHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        // Nöbet tespiti için basit bir hareket eşiği (m/s^2 cinsinden ivme magnitude)
        private const double SeizureMagnitudeThreshold = 25.0;

        public async Task EvaluateMeasurement(int patientId, int deviceId, int measurementTypeId, double value,
                                              double? valueX, double? valueY, double? valueZ, DateTime timestamp)
        {
            // 1) Threshold kontrolü -> Alert + HealthEvent
            var threshold = await _db.Thresholds
                .FirstOrDefaultAsync(t => t.PatientId == patientId && t.MeasurementTypeId == measurementTypeId);

            if (threshold != null && (value < threshold.MinValue || value > threshold.MaxValue))
            {
                var severityId = ResolveSeverity(measurementTypeId, value, threshold);
                var alert = new Alert
                {
                    PatientId = patientId,
                    MeasurementTypeId = measurementTypeId,
                    ThresholdId = threshold.Id,
                    SeverityId = severityId,
                    Value = value,
                    ThresholdMinSnapshot = threshold.MinValue,
                    ThresholdMaxSnapshot = threshold.MaxValue,
                    IsReviewed = false
                };
                _db.Alerts.Add(alert);

                var eventTypeId = ResolveEventTypeFromMeasurement(measurementTypeId, value, threshold);
                if (eventTypeId.HasValue)
                {
                    _db.HealthEvents.Add(new HealthEvent
                    {
                        PatientId = patientId,
                        EventTypeId = eventTypeId.Value,
                        SeverityId = severityId,
                        AlertSourceId = SeedConstants.AlertSources.Sensor,
                        Value = value,
                        StartTimestamp = timestamp,
                    });
                }

                await _db.SaveChangesAsync();
                await BroadcastAlert(patientId, alert, measurementTypeId);
            }

            // 2) Nöbet (seizure) — accelerometer üzerinden
            if (valueX.HasValue && valueY.HasValue && valueZ.HasValue)
            {
                var magnitude = Math.Sqrt(valueX.Value * valueX.Value +
                                          valueY.Value * valueY.Value +
                                          valueZ.Value * valueZ.Value);
                if (magnitude >= SeizureMagnitudeThreshold)
                {
                    var seizure = new HealthEvent
                    {
                        PatientId = patientId,
                        EventTypeId = SeedConstants.EventTypes.Seizure,
                        SeverityId = SeedConstants.Severities.Critical,
                        AlertSourceId = SeedConstants.AlertSources.Sensor,
                        Value = magnitude,
                        StartTimestamp = timestamp,
                    };
                    _db.HealthEvents.Add(seizure);
                    await _db.SaveChangesAsync();

                    await _hub.Clients.Group($"patient_{patientId}")
                        .SendAsync("HealthEvent", new
                        {
                            patientId,
                            eventType = "Seizure",
                            severity = "Critical",
                            value = magnitude,
                            timestamp
                        });
                }
            }
        }

        private static int ResolveSeverity(int measurementTypeId, double value, Threshold t)
        {
            // Aralığın ne kadar dışında olduğuna göre severity
            var range = Math.Max(t.MaxValue - t.MinValue, 1);
            var deviation = value < t.MinValue ? (t.MinValue - value) : (value - t.MaxValue);
            var ratio = deviation / range;

            if (ratio >= 0.40) return SeedConstants.Severities.Critical;
            if (ratio >= 0.20) return SeedConstants.Severities.High;
            if (ratio >= 0.10) return SeedConstants.Severities.Medium;
            return SeedConstants.Severities.Low;
        }

        private static int? ResolveEventTypeFromMeasurement(int measurementTypeId, double value, Threshold t)
        {
            return measurementTypeId switch
            {
                SeedConstants.MeasurementTypes.SpO2 when value < t.MinValue => SeedConstants.EventTypes.LowSpO2,
                SeedConstants.MeasurementTypes.HeartRate when value > t.MaxValue => SeedConstants.EventTypes.HighHeartRate,
                SeedConstants.MeasurementTypes.HeartRate when value < t.MinValue => SeedConstants.EventTypes.LowHeartRate,
                SeedConstants.MeasurementTypes.Respiration when value < t.MinValue => SeedConstants.EventTypes.Apnea,
                _ => null
            };
        }

        private async Task BroadcastAlert(int patientId, Alert alert, int measurementTypeId)
        {
            var mtName = await _db.MeasurementTypes.Where(m => m.Id == measurementTypeId)
                                                   .Select(m => m.Name).FirstOrDefaultAsync() ?? "Unknown";
            var sevName = await _db.Severities.Where(s => s.Id == alert.SeverityId)
                                              .Select(s => s.Name).FirstOrDefaultAsync() ?? "Unknown";

            await _hub.Clients.Group($"patient_{patientId}")
                .SendAsync("Alert", new
                {
                    id = alert.Id,
                    patientId,
                    measurementType = mtName,
                    severity = sevName,
                    value = alert.Value,
                    thresholdMin = alert.ThresholdMinSnapshot,
                    thresholdMax = alert.ThresholdMaxSnapshot,
                    createdAt = alert.CreatedDate
                });
        }

        public async Task<ResponseModel<List<AlertDto>>> GetLatestAlerts(int patientId, int take = 20)
        {
            var alerts = await _db.Alerts
                .Include(a => a.MeasurementType)
                .Include(a => a.Severity)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.CreatedDate)
                .Take(take)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    MeasurementType = a.MeasurementType.Name,
                    Severity = a.Severity.Name,
                    Value = a.Value,
                    ThresholdMin = a.ThresholdMinSnapshot,
                    ThresholdMax = a.ThresholdMaxSnapshot,
                    IsReviewed = a.IsReviewed,
                    CreatedAt = a.CreatedDate,
                    Message = $"{a.MeasurementType.Name} = {a.Value} {a.MeasurementType.Unit} (eşik {a.ThresholdMinSnapshot}-{a.ThresholdMaxSnapshot})"
                })
                .ToListAsync();

            return ResponseManager.CreateSuccess(alerts);
        }

        public async Task<ResponseModel> MarkReviewed(int alertId)
        {
            var alert = await _db.Alerts.FindAsync(alertId);
            if (alert == null) return ResponseManager.CreateError("Alert bulunamadı");
            alert.IsReviewed = true;
            await _db.SaveChangesAsync();
            return ResponseManager.CreateSuccess();
        }
    }
}
