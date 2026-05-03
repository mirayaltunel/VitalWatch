using System;

namespace VitalWatch.Api.Models.Responses
{
    public class DeviceDto
    {
        public string Name { get; set; }
        public string SerialNo { get; set; }
        public bool IsConnected { get; set; }
        public int BatteryLevel { get; set; }
        public DateTime? LastSync { get; set; }
        public string FirmwareVersion { get; set; }
    }
}
