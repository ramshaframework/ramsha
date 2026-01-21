using Ramsha;

var builder = WebApplication.CreateBuilder(args);

var ramsha = builder.Services.AddRamsha(ramsha =>
{
    ramsha
    .AddIdentity()
    .AddAccount()
    .AddAccountWebAuth()
    .AddSettingsManagement()
    .AddPermissions()
    .AddEFSqlServer();
});

builder.Services.AddRamshaDbContext<AppDbContext>();

var app = builder.Build();

app.MapDelete("test", () =>
{
    return "this is test endpoint";
});

app.UseRamsha();

app.Run();
