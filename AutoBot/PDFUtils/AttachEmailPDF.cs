using System.IO;
using CoreEntities.Business.Entities; // Assuming FileTypes is from here
using WaterNut.DataSpace; // Assuming BaseDataModel is from here

namespace AutoBot
{
    public partial class PDFUtils
    {
        public static void AttachEmailPDF(FileTypes ft, FileInfo[] fs)
        {
            BaseDataModel.AttachEmailPDF(ft.AsycudaDocumentSetId, ft.EmailId);
        }
    }
}