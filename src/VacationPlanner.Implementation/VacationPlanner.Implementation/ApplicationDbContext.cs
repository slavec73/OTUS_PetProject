using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Models;

namespace VacationPlanner.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Position> Positions { get; set; }
        public DbSet<GlobalVacationSetting> GlobalVacationSettings { get; set; }
        public DbSet<PositionVacationSetting> PositionVacationSettings { get; set; }

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
        }
    }
}
