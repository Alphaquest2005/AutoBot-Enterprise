using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        public static void ReportUnimportedFile(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId,
            EmailDownloader.Client client, string pdftxt, string error,
            List<Line> failedlst)
        {
            var fileInfo = new FileInfo(file);

            var txtFile = WriteTextFile(file, pdftxt);
            var body = CreateEmail(file, client, error, failedlst, fileInfo, txtFile); // Assuming CreateEmail is accessible or moved
            CreateTestCase(file, failedlst, txtFile, body); // Assuming CreateTestCase is accessible or moved


            SaveImportError(asycudaDocumentSets, file, emailId, fileTypeId, pdftxt, error, failedlst, fileInfo); // Assuming SaveImportError is accessible or moved
        }
    }
}