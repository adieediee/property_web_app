using PropertyWebApp.Components;
using Microsoft.EntityFrameworkCore;
using PropertyWebApp.Data;
using PropertyWebApp;

using PropertyWebApp.Models.Services;
using PropertyWebApp.Models.Services.PropertyWebApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using PropertyWebApp.Components.Pages.ViewModels;



var builder = WebApplication.CreateBuilder(args);

// Services:
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddSingleton<UserStateService>();

// Services pre databazu:
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<IssueService>();
builder.Services.AddScoped<RentalService>();
// Context factory, solution pre konflikty
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// DbContext a pripojenie
//builder.Services.AddDbContext<AppDbContext>(options =>
// options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(provider =>
{
    var dbContextFactory = provider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    return dbContextFactory.CreateDbContext();
});


// Registrácia Identity služieb
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
//velkost suboru
builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 15 * 1024 * 1024; // 15 MB
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 15 * 1024 * 1024; // 15 MB
});
Console.WriteLine($"MaxRequestBodySize: {builder.Configuration["Kestrel:Limits:MaxRequestBodySize"]}");

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

//MVVM
builder.Services.AddScoped<IssueScreenViewModel>();
builder.Services.AddScoped<TenantDashboardViewModel>();
builder.Services.AddScoped<PropertyScreenViewModel>();
builder.Services.AddScoped<EditIssueViewModel>();
builder.Services.AddScoped<HomePageViewModel>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

//roly pre authorized, nech sa neprihlaseny nedostane na dhasboard

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});
//az tu to konci 


var app = builder.Build();

app.MapRazorPages();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();




app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    //delete pre vyvoj
    //dbContext.Database.EnsureDeleted();
    // Zabezpeè, že databáza existuje
    dbContext.Database.Migrate();

    // AK je databaya prazdna
    if (!dbContext.Users.Any() && !dbContext.Roles.Any()) // Kontroluj pod¾a tabuliek, ktoré seeduješ
    {
        // Naseeduj databázu, ak je prázdna
       await DatabaseSeeder.Seed(app.Services);
    }
}
using (var scope = app.Services.CreateScope())
{
    var userStateService = scope.ServiceProvider.GetRequiredService<UserStateService>();
    var authenticationStateProvider = scope.ServiceProvider.GetRequiredService<AuthenticationStateProvider>();

    // Inicializuj meno používate¾a
   // await userStateService.InitializeAsync(authenticationStateProvider);
}



app.Run();
