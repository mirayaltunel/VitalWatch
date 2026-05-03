using System;
using VitalWatch.Api.Enums;

namespace VitalWatch.Api.Entities
{
    public class Device : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public string DeviceName { get; set; }
        public DeviceType DeviceType { get; set; }
        public int BatteryLevel { get; set; }
        public bool IsConnected { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}
