using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Имя метода конфигурации сервисов
    /// </summary>
    private const string ConfigureServicesMethodName = "ConfigureServices";

    /// <summary>
    /// Использует класс-конфигуратор
    /// </summary>
    public static IServiceCollection UseStartup<TStartup>(this IServiceCollection services,
        IConfiguration configuration)
        where TStartup : class
    {
        var startupType = typeof(TStartup);
        var cfgServicesMethod =
            startupType.GetMethod(ConfigureServicesMethodName, new Type[] { typeof(IServiceCollection) });
        var hasConfigCtor = startupType.GetConstructor(new Type[] { typeof(IConfiguration) }) != null;
        var startup = hasConfigCtor
            ? (TStartup)Activator.CreateInstance(typeof(TStartup), configuration)!
            : (TStartup)Activator.CreateInstance(typeof(TStartup), null)!;

        cfgServicesMethod?.Invoke(startup, new object[] { services });

        return services;
    }
}