using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using WaterNut.DataSpace.PipelineInfrastructure; // Added using directive

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    public static async Task<Dictionary<string, (string file, string, ImportStatus Success)>> Import(string file,
        int fileTypeId, string emailId, bool overWriteExisting,
        List<AsycudaDocumentSet> docSet, FileTypes fileType, EmailDownloader.Client client)
    {
        Console.WriteLine(
            $"[OCR DEBUG] InvoiceReader.Import: Starting import for file '{file}', FileTypeId: {fileTypeId}, EmailId: '{emailId}'");

        var context = new PipelineInfrastructure.InvoiceProcessingContext
        {
            FilePath = file,
            FileTypeId = fileTypeId,
            EmailId = emailId,
            OverWriteExisting = overWriteExisting,
            DocSet = docSet,
            FileType = fileType,
            Client = client
        };

        var steps = new List<IPipelineStep<PipelineInfrastructure.InvoiceProcessingContext>>();

        // Step 1: Get PDF Text
        steps.Add(new PipelineInfrastructure.GetPdfTextStep());

        // Step 2: Get Templates
        steps.Add(new PipelineInfrastructure.GetTemplatesStep());

        // Step 3: Get Possible Invoices
        steps.Add(new PipelineInfrastructure.GetPossibleInvoicesStep());

        var initialPipeline = new PipelineInfrastructure.PipelineRunner<PipelineInfrastructure.InvoiceProcessingContext>(steps);
        await initialPipeline.Run(context).ConfigureAwait(false); // Run steps that populate context

        // Now process each possible invoice using a separate step or a sub-pipeline
        if (context.PossibleInvoices != null)
        {
            var templateProcessingSteps = new List<IPipelineStep<PipelineInfrastructure.InvoiceProcessingContext>>();
            var possibleInvoicesList = context.PossibleInvoices.ToList();
            for (int i = 0; i < possibleInvoicesList.Count; i++)
            {
                var template = possibleInvoicesList[i];
                templateProcessingSteps.Add(new PipelineInfrastructure.ProcessInvoiceTemplateStep(template, i == possibleInvoicesList.Count - 1));
            }

            var templatePipeline = new PipelineInfrastructure.PipelineRunner<PipelineInfrastructure.InvoiceProcessingContext>(templateProcessingSteps);
            // We need to run each template processing step individually to update the context.Imports
             foreach (var step in templateProcessingSteps)
            {
                await step.Execute(context).ConfigureAwait(false);
            }
        }


        Console.WriteLine(
            $"[OCR DEBUG] InvoiceReader.Import: Finished import for file '{file}'. Results: {context.Imports.Count} entries.");
        return context.Imports;
    }
}