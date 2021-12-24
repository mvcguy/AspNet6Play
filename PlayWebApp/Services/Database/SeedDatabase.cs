using Microsoft.AspNetCore.Identity;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.AppManagement.Repository;
using PlayWebApp.Services.CustomerManagement;
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

        var userRepo = new UserManagementRepository(context);
        var appRepo = new AppMgtRepository(context);
        var appSrv = new AppMgtService(appRepo);
        var userSrv = new UserManagementService(userRepo, appSrv);
        var appContext = new StartupAppContext { TenantId = "", UserId = "" };

        var inRepo = new InventoryRepository(context, appContext);
        var inSrv = new InventoryService(inRepo);

        var cusRepo = new CustomerRepository(context, appContext);
        var cusSrv = new CustomerService(cusRepo);

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

        if (context.Users.Count() == 0)
        {
            userId = AddUser(userSrv, tenantId);
        }
        else
        {
            userId = context.Users.FirstOrDefault()?.Id;
        }

        appContext.UserId = userId;
        appContext.TenantId = tenantId;

        if (context.StockItems.Count() == 0)
        {
            AddStockItems(inSrv);
        }

        if(context.Customers.Count() == 0)
        {
            AddCustomers(cusSrv);
        }
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

    public static string AddUser(UserManagementService srv, string tenantId)
    {
        var uvm = new ApplicationUserUpdateVm()
        {
            UserName = "shahid.ali",
            Email = "shahid.ali@play.com",
            FirstName = "Shahid",
            LastName = "Khan",
            TenantCode = tenantId,
        };
        var result = srv.CreateUser(uvm).Result;
        return result.EntityId;
    }

    public static void AddStockItems(InventoryService srv)
    {

    }

    public static void AddCustomers(CustomerService srv)
    {

    }
}

