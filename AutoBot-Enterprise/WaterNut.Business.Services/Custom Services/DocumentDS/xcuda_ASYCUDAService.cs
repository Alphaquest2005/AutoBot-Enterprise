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
                CleanAsycudaDocument.Execute(res);

                return await Updatexcuda_ASYCUDA(res).ConfigureAwait(false);
            }
            return doc;
        }
    }
}



