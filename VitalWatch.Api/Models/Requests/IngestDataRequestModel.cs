namespace VitalWatch.Api.Models.Requests
{
    public class IngestDataRequestModel
    {
        public int PatientId { get; set; }
        public int DeviceId { get; set; }
        public int MeasurementTypeId { get; set; }
        public double Value { get; set; }
        public double? ValueX { get; set; }
        public double? ValueY { get; set; }
        public double? ValueZ { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
