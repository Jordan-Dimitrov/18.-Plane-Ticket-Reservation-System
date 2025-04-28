using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyFly.Persistence.Configurations
{
    internal class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder
                .Property<byte[]>("Version")
                .IsRowVersion();

            builder
                .HasMany(s => s.Tickets)
                .WithOne(t => t.Seat)
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}