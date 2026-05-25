using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("articles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(400)
            .IsRequired();

        builder.Property(x => x.Summary)
            .HasMaxLength(1000);

        builder.Property(x => x.SeoTitle)
            .HasMaxLength(300);

        builder.Property(x => x.SeoDescription)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.PublishedAt);

        builder.HasOne(x => x.Category)
    .WithMany()
    .HasForeignKey(x => x.CategoryId)
    .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.FeaturedImage)
            .WithMany()
            .HasForeignKey(x => x.FeaturedImageId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}