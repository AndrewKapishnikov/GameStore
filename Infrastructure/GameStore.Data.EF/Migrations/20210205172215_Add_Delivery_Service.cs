using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.EF.Migrations
{
    public partial class Add_Delivery_Service : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryDescription",
                table: "Orders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryName",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryParameters",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryPrice",
                table: "Orders",
                type: "decimal(15,2)",
                precision: 15,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryDescription",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryParameters",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPrice",
                table: "Orders");
        }
    }
}
