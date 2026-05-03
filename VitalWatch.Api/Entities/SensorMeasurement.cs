using System;
using VitalWatch.Api.Enums;

namespace VitalWatch.Api.Entities
{
    public class SensorMeasurement : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DeviceType DeviceType { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public double Value { get; set; }
        public string? Unit { get; set; }
        public DateTime Timestamp { get; set; }

        public int? HealthEventId { get; set; }
        public HealthEvent? HealthEvent { get; set; }
    }
}
