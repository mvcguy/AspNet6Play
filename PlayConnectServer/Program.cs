
using IdentityServer4;
using Microsoft.EntityFrameworkCore;
using PlayConnectServer.AppConfig;
using PlayConnectServer.CustomUserStore;
using PlayConnectServer.Quickstart;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

bool.TryParse(builder.Configuration["ConnectionStrings:UseSqlServer"], out var useSqlServer);

if (useSqlServer)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionSqlServer");
    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionSqlLite");
    services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
}

services.AddControllersWithViews();

var builderX = services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
    options.EmitStaticAudienceClaim = true;
    //options.Cors.CorsPolicyName = "mg.services";

}).AddAppUsers();

// in-memory, code config
builderX.AddInMemoryIdentityResources(Config.IdentityResources);
builderX.AddInMemoryApiScopes(Config.ApiScopes);
builderX.AddInMemoryClients(Config.Clients);
builderX.AddInMemoryApiResources(Config.ApiResources);

// not recommended for production - you need to store your key material somewhere secure
builderX.AddDeveloperSigningCredential();

services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

        // register your IdentityServer with Google at https://console.developers.google.com
        // enable the Google+ API
        // set the redirect URI to https://localhost:5001/signin-google
        options.ClientId = "copy client ID from Google here";
        options.ClientSecret = "copy client secret from Google here";
    });

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run();
