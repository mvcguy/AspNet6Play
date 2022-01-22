﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlayWebApp.Services.Database;

#nullable disable

namespace PlayWebApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Address", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("char");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("StreetAddress")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantId")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Booking", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime>("BookingDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("ShippingAddressId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ShippingAddressId");

                    b.ToTable("Booking", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.BookingItem", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("BookingId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<decimal?>("Discount")
                        .HasColumnType("decimal(2,4)");

                    b.Property<decimal?>("ExtCost")
                        .IsRequired()
                        .HasColumnType("decimal(2,4)");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Quantity")
                        .IsRequired()
                        .HasColumnType("nvarchar");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("StockItemId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<decimal?>("UnitCost")
                        .HasColumnType("decimal(2,4)");

                    b.HasKey("Id");

                    b.HasIndex("BookingId");

                    b.HasIndex("StockItemId");

                    b.ToTable("BookingItem", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.BusinessEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("DefaultAddressId")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("DefaultAddressId")
                        .IsUnique();

                    b.ToTable("BEntity", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.StockItem", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("StockItem", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.StockItemPrice", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<decimal>("BreakQty")
                        .HasColumnType("decimal(2, 4)");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EffectiveFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("StockItemId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<decimal>("UnitCost")
                        .HasColumnType("decimal(2, 4)");

                    b.Property<string>("UnitOfMeasure")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("char");

                    b.HasKey("Id");

                    b.HasIndex("StockItemId");

                    b.ToTable("StockItemPrice", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Tenant", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("char");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<DateTime?>("ModifiedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("RefNbr")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar");

                    b.Property<string>("TenantName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Customer", b =>
                {
                    b.HasBaseType("PlayWebApp.Services.Database.Model.BusinessEntity");

                    b.Property<string>("PaymentMethod")
                        .HasColumnType("TEXT");

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.CustomerAddress", b =>
                {
                    b.HasBaseType("PlayWebApp.Services.Database.Model.Address");

                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar");

                    b.HasIndex("CustomerId");

                    b.ToTable("CustomerAddress", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Supplier", b =>
                {
                    b.HasBaseType("PlayWebApp.Services.Database.Model.BusinessEntity");

                    b.Property<string>("ExpenseAccount")
                        .HasColumnType("TEXT");

                    b.ToTable("Supplier", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.SupplierAddress", b =>
                {
                    b.HasBaseType("PlayWebApp.Services.Database.Model.Address");

                    b.Property<string>("SupplierId")
                        .HasColumnType("nvarchar");

                    b.HasIndex("SupplierId");

                    b.ToTable("SupplierAddress", (string)null);
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Booking", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.Customer", "Customer")
                        .WithMany("Bookings")
                        .HasForeignKey("CustomerId");

                    b.HasOne("PlayWebApp.Services.Database.Model.Address", "ShippingAddress")
                        .WithMany()
                        .HasForeignKey("ShippingAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("ShippingAddress");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.BookingItem", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.Booking", "Booking")
                        .WithMany("BookingItems")
                        .HasForeignKey("BookingId");

                    b.HasOne("PlayWebApp.Services.Database.Model.StockItem", "StockItem")
                        .WithMany("BookingItems")
                        .HasForeignKey("StockItemId");

                    b.Navigation("Booking");

                    b.Navigation("StockItem");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.BusinessEntity", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.Address", "DefaultAddress")
                        .WithOne()
                        .HasForeignKey("PlayWebApp.Services.Database.Model.BusinessEntity", "DefaultAddressId");

                    b.Navigation("DefaultAddress");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.StockItemPrice", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.StockItem", "StockItem")
                        .WithMany("StockItemPrices")
                        .HasForeignKey("StockItemId");

                    b.Navigation("StockItem");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Customer", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.BusinessEntity", null)
                        .WithOne()
                        .HasForeignKey("PlayWebApp.Services.Database.Model.Customer", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.CustomerAddress", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.Customer", "Customer")
                        .WithMany("Addresses")
                        .HasForeignKey("CustomerId");

                    b.HasOne("PlayWebApp.Services.Database.Model.Address", null)
                        .WithOne()
                        .HasForeignKey("PlayWebApp.Services.Database.Model.CustomerAddress", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Supplier", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.BusinessEntity", null)
                        .WithOne()
                        .HasForeignKey("PlayWebApp.Services.Database.Model.Supplier", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.SupplierAddress", b =>
                {
                    b.HasOne("PlayWebApp.Services.Database.Model.Address", null)
                        .WithOne()
                        .HasForeignKey("PlayWebApp.Services.Database.Model.SupplierAddress", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PlayWebApp.Services.Database.Model.Supplier", "Supplier")
                        .WithMany("Addresses")
                        .HasForeignKey("SupplierId");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Booking", b =>
                {
                    b.Navigation("BookingItems");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.StockItem", b =>
                {
                    b.Navigation("BookingItems");

                    b.Navigation("StockItemPrices");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Customer", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Bookings");
                });

            modelBuilder.Entity("PlayWebApp.Services.Database.Model.Supplier", b =>
                {
                    b.Navigation("Addresses");
                });
#pragma warning restore 612, 618
        }
    }
}
