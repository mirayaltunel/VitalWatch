namespace VitalWatch.Api.Models.Responses
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
