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
                .Property<byte[]>("Version");

            builder.
                Property("Version")
                .IsRowVersion();
        }
    }
}
