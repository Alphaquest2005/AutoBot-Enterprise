using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    private static OCR_FailedLines CreateFailedLines(AsycudaDocumentSet_Attachments att, Line x)
    {
        return new OCR_FailedLines(true)
        {
            TrackingState = TrackingState.Added,
            DocSetAttachmentId = att.Id,
            LineId = x.OCR_Lines.Id,
            Resolved = false,
            OCR_FailedFields = x.FailedFields.SelectMany(z =>
                    z.SelectMany(q => q.Value.Select(w => w.Key)))
                .DistinctBy(z => z.fields.Id)
                .Select(z => new OCR_FailedFields(true)
                {
                    TrackingState = TrackingState.Added,
                    FieldId = z.fields.Id,
                })
                .ToList()
        };
    }
}