using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using AYAYABot.util;

namespace AYAYABot.events
{
    //This is the general class that will determine what to do when a message created event is received by the bot
    internal class MessageCreatedEventHandler
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static public async Task messageCreated(DiscordClient discordClient, MessageCreateEventArgs message)
        {
            //Run message reaction methods on the received message
            sendAyayaEmote(message);

        }

        //Message reaction methods below

        private static async void sendAyayaEmote(MessageCreateEventArgs message)
        {
            //Determine if the guild that the message was sent in has the ayaya emote or the appropriate ayaya phrase
            foreach (string ayayaPhrase in ConfigManager.config["ayayaEmoteNames"].Split(";"))
            {
                //We found a message with ayaya in it so we need to react with an ayaya emote
                if (message.Message.Content.ToLower().Contains(ayayaPhrase.ToLower()))
                {
                    //Retrieve the ayaya emote for the guild that the message was sent in
                    DiscordEmoji ayayaEmote = GuildEmoteManager.retrieveAyayaEmoteForGuild(message.Channel.Guild.Id);

                    //Verify that the ayayaEmote id is not null, if it was it would mean that there is no ayaya emote for this guild
                    if (ayayaEmote != null)
                    {
                        //Respond to the message with emote
                        log.debug("Responding to message [" + message.Message.Id + "] with an ayaya emote.");

                        await message.Message.CreateReactionAsync(ayayaEmote);
                    }

                    break; //Don't need to check against the other ayaya triggers because we found one already
                }
            }
        }

    }
}
