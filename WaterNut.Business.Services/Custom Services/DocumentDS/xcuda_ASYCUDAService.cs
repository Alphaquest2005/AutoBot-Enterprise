using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using System.Linq;

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
                
                res.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = null;  //its a shared resource in multi threading
                res.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = null;
                res.xcuda_ASYCUDA_ExtendedProperties.Document_Type = null;
                res.xcuda_ASYCUDA_ExtendedProperties.ExportTemplate = null;
                return await Updatexcuda_ASYCUDA(res).ConfigureAwait(false);
               
            }
            return doc;
        }
    }
}



