namespace VitalWatch.Api.Entities
{
    public class ReportDetail : BaseEntity
    {
        public int ReportId { get; set; }
        public Report Report { get; set; }

        public int MeasurementTypeId { get; set; }
        public MeasurementType MeasurementType { get; set; }

        public double AvgValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int CriticalCount { get; set; }
    }
}
