using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.EF.Migrations
{
    public partial class Add_OrderReviewed_For_OrderDTO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OrderReviewed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderReviewed",
                table: "Orders");
        }
    }
}
