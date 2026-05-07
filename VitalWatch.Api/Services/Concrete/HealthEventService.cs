using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class HealthEventService : IHealthEventService
    {
        private readonly VitalWatchDbContext _db;

        public HealthEventService(VitalWatchDbContext db) { _db = db; }

        public async Task<ResponseModel<List<AlertDto>>> GetLatestAlerts(int patientId)
        {
            var alerts = await _db.Alerts
                .Include(a => a.MeasurementType)
                .Include(a => a.Severity)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.CreatedDate)
                .Take(5)
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
                    Message = a.MeasurementType.Name + " ihlali (" + a.Value + " " + a.MeasurementType.Unit + ")"
                }).ToListAsync();

            return ResponseManager.CreateSuccess(alerts);
        }

        public async Task<ResponseModel<ReportSummaryDto>> GetPatientReports(int patientId, DateTime startDate, DateTime endDate)
        {
            var startUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

            var events = await _db.HealthEvents
                .Include(e => e.EventType)
                .Include(e => e.Severity)
                .Where(e => e.PatientId == patientId && e.StartTimestamp >= startUtc && e.StartTimestamp <= endUtc)
                .OrderByDescending(e => e.StartTimestamp)
                .ToListAsync();

            var critical = events.Count(e => e.SeverityId == SeedConstants.Severities.Critical);
            var warning = events.Count(e => e.SeverityId == SeedConstants.Severities.High || e.SeverityId == SeedConstants.Severities.Medium);

            var dto = new ReportSummaryDto
            {
                CriticalCount = critical,
                WarningCount = warning,
                Events = events.Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.EventType.Name,
                    Description = $"{e.EventType.Name} ({e.Severity.Name}) - değer: {e.Value}",
                    Severity = e.Severity.Name,
                    Time = e.StartTimestamp
                }).ToList()
            };

            return ResponseManager.CreateSuccess(dto);
        }
    }
}
