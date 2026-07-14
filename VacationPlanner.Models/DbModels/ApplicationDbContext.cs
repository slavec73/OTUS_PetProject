using Microsoft.EntityFrameworkCore;

namespace VacationPlanner.Models.DbModels
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Position> Positions { get; set; }
        public DbSet<GlobalVacationSetting> GlobalVacationSettings { get; set; }
        public DbSet<PositionVacationSetting> PositionVacationSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GlobalVacationSetting>().HasData(
                new GlobalVacationSetting { Id = 1, DefaultVacationDays = 20 }
            );

            builder.Entity<User>(entity =>
            {
                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });

            builder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Role>().HasData(
                new Role { RoleId = WellKnownRoles.AdministratorId, Name = WellKnownRoles.Administrator },
                new Role { RoleId = WellKnownRoles.HrId, Name = WellKnownRoles.Hr },
                new Role { RoleId = WellKnownRoles.ManagerId, Name = WellKnownRoles.Manager },
                new Role { RoleId = WellKnownRoles.EmployeeId, Name = WellKnownRoles.Employee }
            );

            builder.Entity<User>().HasData(
                new User
                {
                    UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Email = "admin@vacationplanner.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345"),
                    FirstName = "System",
                    LastName = "Administrator",
                    RegistrationDate = DateTime.MinValue,
                    IsActive = true,
                    RoleId = WellKnownRoles.AdministratorId
                }
            );
        }
    }
}
