using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleMediaAndVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "article_media",
                columns: table => new
                {
                    article_id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_article_media", x => new { x.article_id, x.media_id });
                    table.ForeignKey(
                        name: "fk_article_media_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_article_media_media_media_id",
                        column: x => x.media_id,
                        principalTable: "media",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "article_videos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    article_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    video_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_article_videos", x => x.id);
                    table.ForeignKey(
                        name: "fk_article_videos_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_article_media_media_id",
                table: "article_media",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_article_videos_article_id",
                table: "article_videos",
                column: "article_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_media");

            migrationBuilder.DropTable(
                name: "article_videos");
        }
    }
}
