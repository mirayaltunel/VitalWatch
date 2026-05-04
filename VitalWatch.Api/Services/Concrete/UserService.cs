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
        private readonly VitalWatchDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserService(VitalWatchDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<ResponseModel> Register(RegisterRequestModel requestModel)
        {
            if (requestModel.Password != requestModel.PasswordRepeat)
                return ResponseManager.CreateError("Şifreler eşleşmiyor");

            var emailExists = await _dbContext.Users.AnyAsync(u => u.Email == requestModel.Email);
            if (emailExists)
                return ResponseManager.CreateError("Bu e-posta zaten kayıtlı");

            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.GetHash(requestModel.Password, salt);
            var user = new User
            {
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                Email = requestModel.Email,
                Salt = salt,
                PasswordHash = hash,
                Phone = requestModel.Phone,
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return ResponseManager.CreateSuccess();
        }

        public async Task<ResponseModel<LoginResponseDto>> Login(LoginRequestModel requestModel)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == requestModel.Email);
            if (user == null)
                return ResponseManager.CreateError<LoginResponseDto>("Kullanıcı bulunamadı");

            var hash = PasswordHelper.GetHash(requestModel.Password, user.Salt);
            if (user.PasswordHash != hash)
                return ResponseManager.CreateError<LoginResponseDto>("Şifre hatalı");

            var token = JwtHelper.GenerateToken(user, _configuration);

            return ResponseManager.CreateSuccess(new LoginResponseDto
            {
                UserId = user.Id,
                Token = token,
                FullName = $"{user.FirstName} {user.LastName}"
            });
        }
    }
}
