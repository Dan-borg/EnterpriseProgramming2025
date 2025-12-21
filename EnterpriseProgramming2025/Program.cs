using Domain.Interfaces;
using EnterpriseProgramming2025.Data;
using EnterpriseProgramming2025.Presentation.Factory;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity with roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC + custom view locations (your project already has this pattern)
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // Your existing custom locations, keep them if present in your project
        options.ViewLocationFormats.Add("/Presentation/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Presentation/Views/Shared/{0}.cshtml");
    });

// Factory + memory cache + keyed repos
builder.Services.AddSingleton<ImportItemFactory>();
builder.Services.AddMemoryCache();
builder.Services.AddKeyedScoped<ItemsRepository, ItemsInMemoryRepository>("memory");
builder.Services.AddKeyedScoped<ItemsRepository, ItemsDbRepository>("db");

// Approval filter
builder.Services.AddScoped<EnterpriseProgramming2025.Presentation.Filters.ApprovalFilterAttribute>();

var app = builder.Build();

// Migrate & seed admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    var adminEmail = "admin@site.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

// ensure wwwroot/images exists
var imagesDir = Path.Combine(app.Environment.WebRootPath, "images");
if (!Directory.Exists(imagesDir))
    Directory.CreateDirectory(imagesDir);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create Owner role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Owner"))
    {
        await roleManager.CreateAsync(new IdentityRole("Owner"));
    }

    // Create owner user
    var ownerEmail = "owner@site.com";
    var ownerPassword = "Owner123!";

    var ownerUser = await userManager.FindByEmailAsync(ownerEmail);
    if (ownerUser == null)
    {
        ownerUser = new IdentityUser
        {
            UserName = ownerEmail,
            Email = ownerEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(ownerUser, ownerPassword);
        await userManager.AddToRoleAsync(ownerUser, "Owner");
    }
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Owner"))
        await roleManager.CreateAsync(new IdentityRole("Owner"));

    var user = await userManager.FindByEmailAsync("owner@site.com");
    if (user != null && !await userManager.IsInRoleAsync(user, "Owner"))
        await userManager.AddToRoleAsync(user, "Owner");
}


app.Run();
