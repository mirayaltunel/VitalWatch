using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;

namespace VitalWatch.Api.Services.Abstract
{
    public interface IAlertService
    {
        /// <summary>
        /// Bir ölçüm için threshold ihlali ve nöbet tespiti yapar.
        /// İhlal varsa Alert + HealthEvent kaydeder ve SignalR ile yayınlar.
        /// </summary>
        Task EvaluateMeasurement(int patientId, int deviceId, int measurementTypeId, double value,
                                 double? valueX, double? valueY, double? valueZ, DateTime timestamp);

        Task<ResponseModel<List<AlertDto>>> GetLatestAlerts(int patientId, int take = 20);
        Task<ResponseModel> MarkReviewed(int alertId);
    }
}
