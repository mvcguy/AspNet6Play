using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Migrations
{
    public partial class User_Audit_Props_Not_Required : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "User",
                type: "nvarchar",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "User",
                type: "nvarchar",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar",
                oldMaxLength: 128);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "User",
                type: "nvarchar",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar",
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "User",
                type: "nvarchar",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar",
                oldMaxLength: 128,
                oldNullable: true);
        }
    }
}
