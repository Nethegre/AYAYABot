using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using AYAYABot.util;

namespace AYAYABot.events
{
    internal class GuildMemberAddedEventHandler
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Main method that will handle a new members joining the guild and will welcome them into the server
        public static async Task welcomeNewGuildMember(DiscordClient client, GuildMemberAddEventArgs eventArgs)
        {
            string welcomeImage = ConfigManager.config["welcomeImage"];
            string welcomeMessage = ConfigManager.config["welcomeMessage"];

            //Retreive the FileStream for the AYAYA image
            FileStream ayayaImage = ResourceManager.retrieveResource(welcomeImage);

            if (ayayaImage != null)
            {
                //Retreive the default channel for the guild
                DiscordChannel channel = eventArgs.Guild.GetDefaultChannel();

                //Build the message that should be sent
                DiscordMessageBuilder builder = new DiscordMessageBuilder();
                builder.WithContent($"{eventArgs.Member.Mention} " + welcomeMessage)
                    .WithAllowedMention(new UserMention(eventArgs.Member));
                builder.WithFile(ayayaImage);

                //Send the message
                await builder.SendAsync(channel);
            }
            else
            {
                log.warn("Failed to retreive welcome image [" + welcomeImage + "]");
            }
        }

    }
}
