namespace VitalWatch.Api.Entities
{
    public class Patient : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public int GenderId { get; set; }
        public Gender Gender { get; set; }

        public string PatientShareCode { get; set; }
    }
}
