using System;
using ARX_PME_Connector.API;
using ARX_PME_Connector.Commands;

//
// Arx-Pme connector
// 

namespace ARX_PME_Connector
{
    class Program
    {
        //
        // Main Function
        //

        static async Task Main(string[] args)
        {
            //Start message
            LogHandler.Message message = new LogHandler.Message { MessageTitle = "Start ARX-PME_Connector", MessageContent = "", MessageDateTime = DateTime.Now };
            LogHandler.addMessage(message);

            //
            // Check args
            //

            // Variables
            string noArgsError = "No Args error. Check documentation.";
            
            //If there is no ARGS show noArgsError
            if(args.Length == 0)
            {
                //No args message
                message = new LogHandler.Message { MessageTitle = "Error", MessageContent = noArgsError, MessageDateTime = DateTime.Now };
                LogHandler.addMessage(message);
            }
            else
            {
                //
                // If there is Args : handle Args
                //

                LogArgs(args);

                try
                {
                    await CommandsHandler.HandleCommand(args);
                }
                catch (Exception ex)
                {
                    //If there is an error
                    //log message.

                    message = new LogHandler.Message { MessageTitle = "Error", MessageContent = ex.Message, MessageDateTime = DateTime.Now };
                    LogHandler.addMessage(message);
                }
            }

            //End message
            message = new LogHandler.Message { MessageTitle = "End ARX-PME_Connector", MessageContent = "", MessageDateTime = DateTime.Now };
            LogHandler.addMessage(message);
        }
    
        /// <summary>
        /// Write all args (input) in log.txt.
        /// </summary>
        /// <param name="args"></param>
        static void LogArgs(string[] args)
        {
            //variable
            string strArgs = "";

            //Log args
            foreach (var arg in args)
                strArgs += " " + arg;

            LogHandler.Message message = new LogHandler.Message { MessageTitle = "Input", MessageContent = strArgs, MessageDateTime = DateTime.Now };
            LogHandler.addMessage(message);
        }
    }
}