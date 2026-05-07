using VitalWatch.Api.Enums;

namespace VitalWatch.Api.Models.Requests
{
    public class IngestDataRequestModel
    {
        public int PatientId { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public DeviceType DeviceType { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
