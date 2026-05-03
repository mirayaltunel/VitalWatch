namespace VitalWatch.Api.Services.Abstract
{
    public interface ISensorMeasurementService
    {
        Task<ResponseManage.ResponseModel<Models.Responses.LiveVitalsDto>> GetLatestVitals(int patientId);
        Task<ResponseManage.ResponseModel<List<Entities.SensorMeasurement>>> GetVitalHistory(int patientId, Enums.MeasurementType type);
    }
}
