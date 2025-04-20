// using CoreEntities.Business.Entities;
// using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
// using WaterNut.DataSpace.PipelineInfrastructure; // Added using directive

// namespace WaterNut.DataSpace;

// public partial class InvoiceReader
// {
//     public static async Task<Dictionary<string, (string file, string, ImportStatus Success)>> Import(string file,
//         int fileTypeId, string emailId, bool overWriteExisting,
//         List<AsycudaDocumentSet> docSet, FileTypes fileType, EmailDownloader.Client client)
//     {
//         Console.WriteLine(
//             $"[OCR DEBUG] InvoiceReader.Import: Starting import for file '{file}', FileTypeId: {fileTypeId}, EmailId: '{emailId}'");

//         var context = new InvoiceProcessingContext
//         {
//             FilePath = file,
//             FileTypeId = fileTypeId,
//             EmailId = emailId,
//             OverWriteExisting = overWriteExisting,
//             DocSet = docSet,
//             FileType = fileType,
//             Client = client
//         };

//         // Define the main pipeline steps
//         var mainPipelineSteps = new List<IPipelineStep<InvoiceProcessingContext>>
//         {
//             new GetPdfTextStep(),
//             new GetTemplatesStep(),
//             new GetPossibleInvoicesStep(),
//             new ProcessAllPossibleInvoicesStep(), // This step runs the sub-pipeline for each invoice
//             new ReturnImportsStep() // This step determines overall success and is the last step
//         };

//         // Create and run the main pipeline
//         var mainPipelineRunner = new PipelineRunner<InvoiceProcessingContext>(mainPipelineSteps);
//         bool overallSuccess = await mainPipelineRunner.Run(context).ConfigureAwait(false);

//         Console.WriteLine(
//             $"[OCR DEBUG] InvoiceReader.Import: Main pipeline finished for file '{context.FilePath}'. Overall success: {overallSuccess}. Results: {context.Imports.Count} entries.");

//         // The Import method needs to return the Imports dictionary regardless of overall pipeline success
//         return context.Imports;
//     }
// }