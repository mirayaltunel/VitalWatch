namespace VitalWatch.Api.Entities
{
    public class Report : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int ReportTypeId { get; set; }
        public ReportType ReportType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
