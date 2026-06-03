using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleVideoConfiguration
    : IEntityTypeConfiguration<ArticleVideo>
{
    public void Configure(
        EntityTypeBuilder<ArticleVideo> builder)
    {
        builder.ToTable("article_videos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.VideoUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleVideos)
            .HasForeignKey(x => x.ArticleId);
    }
}