﻿using AYAYABot.util;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AYAYABot.events
{
    internal class GuildChannelManager
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //This dictionary will be used to store all of the text channels that are found in the guilds that the bot is in
        public static Dictionary<ulong, List<DiscordChannel>> textChannels = new Dictionary<ulong, List<DiscordChannel>>();
        public static Dictionary<ulong, List<DiscordChannel>> voiceChannels = new Dictionary<ulong, List<DiscordChannel>>();

        //Main method that collects all of the text and voice channels from the guilds that it has joined
        public static async Task startChannelCollecting(DiscordClient discordClient, GuildDownloadCompletedEventArgs eventArgs)
        {
            //Loop through the guilds that the client is connected to
            foreach (DiscordGuild guild in discordClient.Guilds.Values)
            {
                List<DiscordChannel> tChannel = new List<DiscordChannel>();
                List<DiscordChannel> vChannel = new List<DiscordChannel>();

                //Loop through the channels that the guild contains
                foreach (DiscordChannel channel in guild.Channels.Values)
                {
                    //Determine if the channel is a text or voice channel
                    if (channel.Type == ChannelType.Text)
                    {
                        //Add the channel to the list
                        tChannel.Add(channel);
                    }
                    else if (channel.Type == ChannelType.Voice)
                    {
                        //Add the channel to the list
                        vChannel.Add(channel);
                    }
                }

                //Add both channel lists to their dictionaries if the guild doesn't exist in the dictionary
                if (textChannels.ContainsKey(guild.Id))
                {
                    //Replace the list of text channels as it is out of date
                    textChannels[guild.Id] = tChannel;
                }
                else
                {
                    //Add a new entry to the dictionary
                    textChannels.Add(guild.Id, tChannel);
                }

                if (voiceChannels.ContainsKey(guild.Id))
                {
                    //Replace the list of voice channels as it is out of date
                    voiceChannels[guild.Id] = vChannel;
                }
                else
                {
                    //Add a new entry to the dictionary
                    voiceChannels.Add(guild.Id, vChannel);
                }
            }

            log.info("Finished retreiving voice and text channels for guilds");
        }

        //Main method that is activated when a new channel is created
        public static async Task channelCreatedEvent(DiscordClient client, ChannelCreateEventArgs eventArgs)
        {
            //Check the type of channel
            if (eventArgs.Channel.Type == ChannelType.Text)
            {
                log.info("Found new text channel [" + eventArgs.Channel.Id + "] in guild [" + eventArgs.Guild.Id + "]");

                //Check if the guild already exists in the channel dictionary
                if (textChannels.ContainsKey(eventArgs.Guild.Id))
                {
                    //Add the channel to the list
                    textChannels[eventArgs.Guild.Id].Add(eventArgs.Channel);
                }
                else
                {
                    //Create new dictionary entry for the guild
                    textChannels.Add(eventArgs.Guild.Id, new List<DiscordChannel> { eventArgs.Channel });
                }
            }
            else if (eventArgs.Channel.Type == ChannelType.Voice)
            {
                log.info("Found new voice channel [" + eventArgs.Channel.Id + "] in guild [" + eventArgs.Guild.Id + "]");

                //Check if the guild already exists in the channel dictionary
                if (voiceChannels.ContainsKey(eventArgs.Guild.Id))
                {
                    //Add the channel to the list
                    voiceChannels[eventArgs.Guild.Id].Add(eventArgs.Channel);
                }
                else
                {
                    //Create new dictionary entry for the guild
                    voiceChannels.Add(eventArgs.Guild.Id, new List<DiscordChannel> { eventArgs.Channel });
                }
            }
        }

         


        public static List<DiscordChannel> retrieveDiscordChannelsByTypeAndGuildId(ulong id, ChannelType type)
        {
            List<DiscordChannel> response = new List<DiscordChannel>();

            switch (type)
            {
                case ChannelType.Text:
                    {
                        //Check if the guild has any text channels
                        if (textChannels.ContainsKey(id))
                        {
                            response = textChannels[id];
                        }
                        else
                        {
                            //Log warning and return empty list
                            log.warn("Failed to find any text channels for guild [" + id + "]");
                        }
                        break;
                    }
                case ChannelType.Voice:
                    {
                        //Check if the guild has any text channels
                        if (voiceChannels.ContainsKey(id))
                        {
                            response = voiceChannels[id];
                        }
                        else
                        {
                            //Log warning and return empty list
                            log.warn("Failed to find any voice channels for guild [" + id + "]");
                        }
                        break;
                    }
            }

            return response;
        }

    }
}
