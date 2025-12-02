using ERP_MVC.Services.Inventory.Package; // For PackageTypeService
using ERP_MVC.Services.Inventory.Product;  // For ProductService
using ERP_MVC.Services.InventoryAdjustment;
using ERP_MVC.Services.User;
using ERP_MVC.Services.Warehouse;
using Microsoft.AspNetCore.Authentication.Cookies;        // For WarehouseService   

namespace ERP_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddControllersWithViews();

        
            builder.Services.AddHttpClient<ProductService>();
            builder.Services.AddHttpClient<PackageTypeService>();
            builder.Services.AddHttpClient<InventoryAdjustmentService>();
            builder.Services.AddHttpClient<WarehouseService>();
            builder.Services.AddHttpClient<AccountService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

                options.LoginPath = "/Account/login";
                options.AccessDeniedPath = "/";
            });
            builder.Services.AddAuthorization();


            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

           
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            // New .NET 9 Static Assets feature
            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}")
                .WithStaticAssets();

            app.Run();
        }
    }
}