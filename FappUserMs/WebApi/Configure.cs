namespace WebApi;

public static class Configure
{
    public static WebApplicationBuilder ConfigureWebApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        
        return builder;
    }

}