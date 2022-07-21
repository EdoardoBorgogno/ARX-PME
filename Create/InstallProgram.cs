using System.Diagnostics;
using Microsoft.Win32.TaskScheduler;

namespace ARX_PME_Updater
{
    /// <summary>
    /// Handle program installation.
    /// </summary>
    internal class InstallProgram
    {
        /// <summary>
        /// Create ARX-PME folders/task and download program from github.
        /// </summary>
        public static async System.Threading.Tasks.Task Install()
        {
            // Create base directories
            createBaseDirectory();

            // Download latest version
            await DownloadRelease.downloadLatestVersion();

            //Add to enviroment variable
            addToEnviromentVariable();

            // Create task
            createScheduledTask();
        }

        /// <summary>
        /// Add my program (ARX-PME_Connector.exe) to enviroment variables.
        /// </summary>
        private static void addToEnviromentVariable()
        {
            var name = "Path";
            var scope = EnvironmentVariableTarget.Machine;
            var oldValue = Environment.GetEnvironmentVariable(name, scope);
            var path = Environment.GetFolderPath(
                       Environment.SpecialFolder.ProgramFilesX86) + "/ARX-PME/ARX-PME/";
            var newValue = oldValue + $";{path}";

            Environment.SetEnvironmentVariable(name, newValue, scope);
        }

        /// <summary>
        /// Create base directory for contains program files.
        /// </summary>
        private static void createBaseDirectory()
        {
            //get x86 folder path
            string pathX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            //get programDate path
            string pathProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            // Create base directory in x86
            Directory.CreateDirectory(pathX86 + "/ARX-PME");

            // Create base directory in programData
            Directory.CreateDirectory(pathProgramData + "/ARX-PME");
        }

        /// <summary>
        /// Create scheduled task for check if there are updates.
        /// </summary>
        private static void createScheduledTask()
        {
            //get x86 folder path
            string pathX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            using (TaskService ts = new TaskService())
            {
                // Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "ARX-PME update task.";

                // Create a trigger that will fire the task at this time every other day
                DateTime date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day + 2, 4, 30, 0);
                td.Triggers.Add(new DailyTrigger { DaysInterval = 2, StartBoundary = date });

                //run task with admin user
                td.Principal.RunLevel = TaskRunLevel.Highest;

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction(pathX86 + "/ARX-PME/Update/ARX-PME_Updater.exe"));

                // Register the task in the root folder.
                // (Use the username here to ensure remote registration works.)
                ts.RootFolder.RegisterTaskDefinition(@"ARX-PME", td);
            }
        }
    }
}
