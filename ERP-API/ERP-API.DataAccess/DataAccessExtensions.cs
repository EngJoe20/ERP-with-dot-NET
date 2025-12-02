using ERP_API.DataAccess.Interfaces;
using ERP_API.DataAccess.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_API.DataAccess.DataContext.ERP_API.DataAccess.DataContext;

namespace ERP_API.DataAccess
{
    public static class DataAccessExtensions
    {
        //public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        //{
        //    // --- MOCK SETUP ---
        //    // We don't need SQL Server connection strings yet.

        //    // Just register the UnitOfWork. 
        //    // Since our UnitOfWork initializes the Mock Repositories internally, this is all we need.
        //    services.AddScoped<IErpUnitOfWork, ErpUnitOfWork>();

        //    // --- REAL DB SETUP (For Later - Keep this commented out) ---
        //    /*
        //    services.AddDbContext<ErpDBContext>(options =>
        //    {
        //        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        //    });
        //    */

        //    return services;
        //}

        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Register DbContext with SQL Server
            services.AddDbContext<ErpDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // 2. Register UnitOfWork
            services.AddScoped<IErpUnitOfWork, ErpUnitOfWork>();

            return services;
        }
    }
}
