namespace VitalWatch.Api.Entities
{
    public class PatientDisease : BaseEntity
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

        public DateTime DiagnosedAt { get; set; }
    }
}
