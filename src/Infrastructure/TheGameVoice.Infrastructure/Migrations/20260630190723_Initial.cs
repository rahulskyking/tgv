using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_article_review_points_article_id",
                table: "article_review_points");

            migrationBuilder.AlterColumn<string>(
                name: "text",
                table: "article_review_points",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_article_review_points_article_id_type_display_order",
                table: "article_review_points",
                columns: new[] { "article_id", "type", "display_order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_article_review_points_article_id_type_display_order",
                table: "article_review_points");

            migrationBuilder.AlterColumn<string>(
                name: "text",
                table: "article_review_points",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "ix_article_review_points_article_id",
                table: "article_review_points",
                column: "article_id");
        }
    }
}
