using Microsoft.EntityFrameworkCore;
using VitalWatch.Api.Entities;

namespace VitalWatch.Api.EFConfiguration
{
    public class VitalWatchDbContext : DbContext
    {
        public VitalWatchDbContext(DbContextOptions<VitalWatchDbContext> options) : base(options) { }

        // Ana tablolar
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<UserPatient> UserPatients { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<PatientDisease> PatientDiseases { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<SensorMeasurement> SensorMeasurements { get; set; }
        public DbSet<HealthEvent> HealthEvents { get; set; }
        public DbSet<Threshold> Thresholds { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportDetail> ReportDetails { get; set; }

        // Lookup tablolar
        public DbSet<Role> Roles { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<DeviceStatus> DeviceStatuses { get; set; }
        public DbSet<MeasurementType> MeasurementTypes { get; set; }
        public DbSet<Severity> Severities { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<RelationshipType> RelationshipTypes { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }
        public DbSet<AlertSource> AlertSources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ----------------- User -----------------
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Phone).HasMaxLength(20).IsRequired(false);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Salt).IsRequired();

                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- Patient -----------------
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(50);
                entity.Property(p => p.BirthDate).IsRequired();
                entity.Property(p => p.PatientShareCode).IsRequired().HasMaxLength(12);
                entity.HasIndex(p => p.PatientShareCode).IsUnique();

                entity.HasOne(p => p.Gender)
                      .WithMany()
                      .HasForeignKey(p => p.GenderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- UserPatient -----------------
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

                entity.HasOne(up => up.RelationshipType)
                      .WithMany()
                      .HasForeignKey(up => up.RelationshipTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- PatientDisease -----------------
            modelBuilder.Entity<PatientDisease>(entity =>
            {
                entity.HasOne(pd => pd.Patient)
                      .WithMany()
                      .HasForeignKey(pd => pd.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pd => pd.Disease)
                      .WithMany()
                      .HasForeignKey(pd => pd.DiseaseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- Device -----------------
            modelBuilder.Entity<Device>(entity =>
            {
                entity.Property(d => d.DeviceName).IsRequired().HasMaxLength(100);

                entity.HasOne(d => d.Patient)
                      .WithMany()
                      .HasForeignKey(d => d.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                      .WithMany()
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.DeviceType)
                      .WithMany()
                      .HasForeignKey(d => d.DeviceTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.DeviceStatus)
                      .WithMany()
                      .HasForeignKey(d => d.DeviceStatusId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- SensorMeasurement -----------------
            modelBuilder.Entity<SensorMeasurement>(entity =>
            {
                entity.HasOne(sm => sm.Patient)
                      .WithMany()
                      .HasForeignKey(sm => sm.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sm => sm.Device)
                      .WithMany()
                      .HasForeignKey(sm => sm.DeviceId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sm => sm.MeasurementType)
                      .WithMany()
                      .HasForeignKey(sm => sm.MeasurementTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(sm => new { sm.PatientId, sm.MeasurementTypeId, sm.Timestamp });
            });

            // ----------------- HealthEvent -----------------
            modelBuilder.Entity<HealthEvent>(entity =>
            {
                entity.HasOne(he => he.Patient)
                      .WithMany()
                      .HasForeignKey(he => he.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(he => he.EventType)
                      .WithMany()
                      .HasForeignKey(he => he.EventTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(he => he.Severity)
                      .WithMany()
                      .HasForeignKey(he => he.SeverityId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(he => he.AlertSource)
                      .WithMany()
                      .HasForeignKey(he => he.AlertSourceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- Threshold -----------------
            modelBuilder.Entity<Threshold>(entity =>
            {
                entity.HasOne(t => t.Patient)
                      .WithMany()
                      .HasForeignKey(t => t.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.MeasurementType)
                      .WithMany()
                      .HasForeignKey(t => t.MeasurementTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(t => new { t.PatientId, t.MeasurementTypeId }).IsUnique();
            });

            // ----------------- Alert -----------------
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.HasOne(a => a.Patient)
                      .WithMany()
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.MeasurementType)
                      .WithMany()
                      .HasForeignKey(a => a.MeasurementTypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Threshold)
                      .WithMany()
                      .HasForeignKey(a => a.ThresholdId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.Severity)
                      .WithMany()
                      .HasForeignKey(a => a.SeverityId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- Report -----------------
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasOne(r => r.Patient)
                      .WithMany()
                      .HasForeignKey(r => r.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.ReportType)
                      .WithMany()
                      .HasForeignKey(r => r.ReportTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- ReportDetail -----------------
            modelBuilder.Entity<ReportDetail>(entity =>
            {
                entity.HasOne(rd => rd.Report)
                      .WithMany()
                      .HasForeignKey(rd => rd.ReportId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rd => rd.MeasurementType)
                      .WithMany()
                      .HasForeignKey(rd => rd.MeasurementTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ----------------- LOOKUP SEED -----------------
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Caregiver", CreatedDate = seedDate },
                new Role { Id = 2, Name = "Relative", CreatedDate = seedDate }
            );

            modelBuilder.Entity<Gender>().HasData(
                new Gender { Id = 1, Name = "Male", CreatedDate = seedDate },
                new Gender { Id = 2, Name = "Female", CreatedDate = seedDate },
                new Gender { Id = 3, Name = "Other", CreatedDate = seedDate }
            );

            modelBuilder.Entity<DeviceType>().HasData(
                new DeviceType { Id = 1, Name = "SmartWatch", CreatedDate = seedDate },
                new DeviceType { Id = 2, Name = "PatientUnit", CreatedDate = seedDate },
                new DeviceType { Id = 3, Name = "MotionSensor", CreatedDate = seedDate },
                new DeviceType { Id = 4, Name = "PulseOximeter", CreatedDate = seedDate }
            );

            modelBuilder.Entity<DeviceStatus>().HasData(
                new DeviceStatus { Id = 1, Name = "Active", CreatedDate = seedDate },
                new DeviceStatus { Id = 2, Name = "Inactive", CreatedDate = seedDate },
                new DeviceStatus { Id = 3, Name = "Maintenance", CreatedDate = seedDate }
            );

            modelBuilder.Entity<MeasurementType>().HasData(
                new MeasurementType { Id = 1, Name = "HeartRate", Unit = "bpm", Description = "Kalp atış hızı", CreatedDate = seedDate },
                new MeasurementType { Id = 2, Name = "SpO2", Unit = "%", Description = "Kandaki oksijen doygunluğu", CreatedDate = seedDate },
                new MeasurementType { Id = 3, Name = "Respiration", Unit = "rpm", Description = "Solunum hızı", CreatedDate = seedDate },
                new MeasurementType { Id = 4, Name = "AccelerometerX", Unit = "g", Description = "X ekseni ivme", CreatedDate = seedDate },
                new MeasurementType { Id = 5, Name = "AccelerometerY", Unit = "g", Description = "Y ekseni ivme", CreatedDate = seedDate },
                new MeasurementType { Id = 6, Name = "AccelerometerZ", Unit = "g", Description = "Z ekseni ivme", CreatedDate = seedDate },
                new MeasurementType { Id = 7, Name = "BodyTemperature", Unit = "°C", Description = "Vücut sıcaklığı", CreatedDate = seedDate }
            );

            modelBuilder.Entity<Severity>().HasData(
                new Severity { Id = 1, Name = "Low", Level = 1, CreatedDate = seedDate },
                new Severity { Id = 2, Name = "Medium", Level = 2, CreatedDate = seedDate },
                new Severity { Id = 3, Name = "High", Level = 3, CreatedDate = seedDate },
                new Severity { Id = 4, Name = "Critical", Level = 4, CreatedDate = seedDate }
            );

            modelBuilder.Entity<EventType>().HasData(
                new EventType { Id = 1, Name = "Seizure", Description = "Nöbet tespit edildi", CreatedDate = seedDate },
                new EventType { Id = 2, Name = "FallDetected", Description = "Düşme tespit edildi", CreatedDate = seedDate },
                new EventType { Id = 3, Name = "LowSpO2", Description = "Oksijen doygunluğu düşük", CreatedDate = seedDate },
                new EventType { Id = 4, Name = "HighHeartRate", Description = "Yüksek nabız", CreatedDate = seedDate },
                new EventType { Id = 5, Name = "LowHeartRate", Description = "Düşük nabız", CreatedDate = seedDate },
                new EventType { Id = 6, Name = "Apnea", Description = "Solunum duraklaması", CreatedDate = seedDate }
            );

            modelBuilder.Entity<RelationshipType>().HasData(
                new RelationshipType { Id = 1, Name = "Caregiver", CreatedDate = seedDate },
                new RelationshipType { Id = 2, Name = "Relative", CreatedDate = seedDate }
            );

            modelBuilder.Entity<ReportType>().HasData(
                new ReportType { Id = 1, Name = "Daily", CreatedDate = seedDate },
                new ReportType { Id = 2, Name = "Weekly", CreatedDate = seedDate },
                new ReportType { Id = 3, Name = "CriticalSummary", CreatedDate = seedDate }
            );

            modelBuilder.Entity<AlertSource>().HasData(
                new AlertSource { Id = 1, Name = "Sensor", CreatedDate = seedDate },
                new AlertSource { Id = 2, Name = "Manual", CreatedDate = seedDate },
                new AlertSource { Id = 3, Name = "System", CreatedDate = seedDate }
            );

            // Sık karşılaşılan hastalıklar (kullanıcı listeden seçer, yoksa custom girer)
            modelBuilder.Entity<Disease>().HasData(
                new Disease { Id = 1, Name = "Epilepsi", CreatedDate = seedDate },
                new Disease { Id = 2, Name = "KOAH", CreatedDate = seedDate },
                new Disease { Id = 3, Name = "Diyabet", CreatedDate = seedDate },
                new Disease { Id = 4, Name = "Hipertansiyon", CreatedDate = seedDate },
                new Disease { Id = 5, Name = "Alzheimer", CreatedDate = seedDate },
                new Disease { Id = 6, Name = "Parkinson", CreatedDate = seedDate },
                new Disease { Id = 7, Name = "Astım", CreatedDate = seedDate },
                new Disease { Id = 8, Name = "Kalp Yetmezliği", CreatedDate = seedDate },
                new Disease { Id = 9, Name = "Demans", CreatedDate = seedDate },
                new Disease { Id = 10, Name = "Felç", CreatedDate = seedDate },
                new Disease { Id = 11, Name = "Diğer", CreatedDate = seedDate }
            );
        }
    }
}
