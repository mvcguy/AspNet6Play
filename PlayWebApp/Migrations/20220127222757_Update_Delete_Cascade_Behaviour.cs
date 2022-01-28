using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Migrations
{
    public partial class Update_Delete_Cascade_Behaviour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItem_Booking_BookingId",
                table: "BookingItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddress_Customer_CustomerId",
                table: "CustomerAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_StockItemPrice_StockItem_StockItemId",
                table: "StockItemPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierAddress_Supplier_SupplierId",
                table: "SupplierAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItem_Booking_BookingId",
                table: "BookingItem",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddress_Customer_CustomerId",
                table: "CustomerAddress",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockItemPrice_StockItem_StockItemId",
                table: "StockItemPrice",
                column: "StockItemId",
                principalTable: "StockItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierAddress_Supplier_SupplierId",
                table: "SupplierAddress",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItem_Booking_BookingId",
                table: "BookingItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddress_Customer_CustomerId",
                table: "CustomerAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_StockItemPrice_StockItem_StockItemId",
                table: "StockItemPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierAddress_Supplier_SupplierId",
                table: "SupplierAddress");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItem_Booking_BookingId",
                table: "BookingItem",
                column: "BookingId",
                principalTable: "Booking",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddress_Customer_CustomerId",
                table: "CustomerAddress",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockItemPrice_StockItem_StockItemId",
                table: "StockItemPrice",
                column: "StockItemId",
                principalTable: "StockItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierAddress_Supplier_SupplierId",
                table: "SupplierAddress",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");
        }
    }
}
