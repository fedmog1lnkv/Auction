using Domain.Lots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class LotPhotoConfiguration : IEntityTypeConfiguration<LotPhoto>
{
    public void Configure(EntityTypeBuilder<LotPhoto> builder)
    {
        builder.ToTable("lot_photos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ThumbKey)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.MediumKey)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.LargeKey)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasIndex(x => x.LotId);

        builder.HasIndex(x => new { x.LotId, x.SortOrder })
            .IsUnique();

        builder.HasOne<Lot>()
            .WithMany()
            .HasForeignKey(x => x.LotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}