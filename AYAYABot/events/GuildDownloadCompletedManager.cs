using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using AYAYABot.background;

namespace AYAYABot.events
{
    internal class GuildDownloadCompletedManager
    {
        //This method will call any other classes/methods that need to be triggered when the guilds are finished downloading
        public static async Task GuildDownloadCompleted(DiscordClient client, GuildDownloadCompletedEventArgs eventArgs)
        {
            //These are intentionally run syncronously so that the readyForBackground is only triggered after they are finished

            //Call the emote manager to collect emotes from guilds
            GuildEmoteManager.startEmoteCollecting(client, eventArgs);

            //Call the channel manager to gather channels from guilds
            GuildChannelManager.startChannelCollecting(client, eventArgs);

            //Set the flag on the background task service to allow background threads to start
            MainBackground.readyForBackgroundTrue();

        }
    }
}
