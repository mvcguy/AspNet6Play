using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlayWebApp.Services.AppManagement;
using PlayWebApp.Services.AppManagement.Repository;
using PlayWebApp.Services.Database;
using PlayWebApp.Services.Database.Model;
using PlayWebApp.Services.DataNavigation;
using PlayWebApp.Services.Filters;
using PlayWebApp.Services.Identity;
using PlayWebApp.Services.Identity.Repository;
using PlayWebApp.Services.Logistics.BookingMgt;
using PlayWebApp.Services.Logistics.BookingMgt.Repository;
using PlayWebApp.Services.Logistics.InventoryMgt;
using PlayWebApp.Services.Logistics.InventoryMgt.Repository;
using PlayWebApp.Services.Logistics.LocationMgt;
using PlayWebApp.Services.Logistics.LocationMgt.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// tag start
// builder.Services.AddAuthentication(o =>
// {
//     o.DefaultScheme = IdentityConstants.ApplicationScheme;
//     o.DefaultSignInScheme = IdentityConstants.ExternalScheme;    
// })
// .AddIdentityCookies(o => { });

// var identityService = builder.Services.AddIdentityCore<IdentityUser>(o =>
// {
//     o.Stores.MaxLengthForKeys = 128;
//     o.SignIn.RequireConfirmedAccount = true;

// })
//     .AddDefaultTokenProviders()
//     .AddEntityFrameworkStores<ApplicationDbContext>();

// identityService.AddSignInManager();
// tag ends

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.Authority = "https://localhost:5001";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience= false,
    };
});

builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, AdditionalUserClaimsPrincipalFactory>();


builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Logistics", "/");
});

builder.Services.AddLogging((options) =>
{
    options.AddConsole();
    options.AddDebug();
});

builder.Services.AddMvcCore(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
});

builder.Services.AddScoped<UserManagementRepository>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<AppMgtRepository>();
builder.Services.AddScoped<AppMgtService>();

builder.Services.AddScoped<INavigationRepository<Address>, LocationRepository>();
builder.Services.AddScoped<LocationService>();

builder.Services.AddScoped<INavigationRepository<StockItem>, InventoryRepository>();
builder.Services.AddScoped<InventoryService>();

builder.Services.AddScoped<INavigationRepository<Booking>, BookingRepository>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<IPlayAppContext, PlayAppContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var usrMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var usrMgtSrv = scope.ServiceProvider.GetRequiredService<UserManagementService>();

    SeedDatabase.Seed(ctx, usrMgr, usrMgtSrv);

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(options =>
{
    options.MapControllers();
    options.MapRazorPages();
});


app.Run();
