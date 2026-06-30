using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheGameVoice.Domain.Entities;
using TheGameVoice.Infrastructure.Identity.Entities;
using TheGameVoice.Infrastructure.Persistence.Extensions;

namespace TheGameVoice.Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();

    public DbSet<Game> Games => Set<Game>();

    public DbSet<Review> Reviews => Set<Review>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();
    public DbSet<Media> Media => Set<Media>();
    public DbSet<ArticleReviewPoint> ArticleReviewPoints
    => Set<ArticleReviewPoint>();
    public DbSet<ArticleGame> ArticleGames => Set<ArticleGame>();
    public DbSet<ArticleMedia> ArticleMedia
        => Set<ArticleMedia>();

    public DbSet<ArticleVideo> ArticleVideos
        => Set<ArticleVideo>();
    public DbSet<ArticleView>
    ArticleViews
    {
        get;
        set;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
        builder.Entity<ArticleTag>()
            .HasKey(x =>
                new
                {
                    x.ArticleId,
                    x.TagId
                });

        builder.Entity<ArticleTag>()
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleTags)
            .HasForeignKey(x => x.ArticleId);

        builder.Entity<ArticleTag>()
            .HasOne(x => x.Tag)
            .WithMany(x => x.ArticleTags)
            .HasForeignKey(x => x.TagId);
        builder.ApplySoftDeleteQueryFilters();

        builder.Entity<ArticleGame>()
    .HasKey(x =>
        new
        {
            x.ArticleId,
            x.GameId
        });

        builder.Entity<ArticleGame>()
            .HasOne(x => x.Article)
            .WithMany(x => x.ArticleGames)
            .HasForeignKey(x => x.ArticleId);

        builder.Entity<ArticleGame>()
            .HasOne(x => x.Game)
            .WithMany(x => x.ArticleGames)
            .HasForeignKey(x => x.GameId);
    }

    public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is Domain.Common.Interfaces.IAuditableEntity
                && (e.State == EntityState.Added
                || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity =
                (Domain.Common.Interfaces.IAuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }

            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}