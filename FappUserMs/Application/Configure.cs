using Application.Repositories;
using Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Configure
{
    public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
    {
        #region Services

        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<FriendshipService>();
        builder.Services.AddScoped<UserService>();

        #endregion

        #region Repositories

        builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<FriendshipRepository>();

        #endregion

        return builder;
    }
}