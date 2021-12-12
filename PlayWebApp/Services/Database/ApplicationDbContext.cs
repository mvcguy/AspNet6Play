using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services.Database.Model;

namespace PlayWebApp.Services.Database;

public class ApplicationDbContext : IdentityDbContext
{

    public DbSet<Booking> Bookings { get; set; } = null!;

    public DbSet<StockItem> StockItems { get; set; } = null!;

    public DbSet<StockItemPrice> StockItemPrices { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; } = null!;

    public DbSet<Tenant> Tenants { get; set; } = null!;


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
        var user = builder.Entity<IdentityUser>();

        var userExt = builder.Entity<IdentityUserExt>().ToTable("UserExt");
        
        userExt.Property(x => x.FirstName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
        userExt.Property(x => x.LastName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
        userExt.Property(x => x.TenantId).IsRequired(false);
        builder.Entity<IdentityUserExt>().Ignore(x => x.Code);

        stockItem.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);

        booking.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);

        bookingItem.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);

        address.Property(x => x.City).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.Country).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.PostalCode).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.StreetAddress).HasColumnType("nvarchar").HasMaxLength(128);
        
        user.HasOne(typeof(IdentityUserExt)).WithOne("User").OnDelete(DeleteBehavior.Cascade);
        user.HasMany(typeof(Address)).WithOne("User").OnDelete(DeleteBehavior.Cascade);
        user.HasMany(typeof(Booking)).WithOne("User").OnDelete(DeleteBehavior.Cascade);
        user.HasMany(typeof(BookingItem)).WithOne("User").OnDelete(DeleteBehavior.Cascade);
        user.HasMany(typeof(StockItem)).WithOne("User").OnDelete(DeleteBehavior.Cascade);
        user.HasMany(typeof(StockItemPrice)).WithOne("User").OnDelete(DeleteBehavior.Cascade);


        //
        // booking has many booking items
        // one booking item appears in only one booking
        //
        booking.HasMany(b => b.BookingItems).WithOne(s => s.Booking).HasForeignKey(x => x.BookingId);
        booking.HasOne(x => x.ShippingAddress);
        booking.Property(x => x.ShippingAddressId).IsRequired();
        booking.Property(x => x.UserId).IsRequired();

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

