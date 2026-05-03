using System;

namespace VitalWatch.Api.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int EventId { get; set; }
        public HealthEvent HealthEvent { get; set; }

        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }
}
