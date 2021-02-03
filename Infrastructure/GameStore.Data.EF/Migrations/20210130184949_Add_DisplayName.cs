using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.EF.Migrations
{
    public partial class Add_DisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OrderItems",
                type: "decimal(15,2)",
                precision: 15,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldPrecision: 15,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "decimal(15,2)",
                precision: 15,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldPrecision: 15,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "AspNetUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OrderItems",
                type: "money",
                precision: 15,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)",
                oldPrecision: 15,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Games",
                type: "money",
                precision: 15,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(15,2)",
                oldPrecision: 15,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "AspNetUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
