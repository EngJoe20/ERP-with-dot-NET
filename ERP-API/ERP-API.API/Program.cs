using ERP_API.API;
using ERP_API.Application;      // To access AddApplicationServices
using ERP_API.DataAccess;           // To access AddDataAccessServices
using ERP_API.DataAccess.Identity;
using Microsoft.AspNetCore.Builder; // Add this using directive
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI; // Add this using directive

namespace ERP_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            builder.Services.AddCustomSwaggerGen()
                .AddCustomCORS();



            builder.Services.AddHttpContextAccessor();



            var JwtSection = builder.Configuration.GetSection(JwtOptions.sectionName);

            builder.Services.Configure<JwtOptions>(JwtSection);

            builder.Services
                .AddDataAccessServices(builder.Configuration)
                .AddAuthenticationWithJWT();

            builder.Services.AddAuthorization();


            builder.Services.AddApplicationServices();



            var app = builder.Build();
            app.UseCors("mvc.app");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                _ = app.SeedRoles();
            }

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();

        }
    }
}