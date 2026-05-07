namespace VitalWatch.Api.Services.Abstract
{
    public interface IPatientService
    {
        Task<ResponseManage.ResponseModel<Models.Responses.AddPatientResponseDto>> AddPatient(Models.Requests.AddPatientRequestModel request, int caregiverUserId);
        Task<ResponseManage.ResponseModel<string>> GetPatientShareCode(int patientId, int userId);
        Task<ResponseManage.ResponseModel<List<Models.Responses.PatientListDto>>> GetMyPatients(int userId);
        Task<ResponseManage.ResponseModel<int?>> VerifyPatientCode(string code, int userId);
    }
}
