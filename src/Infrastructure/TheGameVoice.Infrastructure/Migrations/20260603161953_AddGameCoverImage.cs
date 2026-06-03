using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games");

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
                name: "fk_games_media_cover_image_id",
                table: "games",
                column: "cover_image_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_media_media_cover_image_id",
                table: "media",
                column: "cover_image_id",
                principalTable: "media",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "fk_media_media_cover_image_id",
                table: "media");

            migrationBuilder.DropIndex(
                name: "ix_media_cover_image_id",
                table: "media");

            migrationBuilder.DropColumn(
                name: "cover_image_id",
                table: "media");

            migrationBuilder.AddForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games",
                column: "cover_image_id",
                principalTable: "media",
                principalColumn: "id");
        }
    }
}
