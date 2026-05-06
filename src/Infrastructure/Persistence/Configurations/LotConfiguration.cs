using Domain.Lots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class LotConfiguration : IEntityTypeConfiguration<Lot>
{
    public void Configure(EntityTypeBuilder<Lot> builder)
    {
        builder.ToTable("lots");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(5000);

        builder.Property(x => x.StartingPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.MinBidStep)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.CurrentPrice)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Version)
            .IsConcurrencyToken();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.StartsAt).IsRequired();
        builder.Property(x => x.EndsAt).IsRequired();

        builder.HasIndex(x => x.SellerId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.EndsAt);
    }
}