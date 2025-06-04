//using AutoMapper;
//using Daraz101_Data;
//using Daraz101_Services;
//using Daraz101_Services.Mapping;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        var builder = WebApplication.CreateBuilder(args);

//        // -----------------------------------
//        // 1. Configure Services
//        // -----------------------------------

//        // Add DbContext
//        builder.Services.AddDbContext<ApplicationDbContext>(options =>
//            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//        // Add Identity
//        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//            .AddEntityFrameworkStores<ApplicationDbContext>()
//            .AddDefaultTokenProviders();

//        // Add AutoMapper - FIXED REGISTRATION
//        builder.Services.AddAutoMapper(typeof(MappingProfile)); // Use the profile type

//        // Add MVC and API Controllers with Anti-Forgery protection
//        builder.Services.AddControllersWithViews(options =>
//        {
//            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
//        });

//        // Add API Controller support
//        builder.Services.AddControllers(); // Required for [ApiController]

//        builder.Services.AddSession(options =>
//        {
//            options.IdleTimeout = TimeSpan.FromMinutes(20);
//            options.Cookie.HttpOnly = true;
//            options.Cookie.IsEssential = true;
//        });

//        // Register Application Services
//        builder.Services.AddScoped<IProductService, ProductService>();
//        builder.Services.AddScoped<ICartService, CartService>();
//        builder.Services.AddScoped<IOrderService, OrderService>();
//        builder.Services.AddScoped<IUserProfileService, UserService>();
//        builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//        // Configure Application Cookie
//        builder.Services.ConfigureApplicationCookie(options =>
//        {
//            options.LoginPath = "/Account/Login";
//            options.AccessDeniedPath = "/Account/AccessDenied";
//            // Add this for API authentication compatibility
//            options.Events.OnRedirectToLogin = context =>
//            {
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                return Task.CompletedTask;
//            };
//        });

//        var app = builder.Build();

//        // -----------------------------------
//        // 2. Seed Roles & Admin User
//        // -----------------------------------
//        //using (var scope = app.Services.CreateScope())
//        //{
//        //    var services = scope.ServiceProvider;
//        //    var context = services.GetRequiredService<ApplicationDbContext>();

//        //    try
//        //    {
//        //        //var context = services.GetRequiredService<ApplicationDbContext>();
//        //        await context.Database.MigrateAsync(); // Apply pending migrations

//        //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//        //        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

//        //        string[] roles = { "Admin", "User" };
//        //        foreach (var role in roles)
//        //        {
//        //            if (!await roleManager.RoleExistsAsync(role))
//        //                await roleManager.CreateAsync(new IdentityRole(role));
//        //        }

//        //        var adminEmail = "admin@daraz.com";
//        //        var adminUser = await userManager.FindByEmailAsync(adminEmail);

//        //        if (adminUser == null)
//        //        {
//        //            adminUser = new ApplicationUser
//        //            {
//        //                UserName = adminEmail,
//        //                Email = adminEmail,
//        //                EmailConfirmed = true,
//        //                FullName = "Admin User"
//        //            };

//        //            var result = await userManager.CreateAsync(adminUser, "Admin@123");
//        //            if (result.Succeeded)
//        //            {
//        //                await userManager.AddToRoleAsync(adminUser, "Admin");
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var logger = services.GetRequiredService<ILogger<Program>>();
//        //        logger.LogError(ex, "An error occurred during DB seeding or role creation.");
//        //    }

//        //    // Seed sample products if none exist
//        //    if (!context.Products.Any())
//        //    {
//        //        var sampleProducts = new List<Product>
//        //        {
//        //            new Product { Name = "Smartphone X", Price = 299.99m, ImageUrl = "/images/smartphone.jpg", IsFeatured = true, StockQuantity = 50, Description = "A powerful smartphone with modern features." },
//        //            new Product { Name = "Wireless Headphones", Price = 79.99m, ImageUrl = "/images/headphones.jpg", IsFeatured = false, StockQuantity = 100, Description = "High-quality sound with noise cancellation." },
//        //            new Product { Name = "Laptop Pro", Price = 899.00m, ImageUrl = "/images/laptop.jpg", IsFeatured = true, StockQuantity = 25, Description = "A high-performance laptop for professionals." },
//        //            new Product { Name = "Smartwatch Lite", Price = 149.49m, ImageUrl = "/images/smartwatch.jpg", IsFeatured = false, StockQuantity = 70, Description = "Stay connected and track your fitness." },
//        //            new Product { Name = "4K Monitor", Price = 329.00m, ImageUrl = "/images/monitor.jpg", IsFeatured = true, StockQuantity = 30, Description = "Crisp visuals with ultra HD resolution." }
//        //        };

//        //        context.Products.AddRange(sampleProducts);
//        //        await context.SaveChangesAsync();
//        //    }
//        //}



//        // -----------------------------------
//        // 3. Configure Middleware
//        // -----------------------------------

//        if (!app.Environment.IsDevelopment())
//        {
//            app.UseExceptionHandler("/Home/Error");
//            app.UseHsts();
//        }
//        else
//        {
//            app.UseDeveloperExceptionPage();
//        }


//        app.UseHttpsRedirection();
//        app.UseStaticFiles();

//        app.UseRouting();

//        // Add session middleware BEFORE authentication
//        app.UseSession();

//        app.UseAuthentication();
//        app.UseAuthorization();  // Must come after UseAuthentication()

//        // Map controllers
//        app.MapControllers();     // Maps attribute-routed API controllers
//        app.MapControllerRoute(   // Maps conventional MVC routes
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}");

//        await app.RunAsync();
//    }
//}

using AutoMapper;
using Daraz101_Data;
using Daraz101_Services;
using Daraz101_Services.Mapping;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // -----------------------------------
        // 1. Configure Services
        // -----------------------------------

        // Add DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Add AutoMapper
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        // Add MVC with Anti-Forgery protection
        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        // Add support for API controllers (optional if using APIs)
        builder.Services.AddControllers();

        // Add session support
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Add Database Developer Page Exception Filter
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Register Application Services
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IUserProfileService, UserService>();

        // Configure Application Cookie
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";

            // For API compatibility
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });

        var app = builder.Build();

        // -----------------------------------
        // 2. Configure Middleware
        // -----------------------------------

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();          
        app.UseAuthentication();   
        app.UseAuthorization();

        app.MapControllers(); 
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}
