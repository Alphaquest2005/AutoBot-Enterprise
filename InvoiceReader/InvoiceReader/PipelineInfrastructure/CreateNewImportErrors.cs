using CoreEntities.Business.Entities;
using OCR.Business.Entities;

namespace WaterNut.DataSpace.PipelineInfrastructure;

public static partial class InvoiceProcessingUtils
{
    private static ImportErrors CreateNewImportErrors(string pdftxt, string error, List<Line> failedlst,
        AsycudaDocumentSet_Attachments att)
    {
        ImportErrors importErr;
        importErr = new ImportErrors(true) { Id = att.Id };
        UpdateImportError(pdftxt, error, failedlst, importErr, att);
        return importErr;
    }
}