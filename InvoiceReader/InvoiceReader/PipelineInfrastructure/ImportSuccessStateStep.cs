using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Threading.Tasks;
using System.Text;
using DocumentDS.Business.Entities;
using System.IO;
using WaterNut.DataSpace;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ProcessInvoiceTemplateStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static bool ImportSuccessState(string file, string emailId, FileTypes fileType, bool overWriteExisting,
            List<AsycudaDocumentSet> docSet, WaterNut.DataSpace.Invoice tmp, List<dynamic> csvLines)
        {
            if (fileType.Id != tmp.OcrInvoices.FileTypeId)
                fileType = FileTypeManager.GetFileType(tmp.OcrInvoices.FileTypeId);


            return new DataFileProcessor().Process(new DataFile(fileType, docSet, overWriteExisting,
                emailId,
                file, csvLines)).GetAwaiter().GetResult();
        }
    }
}