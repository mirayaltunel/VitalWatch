using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Enums;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class SensorMeasurementService : ISensorMeasurementService
    {
        private readonly VitalWatchDbContext _dbContext;

        public SensorMeasurementService(VitalWatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<LiveVitalsDto>> GetLatestVitals(int patientId)
        {
            var heartRate = await _dbContext.SensorMeasurements
                .Where(sm => sm.PatientId == patientId && sm.MeasurementType == MeasurementType.HeartRate)
                .OrderByDescending(sm => sm.Timestamp)
                .FirstOrDefaultAsync();

            var spo2 = await _dbContext.SensorMeasurements
                .Where(sm => sm.PatientId == patientId && sm.MeasurementType == MeasurementType.SpO2)
                .OrderByDescending(sm => sm.Timestamp)
                .FirstOrDefaultAsync();

            var respiration = await _dbContext.SensorMeasurements
                .Where(sm => sm.PatientId == patientId && sm.MeasurementType == MeasurementType.Respiration)
                .OrderByDescending(sm => sm.Timestamp)
                .FirstOrDefaultAsync();

            var dto = new LiveVitalsDto
            {
                Pulse = heartRate?.Value ?? 0,
                SpO2 = spo2?.Value ?? 0,
                Respiration = respiration?.Value ?? 0,
                Timestamp = heartRate?.Timestamp ?? DateTime.UtcNow
            };

            return ResponseManager.CreateSuccess(dto);
        }

        public async Task<ResponseModel<List<Entities.SensorMeasurement>>> GetVitalHistory(int patientId, MeasurementType type)
        {
            var history = await _dbContext.SensorMeasurements
                .Where(sm => sm.PatientId == patientId && sm.MeasurementType == type)
                .OrderByDescending(sm => sm.Timestamp)
                .Take(20) // Son 20 ölçüm örneği
                .ToListAsync();

            return ResponseManager.CreateSuccess(history);
        }
    }
}
