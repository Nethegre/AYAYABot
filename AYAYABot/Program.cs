using Microsoft.Extensions.Configuration;
using DSharpPlus;
using AYAYABot.util;
using AYAYABot.events;

namespace AYAYABot
{
    class Program
    {
        //Class level variables
        private static DiscordClient _client;

        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {

            //Create the discord config
            DiscordConfiguration discordConfig = new DiscordConfiguration();

            //Retrieve the auth token here
            discordConfig.Token = LoadBotToken.loadBotTokenFromFile();
            discordConfig.TokenType = TokenType.Bot;
            //Not sure if we need to change the intents in the future
            discordConfig.Intents = DiscordIntents.AllUnprivileged;

            //Create the discord client based on the config
            _client = new DiscordClient(discordConfig);

            //Add lambda events below ----------------------------------------------------

            //Once the guild download has completed we are able to gather the emotes from the guilds
            _client.GuildDownloadCompleted += GuildDownloadCompletedManager.GuildDownloadCompleted;

            //If a discord emoji is updated send it to the emote gatherer
            _client.GuildEmojisUpdated += GuildEmoteManager.updateGuildEmojis;

            //If a discord channel is added/updated run this
            _client.ChannelCreated += GuildChannelManager.channelCreatedEvent;

            //Run our custom message created event handler
            _client.MessageCreated += MessageCreatedEventHandler.messageCreated;

            //Run the welcome event when a guild member is added
            _client.GuildMemberAdded += GuildMemberAddedEventHandler.welcomeNewGuildMember;

            //End lambda events ----------------------------------------------------------

            //Wait for the discord connection to happen
            await _client.ConnectAsync();

            //Allow this thread to go into the background
            await Task.Delay(-1);
        }

        //This method will be used to shutdown the bot
        public static async Task disconnectAndShutdown()
        {
            log.info("Shutting down the discord bot.");

            //Disconnect the bot cleanly from the discord server
            await _client.DisconnectAsync();

            //Dispose of the client resources
            _client.Dispose();

            //Send a sigterm signal to this program
            System.Environment.Exit(-1);
        }
    }
}