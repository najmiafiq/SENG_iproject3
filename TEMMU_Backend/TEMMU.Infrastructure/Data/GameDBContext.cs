using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TEMMU.Core.Entities;

namespace TEMMU.Infrastructure.Data
{

    // CustomUser is needed for IdentityDBContext
    public class ApplicationUser : IdentityUser { }
    
    public class GameDBContext : IdentityDbContext<ApplicationUser>
    {
        public GameDBContext(DbContextOptions<GameDBContext> options)
            : base(options)
        {
        }

        public DbSet<Core.Entities.FighterCharacter> Fighters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional configurations can be added here if needed

            // seed initial data
            modelBuilder.Entity<Core.Entities.FighterCharacter>().HasData(
                new FighterCharacter { Id = 1, name = "Kai", style = "Karate", healthBase = 1000, attackMultiplier = 1.2, defenseMultiplier = 0.9, speed = 8, matchesPlayed = 50, wins = 45 },
                new FighterCharacter { Id = 2, name = "Lana", style = "Judo", healthBase = 950, attackMultiplier = 1.0, defenseMultiplier = 1.1, speed = 6, matchesPlayed = 50, wins = 20 }
            );
        }
    }
}
