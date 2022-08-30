using AYAYABot.util;
using AYAYABot.events;
using DSharpPlus.Entities;
using DSharpPlus;

namespace AYAYABot.background
{
    internal class RandomTextToSpeech
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Retrieve min and max timer values from config
        private static int randomTTSMaxTimer = Convert.ToInt32(ConfigManager.config["randomTTSMaxTimer"]);
        private static int randomTTSMinTimer = Convert.ToInt32(ConfigManager.config["randomTTSMinTimer"]);
        private static int randomTTSEventChance = Convert.ToInt32(ConfigManager.config["randomTTSEventChance"]);
        private static string[] randomTTSMessages = ConfigManager.getConfigList("randomTTSMessages");
        private static string randomTTSRestrictedTimeStart = ConfigManager.config["randomTTSRestrictedTimeStart"];
        private static string randomTTSRestrictedTimeEnd = ConfigManager.config["randomTTSRestrictedTimeEnd"];

        //Start of processing method
        public static async Task RandomTTS(DiscordClient client)
        {
            //Always surround your background threads with try catch and logging so you can see the exceptions, otherwise they don't get shown
            try
            {
                //Log the start of the random TTS task
                log.info("Starting the random TTS background process");

                //Loop till the shutdown bool tells the process to stop
                while (!MainBackground.shutdownProcess())
                {
                    //Generate randomized wait timer based on config
                    Random random = new Random();
                    int delay = random.Next(randomTTSMinTimer, randomTTSMaxTimer);

                    log.info("Sleeping for [" + delay + "] ms before attempting next tts event");

                    //Sleep for delay amount
                    Thread.Sleep(delay);

                    //Check for restricted tts time and skip processing if it is during off hours for tts
                    if (DateTime.Now > Convert.ToDateTime(randomTTSRestrictedTimeStart) && DateTime.Now < Convert.ToDateTime(randomTTSRestrictedTimeEnd))
                    {
                        //Don't process tts events as it is during "sleeping" hours
                        log.debug("Prevented a tts event due to it being a time [" + DateTime.Now + "] during sleeping hours");
                        continue;
                    }

                    //Roll a randomizer based on settings to see if ayaya tts event actually happens
                    int eventNum = random.Next(0, 100);

                    //Event happens if num is less than or equal to event chance setting
                    if (eventNum <= randomTTSEventChance)
                    {
                        log.info("Starting AYAYA TTS event.");

                        //Randomize the guild that the AYAYA event is happening in
                        int guildNum = random.Next(0, GuildChannelManager.textChannels.Count);

                        ulong guildId = GuildChannelManager.textChannels.ElementAt(guildNum).Key;

                        //Retreive the list of channels with perms from the guild
                        List<DiscordChannel> channelsWithPerms = GuildChannelManager.retrieveDiscordChannelsByTypeGuildIdAndPerms(guildId, ChannelType.Text, client, Permissions.SendTtsMessages);

                        //TEMP adding this fix in so that at least one channel is retreived that has permissions, the permissions are not working
                        //Retreive the default text channel for the selected guild
                        DiscordChannel tempChannel = GuildChannelManager.defaultTextChannels[guildId];
                        channelsWithPerms.Add(tempChannel);

                        //Check if the guild has any channels with the correct permissions
                        if (channelsWithPerms.Count > 0)
                        {
                            //Randomize the text channel to send the tts
                            int channelNum = random.Next(0, channelsWithPerms.Count);

                            DiscordChannel textChannel = channelsWithPerms[channelNum];

                            log.info("Choose textChannel [" + textChannel.Name + "] for TTS event in guild [" + textChannel.Guild.Name + "]");

                            //Randomize the tts message that is going to be sent
                            string ttsMessage = randomTTSMessages[random.Next(0, randomTTSMessages.Count())];

                            //Retreive AYAYA emote for guild if it exists
                            DiscordEmoji ayayaEmote = GuildEmoteManager.retrieveAyayaEmoteForGuild(guildId);

                            //Build message and send to channel
                            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();

                            //Add the ayaya emote to the message if it exists for the guild
                            if (ayayaEmote != null)
                            {
                                messageBuilder.Content = $"{ayayaEmote} " + ttsMessage + $" {ayayaEmote}";
                            }
                            else
                            {
                                messageBuilder.Content = ttsMessage;
                            }

                            //Add TTS to the message
                            messageBuilder.IsTTS = true;

                            //Send message in choosen channel
                            await messageBuilder.SendAsync(textChannel);
                        }
                        else
                        {
                            log.warn("Attempted to retreive text channels from guild [" + guildId + "] and failed to find any with send tts permissions.");
                        }
                    }
                    else
                    {
                        log.info("Failed to start TTS event with randomizer [" + eventNum + "], it needs to be below [" + randomTTSEventChance + "]");
                    }
                }
            }
            catch (Exception ex)
            {
                log.error("Exception while executing the random tts process [" + ex.Message + "]");
            }
        }

    }
}
