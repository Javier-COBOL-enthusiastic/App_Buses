//Pedí ayuda a una IA para esto, ya que no entendía muy bien, y pues aún falta mejorarlo, pues no funciona en este momento tal y como esta

using Microsoft.EntityFrameworkCore;
using BusTrackSV.Models; // Importa el namespace donde están tus modelos

namespace BusTrackSV.Data
{
    public class BusTrackSvDbContext : DbContext
    {
        // Constructor requerido para inyección de dependencias
        public BusTrackSvDbContext(DbContextOptions<BusTrackSvDbContext> options)
            : base(options)
        {
        }

        // DbSets representan las tablas de la base de datos
        public DbSet<Usuario> Usuarios { get; set; } = default!;
        public DbSet<Bus> Buses { get; set; } = default!;
        public DbSet<Ruta> Rutas { get; set; } = default!;
        public DbSet<Conductor> Conductores { get; set; } = default!;

        // Opcional: Configuraciones adicionales para el mapeo (ej. nombres de tablas)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ejemplo: configurar la clave principal de Usuario
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
