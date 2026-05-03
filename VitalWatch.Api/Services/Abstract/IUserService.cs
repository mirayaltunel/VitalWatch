using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.ResponseManage;

namespace VitalWatch.Api.Services.Abstract
{
    public interface IUserService
    {
        Task<ResponseModel> Register(RegisterRequestModel requestModel);
        Task<ResponseModel<int>> Login(LoginRequestModel requestModel);
    }
}
