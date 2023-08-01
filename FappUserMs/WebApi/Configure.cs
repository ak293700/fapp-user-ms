using Application.Common.Interfaces;
using Infrastructure;

namespace WebApi;

public static class Configure
{
    public static WebApplicationBuilder ConfigureWebApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        #region Services
        
        builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        #endregion
        
        return builder;
    }

}