using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheGameVoice.Infrastructure.Migrations
{
    public partial class AddHomepagePins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing articles remain unpinned; the current homepage order is preserved.
            migrationBuilder.AddColumn<bool>(
                name: "is_homepage_pinned",
                table: "articles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "homepage_pin_order",
                table: "articles",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "homepage_pin_order", table: "articles");
            migrationBuilder.DropColumn(name: "is_homepage_pinned", table: "articles");
        }
    }
}
