using AYAYABot.util;
using AYAYABot.events;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using DSharpPlus;

namespace AYAYABot.background
{
    internal class JoinVoiceChannel
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Retreive config options
        private static int voiceJoinEventChance = Convert.ToInt32(ConfigManager.config["voiceJoinEventChance"]);
        private static int voiceJoinMinTimer = Convert.ToInt32(ConfigManager.config["voiceJoinMinTimer"]);
        private static int voiceJoinMaxTimer = Convert.ToInt32(ConfigManager.config["voiceJoinMaxTimer"]);
        private static int voiceJoinPollTimer = Convert.ToInt32(ConfigManager.config["voiceJoinPollTimer"]);
        private static int voiceJoinSpeakDelay = Convert.ToInt32(ConfigManager.config["voiceJoinSpeakDelay"]);
        private static int voiceJoinJoinDelay = Convert.ToInt32(ConfigManager.config["voiceJoinJoinDelay"]);
        private static string[] voiceJoinMessageResource = ConfigManager.getConfigList("voiceJoinMessageResource");

        //Discord client private variable
        private readonly DiscordClient client;

        //Main constructor
        public JoinVoiceChannel(DiscordClient client)
        {
            this.client = client;
        }

        //Start of processing method
        public async Task RandomVoiceJoin()
        {
            //Always surround your background threads with try catch and logging so you can see the exceptions, otherwise they don't get shown
            try
            {
                //Log the start of the random TTS task
                log.info("Starting the random voice join process");

                //Loop till the shutdown bool tells the process to stop
                while (!MainBackground.shutdownProcess())
                {
                    //Generate randomized wait timer based on config
                    Random random = new Random();
                    int delay = random.Next(voiceJoinMinTimer, voiceJoinMaxTimer);

                    log.info("Sleeping for [" + delay + "] ms before attempting next voice join event");

                    //Sleep for delay amount
                    Thread.Sleep(delay);

                    //Check to find voice channels that have people in them
                    DiscordChannel voiceChannel;

                    //Loop untill a person is found in a voice channel
                    while(!retreiveVoiceChannelWPeopleInThem(out voiceChannel))
                    {
                        //Sleep for the amount of time configured by the poll timer to prevent checking the voice channels over and over again
                        Thread.Sleep(voiceJoinPollTimer);
                    }

                    //Verify that the voice channel retreived is not null
                    if (voiceChannel != null)
                    {
                        log.info("Found voice channel [" + voiceChannel.Name + "] in server [" + voiceChannel.Guild.Name + "] with person in it.");

                        //Roll a randomizer based on settings to see if join voice event actually happens
                        int eventNum = random.Next(0, 100);

                        //Event happens if num is less than or equal to event chance setting
                        if (eventNum <= voiceJoinEventChance)
                        {
                            //Wait configured amount of time before joining the channel
                            Thread.Sleep(voiceJoinJoinDelay);

                            //Join the voice channel
                            VoiceNextConnection vCon = await voiceChannel.ConnectAsync();

                            log.info("Connected to voice channel, sleeping before playing audio.");

                            //Wait configured amount of time before sending the audio
                            Thread.Sleep(voiceJoinSpeakDelay);

                            //Retreive random audio file name
                            int audioFileNum = random.Next(voiceJoinMessageResource.Count());

                            //Retreive the actual stream from the resource manager
                            Stream audioFile = ResourceManager.retrieveResource(voiceJoinMessageResource[audioFileNum]);

                            //Get the transmit sink
                            using (VoiceTransmitSink transmit = vCon.GetTransmitSink())
                            {
                                //Play the audio over the transmit sink
                                await audioFile.CopyToAsync(transmit);
                            }

                            //Wait configured amount of time before disconnecting from the channel
                            Thread.Sleep(voiceJoinJoinDelay);

                            //Disconnect from the voice channel
                            vCon.Disconnect();
                        }
                    }
                    else
                    {
                        log.error("The retreived voice channel that is supposed to have people in it was null.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.error("Exception while executing the random voice join process [" + ex.Message + "]");
            }
        }

        private static bool retreiveVoiceChannelWPeopleInThem(out DiscordChannel channel)
        {
            //Found person is default false
            bool foundPersonInVoiceChannel = false;

            //Channel is also default null
            channel = null;

            //Retreive voice channels that have speak permissions from 
            List<DiscordChannel> voiceChannelsWPeopleInThem = new List<DiscordChannel>();

            //Loop through all of the guilds
            foreach (ulong guildId in GuildChannelManager.voiceChannels.Keys)
            {
                //Loop through all of the voice channels that have permissions
                foreach (DiscordChannel voiceChannel in GuildChannelManager.retrieveDiscordChannelsByTypeGuildIdAndPerms(guildId, ChannelType.Voice, Permissions.Speak))
                {
                    //Check if the voiceChannel has people in it
                    if (voiceChannel.Users.Count > 0) //TODO not sure if this does a live pull from the discord API
                    {
                        //Add this channel to the list of channels with people in them
                        voiceChannelsWPeopleInThem.Add(voiceChannel);
                    }
                }
            }

            //If the list of voiceChannelsWPeople in them is greater than 0, than pull one out a random
            if (voiceChannelsWPeopleInThem.Count > 0)
            {
                foundPersonInVoiceChannel = true;

                Random random = new Random();

                int vInt = random.Next(voiceChannelsWPeopleInThem.Count);

                channel = voiceChannelsWPeopleInThem[vInt];
            }
            else
            {
                log.debug("Failed to find any voice channels with people in them.");
            }

            return foundPersonInVoiceChannel;
        }

    }
}
