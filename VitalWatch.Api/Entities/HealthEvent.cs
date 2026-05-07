namespace VitalWatch.Api.Entities
{
    public class HealthEvent : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }

        public int SeverityId { get; set; }
        public Severity Severity { get; set; }

        public int AlertSourceId { get; set; }
        public AlertSource AlertSource { get; set; }

        public double? Value { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime? EndTimestamp { get; set; }
    }
}
