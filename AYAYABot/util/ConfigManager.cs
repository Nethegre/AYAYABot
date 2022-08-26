using Microsoft.Extensions.Configuration;

namespace AYAYABot.util
{
    internal class ConfigManager
    {
        public static IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
    }
}
