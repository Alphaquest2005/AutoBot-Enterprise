using System.Threading.Tasks;
using AdjustmentQS.Client.Services;

namespace AdjustmentQS.Client.Repositories
{
    public partial class AdjustmentOverRepository
    {
        public async Task CreateOPS(string filterExpression, object perInvoice, int asycudaDocumentSetId)
        {
            using (var t = new AdjustmentOverClient())
            {
                await t.CreateOPS(filterExpression, perInvoice,  asycudaDocumentSetId).ConfigureAwait(false);
            }
        }
    }
}