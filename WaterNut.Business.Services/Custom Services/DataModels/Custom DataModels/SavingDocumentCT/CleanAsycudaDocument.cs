using DocumentDS.Business.Entities;
using TrackableEntities;

namespace DocumentDS.Business.Services
{
    public class CleanAsycudaDocument
    {
        public static void Execute(xcuda_ASYCUDA res)
        {
            if (res.TrackingState == TrackingState.Added)
            {
                res.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet =
                    null; //its a shared resource in multi threading
                res.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = null;

                res.AsycudaDocument_Attachments.ForEach(x =>
                {
                    //x.Attachment.AsycudaDocumentSet_Attachments.ForEach(z =>
                    //{
                    //    z.AsycudaDocumentSet = null;
                    //    z.Attachment = null;
                    //});

                    x.Attachment = null;
                }); //.Where(x => x.TrackingState != TrackingState.Added)

                //res.xcuda_ASYCUDA_ExtendedProperties.ExportTemplate = null;
                //res.xcuda_General_information = null;
                //res.xcuda_Identification = null;
                //res.xcuda_Property = null;
                //res.xcuda_Valuation = null;
                //res.xcuda_Declarant = null;
                if (res.xcuda_Traders.xcuda_Exporter.TrackingState == TrackingState.Added
                    && string.IsNullOrEmpty(res.xcuda_Traders.xcuda_Exporter.Exporter_name)) res.xcuda_Traders = null;
            }
        }
    }
}