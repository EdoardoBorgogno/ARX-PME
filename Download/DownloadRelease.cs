using Octokit;
using System.Diagnostics;

namespace ARX_PME_Updater
{
    /// <summary>
    /// Get latest version from github.
    /// </summary>
    internal static class DownloadRelease
    {

        /// <summary>
        /// Download latest version of ARX-PME
        /// </summary>
        public static async Task downloadLatestVersion()
        {
            Release latestRelease = await GithubRelease.getLatestVersion();

            // Get release.json content
            string releaseTag = latestRelease.TagName;
            ReleaseClass.Root releaseJson = await GithubRelease.getReleaseJson(releaseTag);

            // Get list of assets to download
            string[] assetsToDownload = { "unchanged", "added", "updated" };
            List<ReleaseClass.Info> listOfAsset = getAssets(assetsToDownload, releaseJson.assets);

            // Download all assets
            bool endedCorrectly = await downloadAndCreation(listOfAsset, releaseTag);

            //If installation ended correctly
            if(endedCorrectly)
            {
                //Move file in download folder
                Download.File.Move(listOfAsset);

                //Set new version
                Config.setVersion(releaseTag);

                //Write message
                Logger.LogHandler.Message message = new Logger.LogHandler.Message { MessageTitle = "Installation ended correctly!", MessageContent = "", MessageDateTime = DateTime.Now };
                Logger.LogHandler.addMessage(message);
            }
            else
            {
                //Path
                string location = "\\ARX-PME\\Update\\Download";
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + location;

                // Clear download folder
                Download.File.Empty(new DirectoryInfo(path));

                //Write message
                Logger.LogHandler.Message message = new Logger.LogHandler.Message { MessageTitle = "Installation error", MessageContent = "Something went wrong!", MessageDateTime = DateTime.Now };
                Logger.LogHandler.addMessage(message);
            }
        }

        /// <summary>
        /// Update Software.
        /// </summary>
        public static async Task updateToLatestVersion()
        {
            Release latestRelease = await GithubRelease.getLatestVersion();

            string latestReleaseTag = latestRelease.TagName;
            int versionComparison = compareVersion(latestReleaseTag); //==> compare version

            if (versionComparison < 0)
            {
                // The version on GitHub is more up than this local release.

                //Get previous version
                ReleaseClass.Root releaseJson = await GithubRelease.getReleaseJson(latestReleaseTag);
                string previousVersion = releaseJson.previousVersion;

                //If previous version is equal to local version, consecutive download
                if (previousVersion == Config.getVersion())
                    await consecutiveUpdate(latestRelease, releaseJson);
                else
                    await missedVersionUpdate(latestRelease, releaseJson);
            }
        }

        /// <summary>
        /// Download latest version, download only updated and added files.
        /// </summary>
        private static async Task consecutiveUpdate(Release latestRelease, ReleaseClass.Root releaseJson)
        {
            //Write message
            Logger.LogHandler.Message message = new Logger.LogHandler.Message { MessageTitle = "Start consecutive Update", MessageContent = "", MessageDateTime = DateTime.Now };
            Logger.LogHandler.addMessage(message);

            string releaseTag = latestRelease.TagName;

            // Get list of assets to download
            string[] assetsToDownload = { "added", "updated" };
            List<ReleaseClass.Info> listOfAsset = getAssets(assetsToDownload, releaseJson.assets);

            // Download all assets
            bool endedCorrectly = await downloadAndCreation(listOfAsset, releaseTag);

            //If installation ended correctly
            if (endedCorrectly)
            {
                bool deleteTempUpdater = false;

                Download.File.mergeConfig(listOfAsset);

                //Delete removed files
                List<ReleaseClass.Info> assetToRemove = getAssets(new string[] { "removed" }, releaseJson.assets);
                Download.File.Remove(assetToRemove);

                //Move file in download folder
                deleteTempUpdater = Download.File.Move(listOfAsset);

                //Set new version
                Config.setVersion(releaseTag);

                //If there is a temp_Updater.exe, delete it
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + 
                                                        "/ARX-PME/Update/ARX-PME_Updater.exe";
                if (deleteTempUpdater)
                    Process.Start(path, "delete_temp");

                //Write message
                message = new Logger.LogHandler.Message { MessageTitle = "Update ended correctly!", MessageContent = "", MessageDateTime = DateTime.Now };
                Logger.LogHandler.addMessage(message);
            }
            else
            {
                //Path
                string location = "\\ARX-PME\\Update\\Download";
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + location;

                // Clear download folder
                Download.File.Empty(new DirectoryInfo(path));

                //Write message
                message = new Logger.LogHandler.Message { MessageTitle = "Update error", MessageContent = "Something went wrong!", MessageDateTime = DateTime.Now };
                Logger.LogHandler.addMessage(message);
            }
        }

