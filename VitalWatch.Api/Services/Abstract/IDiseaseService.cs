using VitalWatch.Api.Entities;
using VitalWatch.Api.ResponseManage;

namespace VitalWatch.Api.Services.Abstract
{
    public interface IDiseaseService
    {
        Task<ResponseModel<List<Disease>>> GetAll();
    }
}
