using ERP_API.DataAccess.DataContext;
using ERP_API.DataAccess.DataContext.ERP_API.DataAccess.DataContext;
using ERP_API.DataAccess.Entities.User;
using ERP_API.DataAccess.Identity;
using ERP_API.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess
{
    public static class DataAccessExtensions
    {

        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Register DbContext with SQL Server
            services.AddDbContext<ErpDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;


            })
               .AddEntityFrameworkStores<ErpDBContext>();

            // 2. Register UnitOfWork
            services.AddScoped<IErpUnitOfWork, ErpUnitOfWork>();

            return services;
        }

        public static IServiceCollection AddAuthenticationWithJWT(this IServiceCollection services)
        {
            IOptions<JwtOptions> jwtOptions = services
                .BuildServiceProvider()
                .GetRequiredService<IOptions<JwtOptions>>();

            var encodedKey = Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey);
            SecurityKey key = new SymmetricSecurityKey(encodedKey);



            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })

                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtOptions.Value.Issuer,
                        ValidAudience = jwtOptions.Value.Audience,
                        IssuerSigningKey = key
                    };
                });

            services.AddScoped<ITokenManager, TokenManager>();
            return services;
        }


        public static async Task SeedRoles(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {

                    var services = scope.ServiceProvider;
                    var dbContext = services.GetRequiredService<ErpDBContext>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    List<string> roles = new List<string>
                {
                    "admins","managers","users"
                };

                    if (!await dbContext.Database.CanConnectAsync())
                    {
                        await dbContext.Database.MigrateAsync();
                    }

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                            await roleManager.CreateAsync(new IdentityRole(role));
                    }

                }
            }
        }
    }
}
