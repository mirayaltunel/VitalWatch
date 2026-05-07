namespace VitalWatch.Api.Entities
{
    public class SensorMeasurement : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public int MeasurementTypeId { get; set; }
        public MeasurementType MeasurementType { get; set; }

        public DateTime Timestamp { get; set; }

        public double Value { get; set; }
        public double? ValueX { get; set; }
        public double? ValueY { get; set; }
        public double? ValueZ { get; set; }
    }
}
