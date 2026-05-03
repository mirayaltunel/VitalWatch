namespace VitalWatch.Api.Services.Abstract
{
    public interface IDeviceService
    {
        Task<ResponseManage.ResponseModel<List<Models.Responses.DeviceDto>>> GetPatientDevices(int patientId);
    }
}
