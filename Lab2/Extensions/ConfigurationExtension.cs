using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace Lab2.Extensions;

public static class ConfigurationExtension
{
    public static IConfigurationBuilder CreateConfigurationBuilder(this IConfigurationBuilder configurationBuilder)
    {
        var resourcePath = (new DirectoryInfo(Environment.CurrentDirectory))!
            .Parent!
            .Parent!
            .Parent!
            .FullName;
        var paths = new Dictionary<string, string>
        {
            ["resourcesPath"] = resourcePath
        };

        return configurationBuilder
            .AddJsonFile("appSettings.json", false, false)
            .AddInMemoryCollection();
    }
}