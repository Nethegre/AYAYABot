using DSharpPlus;
using AYAYABot.util;

namespace AYAYABot.background
{
    //This class will be used to manage background processes
    internal class MainBackground
    {
        //Create an instance of the log manager class
        readonly static LogManager log = new LogManager(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Variable that is used to tell the background processes to turn off
        private static bool shutdown = false;

        //Variable that is used to tell the background thread starter that the guild download is completed and that the threads are ready for processing
        private static bool readyForBackground = false;

        //Method to set the readyForBackground to true, should only be called by the GuildDownloadCompletedManager
        public static void readyForBackgroundTrue()
        {
            readyForBackground = true;
        }

        //List of background tasks to keep track of
        private static List<Task> backgroundThreads = new List<Task>();

        //Create list of valid "shutdown" states for background threads
        private static TaskStatus[] shutdownStates = new TaskStatus[] { TaskStatus.Faulted, TaskStatus.RanToCompletion, TaskStatus.Canceled };

        //Public method used to retrieve the value of the shutdown bool
        public static bool shutdownProcess()
        {
            return shutdown;
        }

        //Main method to startup background processes
        public static async Task Startup(DiscordClient client)
        {
            //Wait till the readyForBackground flag to be true before starting background threads
            while (!readyForBackground)
            {
                Thread.Sleep(500);
            }

            log.info("Starting to create background threads.");

            //Startup the background tasks and add them to the list of backgroundTasks
            backgroundThreads.Add(RandomTextToSpeech.RandomTTS(client));

            //TODO Start a background thread to keep track of background threads that are completed

        }

        //Main method to shutdown background processes
        public static void Shutdown()
        {
            //Retrieve the shutdown timer from config
            int shutdownTimer = Convert.ToInt32(ConfigManager.config["backgroundThreadShutdownTimer"]);

            //Set the shutdown bool to true
            shutdown = true;

            //Sleep for the shutdownTimer length
            Thread.Sleep(shutdownTimer);

            //Loop through the background threads
            foreach (Task thread in backgroundThreads)
            {
                //Check if the thread is in a shutdown state
                if (!shutdownStates.Contains(thread.Status))
                {
                    //The thread is not considered shutdown so we need to shut it down
                    try
                    {
                        thread.Dispose();
                    }
                    catch (Exception ex)
                    {
                        log.error("Exception while shutting down background thread [" + ex.Message + "]");
                    }
                }
            }
        }


    }
}
