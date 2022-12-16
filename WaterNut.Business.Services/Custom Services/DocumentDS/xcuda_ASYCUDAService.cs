using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using System.Linq;
using MoreLinq;
using TrackableEntities;

namespace DocumentDS.Business.Services
{
   
    public partial class xcuda_ASYCUDAService 
    {
        //private readonly DocumentDSContext dbContext;
        public async Task<xcuda_ASYCUDA> CleanAndUpdateXcuda_ASYCUDA(xcuda_ASYCUDA doc)
        {

            var res = doc.ModifiedProperties == null? doc: doc.ChangeTracker.GetChanges().FirstOrDefault();
            if (res != null)
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

                    });//.Where(x => x.TrackingState != TrackingState.Added)

                    //res.xcuda_ASYCUDA_ExtendedProperties.ExportTemplate = null;
                    //res.xcuda_General_information = null;
                    //res.xcuda_Identification = null;
                    //res.xcuda_Property = null;
                    //res.xcuda_Valuation = null;
                    //res.xcuda_Declarant = null;
                    if (res.xcuda_Traders.xcuda_Exporter.TrackingState == TrackingState.Added
                        && string.IsNullOrEmpty(res.xcuda_Traders.xcuda_Exporter.Exporter_name)) res.xcuda_Traders = null;
                }

                return await Updatexcuda_ASYCUDA(res).ConfigureAwait(false);
               
            }
            return doc;
        }
    }
}



