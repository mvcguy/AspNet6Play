using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Data.Migrations
{
    public partial class Booking_Add_Booking_Number : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingNumber",
                table: "Booking",
                type: "nvarchar",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingNumber",
                table: "Booking");
        }
    }
}
