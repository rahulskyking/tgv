using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    public partial class AddCategoryNavigationVisibility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing navigation remains visible after upgrade.
            migrationBuilder.AddColumn<bool>(
                name: "show_in_navigation",
                table: "categories",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "show_in_navigation", table: "categories");
        }
    }
}
