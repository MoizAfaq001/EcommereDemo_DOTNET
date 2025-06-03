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

        // Map controllers and routes
        app.MapControllers(); 
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}
