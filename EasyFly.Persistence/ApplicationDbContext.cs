using EasyFly.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyFly.Persistence
{
    public sealed class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Plane> Planes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            modelBuilder.Entity<Airport>().HasQueryFilter(x => EF.Property<DateTime>(x, "DeletedAt") == null);
            modelBuilder.Entity<Flight>().HasQueryFilter(x => EF.Property<DateTime>(x, "DeletedAt") == null);
            modelBuilder.Entity<Plane>().HasQueryFilter(x => EF.Property<DateTime>(x, "DeletedAt") == null);
            modelBuilder.Entity<Seat>().HasQueryFilter(x => EF.Property<DateTime>(x, "DeletedAt") == null);
            modelBuilder.Entity<Ticket>().HasQueryFilter(x => EF.Property<DateTime>(x, "DeletedAt") == null);
            modelBuilder.Entity<Audit>().HasQueryFilter(x => EF.Property<DateTime>(x, "DeletedAt") == null);
            modelBuilder.Entity<User>().HasQueryFilter(x => EF.Property<DateTime>(x, "DeletedAt") == null);
        }
    }
}
