using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <summary>
    /// Adds the audience segment (PC / Console vs Mobile gaming) to articles,
    /// categories and games.
    ///
    /// Stored as an integer bit flag: 1 = PC/Console, 2 = Mobile, 3 = both.
    ///
    /// Backfill applied by the column defaults:
    ///   * articles   -> 1 (all existing editorial content is PC / console)
    ///   * games      -> 1
    ///   * categories -> 3 (stay visible in both site modes)
    /// </summary>
    public partial class AddGameSegment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "segment",
                table: "articles",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "segment",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "segment",
                table: "categories",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.CreateIndex(
                name: "ix_articles_segment_status_published_at",
                table: "articles",
                columns: new[] { "segment", "status", "published_at" });

            migrationBuilder.CreateIndex(
                name: "ix_games_segment",
                table: "games",
                column: "segment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_games_segment",
                table: "games");

            migrationBuilder.DropIndex(
                name: "ix_articles_segment_status_published_at",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "segment",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "segment",
                table: "games");

            migrationBuilder.DropColumn(
                name: "segment",
                table: "articles");
        }
    }
}
