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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // FirstName: Zorunlu ve max 50 karakter
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                // LastName: Zorunlu ve max 50 karakter
                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                // Email: Zorunlu, max 100 karakter ve Benzersiz (Unique)
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email).IsUnique();

                // Phone: Boş bırakılabilir (Nullable) ve sabit uzunluk
                entity.Property(u => u.Phone)
                    .HasMaxLength(20)
                    .IsRequired(false);

                // Password ve Salt genelde sabit uzunluktadır (Hash algoritmana göre)
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Salt).IsRequired();
            });
        }
    }
}
