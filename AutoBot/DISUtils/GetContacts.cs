using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, Contacts are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _databaseCommandTimeout is defined elsewhere or needs moving
        // private static readonly int _databaseCommandTimeout = 30;

        private static string[] GetContacts(List<string> roles)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                var contacts = ctx.Contacts.Where(x => roles.Any(r => r == x.Role))
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => x.EmailAddress).Distinct().ToArray();
                return contacts;
            }
        }
    }
}