using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.Entities;

namespace VitalWatch.Api.EFConfiguration
{
    public class VitalWatchDbContext : DbContext
    {
        public VitalWatchDbContext(DbContextOptions<VitalWatchDbContext> options) : base(options)
        { 
        }
        public DbSet<User> Users { get; set; }
    }
}
