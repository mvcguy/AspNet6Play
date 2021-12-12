using Microsoft.AspNetCore.Identity;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Services.Database;


public class SeedDatabase
{


    /// <summary>
    /// Adds a tenant and an app user and their address to the db
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    public static void Seed(ApplicationDbContext context, UserManager<IdentityUser> userManager, UserManagementService usrMgtSrv)
    {
        if (context.Tenants.Count() == 0)
        {
            AddIdentityUserAndDefaultAddress(context, userManager, usrMgtSrv, out _, out _);
        }
    }

    /// <summary>
    /// Add a tenant, user, address and more items e.g, stock items to the db
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    public static void Seed2(ApplicationDbContext context, UserManager<IdentityUser> userManager, UserManagementService usrMgtSrv)
    {

        AddIdentityUserAndDefaultAddress(context, userManager, usrMgtSrv, out var userId, out var addressId);

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
        UserManager<IdentityUser> userManager, UserManagementService usrMgtSrv, out string userId, out string addressId)
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

        if (context.Set<IdentityUserExt>().Count() > 0)
        {
            var userExt = context.Set<IdentityUserExt>().FirstOrDefault();
            userId = userExt.Id;
            addressId = userExt.DefaultAddressId;
            return;
        }

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "shahid.ali@play.com",
            UserName = "shahid.ali@play.com"
        };

        var res = userManager.CreateAsync(user, "Shahid@123").Result;

        var token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
        var eRes = userManager.ConfirmEmailAsync(user, token).Result;

        //
        // create Identity user ext
        //
        var uvm = new IdentityUserUpdateVm()
        {
            UserId = user.Id,
            FirstName = "Shahid",
            LastName = "Khan",
            TenantCode = tenant.Entity.TenantCode,
            DefaultAddress = new AddressUpdateVm
            {
                AddressCode = "Shipping",
                StreetAddress = "Moniba street",
                City = "Chd",
                PostalCode = "4000",
                Country = "PK",
            }
        };
        var result = usrMgtSrv.CreateIdentityUserExt(uvm).Result;

        //
        // set OUT params
        //
        addressId = usrMgtSrv.GetIdentityUserExt(user.Id).Result.DefaultAddressId;
        userId = user.Id;
    }

}

