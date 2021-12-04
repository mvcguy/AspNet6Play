using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Data.Migrations
{
    public partial class UserExt_Table_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Address",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserExt",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultAddressId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExt", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserExt_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                table: "Address",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_UserExt_UserId",
                table: "Address",
                column: "UserId",
                principalTable: "UserExt",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_UserExt_UserId",
                table: "Address");

            migrationBuilder.DropTable(
                name: "UserExt");

            migrationBuilder.DropIndex(
                name: "IX_Address_UserId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Address");
        }
    }
}
