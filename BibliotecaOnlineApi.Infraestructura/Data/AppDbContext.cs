using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaOnlineApi.Infraestructura.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de índices
            modelBuilder.Entity<Libro>()
                .HasIndex(l => new { l.Titulo, l.Autor });
        }

    }
}
