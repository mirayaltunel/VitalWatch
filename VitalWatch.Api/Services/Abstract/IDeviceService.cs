using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;

namespace VitalWatch.Api.Services.Abstract
{
    public interface IDeviceService
    {
        Task<ResponseModel<List<DeviceDto>>> GetPatientDevices(int patientId);
        Task<ResponseModel<List<DeviceDto>>> GetUserDevices(int userId);
        Task<ResponseModel<int>> AddDevice(AddDeviceRequestModel model);
        Task<ResponseModel> UpdateStatus(int deviceId, int statusId, int? batteryLevel);
        Task<ResponseModel> Delete(int deviceId);
    }
}
