using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARX_PME_Updater
{
    /// <summary>
    /// Handle program update.
    /// </summary>
    internal class UpdateProgram
    {
        /// <summary>
        /// Update program from github.
        /// </summary>
        public static async Task CheckAndUpdate()
        {
            // Update to latest version
            await DownloadRelease.updateToLatestVersion();
        }
    }
}
