namespace Defast.Bot.Api.Configurations;

public static partial class HostConfigurations
{
    public static ValueTask<WebApplicationBuilder> ConfigureAsync(this WebApplicationBuilder builder)
    {
        builder
            .AddCaching()
            .AddBusinessLogicInfrastructure()
            .AddPersistence()
            .AddExposers()
            .AddDevTools()
            .AddMappers();
        
        return new(builder);
    }

    public static ValueTask<WebApplication> ConfigureAsync(this WebApplication app)
    {

        app
            .UseExposers();
        
        return new(app);
    }
}