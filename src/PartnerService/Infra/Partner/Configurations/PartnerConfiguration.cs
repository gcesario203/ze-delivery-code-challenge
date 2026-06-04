
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnerService.Domain.Partner.Entities;

namespace PartnerService.Infra.Partner.Persistence;

public class PartnerConfiguration : IEntityTypeConfiguration<PartnerEntity>
{
    public void Configure(EntityTypeBuilder<PartnerEntity> builder)
    {
        builder.ToTable("Partners");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TradingName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.OwnerName)
            .IsRequired()
            .HasMaxLength(255);

        builder.OwnsOne(p => p.Document, d =>
        {
            d.Property(doc => doc.Value)
                .IsRequired()
                .HasMaxLength(14)
                .HasColumnName("Document");
        });
    }
}