using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
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
var services = builder.Services;

// Add services to the container.

bool.TryParse(builder.Configuration["ConnectionStrings:UseSqlServer"], out var useSqlServer);

if (useSqlServer)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionSqlServer");
    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionSqlLite");
    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
}


services.AddDatabaseDeveloperPageExceptionFilter();
services.AddHttpContextAccessor();

services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://localhost:5001";
    options.ClientId = "mvc";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("profile");
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClaimActions.MapJsonKey(CustomClaimTypes.TenantId, CustomClaimTypes.TenantId);

}).AddJwtBearer(options =>
{
    options.Authority = "https://localhost:5001";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});


// tag start
// services.AddAuthentication(o =>
// {
//     o.DefaultScheme = IdentityConstants.ApplicationScheme;
//     o.DefaultSignInScheme = IdentityConstants.ExternalScheme;    
// })
// .AddIdentityCookies(o => { });

// var identityService = services.AddIdentityCore<ApplicationUser>(o =>
// {
//     o.Stores.MaxLengthForKeys = 128;
//     o.SignIn.RequireConfirmedAccount = true;

// })
//     .AddDefaultTokenProviders()
//     .AddEntityFrameworkStores<ApplicationDbContext>();

// identityService.AddSignInManager();
// tag ends

// services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AdditionalUserClaimsPrincipalFactory>();


services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Logistics", "/");
});

services.AddLogging((options) =>
{
    options.AddConsole();
    options.AddDebug();
});

services.AddMvcCore(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});

services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
});




services.AddScoped<INavigationRepository<SupplierAddress>, SupplierLocationRepository>();
services.AddScoped<INavigationRepository<CustomerAddress>, CustomerLocationRepository>();
services.AddScoped<INavigationRepository<ApplicationUser>, UserRepository>();
services.AddScoped<INavigationRepository<Tenant>, TenantRepository>();
services.AddScoped<INavigationRepository<StockItem>, InventoryRepository>();
services.AddScoped<INavigationRepository<Booking>, BookingRepository>();

services.AddScoped<UserService>();
services.AddScoped<TenantService>();
services.AddScoped<CustomerLocationService>();
services.AddScoped<SupplierLocationService>();
services.AddScoped<InventoryService>();
services.AddScoped<BookingService>();
services.AddScoped<IPlayAppContext, PlayAppContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedDatabase.Seed(ctx);
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
    options.MapControllers().RequireAuthorization();
    options.MapRazorPages().RequireAuthorization();
});


app.Run();
