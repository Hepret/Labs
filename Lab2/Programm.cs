using Lab2.Extensions;
using Lab2.Generators;
using Lab2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2;

public static class Program
{
    /// <summary>
    ///     Сервисы
    /// </summary>
    private static readonly ServiceProvider Services;

    /// <summary>
    ///     Конфигурация
    /// </summary>
    private static readonly IConfiguration Configuration;

    static Program()
    {
        var config = new ConfigurationBuilder()
            .CreateConfigurationBuilder()
            .Build();
        var serviceProvider = new ServiceCollection()
            .UseStartup<Startup>(config)
            .BuildServiceProvider();
        Services = serviceProvider;
        Configuration = config;
    }

    /// <summary>
    ///     Точка входа в программу
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var imageGenerator = Services.GetRequiredService<PrngImageGeneratorService>();
        imageGenerator.AddGenerator<DefaultGenerator>();
        imageGenerator.AddGenerator<LinearCongruentialGenerator>();
        imageGenerator.CreateImages();

        var statisticTest = Services.GetRequiredService<PrngStatisticTest>();
        statisticTest.AddGenerator<DefaultGenerator>();
        statisticTest.AddGenerator<LinearCongruentialGenerator>();
        statisticTest.Run();
        
        Console.WriteLine("Success");
    }
}