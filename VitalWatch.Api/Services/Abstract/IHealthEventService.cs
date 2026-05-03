namespace VitalWatch.Api.Services.Abstract
{
    public interface IHealthEventService
    {
        Task<ResponseManage.ResponseModel<List<Models.Responses.AlertDto>>> GetLatestAlerts(int patientId);
        Task<ResponseManage.ResponseModel<Models.Responses.ReportSummaryDto>> GetPatientReports(int patientId, DateTime startDate, DateTime endDate);
    }
}
