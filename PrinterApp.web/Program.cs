using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PrinterApp.Data;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Services.Implementations;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.PermissionAuthorizationHandler;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

// Add Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

// Add Authorization
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPermissionRoleService, PermissionRoleService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<ICoreService, CoreService>();
builder.Services.AddScoped<ICartonService, CartonService>(); 
builder.Services.AddScoped<IRollDirectionService, RollDirectionService>();
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<IKnifeService, KnifeService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IRawMaterialService, RawMaterialService>();
builder.Services.AddScoped<IManufacturingAdditionService, ManufacturingAdditionService>();
builder.Services.AddScoped<IProductService, ProductService>();
var app = builder.Build();

// Seed Database with Roles and Permissions
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData(services);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use Request Localization
var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Seed Data Method
async Task SeedData(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

    // إنشاء الأدوار
    string[] roleNames = { "Admin", "Manager", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // إنشاء الصلاحيات الأساسية مع الأدوار
    if (!context.Permissions.Any())
    {
        // صلاحية إدارة المستخدمين
        var usersPermission = new Permission
        {
            Name = "Users Management",
            Code = "USERS",
            Description = "Manage system users",
            CreatedDate = DateTime.Now
        };
        context.Permissions.Add(usersPermission);
        await context.SaveChangesAsync();

        var userRoles = new List<PermissionRole>
        {
            new PermissionRole { PermissionId = usersPermission.Id, RoleName = "View", Description = "View users list", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = usersPermission.Id, RoleName = "Create", Description = "Create new users", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = usersPermission.Id, RoleName = "Edit", Description = "Edit user details", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = usersPermission.Id, RoleName = "Delete", Description = "Delete users", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = usersPermission.Id, RoleName = "Manage", Description = "Full access to users", CreatedDate = DateTime.Now }
        };
        context.PermissionRoles.AddRange(userRoles);

        // صلاحية إدارة الصلاحيات
        var permissionsPermission = new Permission
        {
            Name = "Permissions Management",
            Code = "PERMISSIONS",
            Description = "Manage system permissions",
            CreatedDate = DateTime.Now
        };
        context.Permissions.Add(permissionsPermission);
        await context.SaveChangesAsync();

        var permissionRoles = new List<PermissionRole>
        {
            new PermissionRole { PermissionId = permissionsPermission.Id, RoleName = "View", Description = "View permissions list", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = permissionsPermission.Id, RoleName = "Create", Description = "Create new permissions", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = permissionsPermission.Id, RoleName = "Edit", Description = "Edit permission details", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = permissionsPermission.Id, RoleName = "Delete", Description = "Delete permissions", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = permissionsPermission.Id, RoleName = "Manage", Description = "Full access to permissions", CreatedDate = DateTime.Now }
        };
        context.PermissionRoles.AddRange(permissionRoles);

        // صلاحية الإعدادات
        var settingsPermission = new Permission
        {
            Name = "Settings Management",
            Code = "SETTINGS",
            Description = "Manage system settings",
            CreatedDate = DateTime.Now
        };
        context.Permissions.Add(settingsPermission);
        await context.SaveChangesAsync();

        var settingsRoles = new List<PermissionRole>
        {
            new PermissionRole { PermissionId = settingsPermission.Id, RoleName = "View", Description = "View settings", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = settingsPermission.Id, RoleName = "Manage", Description = "Full access to settings", CreatedDate = DateTime.Now }
        };
        context.PermissionRoles.AddRange(settingsRoles);

        // صلاحية التقارير
        var reportsPermission = new Permission
        {
            Name = "Reports",
            Code = "REPORTS",
            Description = "Access system reports",
            CreatedDate = DateTime.Now
        };
        context.Permissions.Add(reportsPermission);
        await context.SaveChangesAsync();

        var reportRoles = new List<PermissionRole>
        {
            new PermissionRole { PermissionId = reportsPermission.Id, RoleName = "View", Description = "View reports", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = reportsPermission.Id, RoleName = "Export", Description = "Export reports", CreatedDate = DateTime.Now },
            new PermissionRole { PermissionId = reportsPermission.Id, RoleName = "Manage", Description = "Full access to reports", CreatedDate = DateTime.Now }
        };
        context.PermissionRoles.AddRange(reportRoles);

        await context.SaveChangesAsync();
    }

    // إنشاء الإعدادات الافتراضية
    if (!context.SystemSettings.Any())
    {
        var defaultSettings = new List<SystemSetting>
        {
            new SystemSetting { Key = "ApplicationName", Value = "Permission App", Description = "Application display name", LastModified = DateTime.Now },
            new SystemSetting { Key = "DefaultLanguage", Value = "en", Description = "Default system language", LastModified = DateTime.Now },
            new SystemSetting { Key = "Theme", Value = "light", Description = "UI theme", LastModified = DateTime.Now },
            new SystemSetting { Key = "ItemsPerPage", Value = "10", Description = "Number of items per page", LastModified = DateTime.Now },
            new SystemSetting { Key = "EnableEmailNotifications", Value = "true", Description = "Enable email notifications", LastModified = DateTime.Now },
            new SystemSetting { Key = "SessionTimeout", Value = "60", Description = "Session timeout in minutes", LastModified = DateTime.Now },
            new SystemSetting { Key = "DateFormat", Value = "dd/MM/yyyy", Description = "Date display format", LastModified = DateTime.Now },
            new SystemSetting { Key = "TimeFormat", Value = "HH:mm", Description = "Time display format", LastModified = DateTime.Now }
        };
        context.SystemSettings.AddRange(defaultSettings);
        await context.SaveChangesAsync();
    }

    // إنشاء مستخدم Admin افتراضي
    var adminEmail = "admin@admin.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // إنشاء مستخدم Manager افتراضي
    var managerEmail = "manager@test.com";
    var managerUser = await userManager.FindByEmailAsync(managerEmail);

    if (managerUser == null)
    {
        managerUser = new ApplicationUser
        {
            UserName = managerEmail,
            Email = managerEmail,
            FirstName = "Manager",
            LastName = "Test",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(managerUser, "Manager@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(managerUser, "Manager");

            // منح Manager صلاحيات محددة
            var usersPermission = context.Permissions.FirstOrDefault(p => p.Code == "USERS");
            if (usersPermission != null)
            {
                var viewRole = context.PermissionRoles.FirstOrDefault(r => r.PermissionId == usersPermission.Id && r.RoleName == "View");
                var editRole = context.PermissionRoles.FirstOrDefault(r => r.PermissionId == usersPermission.Id && r.RoleName == "Edit");

                if (viewRole != null)
                {
                    context.UserPermissions.Add(new UserPermission
                    {
                        UserId = managerUser.Id,
                        PermissionId = usersPermission.Id,
                        PermissionRoleId = viewRole.Id,
                        GrantedDate = DateTime.Now
                    });
                }

                if (editRole != null)
                {
                    context.UserPermissions.Add(new UserPermission
                    {
                        UserId = managerUser.Id,
                        PermissionId = usersPermission.Id,
                        PermissionRoleId = editRole.Id,
                        GrantedDate = DateTime.Now
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}