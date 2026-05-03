using System;

namespace VitalWatch.Api.Entities
{
    public class SeizureEvent : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Duration { get; set; }
    }
}
