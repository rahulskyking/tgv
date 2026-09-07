using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Domain.Enums;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleConfiguration
    : IEntityTypeConfiguration<Article>
{
    public void Configure(
        EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(350)
            .IsRequired();

        // Audience segment (PC/Console, Mobile or both). Stored as the
        // integer bit flag; existing rows were backfilled to PC/Console.
        builder.Property(x => x.Segment)
            .HasDefaultValue(GameSegment.PcConsole);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.AuthorId);

        builder.HasIndex(x => x.CategoryId);

        builder.HasIndex(x => x.PublishedAt);

        builder.HasIndex(x => new
        {
            x.Status,
            x.ScheduledPublishAt
        });

        // Supports "most read" dashboards and the public most-read widget:
        // WHERE status = Published ORDER BY view_count DESC LIMIT n
        builder.HasIndex(x => new
        {
            x.Status,
            x.ViewCount
        });

        // Every public query is now "published articles of the current
        // site mode, newest first".
        builder.HasIndex(x => new
        {
            x.Segment,
            x.Status,
            x.PublishedAt
        });

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(x => x.FeaturedImage)
            .WithMany()
            .HasForeignKey(x => x.FeaturedImageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}