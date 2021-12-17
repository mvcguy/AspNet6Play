
using PlayConnectServer.AppConfig;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var clients = Config.Clients;
services.AddIdentityServer(options =>
{
    
}).AddInMemoryApiScopes(Config.ApiScopes)
.AddInMemoryClients(clients);

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseIdentityServer();

app.Run();
