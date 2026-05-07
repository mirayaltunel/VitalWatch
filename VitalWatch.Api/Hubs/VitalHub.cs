using Microsoft.AspNetCore.SignalR;

namespace VitalWatch.Api.Hubs
{
    public class VitalHub : Hub
    {
        // Client patientId'ye göre gruba katılır
        public async Task JoinPatientGroup(string patientId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"patient_{patientId}");
        }

        public async Task LeavePatientGroup(string patientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"patient_{patientId}");
        }
    }
}
