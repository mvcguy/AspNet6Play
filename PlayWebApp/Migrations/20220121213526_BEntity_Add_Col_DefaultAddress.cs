using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Migrations
{
    public partial class BEntity_Add_Col_DefaultAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultAddressId",
                table: "BEntity",
                type: "nvarchar",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BEntity_DefaultAddressId",
                table: "BEntity",
                column: "DefaultAddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BEntity_Address_DefaultAddressId",
                table: "BEntity",
                column: "DefaultAddressId",
                principalTable: "Address",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BEntity_Address_DefaultAddressId",
                table: "BEntity");

            migrationBuilder.DropIndex(
                name: "IX_BEntity_DefaultAddressId",
                table: "BEntity");

            migrationBuilder.DropColumn(
                name: "DefaultAddressId",
                table: "BEntity");
        }
    }
}
