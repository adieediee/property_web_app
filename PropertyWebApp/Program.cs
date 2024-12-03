using PropertyWebApp.Components;
using Microsoft.EntityFrameworkCore;
using PropertyWebApp.Data;
using PropertyWebApp;

using PropertyWebApp.Models.Services; // Namespace, kde máte PropertyService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register PropertyService
builder.Services.AddSingleton<PropertyService>(); // Alebo AddScoped, ak závisí na DbContext

// DbContext a pripojenie
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 15 * 1024 * 1024; // 15 MB
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 15 * 1024 * 1024; // 15 MB
});
Console.WriteLine($"MaxRequestBodySize: {builder.Configuration["Kestrel:Limits:MaxRequestBodySize"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Ak databáza ešte neexistuje, vytvor ju
    dbContext.Database.EnsureDeleted(); // Toto zmaže existujúcu databázu (iba pre vývoj!)
    dbContext.Database.EnsureCreated();

    // Zavolanie seedovacej metódy
    await DatabaseSeeder.Seed(app.Services);
}

app.Run();
