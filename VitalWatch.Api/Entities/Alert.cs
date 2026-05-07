namespace VitalWatch.Api.Entities
{
    public class Alert : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int MeasurementTypeId { get; set; }
        public MeasurementType MeasurementType { get; set; }

        public int? ThresholdId { get; set; }
        public Threshold? Threshold { get; set; }

        public int SeverityId { get; set; }
        public Severity Severity { get; set; }

        public double Value { get; set; }
        public double ThresholdMinSnapshot { get; set; }
        public double ThresholdMaxSnapshot { get; set; }

        public bool IsReviewed { get; set; }
    }
}
