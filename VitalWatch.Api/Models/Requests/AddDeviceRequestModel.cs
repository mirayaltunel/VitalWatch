namespace VitalWatch.Api.Models.Requests
{
    public class AddDeviceRequestModel
    {
        public int? PatientId { get; set; }   // hasta sensörü
        public int? UserId { get; set; }      // bakıcı cihazı
        public string DeviceName { get; set; }

        /// <summary>1=SmartWatch, 2=PatientUnit, 3=MotionSensor, 4=PulseOximeter</summary>
        public int DeviceTypeId { get; set; }

        /// <summary>1=Active, 2=Inactive, 3=Maintenance</summary>
        public int DeviceStatusId { get; set; } = 1;

        public int? BatteryLevel { get; set; }
    }
}
