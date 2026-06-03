using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleMediaConfiguration
    : IEntityTypeConfiguration<ArticleMedia>
{
    public void Configure(
        EntityTypeBuilder<ArticleMedia> builder)
    {
        builder.HasKey(x =>
            new
            {
                x.ArticleId,
                x.MediaId
            });

        builder
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleMedia)
            .HasForeignKey(x => x.ArticleId);

        builder
            .HasOne(x => x.Media)
            .WithMany(x => x.ArticleMedia)
            .HasForeignKey(x => x.MediaId);

        builder.Property(x => x.SortOrder);
    }
}