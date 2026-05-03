namespace VitalWatch.Api.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string  LastName { get; set; }
        public string Email { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; } //Şifreleme algoritması ile hashlenecek...
        public string? Phone { get; set; }
    }
}
