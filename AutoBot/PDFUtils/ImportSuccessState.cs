using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using WaterNut.Business.Services.Importers; // Assuming DataFileProcessor, DataFile are here

namespace AutoBot
{
    public partial class PDFUtils
    {
        private static async Task<bool> ImportSuccessState(string file, string emailId, FileTypes fileType, bool overWriteExisting,
            List<AsycudaDocumentSet> docSet, List<dynamic> csvLines)
        {
            try
            {
               return await new DataFileProcessor().Process(new DataFile(fileType, docSet, overWriteExisting,
                    emailId,
                    file, csvLines)).ConfigureAwait(false);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }


        }
    }
}