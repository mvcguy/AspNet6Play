using PlayWebApp.Services.Database.Model;

namespace PlayWebApp.Services.Database;

public class SeedDatabase
{
    public static void Run(ApplicationDbContext context)
    {
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

        var address = new Address
        {
            Id = Guid.NewGuid(),
            StreetAddress = "Ingenior Rybergs gate 9A",
            PostalCode = "3024",
            City = "Drammen",
            Country = "Norway"
        };

        context.Addresses.Add(address);
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
            ShippingAddressId = address.Id,
            Description = "Purchase of tyres - pick at the store",
            UserId = "857b2dfe-2fab-4387-b87b-4e71680c5e73",
            BookingDate = new DateTime(2021, 11, 20),
            BookingItems = new BookingItem[] { bookingItem }
        };

        context.Bookings.Add(booking);

        context.SaveChanges();
    }
}

