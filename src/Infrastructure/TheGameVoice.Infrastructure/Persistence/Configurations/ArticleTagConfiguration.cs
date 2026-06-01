using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleTagConfiguration
    : IEntityTypeConfiguration<ArticleTag>
{
    public void Configure(
        EntityTypeBuilder<ArticleTag> builder)
    {
        builder.HasKey(x =>
            new
            {
                x.ArticleId,
                x.TagId
            });

        builder
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleTags)
            .HasForeignKey(x => x.ArticleId);

        builder
            .HasOne(x => x.Tag)
            .WithMany(x => x.ArticleTags)
            .HasForeignKey(x => x.TagId);
    }
}