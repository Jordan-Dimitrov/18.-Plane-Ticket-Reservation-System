using EasyFly.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyFly.Persistence.Configurations
{
    internal class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder
                .Property<byte[]>("Version");

            builder.
                Property("Version")
                .IsRowVersion();
        }
    }
}
