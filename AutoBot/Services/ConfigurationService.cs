using System; // Added for Console
using System.Data.Entity; // For AsNoTracking, Include
using System.Linq;
using CoreEntities.Business.Entities; // For CoreEntitiesContext, ApplicationSettings etc.
using WaterNut.DataSpace; // For BaseDataModel

namespace AutoBot.Services
{
    /// <summary>
    /// Handles loading and setting application configuration settings.
    /// Extracted from AutoBot.Utils class to adhere to SRP.
    /// </summary>
    public static class ConfigurationService
    {
        /// <summary>
        /// Sets the current application settings based on the provided ID.
        /// Loads settings including related FileTypes, Contacts, Actions, Mappings, etc.
        /// Updates the singleton BaseDataModel.Instance.CurrentApplicationSettings.
        /// </summary>
        /// <param name="id">The ApplicationSettingsId to load.</param>
        public static void SetCurrentApplicationSettings(int id)
        {
            // Consider adding error handling (e.g., if settings with ID not found)
            using (var ctx = new CoreEntitiesContext()) // Consider context lifetime management/injection
            {
                // Eager load related entities needed for the application settings
                var appSetting = ctx.ApplicationSettings.AsNoTracking()
                    .Include(x => x.FileTypes.Select(ft => ft.FileTypeContacts.Select(c => c.Contacts))) // Deeper include
                    .Include(x => x.FileTypes.Select(ft => ft.FileTypeActions.Select(a => a.Actions))) // Deeper include
                    .Include(x => x.FileTypes.Select(ft => ft.FileTypeMappings)) // Deeper include
                    .Include(x => x.Declarants)
                    .Include(x => x.EmailMapping)
                    .Where(x => x.IsActive && x.ApplicationSettingsId == id) // Combine Where clauses
                    .FirstOrDefault(); // Use FirstOrDefault to handle case where ID might not exist

                if (appSetting != null)
                {
                    // set BaseDataModel CurrentAppSettings - This creates a dependency on a specific singleton implementation.
                    // Consider returning the appSetting object and letting the caller manage the state.
                    BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                    // TODO: check emails? (Original comment) - What needs to be checked?
                }
                else
                {
                    // Handle case where settings are not found - log error, throw exception?
                    Console.WriteLine($"Warning: ApplicationSettings with ID {id} not found or not active.");
                }
            }
        }
    }
}