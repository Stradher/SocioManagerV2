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
        public DbSet<BoardGameLoan> BoardGameLoans { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomBooking> RoomBookings { get; set; }
        public DbSet<FinanceTransaction> FinanceTransactions { get; set; }
        public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

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

            modelBuilder.Entity<BoardGameLoan>().ToTable("board_game_loans");

            modelBuilder.Entity<BoardGameLoan>()
                .Property(l => l.Id)
                .HasColumnName("id");

            modelBuilder.Entity<BoardGameLoan>()
                .Property(l => l.BoardGameId)
                .HasColumnName("board_game_id");

            modelBuilder.Entity<BoardGameLoan>()
                .Property(l => l.BorrowerId)
                .HasColumnName("borrower_id");

            modelBuilder.Entity<BoardGameLoan>()
                .Property(l => l.LoanDate)
                .HasColumnName("loan_date");

            modelBuilder.Entity<BoardGameLoan>()
                .Property(l => l.ReturnDate)
                .HasColumnName("return_date");

            modelBuilder.Entity<BoardGameLoan>()
                .HasOne(l => l.BoardGame)
                .WithMany()
                .HasForeignKey(l => l.BoardGameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardGameLoan>()
                .HasOne(l => l.Borrower)
                .WithMany()
                .HasForeignKey(l => l.BorrowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>().ToTable("rooms");

            modelBuilder.Entity<Room>()
                .Property(r => r.Id)
                .HasColumnName("id");

            modelBuilder.Entity<Room>()
                .Property(r => r.Name)
                .HasColumnName("name");

            modelBuilder.Entity<Room>()
                .Property(r => r.MaxCapacity)
                .HasColumnName("max_capacity");

            modelBuilder.Entity<RoomBooking>().ToTable("room_bookings");

            modelBuilder.Entity<RoomBooking>()
                .Property(b => b.Id)
                .HasColumnName("id");

            modelBuilder.Entity<RoomBooking>()
                .Property(b => b.RoomId)
                .HasColumnName("room_id");

            modelBuilder.Entity<RoomBooking>()
                .Property(b => b.BookerId)
                .HasColumnName("booker_id");

            modelBuilder.Entity<RoomBooking>()
                .Property(b => b.BookingDate)
                .HasColumnName("booking_date");

            modelBuilder.Entity<RoomBooking>()
                .Property(b => b.DurationMinutes)
                .HasColumnName("duration_minutes");

            modelBuilder.Entity<RoomBooking>()
                .HasOne(b => b.Room)
                .WithMany()
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomBooking>()
                .HasOne(b => b.Booker)
                .WithMany()
                .HasForeignKey(b => b.BookerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FinanceTransaction>().ToTable("finance_transactions");

            modelBuilder.Entity<FinanceTransaction>()
                .Property(t => t.Id).HasColumnName("id");
            modelBuilder.Entity<FinanceTransaction>()
                .Property(t => t.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");
            modelBuilder.Entity<FinanceTransaction>()
                .Property(t => t.Date).HasColumnName("date");
            modelBuilder.Entity<FinanceTransaction>()
                .Property(t => t.Concept).HasColumnName("concept");
            modelBuilder.Entity<FinanceTransaction>()
                .Property(t => t.Type).HasColumnName("type");
            modelBuilder.Entity<FinanceTransaction>()
                .Property(t => t.Category).HasColumnName("category");

            modelBuilder.Entity<SubscriptionPayment>().ToTable("subscription_payments");

            modelBuilder.Entity<SubscriptionPayment>()
                .Property(s => s.Id).HasColumnName("id");
            modelBuilder.Entity<SubscriptionPayment>()
                .Property(s => s.SocioId).HasColumnName("socio_id");
            modelBuilder.Entity<SubscriptionPayment>()
                .Property(s => s.MonthYear).HasColumnName("month_year");
            modelBuilder.Entity<SubscriptionPayment>()
                .Property(s => s.PaymentDate).HasColumnName("payment_date");
            modelBuilder.Entity<SubscriptionPayment>()
                .Property(s => s.Amount).HasColumnName("amount").HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SubscriptionPayment>()
                .HasOne(s => s.Socio)
                .WithMany()
                .HasForeignKey(s => s.SocioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
