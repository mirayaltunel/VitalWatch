using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Enums;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class PatientService : IPatientService
    {
        private readonly VitalWatchDbContext _dbContext;

        public PatientService(VitalWatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> AddPatient(AddPatientRequestModel request, int caregiverUserId)
        {
            try
            {
                var patient = new Patient
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
                    Gender = string.IsNullOrEmpty(request.Gender) ? 'D' : request.Gender[0],
                    PatientShareCode = Guid.NewGuid().ToString().Substring(0, 8)
                };

                _dbContext.Patients.Add(patient);
                await _dbContext.SaveChangesAsync();

                var userPatient = new UserPatient
                {
                    UserId = caregiverUserId,
                    PatientId = patient.Id,
                    RelationshipType = RelationshipType.Caregiver
                };
                _dbContext.UserPatients.Add(userPatient);

                if (!string.IsNullOrEmpty(request.DiseaseName))
                {
                    var disease = await _dbContext.Diseases.FirstOrDefaultAsync(d => d.Name == request.DiseaseName);
                    if (disease == null)
                    {
                        disease = new Disease { Name = request.DiseaseName };
                        _dbContext.Diseases.Add(disease);
                        await _dbContext.SaveChangesAsync();
                    }

                    _dbContext.PatientDiseases.Add(new PatientDisease
                    {
                        PatientId = patient.Id,
                        DiseaseId = disease.Id,
                        DiagnosedAt = DateTime.UtcNow,
                        Severity = Severity.Medium
                    });
                }

                await _dbContext.SaveChangesAsync();
                return ResponseManager.CreateSuccess();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return ResponseManager.CreateError("Sistem Hatası: " + msg);
            }
        }

        public async Task<ResponseModel<List<PatientListDto>>> GetMyPatients(int userId)
        {
            var patients = await _dbContext.UserPatients
                .Include(up => up.Patient)
                .Where(up => up.UserId == userId)
                .Select(up => new PatientListDto
                {
                    Id = up.Patient.Id,
                    FirstName = up.Patient.FirstName,
                    LastName = up.Patient.LastName,
                    Age = DateTime.UtcNow.Year - up.Patient.BirthDate.Year,
                    DiseaseName = _dbContext.PatientDiseases
                                      .Where(pd => pd.PatientId == up.PatientId)
                                      .Select(pd => pd.Disease.Name)
                                      .FirstOrDefault() ?? "Bilinmiyor",
                    IsConnected = _dbContext.Devices.Any(d => d.PatientId == up.PatientId && d.IsConnected)
                })
                .ToListAsync();

            return ResponseManager.CreateSuccess(patients);
        }

        public async Task<ResponseModel<int?>> VerifyPatientCode(string code, int userId)
        {
            try
            {
                var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.PatientShareCode == code);
                if (patient == null)
                {
                    return ResponseManager.CreateError<int?>("Geçersiz hasta kodu");
                }

                // Kullanıcı zaten bu hastaya bağlı mı kontrol et
                var existingLink = await _dbContext.UserPatients
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.PatientId == patient.Id);

                if (existingLink == null)
                {
                    // Bağlı değilse Hasta Yakını (FamilyMember) olarak bağla
                    _dbContext.UserPatients.Add(new UserPatient
                    {
                        UserId = userId,
                        PatientId = patient.Id,
                        RelationshipType = RelationshipType.FamilyMember
                    });
                    await _dbContext.SaveChangesAsync();
                }

                return ResponseManager.CreateSuccess<int?>(patient.Id);
            }
            catch (Exception ex)
            {
                return ResponseManager.CreateError<int?>("Doğrulama sırasında hata oluştu: " + ex.Message);
            }
        }
    }
}
