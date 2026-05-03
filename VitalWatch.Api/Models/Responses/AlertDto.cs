using System;

namespace VitalWatch.Api.Models.Responses
{
    public class AlertDto
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public string Severity { get; set; }
    }
}
