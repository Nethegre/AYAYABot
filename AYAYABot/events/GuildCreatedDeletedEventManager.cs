using AYAYABot.util;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;

namespace AYAYABot.events
{
    internal class GuildCreatedDeletedEventManager
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Method used to handle the addition of a new guild to the bot
        public static async Task guildCreatedEvent(DiscordClient client, GuildCreateEventArgs eventArgs)
        {
            log.info("New guild [" + eventArgs.Guild.Name + "] was added to the bot.");

            //Call a method in the GuildChannelManager to add the guild to the dictionary
            await GuildChannelManager.guildAddedToBot(eventArgs.Guild);

            //Call a method in the GuildEmoteManager to add the guilds emotes to the dictionary
            await GuildEmoteManager.addGuildEmotesToDictionary(eventArgs.Guild);

        }

        //Method used to handle the deletion of a guild to the bot
        public static async Task guildDeletedEvent(DiscordClient client, GuildDeleteEventArgs eventArgs)
        {
            log.info("Guild [" + eventArgs.Guild.Name + "] was removed from the bot.");

            //Call a method in the GuildChannelManager to remove the guild from the dictionary
            await GuildChannelManager.guildRemoveFromBot(eventArgs.Guild);

            //Call a method in the GuildEmoteManager to remove the guilds emotes from the dictionary
            await GuildEmoteManager.removeGuildFromDictionary(eventArgs.Guild);

        }

    }
}
