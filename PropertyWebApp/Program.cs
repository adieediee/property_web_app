using PropertyWebApp.Components;
using Microsoft.EntityFrameworkCore;
using PropertyWebApp.Data;
using PropertyWebApp;

using PropertyWebApp.Models.Services;
using PropertyWebApp.Models.Services.PropertyWebApp.Services; 

var builder = WebApplication.CreateBuilder(args);

// Services:
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Services pre databazu:
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<IssueService>();
// Context factory, solution pre konflikty
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// DbContext a pripojenie
//builder.Services.AddDbContext<AppDbContext>(options =>
   // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


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
    // Ak databáza ešte neexistuje
    dbContext.Database.EnsureDeleted(); // ZMANAZNIE DATABÁZY!!! (pre vývoj)
    dbContext.Database.EnsureCreated();

    // Naseedovanie databázy
    await DatabaseSeeder.Seed(app.Services);
}

app.Run();
