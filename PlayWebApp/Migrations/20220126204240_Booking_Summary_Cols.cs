using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Migrations
{
    public partial class Booking_Summary_Cols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Booking",
                type: "decimal(2,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Booking",
                type: "decimal(2,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LinesTotal",
                table: "Booking",
                type: "decimal(2,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Booking",
                type: "decimal(2,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxableAmount",
                table: "Booking",
                type: "decimal(2,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Booking",
                type: "decimal(2,4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "LinesTotal",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TaxableAmount",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Booking");
        }
    }
}
