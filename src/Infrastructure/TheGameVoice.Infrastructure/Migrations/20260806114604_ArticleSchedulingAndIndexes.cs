using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ArticleSchedulingAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_articles_media_featured_image_id",
                table: "articles");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "articles",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "articles",
                type: "character varying(350)",
                maxLength: 350,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified_at",
                table: "articles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "scheduled_by_id",
                table: "articles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "scheduled_publish_at",
                table: "articles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_articles_author_id",
                table: "articles",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_articles_published_at",
                table: "articles",
                column: "published_at");

            migrationBuilder.CreateIndex(
                name: "ix_articles_slug",
                table: "articles",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_articles_status",
                table: "articles",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_articles_status_scheduled_publish_at",
                table: "articles",
                columns: new[] { "status", "scheduled_publish_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_articles_media_featured_image_id",
                table: "articles",
                column: "featured_image_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_articles_media_featured_image_id",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "ix_articles_author_id",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "ix_articles_published_at",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "ix_articles_slug",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "ix_articles_status",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "ix_articles_status_scheduled_publish_at",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "last_modified_at",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "scheduled_by_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "scheduled_publish_at",
                table: "articles");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "articles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "slug",
                table: "articles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(350)",
                oldMaxLength: 350);

            migrationBuilder.AddForeignKey(
                name: "fk_articles_media_featured_image_id",
                table: "articles",
                column: "featured_image_id",
                principalTable: "media",
                principalColumn: "id");
        }
    }
}
