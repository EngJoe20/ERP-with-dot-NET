using Microsoft.OpenApi.Models;

namespace ERP_API.API
{
    public static class APIExtensions
    {
        public static IServiceCollection AddCustomCORS(this IServiceCollection services)
        {
            services.AddCors(option =>
            {
                option.AddPolicy("mvc.app", policy => {
                    policy

                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    ;

                });

            });
            return services;
        }

        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Authorization Token. Token Form Is \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            return services;
        }


        public static IServiceCollection AddCustom2(this IServiceCollection services)
        {


            return services;
        }

        public static IServiceCollection AddCustom3(this IServiceCollection services)
        {


            return services;
        }

    }
}