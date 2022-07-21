using System.Diagnostics;

namespace ARX_PME_Connector.Commands.Commands_List
{
    /// <summary>
    /// Class that handles the command -version.
    /// </summary>
    internal class Version : ICommand
    {
        // Field
        bool startUpdate = false;

        //
        // Constructor : set and check parameters
        //

        public Version(string[] parameters)
        {
            bool valueIsPresent = ParamHandler.getParamValue(parameters, "-getLatest", out string exitValue);

            //If getLatest is set to true
            if (valueIsPresent && exitValue == "true")
                startUpdate = true; 
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
            //Get version from config.json
            var version = Config.getVersion();

            Console.WriteLine(version); //==> Print version

            //If start update is true
            if (startUpdate)
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + 
                              "/ARX-PME/Update/ARX-PME_Updater.exe");

            return true;
        }
    }
}
