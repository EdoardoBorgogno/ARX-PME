using ARX_PME_Connector.Commands.Commands_List;

namespace ARX_PME_Connector.Commands
{
    /// <summary>
    /// Handle Command.
    /// </summary>
    internal static class CommandsHandler
    {
        /// <summary>
        /// Handle Command.
        /// </summary>
        /// <param name="args">Args array</param>
        /// <returns></returns>
        public async static Task HandleCommand(string[] args)
        {
            // Variables
            string unknownCommandError = "Unknown command error. Check documentation.";
            bool unknowCommandErrorBool = false;
            string[] parameters;
            string command;

            //
            // Legacy command
            //

            args = await LegacyHandler.FromLegacyToNew(args);


            // Get command from args.
            command = args[0];

            // Remove command from args.
            parameters = args.Skip(1).ToArray();

            // Create command interface
            ICommand? commandOBJ = null;

            // Switch on command ==> init commandOBJ
            switch (command)
            {
                case "-activities_in_range":
                    commandOBJ = new ActivitiesInRange(parameters);

                    break;
                case "-update_activity_state":
                    commandOBJ = new UpdateActivityState(parameters);

                    break;
                case "-customer":
                    commandOBJ = new CustomerActions(parameters);

                    break;
                case "-project":
                    commandOBJ = new ProjectActions(parameters);

                    break;
                case "-move":
                    commandOBJ = new MoveActivities(parameters);

                    break;
                case "-version":
                    commandOBJ = new Commands_List.Version(parameters);
                    
                    break;
                case "-change_task_scheduler":
                    commandOBJ = new ChangeTaskScheduler(parameters);

                    break;
                case "-autoupdate":
                    commandOBJ = new Autoupdate(parameters);

                    break;
                default:
                    //Show unknownCommandError
                    LogHandler.Message message = new LogHandler.Message { MessageTitle = "Command Error", MessageContent = unknownCommandError, MessageDateTime = DateTime.Now };
                    LogHandler.addMessage(message);

                    //Set unknowCommandErrorBool
                    unknowCommandErrorBool = true;

                    break;
            }

            //If command is set
            if(unknowCommandErrorBool == false)
            {
                //Execute Command
                await commandOBJ!.Execute();
            }
        }
    }
}
