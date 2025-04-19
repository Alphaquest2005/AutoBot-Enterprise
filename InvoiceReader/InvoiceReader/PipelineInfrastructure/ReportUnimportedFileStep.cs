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
using EmailDownloader;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ProcessInvoiceTemplateStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static void ReportUnimportedFile(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId,
            EmailDownloader.Client client, string pdftxt, string error,
            List<Line> failedlst)
        {
            var fileInfo = new FileInfo(file);

            var txtFile = InvoiceProcessingUtils.WriteTextFile(file, pdftxt);
            var body = InvoiceProcessingUtils.CreateEmail(file, client, error, failedlst, fileInfo, txtFile); // Assuming CreateEmail is accessible or moved
            InvoiceProcessingUtils.CreateTestCase(file, failedlst, txtFile, body); // Assuming CreateTestCase is accessible or moved


            InvoiceProcessingUtils.SaveImportError(asycudaDocumentSets, file, emailId, fileTypeId, pdftxt, error, failedlst, fileInfo); // Assuming SaveImportError is accessible or moved
        }
    }
}