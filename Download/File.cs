using System.Diagnostics;
using System.Text;

namespace ARX_PME_Updater.Download
{
    /// <summary>
    /// Handle file, creation, deletion...
    /// </summary>
    internal static class File
    {
        /// <summary>
        /// Create file in directory, if these doesn't exists create it.
        /// </summary>
        /// <param name="stream">File content</param>
        /// <param name="asset">Assets info</param>
        /// <returns>True if creation ended correctly.</returns>
        public static bool Create(Stream stream, ReleaseClass.Info asset)
        {
            //Path
            string location = "\\ARX-PME\\Update\\Download\\" + asset.name;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + location;

            FileInfo fileInfo = new FileInfo(path);

            try
            {
                //Create dir if doesn't exists
                if (!fileInfo.Exists)
                    Directory.CreateDirectory(fileInfo.Directory!.FullName);

                FileStream fileStream;
                
                //Create file
                fileStream = System.IO.File.Create(path);

                // Copy text
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);

                //Close file
                fileStream.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete all file in list.
        /// </summary>
        /// <param name="listOfAsset"></param>
        public static void Remove(List<ReleaseClass.Info> listOfAsset)
        {
            //Foreach item in listOfAsset, delete it
            foreach (ReleaseClass.Info asset in listOfAsset)
            {
                string path = createPath(asset) + "/" + asset.name;

                //Check if path is not a dir
                if (Path.HasExtension(path))
                {
                    //Delete file if exists
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                else
                {
                    //Delete dir if exists
                    if (Directory.Exists(path))
                        Directory.Delete(path, true);
                }

            }
        }

        /// <summary>
        /// Clear all useless dir and file
        /// </summary>
        public static void EmptyExcept()
        {
            //Clear Program app data
            Empty(new DirectoryInfo(
                                Environment.GetFolderPath(
                                Environment.SpecialFolder.CommonApplicationData)
                                + "/ARX-PME"));

            //Clear X86
            var ARXdir = Directory.GetDirectories(Environment.GetFolderPath(
                                                  Environment.SpecialFolder.ProgramFilesX86)
                                                  + "/ARX-PME");

            var ARXfiles = Directory.GetFiles(Environment.GetFolderPath(
                                              Environment.SpecialFolder.ProgramFilesX86)
                                              + "/ARX-PME");

            //Delete all dir in ARX-PME expect for Update dir
            foreach (var directory in ARXdir)
                if (!directory.Contains("Update"))
                    Directory.Delete(directory, true);

            //Delete all files in ARX-PME
            foreach (var file in ARXfiles)
                System.IO.File.Delete(file);
        }

        /// <summary>
        /// Move all element in Update/Download in their real folder.
        /// </summary>
        /// <param name="listOfAsset"></param>
        /// <returns>True, need to delete temp_Updater.exe.</returns>
        public static bool Move(List<ReleaseClass.Info> listOfAsset)
        {
            //Source Path
            bool updaterIsChanged = false;
            string location = "/ARX-PME/Update/Download";
            string sourcePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + location;

            //Foreach item in listOfAsset (same item in Update/Download).
            foreach (ReleaseClass.Info asset in listOfAsset)
            {
                //If asset isn't the updater
                if(asset.name != "ARX-PME_Updater.exe")
                {
                    // Get real path
                    string sourceFile = sourcePath + "/" + asset.name;
                    string destFile = createPath(asset) + "/" + asset.name;

                    //If destination dir doesn't exists, create it
                    FileInfo fileInfo = new FileInfo(destFile);
                    if (!fileInfo.Exists)
                        Directory.CreateDirectory(fileInfo.Directory!.FullName);

                    //If file already exists, delete it
                    if(System.IO.File.Exists(destFile))
                        System.IO.File.Delete(destFile);

                     //Move file into new folder
                     System.IO.File.Move(sourceFile, destFile);
                }
                else
                {
                    //Rename ARX-PME_Updater.exe

                    string sourceFile = sourcePath + "/" + asset.name;
                    string destFile = createPath(asset);

                    FileInfo fi = new FileInfo(destFile + "/ARX-PME_Updater.exe");
                    if (fi.Exists)
                    {
                        fi.MoveTo(destFile + "/temp_Updater.exe");
                    }

                    //Move new Updater into folder
                    System.IO.File.Move(sourceFile, destFile + "/ARX-PME_Updater.exe");

                    // There is new version of updater.exe
                    updaterIsChanged = true;
                }
            }

            return updaterIsChanged;
        }

        /// <summary>
        /// If config.json is updated, set value of new config to old config.
        /// </summary>
        /// <param name="listOfAsset"></param>
        public static void mergeConfig(List<ReleaseClass.Info> listOfAsset)
        {
            string pathOldJson = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/ARX-PME/Config/Config.json";
            string pathNewJson = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "/ARX-PME/Update/Download/Config.json";

            //If config.json is updated
            int configIsUpdated = listOfAsset.Where(x => x.name == "Config.json").Count();
            if (configIsUpdated > 0)
            {
                string oldJson = System.IO.File.ReadAllText(pathOldJson);
                string newJson = System.IO.File.ReadAllText(pathNewJson);

                newJson = JsonMerge.Merge(newJson, oldJson);

                System.IO.File.WriteAllText(pathNewJson, newJson);
            }
        }

        /// <summary>
        /// If file temp exists, delete it when temp_Updater.exe ends.
        /// </summary>
        /// <param name="path">Path to ARX-PME</param>
        public async static Task DeleteTemp(string path)
        {
            if (System.IO.File.Exists(path + "/Update/temp_Updater.exe"))
            {
                //Get process 
                var pname = Process.GetProcessesByName("temp_Updater.exe");

                //until process end wait for delete
                while (pname.Length > 0)
                    //wait
                    await Task.Delay(100);

                System.IO.File.Delete(path + "/Update/temp_Updater.exe");
            }
        }

        /// <summary>
        /// Create path of an asset.
        /// </summary>
        /// <param name="asset">ReleaseClass.Info object</param>
        /// <returns>Path where locate file.</returns>
        public static string createPath(ReleaseClass.Info asset)
        {
            //Path 
            string path;

            //Base folder
            if (asset.baseFolder == "Program")
                path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            else
                path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            //Location in base folder
            path += "/" + asset.path;

            return path;
        }

        /// <summary>
        /// Clear directory, delete all item in dir.
        /// </summary>
        /// <param name="directory">DirectoryInfo object</param>
        public static void Empty(DirectoryInfo directory)
        {
            //
            // Delete all item in dir
            //

            foreach (FileInfo file in directory.GetFiles())
                file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                subDirectory.Delete(true);
        }
    }
}
