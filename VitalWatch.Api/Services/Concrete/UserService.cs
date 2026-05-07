using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Models.Responses;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly VitalWatchDbContext _db;
        private readonly IConfiguration _configuration;

        public UserService(VitalWatchDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<ResponseModel> Register(RegisterRequestModel m)
        {
            if (m.Password != m.PasswordRepeat)
                return ResponseManager.CreateError("Şifreler eşleşmiyor");

            var email = m.Email.ToLower().Trim();
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return ResponseManager.CreateError("Bu e-posta zaten kayıtlı");

            var roleId = (m.RoleId == SeedConstants.Roles.Caregiver || m.RoleId == SeedConstants.Roles.Relative)
                ? m.RoleId
                : SeedConstants.Roles.Caregiver;

            var salt = PasswordHelper.GenerateSalt();
            var user = new User
            {
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = email,
                Salt = salt,
                PasswordHash = PasswordHelper.GetHash(m.Password, salt),
                Phone = m.Phone?.Trim(),
                RoleId = roleId
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Bakıcı ise otomatik bileklik cihazı oluştur (buzzer/alarm)
            if (roleId == SeedConstants.Roles.Caregiver)
            {
                _db.Devices.Add(new Entities.Device
                {
                    UserId = user.Id,
                    DeviceName = $"{user.FirstName} Bilekliği",
                    DeviceTypeId = SeedConstants.DeviceTypes.SmartWatch,
                    DeviceStatusId = SeedConstants.DeviceStatuses.Active,
                    BatteryLevel = 100,
                    LastSeenAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
            }

            return ResponseManager.CreateSuccess();
        }

        public async Task<ResponseModel<LoginResponseDto>> Login(LoginRequestModel m)
        {
            var email = m.Email.ToLower().Trim();
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return ResponseManager.CreateError<LoginResponseDto>("Kullanıcı bulunamadı");

            if (user.PasswordHash != PasswordHelper.GetHash(m.Password, user.Salt))
                return ResponseManager.CreateError<LoginResponseDto>("Şifre hatalı");

            var token = JwtHelper.GenerateToken(user, _configuration);

            return ResponseManager.CreateSuccess(new LoginResponseDto
            {
                UserId = user.Id,
                Token = token,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role?.Name ?? "Caregiver"
            });
        }
    }
}
