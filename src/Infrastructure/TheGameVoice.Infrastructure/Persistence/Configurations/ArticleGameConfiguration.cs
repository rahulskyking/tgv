using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleGameConfiguration
    : IEntityTypeConfiguration<ArticleGame>
{
    public void Configure(
        EntityTypeBuilder<ArticleGame> builder)
    {
        builder.HasKey(x =>
            new
            {
                x.ArticleId,
                x.GameId
            });

        builder
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleGames)
            .HasForeignKey(x => x.ArticleId);

        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.ArticleGames)
            .HasForeignKey(x => x.GameId);
    }
}