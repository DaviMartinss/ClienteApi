using Microsoft.EntityFrameworkCore;
using ClienteApi.Models;

namespace ClienteApi.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais, caso necessário
            // Com o uso da convenção snake_case, você pode evitar configurações manuais
            modelBuilder.Entity<Cliente>();
        }
    }
}