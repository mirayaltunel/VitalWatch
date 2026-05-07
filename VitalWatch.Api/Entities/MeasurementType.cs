namespace VitalWatch.Api.Entities
{
    public class MeasurementType : BaseEntity
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public string? Description { get; set; }
    }
}
