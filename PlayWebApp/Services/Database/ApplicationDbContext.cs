using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database.Model;

namespace PlayWebApp.Services.Database;

public class ApplicationDbContext : DbContext
{

    public DbSet<ApplicationUser> Users { get; set; }

    public DbSet<Booking> Bookings { get; set; }

    public DbSet<StockItem> StockItems { get; set; }

    public DbSet<StockItemPrice> StockItemPrices { get; set; }

    public DbSet<Address> Addresses { get; set; }

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
        var address = builder.Entity<Address>().ToTable(nameof(Address));
        var stockPrice = builder.Entity<StockItemPrice>().ToTable(nameof(StockItemPrice));
        var applicationUser = builder.Entity<ApplicationUser>().ToTable("User");
        
        applicationUser.Property(x => x.FirstName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
        applicationUser.Property(x => x.LastName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
        applicationUser.Property(x => x.TenantId).IsRequired(false);
        builder.Entity<ApplicationUser>().Ignore(x => x.Code);

        stockItem.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);

        booking.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);

        bookingItem.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);

        address.Property(x => x.City).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.Country).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.PostalCode).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.StreetAddress).HasColumnType("nvarchar").HasMaxLength(128);
        
        //
        // booking has many booking items
        // one booking item appears in only one booking
        //
        booking.HasMany(b => b.BookingItems).WithOne(s => s.Booking).HasForeignKey(x => x.BookingId);
        booking.HasOne(x => x.ShippingAddress);
        booking.Property(x => x.ShippingAddressId).IsRequired();
        booking.Property(x => x.CreatedBy).IsRequired();

        //
        // stockitem has many prices
        // one price is only meant for one particular stock item
        //
        stockItem.HasMany(s => s.StockItemPrices).WithOne(sp => sp.StockItem).HasForeignKey(x => x.StockItemId);

        //
        // stockitem appear in many bookings
        // one booking item is related to only one stock item
        //
        stockItem.HasMany(s => s.BookingItems).WithOne(b => b.StockItem).HasForeignKey(x => x.StockItemId);

    }
}

