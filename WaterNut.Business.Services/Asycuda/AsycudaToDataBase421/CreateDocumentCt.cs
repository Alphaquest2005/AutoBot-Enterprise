using System.IO;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using WaterNut.Business.Entities; // Assuming DocumentCT is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task<DocumentCT> CreateDocumentCt(AsycudaDocumentSet ads, FileInfo file)
        {
            DocumentCT da = await BaseDataModel.Instance.CreateDocumentCt(ads).ConfigureAwait(false); // Assuming CreateDocumentCt exists
            da.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete = false; // Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.SourceFileName = file.FullName; // Potential NullReferenceException
            da.Document.id = a.id; // Assuming 'a' is accessible field, Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false; // Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = ads.AsycudaDocumentSetId; // Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = ads; // Potential NullReferenceException
            //await BaseDataModel.Instance.SaveDocumentCT(da).ConfigureAwait(false); // Assuming SaveDocumentCT exists

            da.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = false; // Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false; // Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate = false; // Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete = false; // Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.Cancelled = false; // Potential NullReferenceException

            return da;
        }
    }
}