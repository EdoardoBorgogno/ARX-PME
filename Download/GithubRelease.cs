using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARX_PME_Updater
{
    /// <summary>
    /// Get data from github repository.
    /// </summary>
    internal static class GithubRelease
    {
        private static string repoOwner = "EdoardoBorgogno"; //==> owner of repository
        private static string repoName = "ARX-PME_Release"; //==> name of repository

        /// <summary>
        /// Get the latest version from github repository.
        /// </summary>
        /// <returns>Release Object.</returns>
        public static async Task<Release> getLatestVersion()
        {
            //
            // Github Api: https://octokitnet.readthedocs.io/en/latest/getting-started/
            //

            GitHubClient client = new GitHubClient(new ProductHeaderValue("ARX-PME"));

            Release release = await client.Repository.Release.GetLatest(repoOwner, repoName);

            return release;
        }

        /// <summary>
        /// Download release.json from github and create ReleaseClass object.
        /// </summary>
        /// <param name="releaseTag">Tag name of release to download.</param>
        /// <returns>ReleaseClass object</returns>
        public static async Task<ReleaseClass.Root> getReleaseJson(string releaseTag)
        {
            //Create http client for download
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("user-agent", "Anything");

            //Url for download
            string urlReleaseJson = $"https://github.com/{repoOwner}/{repoName}/releases" +
                                    $"/download/{releaseTag}/release.json";

            // start download
            HttpResponseMessage response = await client.GetAsync(urlReleaseJson);

            var releaseJson = await response.Content.ReadAsStringAsync(); // get the release.json content

            //Create Release object
            ReleaseClass.Root releaseClass = JsonConvert.DeserializeObject<ReleaseClass.Root>(releaseJson)!;

            return releaseClass;
        }

        /// <summary>
        /// Download asset from github release.
        /// </summary>
        /// <param name="asset">ReleaseObject.asset object.</param>
        /// <param name="releaseTag">Tag name of release to download.</param>
        /// <returns>Stream that contain file</returns>
        public static async Task<Stream?> downloadAsset(ReleaseClass.Info asset, string releaseTag)
        {
            //Create http client for download
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("user-agent", "Anything");

            //Url for download
            string downloadUrl = $"https://github.com/{repoOwner}/{repoName}/releases" +
                                 $"/download/{releaseTag}/{asset.name}";

            // start download
            HttpResponseMessage response = await client.GetAsync(downloadUrl);

            // if download is correct
            if (response.IsSuccessStatusCode)
            {
                HttpContent content = response.Content;

                // If content isn't null
                if(response.Content != null)
                    return await content.ReadAsStreamAsync(); // get the actual content stream
            }

            return null;
        }
    }
}
