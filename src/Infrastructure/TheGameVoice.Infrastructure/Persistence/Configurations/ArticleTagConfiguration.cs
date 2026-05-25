using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleTagConfiguration
    : IEntityTypeConfiguration<ArticleTag>
{
    public void Configure(EntityTypeBuilder<ArticleTag> builder)
    {
        builder.ToTable("article_tags");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.ArticleId,
            x.TagId
        }).IsUnique();
        builder.HasOne(x => x.Article)
            .WithMany(x => x.ArticleTags)
            .HasForeignKey(x => x.ArticleId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
        .WithMany(x => x.ArticleTags)
        .HasForeignKey(x => x.TagId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade);
    }
}