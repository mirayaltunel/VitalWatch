using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;

namespace VitalWatch.Api.Services.Abstract
{
    public interface IThresholdService
    {
        Task<ResponseModel<List<ThresholdDto>>> GetByPatient(int patientId);
        Task<ResponseModel> Upsert(SetThresholdRequestModel model);
        Task<ResponseModel> Delete(int thresholdId);
        Task EnsureDefaultThresholds(int patientId);
    }
}
