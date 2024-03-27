using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MinhaPrimeiraAPI.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }
        public DbSet<IdentityUser> IdentityUser { get; set; }
        public DbSet<IdentityRole> IdentityRole { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRole { get; set; }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaim { get; set; }
        public DbSet<IdentityRoleClaim<string>> IdentityRoleClaim { get; set; }
        public DbSet<IdentityUserLogin<string>> IdentityUserLogin { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>().HasKey(r => r.Id);
            modelBuilder.Entity<IdentityRole>().HasKey(r => r.Id);
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(iur => new { iur.UserId, iur.RoleId });
            modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(iuc => iuc.Id);
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasKey(iurc => iurc.Id);
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(iul => new { iul.LoginProvider, iul.ProviderKey });
        }
    }
}

