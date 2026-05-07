namespace VitalWatch.Api.Entities
{
    public class EventType : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
