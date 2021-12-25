using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayWebApp.Migrations
{
    public partial class Create_App_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    City = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Country = table.Column<string>(type: "char", maxLength: 2, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    RefNbr = table.Column<string>(type: "nvarchar", maxLength: 10, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BEntity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    RefNbr = table.Column<string>(type: "nvarchar", maxLength: 10, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    RefNbr = table.Column<string>(type: "nvarchar", maxLength: 10, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    TenantCode = table.Column<string>(type: "nvarchar", maxLength: 10, nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Country = table.Column<string>(type: "char", maxLength: 2, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_BEntity_Id",
                        column: x => x.Id,
                        principalTable: "BEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    ExpenseAccount = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supplier_BEntity_Id",
                        column: x => x.Id,
                        principalTable: "BEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockItemPrice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    StockItemId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    BreakQty = table.Column<decimal>(type: "decimal(2, 4)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(2, 4)", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "char", maxLength: 10, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    RefNbr = table.Column<string>(type: "nvarchar", maxLength: 10, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItemPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockItemPrice_StockItem_StockItemId",
                        column: x => x.StockItemId,
                        principalTable: "StockItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    BookingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShippingAddressId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    RefNbr = table.Column<string>(type: "nvarchar", maxLength: 10, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_Address_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Booking_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddress",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddress_Address_Id",
                        column: x => x.Id,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerAddress_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SupplierAddress",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    SupplierId = table.Column<string>(type: "nvarchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierAddress_Address_Id",
                        column: x => x.Id,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplierAddress_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BookingItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    StockItemId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    BookingId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Quantity = table.Column<decimal>(type: "nvarchar", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(2,4)", nullable: true),
                    ExtCost = table.Column<decimal>(type: "decimal(2,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(2,4)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    Timestamp = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false),
                    RefNbr = table.Column<string>(type: "nvarchar", maxLength: 10, nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingItem_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BookingItem_StockItem_StockItemId",
                        column: x => x.StockItemId,
                        principalTable: "StockItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_CustomerId",
                table: "Booking",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_ShippingAddressId",
                table: "Booking",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItem_BookingId",
                table: "BookingItem",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItem_StockItemId",
                table: "BookingItem",
                column: "StockItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_CustomerId",
                table: "CustomerAddress",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockItemPrice_StockItemId",
                table: "StockItemPrice",
                column: "StockItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierAddress_SupplierId",
                table: "SupplierAddress",
                column: "SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingItem");

            migrationBuilder.DropTable(
                name: "CustomerAddress");

            migrationBuilder.DropTable(
                name: "StockItemPrice");

            migrationBuilder.DropTable(
                name: "SupplierAddress");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "StockItem");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "BEntity");
        }
    }
}
