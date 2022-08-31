using AYAYABot.util;
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
        public static Dictionary<ulong, DiscordChannel> defaultTextChannels = new Dictionary<ulong, DiscordChannel>();

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

                //Verifies that the guild has at least one text channel
                if (tChannel.Count > 0)
                {
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
                }
                else
                {
                    log.warn("Attempted to retreive text channels from guild [" + guild.Name + "] but it has no text channels");
                }

                //Verifies that the guild has at least one voice channel
                if (vChannel.Count > 0)
                {
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
                else
                {
                    log.warn("Attempted to retreive voice channels from guild [" + guild.Name + "] but it has no voice channels");
                }

                log.info("Retreived " + tChannel.Count + " text channels and " + vChannel.Count + " voice channels for guild [" + guild.Name + "]");

                //Retreive the default text channel for each guild
                defaultTextChannels.Add(guild.Id, guild.GetDefaultChannel());

            }

            log.info("Finished retreiving voice and text channels for guilds");
        }

        //Method that is activated when a new channel is created
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

        //Method that is activated when a channel is deleted
        public static async Task channelDeletedEvent(DiscordClient client, ChannelDeleteEventArgs eventArgs)
        {
            //Check the type of channel
            if (eventArgs.Channel.Type == ChannelType.Text)
            {
                log.info("Removing text channel [" + eventArgs.Channel.Id + "] from guild [" + eventArgs.Guild.Id + "]");

                //Check if the guild already exists in the channel dictionary
                if (textChannels.ContainsKey(eventArgs.Guild.Id))
                {
                    //Check if the dictionary contains the channel that is being removed
                    if (textChannels[eventArgs.Guild.Id].Contains(eventArgs.Channel))
                    {
                        //Remove the channel
                        log.info("Removed text channel [" + eventArgs.Channel.Id + "] from guild [" + eventArgs.Guild.Id + "]");
                        textChannels[eventArgs.Guild.Id].Remove(eventArgs.Channel);

                        //Check if the guild has 0 text channels and remove the dictionary entry if so
                        if (textChannels[eventArgs.Guild.Id].Count() == 0)
                        {
                            textChannels.Remove(eventArgs.Guild.Id);
                        }
                    }
                }
                else
                {
                    //Send warning that there was a delete event for a guild that we do not know about
                    log.warn("Received remove channel event for guild [" + eventArgs.Guild.Id + "] that doesn't exist in the list.");
                }
            }
            else if (eventArgs.Channel.Type == ChannelType.Voice)
            {
                log.info("Removing voice channel [" + eventArgs.Channel.Id + "] from guild [" + eventArgs.Guild.Id + "]");

                //Check if the guild already exists in the channel dictionary
                if (voiceChannels.ContainsKey(eventArgs.Guild.Id))
                {
                    //Check if the dictionary contains the channel that is being removed
                    if (voiceChannels[eventArgs.Guild.Id].Contains(eventArgs.Channel))
                    {
                        //Remove the channel
                        log.info("Removed voice channel [" + eventArgs.Channel.Id + "] from guild [" + eventArgs.Guild.Id + "]");
                        voiceChannels[eventArgs.Guild.Id].Remove(eventArgs.Channel);

                        //Check if the guild has 0 voice channels and remove the dictionary entry if so
                        if (voiceChannels[eventArgs.Guild.Id].Count() == 0)
                        {
                            voiceChannels.Remove(eventArgs.Guild.Id);
                        }
                    }
                }
                else
                {
                    //Send warning that there was a delete event for a guild that we do not know about
                    log.warn("Received remove channel event for guild [" + eventArgs.Guild.Id + "] that doesn't exist in the list.");
                }
            }
        }

        //Method that is activated when a guild is added to the bot
        public static async Task guildAddedToBot(DiscordGuild guild)
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

            //Verify that the guild doesn't already exist in any of the channel lists
            if (textChannels.Keys.Contains(guild.Id) || voiceChannels.Keys.Contains(guild.Id) || defaultTextChannels.Keys.Contains(guild.Id))
            {
                //Log warning that a guild was add to the bot that already existed in the channel lists
                log.warn("New discord guild [" + guild.Name + "] was added to the bot that already existed in one of the channel lists.");
            }

            //Verifies that the guild has at least one text channel
            if (tChannel.Count > 0)
            {
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
            }
            else
            {
                log.warn("Attempted to retreive text channels from guild [" + guild.Name + "] but it has no text channels");
            }

            //Verifies that the guild has at least one voice channel
            if (vChannel.Count > 0)
            {
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
            else
            {
                log.warn("Attempted to retreive voice channels from guild [" + guild.Name + "] but it has no voice channels");
            }

            log.info("Retreived " + tChannel.Count + " text channels and " + vChannel.Count + " voice channels for guild [" + guild.Name + "]");

            //Retreive the default text channel for each guild
            if (defaultTextChannels.Keys.Contains(guild.Id))
            {
                //Replace the value
                defaultTextChannels[guild.Id] = guild.GetDefaultChannel();
            }
            else
            {
                //Add a new value
                defaultTextChannels.Add(guild.Id, guild.GetDefaultChannel());
            }
        }

        //Method that is activated when a guild is removed from a bot
        public static async Task guildRemoveFromBot(DiscordGuild guild)
        {
            //Verify that the guild had channel entries in one of the channel dictionaries
            if (!textChannels.Keys.Contains(guild.Id) && !voiceChannels.Keys.Contains(guild.Id) && !defaultTextChannels.Keys.Contains(guild.Id))
            {
                //Log warning that a guild was removed from the bot that didn't have channels in any of the lists
                log.warn("Guild [" + guild.Name + "] removed from the bot that didn't have any channels");
            }
            else
            {
                //Remove entries if they exist
                if (textChannels.ContainsKey(guild.Id))
                {
                    log.info("Removed guild [" + guild.Name + "] text channels from dictionary.");

                    textChannels.Remove(guild.Id);
                }

                if (voiceChannels.ContainsKey(guild.Id))
                {
                    log.info("Removed guild [" + guild.Name + "] voice channels from dictionary.");

                    voiceChannels.Remove(guild.Id);
                }

                if (defaultTextChannels.ContainsKey(guild.Id))
                {
                    log.info("Removed guild [" + guild.Name + "] default text channel from dictionary.");

                    defaultTextChannels.Remove(guild.Id);
                }
            }
        }

        //Helper methods below

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

            log.info("Retreived [" + response.Count + "] channels from guild [" + id + "]");

            return response;
        }

        public static List<DiscordChannel> retrieveDiscordChannelsByTypeGuildIdAndPerms(ulong id, ChannelType type, DiscordClient client, Permissions requiredPerms)
        {
            //TODO The permissions checking is not working although I am using the recommended permission comparison method

            List<DiscordChannel> response = new List<DiscordChannel>();

            switch (type)
            {
                case ChannelType.Text:
                    {
                        //Check if the guild has any text channels
                        if (textChannels.ContainsKey(id))
                        {
                            //Loop through the text channels to determine if they have the required permissions
                            foreach (DiscordChannel c in textChannels[id])
                            {
                                //Verify that the permissions are not null
                                if (c.UserPermissions != null)
                                {
                                    Permissions perms = c.PermissionsFor(c.Guild.CurrentMember);

                                    if (perms.HasPermission(requiredPerms))
                                    {
                                        response.Add(c);
                                    }
                                }
                            }
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
                            //Loop through the voice channels to determine if they have the required permissions
                            foreach (DiscordChannel c in voiceChannels[id])
                            {
                                //Verify that the permissions are not null
                                if (c.UserPermissions != null)
                                {
                                    Permissions perms = c.PermissionsFor(c.Guild.CurrentMember);

                                    if (perms.HasPermission(requiredPerms))
                                    {
                                        response.Add(c);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Log warning and return empty list
                            log.warn("Failed to find any voice channels for guild [" + id + "]");
                        }
                        break;
                    }
            }

            log.info("Retreived [" + response.Count + "] channels with the permissions [" + requiredPerms + "] from guild [" + id + "]");

            return response;
        }

    }
}
