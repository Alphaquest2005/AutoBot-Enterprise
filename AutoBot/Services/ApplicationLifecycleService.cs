using System.IO;
using System.Windows.Forms; // For Application.Exit()
using FileTypes = CoreEntities.Business.Entities.FileTypes; // Assuming FileTypes is needed based on original signature

namespace AutoBot.Services
{
    /// <summary>
    /// Handles application lifecycle operations like exiting.
    /// Extracted from AutoBot.Utils class to adhere to SRP.
    /// </summary>
    public static class ApplicationLifecycleService
    {
        /// <summary>
        /// Exits the application.
        /// </summary>
        /// <param name="arg1">Original FileTypes argument (purpose unclear, retained for compatibility).</param>
        /// <param name="arg2">Original FileInfo[] argument (purpose unclear, retained for compatibility).</param>
        public static void KillApplication(FileTypes arg1, FileInfo[] arg2) // Renamed for clarity
        {
            // Consider logging or cleanup before exiting if necessary
            Application.Exit();
        }
    }
}