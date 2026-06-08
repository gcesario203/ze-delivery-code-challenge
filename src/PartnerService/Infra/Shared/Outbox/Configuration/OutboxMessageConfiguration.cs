

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PartnerService.Infra.Shared.Outbox.Models;

namespace PartnerService.Infra.Shared.Configuration.Outbox;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxDBModel>
{
    public void Configure(EntityTypeBuilder<OutboxDBModel> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}