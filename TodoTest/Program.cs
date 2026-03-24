using Microsoft.EntityFrameworkCore;
using Npgsql;
using TodoTest.Components;
using TodoTest.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Connexion PostgreSQL
string connectionString;
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (databaseUrl != null)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    connectionString = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = userInfo[0],
        Password = Uri.UnescapeDataString(userInfo[1]),
        SslMode = SslMode.Require
    }.ConnectionString;

    //builder.Services.AddDbContext<AppDbContext>(opt =>
    //    opt.UseNpgsql(connectionString));
}
else // Local/Debug --> SQL Server
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

    //builder.Services.AddDbContext<AppDbContext>(opt =>
    //    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

var app = builder.Build();

// Auto-migration au démarrage
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

if (System.Diagnostics.Debugger.IsAttached) // Pas utile et instable sur RENDER donc je le laisse pour mon debug
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(TodoTest.Client._Imports).Assembly);

await app.RunAsync();
