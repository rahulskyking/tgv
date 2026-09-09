using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    /// <summary>
    /// Adds the writer's Steam profile link to the user table, so it can be
    /// edited from both the admin Users screen and the self-service profile.
    /// </summary>
    public partial class AddSteamUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "steam_url",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "steam_url",
                table: "AspNetUsers");
        }
    }
}
