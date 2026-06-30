using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleReviewPointConfiguration
    : IEntityTypeConfiguration<ArticleReviewPoint>
{
    public void Configure(
        EntityTypeBuilder<ArticleReviewPoint> builder)
    {
        builder.ToTable("article_review_points");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Text)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.HasOne(x => x.Article)
            .WithMany(x => x.ReviewPoints)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.ArticleId,
            x.Type,
            x.DisplayOrder
        });
    }
}