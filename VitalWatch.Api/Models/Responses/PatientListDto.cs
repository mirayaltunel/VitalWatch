namespace VitalWatch.Api.Models.Responses
{
    public class PatientListDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string DiseaseName { get; set; }
        public bool IsConnected { get; set; }
    }
}
