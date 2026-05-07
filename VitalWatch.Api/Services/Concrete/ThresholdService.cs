using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class ThresholdService : IThresholdService
    {
        private readonly VitalWatchDbContext _db;

        public ThresholdService(VitalWatchDbContext db) { _db = db; }

        public async Task<ResponseModel<List<ThresholdDto>>> GetByPatient(int patientId)
        {
            var data = await _db.Thresholds
                .Include(t => t.MeasurementType)
                .Where(t => t.PatientId == patientId)
                .Select(t => new ThresholdDto
                {
                    Id = t.Id,
                    PatientId = t.PatientId,
                    MeasurementTypeId = t.MeasurementTypeId,
                    MeasurementType = t.MeasurementType.Name,
                    Unit = t.MeasurementType.Unit,
                    MinValue = t.MinValue,
                    MaxValue = t.MaxValue
                }).ToListAsync();

            return ResponseManager.CreateSuccess(data);
        }

        public async Task<ResponseModel> Upsert(SetThresholdRequestModel m)
        {
            var existing = await _db.Thresholds.FirstOrDefaultAsync(t =>
                t.PatientId == m.PatientId && t.MeasurementTypeId == m.MeasurementTypeId);

            if (existing == null)
            {
                _db.Thresholds.Add(new Threshold
                {
                    PatientId = m.PatientId,
                    MeasurementTypeId = m.MeasurementTypeId,
                    MinValue = m.MinValue,
                    MaxValue = m.MaxValue
                });
            }
            else
            {
                existing.MinValue = m.MinValue;
                existing.MaxValue = m.MaxValue;
                existing.UpdatedDate = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync();
            return ResponseManager.CreateSuccess();
        }

        public async Task<ResponseModel> Delete(int thresholdId)
        {
            var t = await _db.Thresholds.FindAsync(thresholdId);
            if (t == null) return ResponseManager.CreateError("Eşik bulunamadı");
            _db.Thresholds.Remove(t);
            await _db.SaveChangesAsync();
            return ResponseManager.CreateSuccess();
        }

        /// <summary>
        /// Yeni hasta için varsayılan tıbbi eşikleri oluşturur.
        /// </summary>
        public async Task EnsureDefaultThresholds(int patientId)
        {
            var defaults = new (int Mtype, double Min, double Max)[]
            {
                (SeedConstants.MeasurementTypes.HeartRate,    50, 120),  // bpm
                (SeedConstants.MeasurementTypes.SpO2,         92, 100),  // %
                (SeedConstants.MeasurementTypes.Respiration,  10, 24),   // rpm
                (SeedConstants.MeasurementTypes.BodyTemperature, 36.0, 37.8),
            };

            foreach (var d in defaults)
            {
                var exists = await _db.Thresholds.AnyAsync(t => t.PatientId == patientId && t.MeasurementTypeId == d.Mtype);
                if (!exists)
                {
                    _db.Thresholds.Add(new Threshold
                    {
                        PatientId = patientId,
                        MeasurementTypeId = d.Mtype,
                        MinValue = d.Min,
                        MaxValue = d.Max
                    });
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
