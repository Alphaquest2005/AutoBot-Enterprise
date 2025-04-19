

using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;

namespace CoreEntities.Business.Services
{
   
    public partial class AsycudaDocumentItemService
    {
        public async Task RemoveSelectedItems(IEnumerable<int> lst)
        {

            await
                WaterNut.DataSpace.AsycudaEntrySummaryListModel.Instance.RemoveSelectedItems(lst).ConfigureAwait(false);
        }

        public async Task SaveAsycudaDocumentItem(AsycudaDocumentItem asycudaDocumentItem)
        {
            await
                WaterNut.DataSpace.BaseDataModel.Instance.SaveAsycudaDocumentItem(asycudaDocumentItem)
                    .ConfigureAwait(false);
        }
    }
}



