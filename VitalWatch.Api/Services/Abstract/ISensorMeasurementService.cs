using VitalWatch.Api.Entities;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;

namespace VitalWatch.Api.Services.Abstract
{
    public interface ISensorMeasurementService
    {
        Task<ResponseModel<LiveVitalsDto>> GetLatestVitals(int patientId);
        Task<ResponseModel<List<SensorMeasurement>>> GetVitalHistory(int patientId, int measurementTypeId, int take = 50);
    }
}
