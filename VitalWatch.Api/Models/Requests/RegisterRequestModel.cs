namespace VitalWatch.Api.Models.Requests
{
    public class RegisterRequestModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
        public string? Phone { get; set; }

        /// <summary>1=Caregiver, 2=Relative</summary>
        public int RoleId { get; set; } = 1;
    }
}
