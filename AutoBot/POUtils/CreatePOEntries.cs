using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WaterNut.Business.Entities; // Assuming DocumentCT is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using EmailDownloader; // Assuming EmailDownloader class is here

namespace AutoBot
{
    public partial class POUtils
    {
        public static List<DocumentCT> CreatePOEntries(int docSetId, List<int> entrylst)
        {
            try
            {
                BaseDataModel.Instance.ClearAsycudaDocumentSet((int)docSetId).Wait(); // Assuming ClearAsycudaDocumentSet exists
                BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docSetId, 0); // Assuming UpdateAsycudaDocumentSetLastNumber exists

                var po = CurrentPOInfo(docSetId).FirstOrDefault(); // Calls method in another partial class
                var insts = Path.Combine(po.Item2, "InstructionResults.txt"); // Potential NullReferenceException if po is null
                if (File.Exists(insts)) File.Delete(insts);


                return BaseDataModel.Instance.AddToEntry(entrylst, docSetId,
                    (BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true), true, false).Result; // Assuming AddToEntry exists
            }
            catch (Exception ex)
            {
                EmailDownloader.EmailDownloader.SendEmail(BaseDataModel.GetClient(), null, $"Bug Found", // Assuming GetClient exists
                     EmailDownloader.EmailDownloader.GetContacts("Developer"), $"{ex.Message}\r\n{ex.StackTrace}", // Assuming GetContacts exists
                    Array.Empty<string>());
                throw;
            }
        }
    }
}