using CoreEntities.Business.Entities;
using OCR.Business.Entities;

namespace WaterNut.DataSpace.PipelineInfrastructure;

public static partial class InvoiceProcessingUtils
{
    private static void SaveDbErrorDetails(string pdftxt, string error, List<Line> failedlst,
        List<AsycudaDocumentSet_Attachments> existingAttachment)
    {        
            foreach (var att in existingAttachment)
            {
                ProcessImportError(pdftxt, error, failedlst, att);
            }        
    }

    private static void ProcessImportError(string pdftxt, string error, List<Line> failedlst,  AsycudaDocumentSet_Attachments att)
    {
        using (var ctx = new OCRContext())
        {
        var importErr = ctx.ImportErrors.FirstOrDefault(x => x.Id == att.Id);
        if (importErr == null)
        {
            importErr = CreateNewImportErrors(pdftxt, error, failedlst, att);
            ctx.ImportErrors.Add(importErr);
        }
        else
        {
            UpdateImportError(pdftxt, error, failedlst, importErr, att);
        }

        ctx.SaveChanges();
        }
    }
}