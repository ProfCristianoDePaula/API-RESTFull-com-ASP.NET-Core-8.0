using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MinhaPrimeiraAPI.Data
{
    public class MeuContext : DbContext
    {
        public MeuContext(DbContextOptions<MeuContext> options) : base(options)
        {
        }

        public DbSet<IdentityUser> IdentityUser { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
