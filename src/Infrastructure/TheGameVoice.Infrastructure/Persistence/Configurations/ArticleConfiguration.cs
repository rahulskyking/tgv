using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

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

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.CategoryId);

        builder.HasOne(x => x.FeaturedImage)
            .WithMany()
            .HasForeignKey(x => x.FeaturedImageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}