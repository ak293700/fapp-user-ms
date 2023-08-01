using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Configure
{
    public const string CorsPolicy = "CorsPolicy"; // The name of the cors policy.

    public static WebApplicationBuilder ConfigureInfrastructure(this WebApplicationBuilder builder)
    {
        ConfigureCors(builder.Services);

        return builder;
    }

    private static void ConfigureCors(IServiceCollection services)
    {
        services
            .AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    bld =>
                    {
                        bld.WithOrigins("*") // TODO: Change in prod to the api gateway address
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin()
                            .WithExposedHeaders("Content-Disposition");
                    });
            });
    }
    
    
}