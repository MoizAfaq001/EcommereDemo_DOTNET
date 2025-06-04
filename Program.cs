using AutoMapper;
using Daraz101_Data;
using Daraz101_Services;
using Daraz101_Services.Mapping;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // -----------------------------------
        // 1. Configure Services
        // -----------------------------------

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(30)));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });

        builder.Services.AddControllers();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IUserProfileService, UserService>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";

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

        // ---- Infinite Loop Watchdog Middleware ----
        app.Use(async (context, next) =>
        {
            var cts = new CancellationTokenSource(TimeSpan.FromDays(30));
            var task = next();

            if (await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token)) != task)
            {
                var trace = Environment.StackTrace;

                var logPath = Path.Combine(AppContext.BaseDirectory, "timeout_stacktrace.log");
                await File.AppendAllTextAsync(logPath, $"[{DateTime.UtcNow}] Timeout\n{trace}\n\n");

                throw new TimeoutException("Request exceeded 30s timeout. Stack trace written to timeout_stacktrace.log");
            }
        });


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
