using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "games");

            migrationBuilder.DropColumn(
                name: "developer",
                table: "games");

            migrationBuilder.RenameColumn(
                name: "publisher",
                table: "games",
                newName: "summary");

            migrationBuilder.AddColumn<Guid>(
                name: "cover_image_id",
                table: "games",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "game_id",
                table: "articles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_games_cover_image_id",
                table: "games",
                column: "cover_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_articles_game_id",
                table: "articles",
                column: "game_id");

            migrationBuilder.AddForeignKey(
                name: "fk_articles_games_game_id",
                table: "articles",
                column: "game_id",
                principalTable: "games",
                principalColumn: "id");

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
                name: "fk_articles_games_game_id",
                table: "articles");

            migrationBuilder.DropForeignKey(
                name: "fk_games_media_cover_image_id",
                table: "games");

            migrationBuilder.DropIndex(
                name: "ix_games_cover_image_id",
                table: "games");

            migrationBuilder.DropIndex(
                name: "ix_articles_game_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "cover_image_id",
                table: "games");

            migrationBuilder.DropColumn(
                name: "game_id",
                table: "articles");

            migrationBuilder.RenameColumn(
                name: "summary",
                table: "games",
                newName: "publisher");

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
        }
    }
}
