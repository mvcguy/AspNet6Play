using Microsoft.AspNetCore.Identity;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.AppManagement.Repository;
using PlayWebApp.Services.CustomerManagement;
using PlayWebApp.Services.CustomerManagement.ViewModels;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.Identity;
using PlayWebApp.Services.Identity.Repository;
using PlayWebApp.Services.Identity.ViewModels;
using PlayWebApp.Services.Logistics.InventoryMgt;
using PlayWebApp.Services.Logistics.InventoryMgt.Repository;
using PlayWebApp.Services.Logistics.ViewModels;
#nullable disable

namespace PlayWebApp.Services.Database;


public class StartupAppContext : IPlayAppContext
{
    public string UserId { get; set; }

    public string TenantId { get; set; }
}

public class SeedDatabase
{


    /// <summary>
    /// Adds a tenant and an app user and their address to the db
    /// </summary>
    /// <param name="context"></param>
    /// <param name="userManager"></param>
    public static void Seed(ApplicationDbContext context)
    {


        var appRepo = new AppMgtRepository(context);
        var appSrv = new AppMgtService(appRepo);


        var tenantId = string.Empty;
        var userId = string.Empty;

        if (context.Tenants.Count() == 0)
        {
            tenantId = AddTenant(context);
        }
        else
        {
            tenantId = context.Tenants.FirstOrDefault()?.Id;
        }
        
        var userRepo = new UserRepository(context, new StartupAppContext(){TenantId = tenantId});
        var userSrv = new UserService(userRepo);
        if (context.Users.Count() == 0)
        {
            userId = AddUser(userSrv, tenantId);
        }
        else
        {
            userId = context.Users.FirstOrDefault()?.RefNbr;
        }
        var appContext = new StartupAppContext { TenantId = tenantId, UserId = userId };

        var inRepo = new InventoryRepository(context, appContext);
        var inSrv = new InventoryService(inRepo);

        var cusRepo = new CustomerRepository(context, appContext);
        var cusSrv = new CustomerService(cusRepo);

        if (context.StockItems.Count() == 0)
        {
            AddStockItems(inSrv);
        }

        if (context.Customers.Count() == 0)
        {
            AddCustomers(cusSrv);
        }

        context.SaveChanges();
    }

    public static string AddTenant(ApplicationDbContext context)
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
        return tenant.Entity.Id;
    }

    public static string AddUser(UserService srv, string tenantId)
    {
        var uvm = new AppUserUpdateVm()
        {
            UserName = "shahid.ali",
            Email = "shahid.ali@play.com",
            FirstName = "Shahid",
            LastName = "Khan",
        };
        var result = srv.Add(uvm).Result;
        return result.InternalId;
    }

    public static void AddStockItems(InventoryService srv)
    {
        var items = new List<StockItemUpdateVm>
        {
            new StockItemUpdateVm
            {
                RefNbr = "001",
                ItemDescription = "office chairs"
            }
        };

        // TODO: add support for adding multiple items with one call

        foreach (var item in items)
        {
            var res = srv.Add(item).Result;
        }
    }

    public static void AddCustomers(CustomerService srv)
    {
        var items = new List<CustomerUpdateVm>
        {
            new CustomerUpdateVm
            {
                RefNbr = "C001",
                Name = "Shahid Ali AS",
                Active = true,
            }
        };

        // TODO: add support for adding multiple items with one call

        foreach (var item in items)
        {
            var res = srv.Add(item).Result;
        }
    }
}

