using System;
using DSharpPlus;

namespace AYAYABot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync();
        }

        static async Task MainAsync()
        {
            //Create the discord config
            DiscordConfiguration discordConfig = new DiscordConfiguration();

            //Retrieve the auth token here
            discordConfig.Token = "";
            discordConfig.TokenType = TokenType.Bot;
            //Not sure if we need to change the intents in the future
            discordConfig.Intents = DiscordIntents.AllUnprivileged;

            //Create the discord client based on the config
            DiscordClient discord = new DiscordClient(discordConfig);

            //Wait for the discord connection to happen
            await discord.ConnectAsync();

            //Allow this thread to go into the background
            await Task.Delay(-1);



        }
    }
}