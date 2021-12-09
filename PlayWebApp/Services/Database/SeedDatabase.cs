using Microsoft.AspNetCore.Identity;
using PlayWebApp.Services.Database.Model;
#nullable disable

namespace PlayWebApp.Services.Database;


public class SeedDatabase
{


    /// <summary>
    /// Adds a tenant and an app user and their address to the db
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    public static void Seed(ApplicationDbContext context, UserManagerExt userManager)
    {
        if (context.Tenants.Count() == 0)
        {            
            AddIdentityUserAndDefaultAddress(context, userManager, out var userId, out var addressId);
        }
    }

    /// <summary>
    /// Add a tenant, user, address and more items e.g, stock items to the db
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    public static void Seed2(ApplicationDbContext context, UserManagerExt userManager)
    {

        AddIdentityUserAndDefaultAddress(context, userManager, out var userId, out var addressId);

        var tyres = new StockItem
        {
            Id = Guid.NewGuid().ToString(),
            Code = "Car tyres",
            Description = "4 pieces of winter tyres",
        };

        context.StockItems.Add(tyres);
        context.SaveChanges();

        var tyrePrice = new StockItemPrice
        {
            Id = Guid.NewGuid().ToString(),
            BreakQty = 0,
            EffectiveFrom = DateTime.Today,
            UnitCost = 110,
            UnitOfMeasure = "KG",
            StockItemId = tyres.Id
        };

        context.StockItemPrices.Add(tyrePrice);
        context.SaveChanges();



        var bookingId = Guid.NewGuid().ToString();

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
            Code = "0001",
            ShippingAddressId = addressId,
            Description = "Purchase of tyres - pick at the store",
            UserId = userId,
            BookingDate = new DateTime(2021, 11, 20),
            BookingItems = new BookingItem[] { bookingItem }
        };

        context.Bookings.Add(booking);

        context.SaveChanges();
    }

    
    private static void AddAuditData<TModel>(TModel model, string tenantId, string userId) where TModel : EntityBase
    {
        model.TenantId = tenantId;
        model.UserId = userId;
        model.ModifiedOn = DateTime.UtcNow;
        model.UserId = userId;
        model.ModifiedBy = userId;
        model.CreatedOn = DateTime.UtcNow;
    }

    public static void AddIdentityUserAndDefaultAddress(ApplicationDbContext context,
        UserManagerExt userManager, out string userId, out string addressId)
    {

        var tenant = context.Tenants.Add(new Tenant
            {
                Id = Guid.NewGuid().ToString(),
                Country = "PK",
                TenantCode = "SH-TEST-01",
                TenantName = "Shahid Test company 01",
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            });

            context.SaveChanges();

        if (context.Users.Count() > 0)
        {
            var userX = context.Users.FirstOrDefault();
            userId = userX.Id;
            addressId = context.Set<IdentityUserExt>().FirstOrDefault(x => x.UserId == userX.Id).DefaultAddressId;
            return;
        }

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "shahid.ali@play.com",
            UserName = "shahid.ali@play.com"
        };

        var userExt = new IdentityUserExt
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Shahid",
            LastName = "Khan",
            Code = "Admin"
        };

        AddAuditData(userExt, tenant.Entity.Id, user.Id);

        var res = userManager.CreateAsync(user, userExt, "Shahid@123").Result;

        var token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
        var eRes = userManager.ConfirmEmailAsync(user, token).Result;

        var address = new Address
        {
            Code = "Home",
            Id = Guid.NewGuid().ToString(),
            StreetAddress = "Ml Shahid gate 34H",
            PostalCode = "1234",
            City = "Oslo",
            Country = "Norway",
        };
        AddAuditData(address, tenant.Entity.Id, user.Id);

        context.Addresses.Add(address);
        context.SaveChanges();

        //
        // set default address for the user
        //
        userExt.DefaultAddressId = address.Id;
        context.Update(userExt);
        context.SaveChanges();

        userId = user.Id;
        addressId = address.Id;

    }

}

