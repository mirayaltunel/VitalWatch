namespace VitalWatch.Api.Models.Responses
{
    public class ThresholdDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int MeasurementTypeId { get; set; }
        public string MeasurementType { get; set; }
        public string Unit { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
