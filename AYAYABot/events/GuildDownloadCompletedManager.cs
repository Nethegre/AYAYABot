using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AYAYABot.events
{
    internal class GuildDownloadCompletedManager
    {
        //This method will call any other classes/methods that need to be triggered when the guilds are finished downloading
        public static async Task GuildDownloadCompleted(DiscordClient client, GuildDownloadCompletedEventArgs eventArgs)
        {
            //Call the emote manager to collect emotes from guilds
            await GuildEmoteManager.startEmoteCollecting(client, eventArgs);

            //Call the channel manager to gather channels from guilds
            await GuildChannelManager.startChannelCollecting(client, eventArgs);


        }
    }
}
