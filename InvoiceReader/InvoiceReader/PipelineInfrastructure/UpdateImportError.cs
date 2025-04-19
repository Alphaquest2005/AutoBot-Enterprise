using CoreEntities.Business.Entities;
using OCR.Business.Entities;

namespace WaterNut.DataSpace.PipelineInfrastructure;

public static partial class InvoiceProcessingUtils
{
}

public static partial class InvoiceProcessingUtils
{
    private static void UpdateImportError(string pdftxt, string error, List<Line> failedlst, ImportErrors importErr,
        AsycudaDocumentSet_Attachments att)
    {
        importErr.PdfText = pdftxt;
        importErr.Error = error;
        importErr.EntryDateTime = DateTime.Now;
        importErr.OCR_FailedLines = failedlst.Select(x => CreateFailedLines(att, x)).ToList();
    }
}