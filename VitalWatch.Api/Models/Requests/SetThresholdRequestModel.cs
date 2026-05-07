namespace VitalWatch.Api.Models.Requests
{
    public class SetThresholdRequestModel
    {
        public int PatientId { get; set; }
        public int MeasurementTypeId { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
    }
}
