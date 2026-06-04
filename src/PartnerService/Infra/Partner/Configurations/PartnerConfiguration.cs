
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnerService.Infra.Shared.Models;

namespace PartnerService.Infra.Partner.Persistence;

public class PartnerConfiguration : IEntityTypeConfiguration<PartnerDbModel>
{
    public void Configure(EntityTypeBuilder<PartnerDbModel> builder)
    {
        builder.ToTable("Partners");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TradingName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.OwnerName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Document)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(p => p.Document)
            .IsUnique();
    }
}