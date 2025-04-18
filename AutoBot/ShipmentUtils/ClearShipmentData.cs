using System.IO;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext is here

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void ClearShipmentData(FileTypes fileType, FileInfo[] fileInfos)
        {
            using (var ctx = new EntryDataDSContext())
            {
                ctx.Database.ExecuteSqlCommand($@"DELETE FROM ShipmentBL WHERE (EmailId = N'{fileType.EmailId}')
                                                    delete from ShipmentInvoice WHERE (EmailId = N'{fileType.EmailId}')
                                                    delete from entrydata WHERE (EmailId = N'{fileType.EmailId}')
                                                    delete from ShipmentManifest WHERE (EmailId = N'{fileType.EmailId}')");
            }
        }
    }
}