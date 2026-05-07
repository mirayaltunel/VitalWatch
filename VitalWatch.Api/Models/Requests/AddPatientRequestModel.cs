namespace VitalWatch.Api.Models.Requests
{
    public class AddPatientRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        /// <summary>1=Male, 2=Female, 3=Other</summary>
        public int GenderId { get; set; }

        public string? DiseaseName { get; set; }
        public string? EmergencyPhone { get; set; }
    }
}
