using System.Text;
using Application.Common.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class Configure
{
    public const string CorsPolicy = "CorsPolicy"; // The name of the cors policy.


    public static WebApplicationBuilder ConfigureInfrastructure(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;

        #region DbContext

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        #endregion

        ConfigureCors(services);
        ConfigureAuthentication(builder);

        // Ensure that the migrations are run
        ApplicationDbContext.RunMigrations(builder.Configuration);

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

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        IConfiguration configuration = builder.Configuration;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8
                            .GetBytes(configuration.GetValue<string>(AuthService.AuthTokenLocation)
                                      ?? throw new Exception("Token not found"))
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }
}