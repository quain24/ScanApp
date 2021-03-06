using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScanApp.Domain.Entities;

namespace ScanApp.Infrastructure.Persistence.Configurations
{
    public class GateConfiguration : VersionedEntityConfiguration<Gate>
    {
        public override void Configure(EntityTypeBuilder<Gate> builder)
        {
            builder.ToTable("Gates", "hub");
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Number).IsUnique();

            base.Configure(builder);
        }
    }
}