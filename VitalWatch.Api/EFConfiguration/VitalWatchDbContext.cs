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
        public DbSet<Patient> Patients { get; set; }
        public DbSet<UserPatient> UserPatients { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<PatientDisease> PatientDiseases { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<HealthEvent> HealthEvents { get; set; }
        public DbSet<SensorMeasurement> SensorMeasurements { get; set; }
        public DbSet<SeizureEvent> SeizureEvents { get; set; }
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

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);


                entity.Property(u => u.BirthDate)
                    .IsRequired(true);

                entity.Property(u => u.Gender)
                    .IsRequired(true);
            });

            modelBuilder.Entity<UserPatient>(entity =>
            {
                entity.HasOne(up => up.User)
                      .WithMany()
                      .HasForeignKey(up => up.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(up => up.Patient)
                      .WithMany()
                      .HasForeignKey(up => up.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SensorMeasurement>(entity =>
            {
                entity.HasOne(sm => sm.HealthEvent)
                      .WithMany()
                      .HasForeignKey(sm => sm.HealthEventId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
