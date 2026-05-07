namespace VitalWatch.Api.Entities
{
    public class UserPatient : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int RelationshipTypeId { get; set; }
        public RelationshipType RelationshipType { get; set; }
    }
}
