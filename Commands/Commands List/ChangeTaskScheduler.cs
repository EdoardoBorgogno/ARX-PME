using Microsoft.Win32.TaskScheduler;

namespace ARX_PME_Connector.Commands.Commands_List
{
    /// <summary>
    /// Class that handles the command -change_task_scheduler
    /// </summary>
    internal class ChangeTaskScheduler : ICommand
    {
        //Fields
        private string time;
        private int every;

        //
        // Constructor : set and check parameters
        //

        public ChangeTaskScheduler(string[] parameters)
        {
            // Variables
            string paramError = "Number of parameters incorrect. Check documentation.";
            string paramTypeError = "Params type error;";

            // If there is more or less than 2 parameters show paramError
            if (parameters.Length != 2)
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }
            else
            {
                //Set time
                time = parameters[0];

                //Set Every
                if (!int.TryParse(parameters[1], out int days)) // if is not an integer show paramTypeError
                {
                    LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Type Error", MessageContent = paramTypeError, MessageDateTime = DateTime.Now };
                    LogHandler.addMessage(message);
                }
                else
                {
                    every = days;
                }
            }
        }


        //
        // Methods
        //

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <returns></returns>
        public async Task<dynamic> Execute()
        {
            //If necessary are set
            if (time != null && every > 0)
            {
                //Update task cheduler
                updateScheduledActivity();

                //Update config file
                Config.updateSchedulerData(time, every);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Change time and repetetions of updater scheduled activity.
        /// </summary>
        private bool updateScheduledActivity()
        {
            //get x86 folder path
            string pathX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "ARX-PME update task.";

                // Create a trigger that will fire the task at this time every other day
                DateTime date = new DateTime(DateTime.Today.Year, 
                                    DateTime.Today.Month, 
                                    DateTime.Today.Day + every, 
                                    Convert.ToInt32(time.Split(":")[0]), 
                                    Convert.ToInt32(time.Split(":")[1]),
                                    0);


                td.Triggers.Add(new DailyTrigger { DaysInterval = Convert.ToInt16(every), StartBoundary = date });
                
                //run task with admin user
                td.Principal.RunLevel = TaskRunLevel.Highest;

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction(pathX86 + "/ARX-PME/Update/ARX-PME_Updater.exe"));

                // Register the task in the root folder.
                // (Use the username here to ensure remote registration works.)
                ts.RootFolder.RegisterTaskDefinition(@"ARX-PME", td);
            }

            return true;
        }
    }
}
