using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.EF.Migrations
{
    public partial class Add_CategoryUrlSlug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlSlug",
                table: "Categories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlSlug",
                table: "Categories");
        }
    }
}
