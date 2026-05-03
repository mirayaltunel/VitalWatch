using System;

namespace VitalWatch.Api.Models.Responses
{
    public class EventDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Time { get; set; }
    }
}
