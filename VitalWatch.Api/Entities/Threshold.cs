namespace VitalWatch.Api.Entities
{
    public class Threshold : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int MeasurementTypeId { get; set; }
        public MeasurementType MeasurementType { get; set; }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
