using Microsoft.EntityFrameworkCore;
using SocioManagerV2.Models;
using System.IO;

namespace SocioManagerV2.Data
{
    public class SociosContext : DbContext
    {
        public DbSet<Socio> Socios { get; set; }
        public DbSet<Vetado> Vetados { get; set; }
        public DbSet<BoardGame> BoardGames { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SocioManager",
                "socios.db"
            );

            var directory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            var connectionString = $"Data Source={dbPath};Mode=ReadWriteCreate;Cache=Shared;Pooling=False";
            options.UseSqlite(connectionString, sqliteOptions => 
            {
                sqliteOptions.CommandTimeout(30);
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Socio>().ToTable("socios");

            modelBuilder.Entity<Socio>()
                .Property(s => s.NumeroDeSocio)
                .HasColumnName("numero_de_socio");

            modelBuilder.Entity<Socio>()
                .Property(s => s.NumeroDeTelefono)
                .HasColumnName("numero_de_telefono");

            modelBuilder.Entity<Socio>()
                .Property(s => s.CorreoElectronico)
                .HasColumnName("correo_electronico");

            modelBuilder.Entity<Socio>()
                .Property(s => s.DireccionPostal)
                .HasColumnName("direccion_postal");

            modelBuilder.Entity<Socio>()
                .Property(s => s.FechaNacimiento)
                .HasColumnName("fecha_nacimiento");

            modelBuilder.Entity<Socio>()
                .Property(s => s.FechaDeAlta)
                .HasColumnName("fecha_de_alta");

            modelBuilder.Entity<Socio>()
                .Property(s => s.FechaDeBaja)
                .HasColumnName("fecha_de_baja");

            modelBuilder.Entity<Vetado>().ToTable("vetados");

            modelBuilder.Entity<Vetado>()
                .Property(v => v.id)
                .HasColumnName("id");

            modelBuilder.Entity<Vetado>()
                .Property(v => v.dni)
                .HasColumnName("dni");

            modelBuilder.Entity<Vetado>()
                .Property(v => v.fecha)
                .HasColumnName("fecha");

            modelBuilder.Entity<Vetado>()
                .Property(v => v.motivo)
                .HasColumnName("motivo");

            modelBuilder.Entity<Usuario>().ToTable("usuarios");

                modelBuilder.Entity<Usuario>()
                    .Property(u => u.Id)
                    .HasColumnName("id");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Username)
                .HasColumnName("username");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.email)
                .HasColumnName("email");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.nombre)
                .HasColumnName("nombre");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.apellidos)
                .HasColumnName("apellidos");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.passwordHash)
                .HasColumnName("password_hash");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.isAdmin)
                .HasColumnName("esAdmin");

            modelBuilder.Entity<BoardGame>().ToTable("board_games");

            modelBuilder.Entity<BoardGame>()
                .Property(b => b.Id)
                .HasColumnName("id");

            modelBuilder.Entity<BoardGame>()
                .Property(b => b.Name)
                .HasColumnName("name");

            modelBuilder.Entity<BoardGame>()
                .Property(b => b.OwnerId)
                .HasColumnName("owner_id");

            modelBuilder.Entity<BoardGame>()
                .HasOne(b => b.Owner)
                .WithMany()
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
