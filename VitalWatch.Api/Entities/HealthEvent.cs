using System;
using VitalWatch.Api.Enums;

namespace VitalWatch.Api.Entities
{
    public class HealthEvent : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DeviceType DeviceType { get; set; }
        public EventType EventType { get; set; }
        public double? Value { get; set; }
        public string? Unit { get; set; }
        public Severity Severity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
