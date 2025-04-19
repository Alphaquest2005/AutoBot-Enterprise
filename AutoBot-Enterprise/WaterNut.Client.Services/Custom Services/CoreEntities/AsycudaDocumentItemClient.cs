


using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Client.DTO;

namespace CoreEntities.Client.Services
{
   
    public partial class AsycudaDocumentItemClient 
    {
        public async Task RemoveSelectedItems(IEnumerable<int> lst)
        {
            await Channel.RemoveSelectedItems(lst).ConfigureAwait(false);
        }

        public async Task SaveAsycudaDocumentItem(AsycudaDocumentItem asycudaDocumentItem)
        {
            await Channel.SaveAsycudaDocumentItem(asycudaDocumentItem).ConfigureAwait(false);
        }
    }
}