        /// <summary>
        /// Redownload all software.
        /// </summary>
        private static async Task missedVersionUpdate(Release latestRelease, ReleaseClass.Root releaseJson)
        {
            //Write message
            Logger.LogHandler.Message message = new Logger.LogHandler.Message { MessageTitle = "Start missed version Update", MessageContent = "", MessageDateTime = DateTime.Now };
            Logger.LogHandler.addMessage(message);

            string releaseTag = latestRelease.TagName;

            // Get list of assets to download
            string[] assetsToDownload = { "added", "updated", "unchanged" };
            List<ReleaseClass.Info> listOfAsset = getAssets(assetsToDownload, releaseJson.assets);

            // Download all assets
            bool endedCorrectly = await downloadAndCreation(listOfAsset, releaseTag);

            //If installation ended correctly
            if (endedCorrectly)
            {
                bool deleteTempUpdater = false;

                Download.File.mergeConfig(listOfAsset);

                //Delete all Directory
                Download.File.EmptyExcept();

                deleteTempUpdater = Download.File.Move(listOfAsset);

                //Set new version
                Config.setVersion(releaseTag);

                //If there is a temp_Updater.exe, delete it
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) +
                                                        "/ARX-PME/Update/ARX-PME_Updater.exe";
                if (deleteTempUpdater)
                    Process.Start(path, "delete_temp");

                //Write message
                message = new Logger.LogHandler.Message { MessageTitle = "Update ended correctly!", MessageContent = "", MessageDateTime = DateTime.Now };
                Logger.LogHandler.addMessage(message);
            }
            else
            {
                //Path
                string location = "\\ARX-PME\\Update\\Download";
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + location;

                // Clear download folder
                Download.File.Empty(new DirectoryInfo(path));

                //Write message
                message = new Logger.LogHandler.Message { MessageTitle = "Update error", MessageContent = "Something went wrong!", MessageDateTime = DateTime.Now };
                Logger.LogHandler.addMessage(message);
            }
        }

        /// <summary>
        /// Compare local version with remote version.
        /// </summary>
        /// <param name="latestTagName">Remote release tag</param>
        /// <returns>Int, if < 0, need update.</returns>
        private static int compareVersion(string latestTagName)
        {
            string localReleaseTag = Config.getVersion(); //==> local version

            //Compare version
            int versionComparison = localReleaseTag.CompareTo(latestTagName);

            return versionComparison;
        }

        /// <summary>
        /// Download content from github and create file in download folder.
        /// </summary>
        /// <param name="listOfAsset">List with all asset to download</param>
        /// <param name="releaseTag">Release tag name.</param>
        /// <returns>true if download end creation ended correctly</returns>
        public static async Task<bool> downloadAndCreation(List<ReleaseClass.Info> listOfAsset, string releaseTag)
        {
            bool endedCorrectly = true; //==> false if something went wrong with installation

            // Download assets in list
            foreach (var asset in listOfAsset)
            {
                Stream? stream = await GithubRelease.downloadAsset(asset, releaseTag);

                //If stream is null stop download
                if(stream == null)
                {
                    endedCorrectly = false;
                    break;
                }
                else
                {
                    //If there is an error with file creation stop download
                    if(!Download.File.Create(stream, asset))
                    {
                        endedCorrectly = false;
                        break;
                    }
                }
            }

            return endedCorrectly;
        }

        /// <summary>
        /// Get a list of assets filtred by categories (added, removed, ...).
        /// </summary>
        /// <param name="assetsToGet">Array with category of assets to get (added, updated, ...).</param>
        /// <param name="releaseJsonAssets">Release json object</param>
        /// <returns>List with all assets to get (list filtred by category).</returns>
        private static List<ReleaseClass.Info> getAssets(string[] assetsToGet, ReleaseClass.Assets releaseJsonAssets)
        {
            //List with assets to download
            List<ReleaseClass.Info> assetList = new List<ReleaseClass.Info>();

            //
            // Add asset to list
            // 

            if (assetsToGet.Contains("unchanged"))
                assetList.AddRange(releaseJsonAssets.unchanged);

            if (assetsToGet.Contains("updated"))
                assetList.AddRange(releaseJsonAssets.updated);

            if (assetsToGet.Contains("added"))
                assetList.AddRange(releaseJsonAssets.added);

            if (assetsToGet.Contains("removed"))
                assetList.AddRange(releaseJsonAssets.removed);

            return assetList;
        }
    }
}
