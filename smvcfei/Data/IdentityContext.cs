//Revisado
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using smvcfei.Data.Seed;
using smvcfei.Models;

namespace smvcfei.Data
{
    public class IdentityContext : IdentityDbContext<CustomIdentityUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext>options) : base(options) { }

        public DbSet<CustomIdentityUser> CustomIdentities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.SeedUserIdentityData();
        }
    }
}
