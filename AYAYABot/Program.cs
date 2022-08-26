using Microsoft.Extensions.Configuration;
using DSharpPlus;
using AYAYABot.util;
using AYAYABot.events;

namespace AYAYABot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            //Create an instance of the log manager class
            LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Create the discord config
            DiscordConfiguration discordConfig = new DiscordConfiguration();

            //Retrieve the auth token here
            discordConfig.Token = LoadBotToken.loadBotTokenFromFile();
            discordConfig.TokenType = TokenType.Bot;
            //Not sure if we need to change the intents in the future
            discordConfig.Intents = DiscordIntents.AllUnprivileged;

            //Create the discord client based on the config
            DiscordClient discord = new DiscordClient(discordConfig);

            //Add lambda events below

            //Once the guild download has completed we are able to gather the emotes from the guilds
            discord.GuildDownloadCompleted += GuildEmoteManager.startEmoteCollecting;

            //If a discord emoji is updated send it to the emote gatherer
            discord.GuildEmojisUpdated += GuildEmoteManager.updateGuildEmojis;

            //Run our custom message created event handler
            discord.MessageCreated += MessageCreatedEventHandler.messageCreated;


            //Wait for the discord connection to happen
            await discord.ConnectAsync();

            //Allow this thread to go into the background
            await Task.Delay(-1);
        }
    }
}