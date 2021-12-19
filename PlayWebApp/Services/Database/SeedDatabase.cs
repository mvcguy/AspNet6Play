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
    public static void Seed(ApplicationDbContext context,  UserManagementService usrMgtSrv)
    {
        if (context.Tenants.Count() == 0)
        {
            AddIdentityUserAndDefaultAddress(context, usrMgtSrv);
        }
    }


    public static void AddIdentityUserAndDefaultAddress(ApplicationDbContext context, 
        UserManagementService usrMgtSrv)
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

        if (context.Set<ApplicationUser>().Count() > 0)
        {
            var userExt = context.Set<ApplicationUser>().FirstOrDefault();
            return;
        }

        //
        // create Identity user ext
        //
        var uvm = new ApplicationUserUpdateVm()
        {
            UserName = "shahid.ali",
            Email = "shahid.ali@play.com",
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
        var result = usrMgtSrv.CreateUser(uvm).Result;

    }

}

