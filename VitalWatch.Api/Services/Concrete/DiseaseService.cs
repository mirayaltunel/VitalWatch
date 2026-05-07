using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.EFConfiguration;
using VitalWatch.Api.Entities;
using VitalWatch.Api.ResponseManage;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Services.Concrete
{
    public class DiseaseService : IDiseaseService
    {
        private readonly VitalWatchDbContext _db;
        public DiseaseService(VitalWatchDbContext db) { _db = db; }

        public async Task<ResponseModel<List<Disease>>> GetAll()
        {
            var list = await _db.Diseases.OrderBy(d => d.Name).ToListAsync();
            return ResponseManager.CreateSuccess(list);
        }
    }
}
