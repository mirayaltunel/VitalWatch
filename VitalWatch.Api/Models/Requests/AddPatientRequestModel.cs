using System;

namespace VitalWatch.Api.Models.Requests
{
    public class AddPatientRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public char Gender { get; set; }
        public string DiseaseName { get; set; }
        public string EmergencyPhone { get; set; }
    }
}
