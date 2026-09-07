using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(300)
            .IsRequired();

        // Existing games are PC / Console titles.
        builder.Property(x => x.Segment)
            .HasDefaultValue(GameSegment.PcConsole);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.Segment);
        builder
        .HasOne(x => x.BannerImage)
        .WithMany()
        .HasForeignKey(x => x.BannerImageId)
        .OnDelete(DeleteBehavior.SetNull);
    }
}