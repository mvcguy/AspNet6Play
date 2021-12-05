using Microsoft.AspNetCore.Identity;
using PlayWebApp.Services.Database.Model;

namespace PlayWebApp.Services.Database;


public class SeedDatabase
{


    public static void Run(ApplicationDbContext context, UserManagerExt userManager)
    {

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "shahid.ali@play.com",
            UserName = "shahid.ali@play.com"
        };

        var userExt = new IdentityUserExt
        {
            UserId = user.Id,
            FirstName = "Shahid",
            LastName = "Khan"
        };

        var res = userManager.CreateAsync(user, userExt, "Shahid@123").Result;

        var token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
        var eRes = userManager.ConfirmEmailAsync(user, token).Result;

        var address = new Address
        {
            AddressCode = "Home",
            Id = Guid.NewGuid(),
            StreetAddress = "Ml Shahid gate 34H",
            PostalCode = "1234",
            City = "Oslo",
            Country = "Norway",
            UserId = user.Id
        };

        context.Addresses.Add(address);
        context.SaveChanges();

        //
        // set default address for the user
        //
        userExt.DefaultAddressId = address.Id;
        context.Update(userExt);
        context.SaveChanges();


        var tyres = new StockItem
        {
            Id = Guid.NewGuid(),
            DisplayId = "Car tyres",
            Description = "4 pieces of winter tyres",
        };

        context.StockItems.Add(tyres);
        context.SaveChanges();

        var tyrePrice = new StockItemPrice
        {
            Id = Guid.NewGuid(),
            BreakQty = 0,
            EffectiveFrom = DateTime.Today,
            UnitCost = 110,
            UnitOfMeasure = "KG",
            StockItemId = tyres.Id
        };

        context.StockItemPrices.Add(tyrePrice);
        context.SaveChanges();



        var bookingId = Guid.NewGuid();

        var bookingItem = new BookingItem
        {
            BookingId = bookingId,
            StockItemId = tyres.Id,
            Quantity = 4,
            UnitCost = tyrePrice.UnitCost,
            Discount = 100
        };

        bookingItem.ExtCost = bookingItem.Quantity * bookingItem.UnitCost - bookingItem.Discount;

        var booking = new Booking
        {
            Id = bookingId,
            BookingNumber = "0001",
            ShippingAddressId = address.Id,
            Description = "Purchase of tyres - pick at the store",
            UserId = user.Id,
            BookingDate = new DateTime(2021, 11, 20),
            BookingItems = new BookingItem[] { bookingItem }
        };

        context.Bookings.Add(booking);

        context.SaveChanges();
    }
}

