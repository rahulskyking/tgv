using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GamecsModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games");

            migrationBuilder.AlterColumn<string>(
                name: "summary",
                table: "games",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "banner_image_id",
                table: "games",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "data_source",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "games",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "developer",
                table: "games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "genres",
                table: "games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_steam_sync_at",
                table: "games",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "official_website",
                table: "games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "platforms",
                table: "games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "publisher",
                table: "games",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "steam_app_id",
                table: "games",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "steam_url",
                table: "games",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_games_banner_image_id",
                table: "games",
                column: "banner_image_id");

            migrationBuilder.AddForeignKey(
                name: "fk_games_media_banner_image_id",
                table: "games",
                column: "banner_image_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games",
                column: "cover_image_id",
                principalTable: "media",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_media_banner_image_id",
                table: "games");

            migrationBuilder.DropForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games");

            migrationBuilder.DropIndex(
                name: "ix_games_banner_image_id",
                table: "games");

            migrationBuilder.DropColumn(
                name: "banner_image_id",
                table: "games");

            migrationBuilder.DropColumn(
                name: "data_source",
                table: "games");

            migrationBuilder.DropColumn(
                name: "description",
                table: "games");

            migrationBuilder.DropColumn(
                name: "developer",
                table: "games");

            migrationBuilder.DropColumn(
                name: "genres",
                table: "games");

            migrationBuilder.DropColumn(
                name: "last_steam_sync_at",
                table: "games");

            migrationBuilder.DropColumn(
                name: "official_website",
                table: "games");

            migrationBuilder.DropColumn(
                name: "platforms",
                table: "games");

            migrationBuilder.DropColumn(
                name: "publisher",
                table: "games");

            migrationBuilder.DropColumn(
                name: "steam_app_id",
                table: "games");

            migrationBuilder.DropColumn(
                name: "steam_url",
                table: "games");

            migrationBuilder.AlterColumn<string>(
                name: "summary",
                table: "games",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games",
                column: "cover_image_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
