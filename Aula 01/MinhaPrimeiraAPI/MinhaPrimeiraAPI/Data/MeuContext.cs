using Microsoft.EntityFrameworkCore;

namespace MinhaPrimeiraAPI.Data
{
    public class MeuContext : DbContext
    {
        public MeuContext(DbContextOptions<MeuContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
