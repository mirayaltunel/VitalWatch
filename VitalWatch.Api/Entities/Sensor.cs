using VitalWatch.Api.Enums;

namespace VitalWatch.Api.Entities
{
    public class Sensor : BaseEntity
    {
        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public SensorType SensorType { get; set; }
    }
}
