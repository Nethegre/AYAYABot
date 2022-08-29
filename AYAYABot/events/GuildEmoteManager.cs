using AYAYABot.util;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AYAYABot.events
{
    internal class GuildEmoteManager
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //This dictionary will be used to store all of the emotes that have been found for each server
        public static Dictionary<ulong, List<DiscordEmoji>> guildEmoteDictionary = new Dictionary<ulong, List<DiscordEmoji>>();

        //Main method that initializes the collection of guild emotes and spawns a background thread to keep track of new emotes that are added
        public static async Task startEmoteCollecting(DiscordClient discordClient, GuildDownloadCompletedEventArgs eventArgs)
        {
            //Loop through the guilds from the client
            foreach (ulong guildId in discordClient.Guilds.Keys)
            {
                //Add the emotes to the dictionary
                guildEmoteDictionary.Add(guildId, new List<DiscordEmoji>(discordClient.Guilds[guildId].GetEmojisAsync().GetAwaiter().GetResult()));
            }

            log.info("Finished gathering guild emote information.");
        }

        //Method to add new emotes to the emote dictionary
        public static async Task updateGuildEmojis(DiscordClient discordClient, GuildEmojisUpdateEventArgs eventArgs)
        {
            //Check to make sure that the emote dictionary has an entry for the guild that made the update
            if (guildEmoteDictionary.ContainsKey(eventArgs.Guild.Id))
            {
                //Update the emoji list for the guild that made the change
                guildEmoteDictionary[eventArgs.Guild.Id] = new List<DiscordEmoji>(eventArgs.Guild.GetEmojisAsync().GetAwaiter().GetResult());

                log.info("Updated local guild emote information after detecting an external emote update.");
            }
            else
            {
                log.warn("The emote dictionary didn't contain a reference for a guild [" + eventArgs.Guild.Name + "] that made an update.");

                //Add the guild to the list instead of updating the emotes
                guildEmoteDictionary.Add(eventArgs.Guild.Id, new List<DiscordEmoji> (eventArgs.Guild.GetEmojisAsync().GetAwaiter().GetResult()));
            }
        }

        //Helper methods below

        public static DiscordEmoji retrieveAyayaEmoteForGuild(ulong guildId)
        {
            DiscordEmoji emoteId = null;

            if (guildEmoteDictionary.ContainsKey(guildId))
            {
                string[] ayayaEmoteValues = ConfigManager.config["ayayaEmoteNames"].Split(";");

                //The guild exists in the emote dictionary so we need to see if the emote list contains an emote that is an acceptable ayaya value
                foreach (DiscordEmoji emoji in guildEmoteDictionary[guildId])
                {
                    //Determine if the name is on the acceptable list
                    if (ayayaEmoteValues.Contains(emoji.Name.ToLower()))
                    {
                        emoteId = emoji;
                        break;
                    }
                }

                if (emoteId == null)
                {
                    //Failed to find an acceptable ayaya emote
                    log.info("Failed to find an ayaya emote for guild [" + guildId + "] out of the list [" + ayayaEmoteValues + "]");
                }
            }
            else
            {
                //Going to return 0 here because the guild didn't exist
                log.warn("Attempt to retrieve ayaya emote for guild [" + guildId + "] that doesn't exist in emote dictionary list.");
            }

            return emoteId;
        }

    }
}
