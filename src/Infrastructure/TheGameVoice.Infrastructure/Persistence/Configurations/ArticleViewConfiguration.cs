using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleViewConfiguration
    : IEntityTypeConfiguration<ArticleView>
{
    public void Configure(
        EntityTypeBuilder<ArticleView> builder)
    {
        builder.ToTable("article_views");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Article)
            .WithMany(x => x.ArticleViews)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ViewedAt)
            .IsRequired();
    }
}