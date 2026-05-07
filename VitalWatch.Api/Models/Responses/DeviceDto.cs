namespace VitalWatch.Api.Models.Responses
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DeviceType { get; set; }
        public string Status { get; set; }
        public bool IsConnected { get; set; }
        public int? BatteryLevel { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}
