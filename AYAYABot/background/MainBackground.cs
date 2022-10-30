using DSharpPlus;
using AYAYABot.util;
using System.Collections.Concurrent;

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

        private static int backgroundThreadSleep = Convert.ToInt32(ConfigManager.config["backgroundThreadSleep"]);

        //Method to set the readyForBackground to true, should only be called by the GuildDownloadCompletedManager
        public static void readyForBackgroundTrue()
        {
            readyForBackground = true;
        }

        //List of background tasks to keep track of, concurrent bag is supposedly thread safe
        private static ConcurrentQueue<Task> backgroundThreads = new ConcurrentQueue<Task>();

        //Create list of valid "shutdown" states for background threads
        private static TaskStatus[] shutdownStates = new TaskStatus[] { TaskStatus.Faulted, TaskStatus.RanToCompletion, TaskStatus.Canceled };

        //Public method used to retrieve the value of the shutdown bool
        public static bool shutdownProcess()
        {
            return shutdown;
        }

        //Main method to startup static background processes
        public static async Task Startup(DiscordClient client)
        {
            //Wait till the readyForBackground flag to be true before starting background threads
            while (!readyForBackground)
            {
                Thread.Sleep(500);
            }

            log.info("Starting to create background threads.");

            try
            {
                //Startup the static background tasks and add them to the list of backgroundTasks
                backgroundThreads.Enqueue(Task.Run(new RandomTextToSpeech(client).RandomTTSBackground));
                backgroundThreads.Enqueue(Task.Run(new JoinVoiceChannel().RandomVoiceJoin));
                backgroundThreads.Enqueue(LogManager.ProcessLogs());
            }
            catch (Exception ex)
            {
                log.error("Exception while starting background threads [" + ex.Message + "]");
            }

            try
            {
                //Start a loop to keep track of background threads that are completed
                while (!shutdown)
                {
                    //Try to dequeue each item
                    if (backgroundThreads.TryDequeue(out Task task))
                    {
                        //check if the thread is in a shutdown state
                        if (shutdownStates.Contains(task.Status))
                        {
                            //If it is in a shutdown state log and dispose of it
                            log.info("Found background thread [" + task.Id + "] in shutdown state [" + task.Status + "] , removing from background threads list.");
                        }
                        else
                        {
                            //If it is not in a shutdown state than requeue the thread
                            backgroundThreads.Enqueue(task);
                        }
                    }
                    else
                    {
                        //Sleep for designated amount of time before checking the thread queue again
                        Thread.Sleep(backgroundThreadSleep);
                    }
                }
            }
            catch (Exception ex)
            {
                log.error("Exception while maintaining background tasks [" + ex.Message + "]");
            }
        }

        //Main method to shutdown background processes
        public static void Shutdown()
        {
            try
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
            catch (Exception ex)
            {
                log.error("Exception while attempting to shutdown background threads [" + ex.Message + "]");
            }
        }

        //Main methods to add to the background threads
        public static void AddBackgroundThread(Action task)
        {
            if (!shutdown)
            {
                backgroundThreads.Enqueue(Task.Run(task));
            }
            else
            {
                log.warn("Attempted to start a background action [" + task.Target.GetType().Name + "." + task.Method.Name + "] while shutdown is true.");
            }
        }

        public static void AddBackgroundThread(Task task)
        {
            if (!shutdown)
            {
                backgroundThreads.Enqueue(task);
            }
            else
            {
                log.warn("Attempted to start a background task [" + task.Id + "] while shutdown is true.");
            }
        }

    }
}
