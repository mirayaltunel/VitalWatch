namespace VitalWatch.Api.Helpers
{
    /// <summary>
    /// Seed ediliyor olan lookup tablolarındaki sabit ID'lere
    /// servis kodundan tip-güvenli erişim için kullanılır.
    /// Migration'da seed edilen ID'lerle birebir eşleşmek zorundadır.
    /// </summary>
    public static class SeedConstants
    {
        public static class Roles
        {
            public const int Caregiver = 1;
            public const int Relative = 2;
        }

        public static class Genders
        {
            public const int Male = 1;
            public const int Female = 2;
            public const int Other = 3;
        }

        public static class DeviceTypes
        {
            public const int SmartWatch = 1;
            public const int PatientUnit = 2;
            public const int MotionSensor = 3;
            public const int PulseOximeter = 4;
        }

        public static class DeviceStatuses
        {
            public const int Active = 1;
            public const int Inactive = 2;
            public const int Maintenance = 3;
        }

        public static class MeasurementTypes
        {
            public const int HeartRate = 1;
            public const int SpO2 = 2;
            public const int Respiration = 3;
            public const int AccelerometerX = 4;
            public const int AccelerometerY = 5;
            public const int AccelerometerZ = 6;
            public const int BodyTemperature = 7;
        }

        public static class Severities
        {
            public const int Low = 1;
            public const int Medium = 2;
            public const int High = 3;
            public const int Critical = 4;
        }

        public static class EventTypes
        {
            public const int Seizure = 1;
            public const int FallDetected = 2;
            public const int LowSpO2 = 3;
            public const int HighHeartRate = 4;
            public const int LowHeartRate = 5;
            public const int Apnea = 6;
        }

        public static class RelationshipTypes
        {
            public const int Caregiver = 1;
            public const int Relative = 2;
        }

        public static class ReportTypes
        {
            public const int Daily = 1;
            public const int Weekly = 2;
            public const int CriticalSummary = 3;
        }

        public static class AlertSources
        {
            public const int Sensor = 1;
            public const int Manual = 2;
            public const int System = 3;
        }
    }
}
