using System;

namespace VitalWatch.Api.Models.Responses
{
    public class LiveVitalsDto
    {
        public double Pulse { get; set; }
        public double SpO2 { get; set; }
        public double Respiration { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
