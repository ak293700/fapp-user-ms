using System.Text;
using Application.Common.Interfaces;
using Application.Services;
using FappCommon.Exceptions.InfrastructureExceptions.ConfigurationExceptions.Base;
using FappCommon.Implementations.ICurrentUserServices;
using FappCommon.Interfaces.ICurrentUserServices;
using FappCommon.Mongo4Test;
using FappCommon.Mongo4Test.Implementations;
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

        MongoDbOptions options = new()
        {
            ConnectionStringName = "UserMongoDb",
            DatabaseName = "users"
        };
        services.AddMongoDbContext<IApplicationDbContext, ApplicationDbContext>(options);
        BaseMongoDbContext.RunMigrations<ApplicationDbContext>(options, builder.Configuration);

        #endregion

        #region Services

        services.AddScoped<ICurrentUserServiceString, CurrentUserServiceStringImpl>();

        #endregion

        # region Others

        services.AddHttpContextAccessor();
        ConfigureCors(services);
        ConfigureAuthentication(builder);

        # endregion


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
                                      ?? throw ConfigurationException.ValueNotFoundException.Instance)
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }
}