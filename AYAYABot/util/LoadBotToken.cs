using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace AYAYABot.util
{
    internal class LoadBotToken
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static public string loadBotTokenFromFile()
        {
            string token = "";
            string tokenFile = ConfigManager.config["botTokenFile"];

            //Determine if the bottoken file exists
            if (File.Exists(tokenFile))
            {
                //Retrieve the token and trim off any whitespace characters
                token = File.ReadAllText(tokenFile).Trim();
            }
            else
            {
                log.error("Failed to find the token file [" + tokenFile + "]");
            }

            return token;
        }
    }
}
