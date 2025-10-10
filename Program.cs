using Microsoft.EntityFrameworkCore;
using SMS.DataContext;
using SMS.IRepository;
using SMS.Repository;
using Microsoft.AspNetCore.Identity;
using SMS.Data;
using SMS.Models;
using SMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));  

// builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configure Identity with your custom user and default roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();


// 1. Configure IOptions to read the TermiiSettings section from appsettings.json
builder.Services.Configure<TermiiSettings>(builder.Configuration.GetSection("TermiiSettings"));
// 2. Add IHttpClientFactory to make HTTP requests efficiently
builder.Services.AddHttpClient();
// 3. Register your new SMS service for dependency injection
builder.Services.AddScoped<ISmsService, SmsService>();

// Configure cookie settings to redirect to your new login page
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Add Authentication middleware before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

// ... just before app.Run()
// --- Safer Database Seeding ---
// This prevents the app from crashing if the database is not ready or if there's another seeding issue.
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DbInitializer.SeedRolesAndAdminAsync(services);
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during database initialization.");
}
// --- End Safer Seeding ---

// Required for the scaffolded Identity pages
app.MapRazorPages();


app.Run();
