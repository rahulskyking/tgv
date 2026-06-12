using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleViewCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_media_media_cover_image_id",
                table: "media");

            migrationBuilder.DropIndex(
                name: "ix_media_cover_image_id",
                table: "media");

            migrationBuilder.DropColumn(
                name: "cover_image_id",
                table: "media");

            migrationBuilder.AddColumn<Guid>(
                name: "avatar_image_id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "twitter_url",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "website_url",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "you_tube_url",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "view_count",
                table: "articles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_avatar_image_id",
                table: "AspNetUsers",
                column: "avatar_image_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_media_avatar_image_id",
                table: "AspNetUsers",
                column: "avatar_image_id",
                principalTable: "media",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_media_avatar_image_id",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_avatar_image_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "avatar_image_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "twitter_url",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "website_url",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "you_tube_url",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "view_count",
                table: "articles");

            migrationBuilder.AddColumn<Guid>(
                name: "cover_image_id",
                table: "media",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_media_cover_image_id",
                table: "media",
                column: "cover_image_id");

            migrationBuilder.AddForeignKey(
                name: "fk_media_media_cover_image_id",
                table: "media",
                column: "cover_image_id",
                principalTable: "media",
                principalColumn: "id");
        }
    }
}
