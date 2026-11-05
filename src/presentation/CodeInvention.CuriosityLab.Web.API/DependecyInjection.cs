using CodeInvention.CuriosityLab.Application.Common.Abstractions;
using CodeInvention.CuriosityLab.Persistence;
using CodeInvention.CuriosityLab.Web.API.Middlewares;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CodeInvention.CuriosityLab.Web.API
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            //Swagger Configuration
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Architecture Testing"
                });
                options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                //Specifing that the Bearer token must be included in the header for API calls
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });

                options.CustomSchemaIds(s => s.FullName.Replace("+", "."));
            });

            services.AddSignalR();
            services.AddHttpContextAccessor();
            return services;
        }
        public static WebApplication UsePresentation(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.UseSwagger(options =>
                {
                    options.SerializeAsV2 = true;
                });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                    options.DocExpansion(DocExpansion.None);
                    options.DefaultModelsExpandDepth(-1);
                });
            }

            using (var scope = app.Services.CreateScope())
            {
                var initializer = (ApplicationDbContextInitializer)scope.ServiceProvider.GetRequiredService<IApplicationDbContextInitializer>();
                var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                initializer.Initialize(context);
            }
            return app;
        }
    }
}
