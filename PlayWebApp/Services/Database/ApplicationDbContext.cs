using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database.Model;

namespace PlayWebApp.Services.Database;

public class ApplicationDbContext : DbContext
{

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<StockItem> StockItems { get; set; }

    public DbSet<StockItemPrice> StockItemPrices { get; set; }

    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public DbSet<SupplierAddress> SupplierAddresses { get; set; }

    public DbSet<BusinessEntity> BusinessEntities{ get; set; }


    public DbSet<Tenant> Tenants { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var booking = builder.Entity<Booking>().ToTable(nameof(Booking));
        var stockItem = builder.Entity<StockItem>().ToTable(nameof(StockItem));
        var bookingItem = builder.Entity<BookingItem>().ToTable(nameof(BookingItem));
        var customerAddress = builder.Entity<CustomerAddress>().ToTable("CustomerAddress");
        var supplierAddress = builder.Entity<SupplierAddress>().ToTable("SupplierAddress");

        var stockPrice = builder.Entity<StockItemPrice>().ToTable(nameof(StockItemPrice));
        var applicationUser = builder.Entity<ApplicationUser>().ToTable("User");
        var customer = builder.Entity<Customer>().ToTable("Customer");
        var supplier = builder.Entity<Supplier>().ToTable("Supplier");
        var bEntity = builder.Entity<BusinessEntity>().ToTable("BEntity");

        builder.Entity<Tenant>().Ignore(x => x.TenantId);
        builder.Entity<Tenant>().Property(x => x.CreatedBy).IsRequired(false);
        builder.Entity<Tenant>().Property(x => x.ModifiedBy).IsRequired(false);
        builder.Entity<Tenant>().Property(x => x.CreatedOn).IsRequired(false);
        builder.Entity<Tenant>().Property(x => x.ModifiedOn).IsRequired(false);

        builder.Entity<ApplicationUser>().Property(x => x.CreatedBy).IsRequired(false);
        builder.Entity<ApplicationUser>().Property(x => x.ModifiedBy).IsRequired(false);
        builder.Entity<ApplicationUser>().Property(x => x.CreatedOn).IsRequired(false);
        builder.Entity<ApplicationUser>().Property(x => x.ModifiedOn).IsRequired(false);

        applicationUser.Property(x => x.TenantId).IsRequired(false);

        // booking has many booking items
        // one booking item appears in only one booking
        //
        booking.HasMany(b => b.BookingItems).WithOne(s => s.Booking).HasForeignKey(x => x.BookingId).IsRequired(false);
        booking.HasOne(x => x.ShippingAddress);
        booking.Property(x => x.ShippingAddressId).IsRequired();
        booking.Property(x => x.CreatedBy).IsRequired();

        //
        // stockitem has many prices
        // one price is only meant for one particular stock item
        //
        stockItem.HasMany(s => s.StockItemPrices).WithOne(sp => sp.StockItem).HasForeignKey(x => x.StockItemId).IsRequired(false);

        //
        // stockitem appear in many bookings
        // one booking item is related to only one stock item
        //
        stockItem.HasMany(s => s.BookingItems).WithOne(b => b.StockItem).HasForeignKey(x => x.StockItemId).IsRequired(false);

        //
        // business entities
        //
        customer.HasMany(x => x.Addresses).WithOne(b => b.Customer).HasForeignKey(x => x.CustomerId).IsRequired(false);
        supplier.HasMany(x => x.Addresses).WithOne(b => b.Supplier).HasForeignKey(x => x.SupplierId).IsRequired(false);

        bEntity.HasOne(x => x.DefaultAddress).WithOne().IsRequired(false);

    }
}

