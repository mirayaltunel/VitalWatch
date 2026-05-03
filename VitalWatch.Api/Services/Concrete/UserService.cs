using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.Helpers;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class UserService : IUserService
    {
        private readonly VitalWatchDbContext _dbContext;
        public UserService(VitalWatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel> Register(RegisterRequestModel requestModel)
        {
            if(requestModel.Password != requestModel.PasswordRepeat)
                return ResponseManager.CreateError("Password can not match");

            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.GetHash(requestModel.Password, salt);
            var user = new User
            {
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                Email = requestModel.Email,
                Salt = salt,
                PasswordHash = hash,
                Phone  = requestModel.Phone,
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return ResponseManager.CreateSuccess();
        }
    }
}
