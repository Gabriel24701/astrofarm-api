using AstroFarm.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AstroFarm.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Produtor> Produtores { get; set; }
        public DbSet<Propriedade> Propriedades { get; set; }
        // Adicione as linhas abaixo
        public DbSet<LeituraSatelital> Leituras { get; set; }
        public DbSet<Alerta> Alertas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Propriedade>()
                .HasOne(p => p.Produtor)
                .WithMany(p => p.Propriedades)
                .HasForeignKey(p => p.ProdutorId);
            
        }
    }
}