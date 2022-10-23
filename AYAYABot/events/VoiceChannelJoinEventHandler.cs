using DSharpPlus;
using DSharpPlus.EventArgs;
using AYAYABot.util;
using AYAYABot.background;

namespace AYAYABot.events
{
    internal class VoiceChannelJoinEventHandler
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int papiHelloDelayMs = Convert.ToInt32(ConfigManager.config["papiHelloMinTimer"]);
        private static readonly ulong ownerGuid = Convert.ToUInt64(ConfigManager.config["ownerGuid"]);

        //Variable to keep track of the last time that AYAYA said hi/welcomed papi
        private static DateTime lastPapiHello = DateTime.Now.AddMilliseconds(-papiHelloDelayMs); //Default value is 

        //This method is called whenever a user joins/leaves/moves voice channels
        public static async Task VoiceServerUpdated(DiscordClient client, VoiceStateUpdateEventArgs eventArgs)
        {
            try
            {
                //Check if the user involved in the eventArgs is the bot owner
                if (eventArgs.User.Id == ownerGuid)
                {
                    //Check if the lastPapiHello is less than or equal too the current time plus the papiHelloDelayMs
                    if (lastPapiHello <= DateTime.Now.AddMilliseconds(papiHelloDelayMs))
                    {
                        //Check to make sure that After channel is not null
                        if (eventArgs.After != null)
                        {
                            //Check if the event was triggered by the owner joining a voice channel by checking if "Before.Channel" is different from "After.Channel"
                            if (eventArgs.Before == null || eventArgs.Before.Channel.Id != eventArgs.After.Channel.Id)
                            {
                                log.info("Voice channel change for server owner that is triggering hello sequence.");

                                //Call the JoinVoiceChannel to run the hello sequence
                                MainBackground.AddBackgroundThread(JoinVoiceChannel.JoinOwnerVoiceChannel(eventArgs.After.Channel));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.error("Exception while handling voice state update event [" + ex.Message + "]");
            }
        }

    }
}
