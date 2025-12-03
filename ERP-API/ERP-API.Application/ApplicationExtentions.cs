using ERP_API.Application.Interfaces;
using ERP_API.Application.Interfaces.Inventory;
using ERP_API.Application.Interfaces.User;
using ERP_API.Application.Services;
using ERP_API.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register your Services here
            services.AddScoped<IProductService, ProductService>();



            services.AddScoped<IPackageTypeService, PackageTypeService>();

            services.AddScoped<IWarehouseService, WarehouseService>();

            services.AddScoped<IInventoryAdjustmentService, InventoryAdjustmentService>();

            services.AddScoped<IAccountService, AccountService>();

            return services;
        }
    }
}
