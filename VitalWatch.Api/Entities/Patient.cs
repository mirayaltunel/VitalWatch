namespace VitalWatch.Api.Entities
{
    public class Patient : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public char Gender { get; set; }
        public string PatientShareCode { get; set; } // bir hasta başka user iile erişmek istiyor ise bu kod ile olacak
    }
}
