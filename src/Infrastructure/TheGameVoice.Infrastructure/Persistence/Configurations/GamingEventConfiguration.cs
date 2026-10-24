using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheGameVoice.Domain.Entities;

namespace TheGameVoice.Infrastructure.Persistence.Configurations;

public class GamingEventConfiguration : IEntityTypeConfiguration<GamingEvent>
{
    public void Configure(EntityTypeBuilder<GamingEvent> builder)
    {
        builder.ToTable("events");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(5000);
        builder.Property(x => x.BannerAltText).HasMaxLength(250);
        builder.Property(x => x.Location).HasMaxLength(250);
        builder.Property(x => x.OfficialWebsite).HasMaxLength(1000);
        builder.Property(x => x.StartDate).HasColumnType("date");
        builder.Property(x => x.EndDate).HasColumnType("date");
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => new { x.IsActive, x.ShowInNavigation, x.DisplayOrder });
        builder.HasOne(x => x.BannerImage).WithMany()
            .HasForeignKey(x => x.BannerImageId).OnDelete(DeleteBehavior.SetNull);
    }
}
