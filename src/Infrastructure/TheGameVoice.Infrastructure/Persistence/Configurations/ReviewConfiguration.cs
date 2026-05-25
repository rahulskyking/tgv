using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(400)
            .IsRequired();

        builder.Property(x => x.Score)
            .HasPrecision(3, 1);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasIndex(x => x.GameId);

        builder.HasOne(x => x.Game)
    .WithMany()
    .HasForeignKey(x => x.GameId)
    .OnDelete(DeleteBehavior.Cascade);
    }
}