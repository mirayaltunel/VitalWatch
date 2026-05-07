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
    public class PatientService : IPatientService
    {
        private readonly VitalWatchDbContext _db;
        private readonly IThresholdService _thresholdService;

        public PatientService(VitalWatchDbContext db, IThresholdService thresholdService)
        {
            _db = db;
            _thresholdService = thresholdService;
        }

        public async Task<ResponseModel<AddPatientResponseDto>> AddPatient(AddPatientRequestModel request, int caregiverUserId)
        {
            try
            {
                var patient = new Patient
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
                    GenderId = request.GenderId == 0 ? SeedConstants.Genders.Other : request.GenderId,
                    PatientShareCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()
                };

                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();

                _db.UserPatients.Add(new UserPatient
                {
                    UserId = caregiverUserId,
                    PatientId = patient.Id,
                    RelationshipTypeId = SeedConstants.RelationshipTypes.Caregiver
                });

                if (!string.IsNullOrEmpty(request.DiseaseName))
                {
                    var disease = await _db.Diseases.FirstOrDefaultAsync(d => d.Name == request.DiseaseName);
                    if (disease == null)
                    {
                        disease = new Disease { Name = request.DiseaseName };
                        _db.Diseases.Add(disease);
                        await _db.SaveChangesAsync();
                    }

                    _db.PatientDiseases.Add(new PatientDisease
                    {
                        PatientId = patient.Id,
                        DiseaseId = disease.Id,
                        DiagnosedAt = DateTime.UtcNow
                    });
                }

                await _db.SaveChangesAsync();

                // Hasta için varsayılan threshold'ları oluştur
                await _thresholdService.EnsureDefaultThresholds(patient.Id);

                return ResponseManager.CreateSuccess(new AddPatientResponseDto
                {
                    PatientId = patient.Id,
                    PatientShareCode = patient.PatientShareCode
                });
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return ResponseManager.CreateError<AddPatientResponseDto>("Sistem Hatası: " + msg);
            }
        }

        public async Task<ResponseModel<string>> GetPatientShareCode(int patientId, int userId)
        {
            // Sadece bu hastaya bağlı kullanıcı kodu görebilsin
            var hasAccess = await _db.UserPatients
                .AnyAsync(up => up.UserId == userId && up.PatientId == patientId);
            if (!hasAccess)
                return ResponseManager.CreateError<string>("Bu hastaya erişiminiz yok");

            var code = await _db.Patients
                .Where(p => p.Id == patientId)
                .Select(p => p.PatientShareCode)
                .FirstOrDefaultAsync();

            if (code == null)
                return ResponseManager.CreateError<string>("Hasta bulunamadı");

            return ResponseManager.CreateSuccess<string>(code);
        }

        public async Task<ResponseModel<List<PatientListDto>>> GetMyPatients(int userId)
        {
            var patients = await _db.UserPatients
                .Include(up => up.Patient)
                .Where(up => up.UserId == userId)
                .Select(up => new PatientListDto
                {
                    Id = up.Patient.Id,
                    FirstName = up.Patient.FirstName,
                    LastName = up.Patient.LastName,
                    Age = DateTime.UtcNow.Year - up.Patient.BirthDate.Year,
                    DiseaseName = _db.PatientDiseases
                                      .Where(pd => pd.PatientId == up.PatientId)
                                      .Select(pd => pd.Disease.Name)
                                      .FirstOrDefault() ?? "Bilinmiyor",
                    IsConnected = _db.Devices.Any(d =>
                        d.PatientId == up.PatientId &&
                        d.DeviceStatusId == SeedConstants.DeviceStatuses.Active)
                })
                .ToListAsync();

            return ResponseManager.CreateSuccess(patients);
        }

        public async Task<ResponseModel<int?>> VerifyPatientCode(string code, int userId)
        {
            try
            {
                var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientShareCode == code);
                if (patient == null)
                    return ResponseManager.CreateError<int?>("Geçersiz hasta kodu");

                var existing = await _db.UserPatients
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.PatientId == patient.Id);

                if (existing == null)
                {
                    _db.UserPatients.Add(new UserPatient
                    {
                        UserId = userId,
                        PatientId = patient.Id,
                        RelationshipTypeId = SeedConstants.RelationshipTypes.Relative
                    });
                    await _db.SaveChangesAsync();
                }

                return ResponseManager.CreateSuccess<int?>(patient.Id);
            }
            catch (Exception ex)
            {
                return ResponseManager.CreateError<int?>("Hata: " + ex.Message);
            }
        }
    }
}
