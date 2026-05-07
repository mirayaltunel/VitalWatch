namespace VitalWatch.Api.Models.Responses
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public DateTime Time { get; set; }
    }
}
