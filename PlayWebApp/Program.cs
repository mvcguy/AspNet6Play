using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlayWebApp.Services;
using PlayWebApp.Services.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (ctx.StockItems.Count() <= 0)
    {

        SeedDatabase.Run(ctx);
    }

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
