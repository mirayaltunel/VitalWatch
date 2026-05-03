using System.Collections.Generic;

namespace VitalWatch.Api.Models.Responses
{
    public class ReportSummaryDto
    {
        public int CriticalCount { get; set; }
        public int WarningCount { get; set; }
        public List<EventDto> Events { get; set; } = new List<EventDto>();
    }
}
