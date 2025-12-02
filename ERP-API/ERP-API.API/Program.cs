using ERP_API.DataAccess;           // To access AddDataAccessServices
using ERP_API.Application;      // To access AddApplicationServices
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Builder; // Add this using directive
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

            // OpenAPI / Swagger Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); // or AddOpenApi() depending on .NET version

           
            builder.Services
                .AddDataAccessServices(builder.Configuration) // Wires up UnitOfWork
                .AddApplicationServices();                    // Wires up ProductService

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();   // If using standard Swagger
                app.UseSwaggerUI(); // This requires Microsoft.AspNetCore.Builder
                // OR app.MapOpenApi(); if using .NET 9 OpenApi
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}