using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.EF.Migrations
{
    public partial class Add_Payment_in_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentDescription",
                table: "Orders",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentName",
                table: "Orders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentParameters",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDescription",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentParameters",
                table: "Orders");
        }
    }
}
