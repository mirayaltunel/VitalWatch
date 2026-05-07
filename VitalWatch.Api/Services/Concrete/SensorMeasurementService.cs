using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class SensorMeasurementService : ISensorMeasurementService
    {
        private readonly VitalWatchDbContext _db;

        public SensorMeasurementService(VitalWatchDbContext db) { _db = db; }

        public async Task<ResponseModel<LiveVitalsDto>> GetLatestVitals(int patientId)
        {
            var hr = await Latest(patientId, SeedConstants.MeasurementTypes.HeartRate);
            var sp = await Latest(patientId, SeedConstants.MeasurementTypes.SpO2);
            var rs = await Latest(patientId, SeedConstants.MeasurementTypes.Respiration);

            var dto = new LiveVitalsDto
            {
                Pulse = hr?.Value ?? 0,
                SpO2 = sp?.Value ?? 0,
                Respiration = rs?.Value ?? 0,
                Timestamp = hr?.Timestamp ?? DateTime.UtcNow
            };
            return ResponseManager.CreateSuccess(dto);
        }

        public async Task<ResponseModel<List<SensorMeasurement>>> GetVitalHistory(int patientId, int measurementTypeId, int take = 50)
        {
            var history = await _db.SensorMeasurements
                .Where(sm => sm.PatientId == patientId && sm.MeasurementTypeId == measurementTypeId)
                .OrderByDescending(sm => sm.Timestamp)
                .Take(take)
                .ToListAsync();
            return ResponseManager.CreateSuccess(history);
        }

        private Task<SensorMeasurement?> Latest(int patientId, int measurementTypeId) =>
            _db.SensorMeasurements
                .Where(sm => sm.PatientId == patientId && sm.MeasurementTypeId == measurementTypeId)
                .OrderByDescending(sm => sm.Timestamp)
                .FirstOrDefaultAsync();
    }
}
