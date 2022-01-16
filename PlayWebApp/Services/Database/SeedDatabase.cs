using Microsoft.AspNetCore.Identity;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.AppManagement.Repository;
using PlayWebApp.Services.AppManagement.ViewModels;
using PlayWebApp.Services.Logistics.CustomerManagement;
using PlayWebApp.Services.Logistics.CustomerManagement.ViewModels;
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

        var tenantRepo = new TenantRepository(context, new StartupAppContext { TenantId = "", UserId = "" });
        var tenantSrv = new TenantService(tenantRepo);
        var tenantId = CreateTenant(context, tenantSrv);

        var userRepo = new UserRepository(context, new StartupAppContext() { TenantId = tenantId });
        var userSrv = new UserService(userRepo);
        var userId = CreateUser(context, tenantId, userSrv);

        var appContext = new StartupAppContext { TenantId = tenantId, UserId = userId };
        var inRepo = new InventoryRepository(context, appContext);
        var inSrv = new InventoryService(inRepo);
        var cusRepo = new CustomerRepository(context, appContext);
        var curLocRepo = new CustomerLocationRepository(context, appContext);
        var cusSrv = new CustomerService(cusRepo, curLocRepo);

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

    private static string CreateUser(ApplicationDbContext context, string tenantId, UserService userSrv)
    {
        string userId;
        if (context.Users.Count() == 0)
        {
            userId = AddUser(userSrv, tenantId);
        }
        else
        {
            userId = context.Users.FirstOrDefault()?.RefNbr;
        }

        return userId;
    }

    private static string CreateTenant(ApplicationDbContext context, TenantService srv)
    {
        string tenantId;
        if (context.Tenants.Count() == 0)
        {
            tenantId = AddTenant(srv);
        }
        else
        {
            tenantId = context.Tenants.FirstOrDefault()?.Id;
        }

        return tenantId;
    }

    public static string AddTenant(TenantService srv)
    {
        var vm = new TenantUpdateVm
        {
            Country = "PK",
            RefNbr = "SH-TEST-01",
            Name = "Shahid Test company 01",
        };

        var result = srv.Add(vm).Result;
        return result.InternalId;
    }

    public static string AddUser(UserService srv, string tenantId)
    {
        var uvm = new AppUserUpdateVm()
        {
            UserName = "shahid.ali",
            RefNbr = "shahid.ali",
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

