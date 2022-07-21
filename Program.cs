using System;
using System.Diagnostics;

namespace ARX_PME_Updater
{
    class Program
    {
        //
        // Main Function
        //

        static async Task Main(string[] args)
        {
            //Write message
            Logger.LogHandler.Message message = new Logger.LogHandler.Message { MessageTitle = "Start Updater", MessageContent = "", MessageDateTime = DateTime.Now };
            Logger.LogHandler.addMessage(message);

            // Path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "/ARX-PME";

            // if args contains delete_temp, delete temp_Updater.exe
            if (args.Contains("delete_temp"))
            {
                await Download.File.DeleteTemp(path);

                //Close program
                return;
            }

            //
            // Check if Program Directory already exists
            //

            // Variable
            bool directoryExists;

            directoryExists = Directory.Exists(path);
        
            try
            {
                //If directory exists
                if (directoryExists)
                {
                    // Check For update
                    await UpdateProgram.CheckAndUpdate();
                }
                else
                {
                    // Install ARX-PME_Connector
                    await InstallProgram.Install();
                }
            }
            catch (Exception ex)
            {
                //If there are error => clear download folder
                Download.File.Empty(new DirectoryInfo(path + "/Update/Download"));

                //Write error on logger 
                message = new Logger.LogHandler.Message { MessageTitle = "Error", MessageContent = ex.Message, MessageDateTime = DateTime.Now };
                Logger.LogHandler.addMessage(message);
            }

            //Write message
            message = new Logger.LogHandler.Message { MessageTitle = "End Updater", MessageContent = "", MessageDateTime = DateTime.Now };
            Logger.LogHandler.addMessage(message);
        }
    }
}