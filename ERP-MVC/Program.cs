using ERP_MVC.Services.Inventory.Package; // For PackageTypeService
using ERP_MVC.Services.Inventory.Product;  // For ProductService
using ERP_MVC.Services.Warehouse;        // For WarehouseService   
using ERP_MVC.Services.InventoryAdjustment;

namespace ERP_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================
            // 1. Add services to the container.
            // ==========================================
            builder.Services.AddControllersWithViews();

            // ✅ Register your Client-Side Services (The Bridge to API)
            // This reads "ApiSettings:BaseUrl" from appsettings.json automatically inside the Service
            builder.Services.AddHttpClient<ProductService>();
            builder.Services.AddHttpClient<PackageTypeService>();
            builder.Services.AddHttpClient<InventoryAdjustmentService>();
            builder.Services.AddHttpClient<WarehouseService>();

            var app = builder.Build();

            // ==========================================
            // 2. Configure the HTTP request pipeline.
            // ==========================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            // New .NET 9 Static Assets feature
            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Product}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}