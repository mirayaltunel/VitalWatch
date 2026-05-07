namespace VitalWatch.Api.Models.Responses
{
    public class AlertDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string MeasurementType { get; set; }
        public string Severity { get; set; }
        public double Value { get; set; }
        public double ThresholdMin { get; set; }
        public double ThresholdMax { get; set; }
        public bool IsReviewed { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
    }
}
