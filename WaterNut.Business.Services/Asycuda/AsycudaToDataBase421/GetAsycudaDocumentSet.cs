using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(int docSetId = -1)
        {
            AsycudaDocumentSet ads;
            //db.AsycudaDocumentSet.FirstOrDefault(
            //    x =>
            //    x.Declarant_Reference_Number.Replace(" ", "")
            //     .Contains(refstr.Substring(0, refstr.Length - refstr.IndexOf("-F"))));
            //if (ads == null)
            //{
            if (docSetId == -1)
            {
                // This calls NewAsycudaDocumentSet, which needs to be in its own partial class
                ads = await NewAsycudaDocumentSet(a).ConfigureAwait(false); // Assuming 'a' is accessible field
            }
            else
            {
                ads =
                    await
                        BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false); // Assuming GetAsycudaDocumentSet exists
            }

            return ads;
        }
    }
}