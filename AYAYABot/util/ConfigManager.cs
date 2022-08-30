using Microsoft.Extensions.Configuration;

namespace AYAYABot.util
{
    internal class ConfigManager
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Configure the config json file
        public static IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
    
        //Helper methods below

        public static string[] getConfigList(string configSectionName)
        {
            string[] configList = new string[0];

            try
            {
                //Retreive the config section by the name provided
                IConfigurationSection configSection = config.GetSection(configSectionName);

                //Thank you to https://stackoverflow.com/questions/41329108/asp-net-core-get-json-array-using-iconfiguration for helping me figure this out
                configList = configSection.GetChildren().ToArray().Select(c => c.Value).ToArray();

                log.info("Retreived config section [" + configSectionName + "] with [" + configList.Length + "] items.");

            }
            catch (Exception ex)
            {
                log.error("Exception while attempting to retreive config section [" + configSectionName + "]: Ex. [" + ex.Message + "]");
            }

            return configList;
        }
    }
}
