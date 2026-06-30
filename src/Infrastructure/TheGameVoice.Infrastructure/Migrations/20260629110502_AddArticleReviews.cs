using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_review",
                table: "articles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "review_score",
                table: "articles",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "review_summary",
                table: "articles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "review_verdict",
                table: "articles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "article_review_points",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    article_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("pk_article_review_points", x => x.id);
                    table.ForeignKey(
                        name: "fk_article_review_points_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_article_review_points_article_id",
                table: "article_review_points",
                column: "article_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_review_points");

            migrationBuilder.DropColumn(
                name: "is_review",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "review_score",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "review_summary",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "review_verdict",
                table: "articles");
        }
    }
}
