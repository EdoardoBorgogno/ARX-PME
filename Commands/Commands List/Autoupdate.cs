
using Microsoft.Win32.TaskScheduler;

namespace ARX_PME_Connector.Commands.Commands_List
{
    /// <summary>
    /// Class that handles the command -autoupdate
    /// </summary>
    internal class Autoupdate : ICommand
    {
        //Field
        bool? autoupdate;

        //
        // Constructor : set and check parameters
        //

        public Autoupdate(string[] parameters)
        {
            // Variables
            string paramError = "Number of parameters incorrect. Check documentation.";

            // If there is more or less than 1 parameter show paramError
            if (parameters.Length != 1)
            {
                LogHandler.Message message = new LogHandler.Message { MessageTitle = "Param Error", MessageContent = paramError, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }
            else
            {
                //If parameters[0] is true or false
                if (bool.TryParse(parameters[0], out bool value))
                    autoupdate = value; 
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
            //If autoupdate is set
            if (autoupdate != null)
            {
                //Change config value
                Config.setAutoupdate(autoupdate);

                //Enable/disable task
                updateTask(Convert.ToBoolean(autoupdate));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Enable/Disable task.
        /// </summary>
        /// <param name="autoupdate">Bool value</param>
        private void updateTask(bool autoupdate)
        {
            using (TaskService ts = new TaskService())
            {
                //Get task
                var task = ts.GetTask("ARX-PME");
                task.Definition.Settings.Enabled = autoupdate; //==> disable/enable

                //Save changes
                task.RegisterChanges();
            }
        }
    }
}
