using Lab2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2;

public class Startup
{
    /// <summary>
    /// Конфигурация
    /// </summary>
    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);
        services.AddSingleton<PRNGImageGeneratorService>();
    }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
}