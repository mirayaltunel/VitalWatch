namespace VitalWatch.Api.Entities
{
    public class Device : BaseEntity
    {
        // Hasta sensörleri (oximeter, motion sensor vs) bu alanla bağlanır
        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }

        // Bakıcı tarafındaki cihazlar (bileklik, hasta ünitesi) bu alanla bağlanır
        public int? UserId { get; set; }
        public User? User { get; set; }

        public string DeviceName { get; set; }

        public int DeviceTypeId { get; set; }
        public DeviceType DeviceType { get; set; }

        public int DeviceStatusId { get; set; }
        public DeviceStatus DeviceStatus { get; set; }

        public int? BatteryLevel { get; set; } // null = prize takılı
        public DateTime? LastSeenAt { get; set; }
    }
}
