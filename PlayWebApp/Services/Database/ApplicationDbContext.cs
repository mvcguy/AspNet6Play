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

        var userExt = builder.Entity<IdentityUserExt>().ToTable("UserExt");

        userExt.HasKey(x => x.UserId);
        userExt.Property(x => x.FirstName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();
        userExt.Property(x => x.LastName).HasColumnType("nvarchar").HasMaxLength(128).IsRequired();

        stockItem.HasKey(x => x.Id);
        stockItem.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);
        stockItem.Property(x => x.DisplayId).HasColumnType("nvarchar").HasMaxLength(10);


        booking.HasKey(x => x.Id);
        booking.Property(x => x.BookingNumber).HasColumnType("nvarchar").HasMaxLength(10);
        booking.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);


        bookingItem.HasKey(x => new { x.BookingId, x.StockItemId });
        bookingItem.Property(x => x.Description).HasColumnType("nvarchar").HasMaxLength(128);

        address.HasKey(x => x.Id);
        address.Property(x => x.City).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.Country).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.PostalCode).HasColumnType("nvarchar").HasMaxLength(128);
        address.Property(x => x.StreetAddress).HasColumnType("nvarchar").HasMaxLength(128);

        stockPrice.HasKey(x => x.Id);


        //
        // atleast one default address is required for any user
        //
        userExt.HasOne(x => x.User).WithOne();
        userExt.HasMany(u => u.Addresses).WithOne(x => x.UserExt).HasForeignKey(x => x.UserId);     
        userExt.Property(x => x.DefaultAddressId).IsRequired(false);
        

        //
        // booking has many booking items
        // one booking item appears in only one booking
        //
        booking.HasMany(b => b.BookingItems).WithOne(s => s.Booking).HasForeignKey(x => x.BookingId);
        booking.HasOne(x => x.ShippingAddress);
        booking.Property(x => x.ShippingAddressId).IsRequired();
        booking.HasOne(x => x.User);
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

