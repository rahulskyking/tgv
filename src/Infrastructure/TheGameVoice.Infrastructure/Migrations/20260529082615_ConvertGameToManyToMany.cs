using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertGameToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_articles_games_game_id",
                table: "articles");

            migrationBuilder.DropIndex(
                name: "ix_articles_game_id",
                table: "articles");

            migrationBuilder.DropColumn(
                name: "game_id",
                table: "articles");

            migrationBuilder.CreateTable(
                name: "article_games",
                columns: table => new
                {
                    article_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_article_games", x => new { x.article_id, x.game_id });
                    table.ForeignKey(
                        name: "fk_article_games_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_article_games_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_article_games_game_id",
                table: "article_games",
                column: "game_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_games");

            migrationBuilder.AddColumn<Guid>(
                name: "game_id",
                table: "articles",
                type: "uuid",
                nullable: true);

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
        }
    }
}
