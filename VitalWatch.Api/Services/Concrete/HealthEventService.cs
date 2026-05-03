using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using VitalWatch.Api.Enums;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class HealthEventService : IHealthEventService
    {
        private readonly VitalWatchDbContext _dbContext;

        public HealthEventService(VitalWatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<List<AlertDto>>> GetLatestAlerts(int patientId)
        {
            var alerts = await _dbContext.HealthEvents
                .Where(he => he.PatientId == patientId)
                .OrderByDescending(he => he.Timestamp)
                .Take(5)
                .Select(he => new AlertDto
                {
                    Type = he.EventType.ToString(),
                    Message = he.EventType.ToString() + " tespit edildi. (" + he.Value + " " + he.Unit + ")",
                    Severity = he.Severity.ToString(),
                    Time = he.Timestamp
                }).ToListAsync();
                
            return ResponseManager.CreateSuccess(alerts);
        }

        public async Task<ResponseModel<ReportSummaryDto>> GetPatientReports(int patientId, DateTime startDate, DateTime endDate)
        {
            var events = await _dbContext.HealthEvents
                .Where(he => he.PatientId == patientId && he.Timestamp >= startDate && he.Timestamp <= endDate)
                .OrderByDescending(he => he.Timestamp)
                .ToListAsync();

            var criticalCount = events.Count(e => e.Severity == Severity.Critical);
            var warningCount = events.Count(e => e.Severity == Severity.High || e.Severity == Severity.Medium);

            var eventDtos = events.Select(e => new EventDto
            {
                Title = e.EventType.ToString().ToUpper(),
                Description = $"{e.Value} {e.Unit} - {e.Severity} seviyesinde durum algılandı.",
                Time = e.Timestamp
            }).ToList();

            var dto = new ReportSummaryDto
            {
                CriticalCount = criticalCount,
                WarningCount = warningCount,
                Events = eventDtos
            };

            return ResponseManager.CreateSuccess(dto);
        }
    }
}
