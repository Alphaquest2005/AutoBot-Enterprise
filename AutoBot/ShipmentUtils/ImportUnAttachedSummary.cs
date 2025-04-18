using System;
using System.IO;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using xlsxWriter; // Assuming XlsxWriter is here

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void ImportUnAttachedSummary(FileTypes ft, FileInfo[] fs)
        {
            try
            {
                foreach (var file in fs)
                {
                    var reference = XlsxWriter.SaveUnAttachedSummary(file);
                    ft.EmailId = reference;
                    // Need to update this call if CreateShipmentEmail is moved
                    CreateShipmentEmail(ft, fs);
                    //}

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}