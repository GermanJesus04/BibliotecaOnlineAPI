using BibliotecaOnlineApi.Model.Modelo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaOnlineApi.Infraestructura.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }


    }
}
