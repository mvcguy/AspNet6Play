using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Data.Migrations
{
    public partial class UserExt_Name_Columns_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UserExt",
                type: "nvarchar",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "UserExt",
                type: "nvarchar",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UserExt");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "UserExt");
        }
    }
}
