using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    public partial class AddEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    banner_image_id = table.Column<Guid>(type: "uuid", nullable: true),
                    banner_alt_text = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    location = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    official_website = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    show_in_navigation = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                    table.ForeignKey("fk_events_media_banner_image_id", x => x.banner_image_id,
                        "media", "id", onDelete: ReferentialAction.SetNull);
                });

            // Nullable: existing articles retain their categories and need no event.
            migrationBuilder.AddColumn<Guid>("event_id", "articles", type: "uuid", nullable: true);
            migrationBuilder.CreateIndex("ix_events_slug", "events", "slug", unique: true);
            migrationBuilder.CreateIndex("ix_events_banner_image_id", "events", "banner_image_id");
            migrationBuilder.CreateIndex("ix_events_is_active_show_in_navigation_display_order", "events",
                new[] { "is_active", "show_in_navigation", "display_order" });
            migrationBuilder.CreateIndex("ix_articles_event_id_status_published_at", "articles",
                new[] { "event_id", "status", "published_at" });
            migrationBuilder.AddForeignKey("fk_articles_events_event_id", "articles", "event_id",
                "events", principalColumn: "id", onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("fk_articles_events_event_id", "articles");
            migrationBuilder.DropIndex("ix_articles_event_id_status_published_at", "articles");
            migrationBuilder.DropColumn("event_id", "articles");
            migrationBuilder.DropTable("events");
        }
    }
}
