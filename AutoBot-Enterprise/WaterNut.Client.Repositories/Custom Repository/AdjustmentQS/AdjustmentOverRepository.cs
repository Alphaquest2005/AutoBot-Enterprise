using System.Threading.Tasks;
using AdjustmentQS.Client.Services;

namespace AdjustmentQS.Client.Repositories
{
    public partial class AdjustmentOverRepository
    {
        public async Task CreateOPS(string filterExpression, object perInvoice, string adjustmentType, int asycudaDocumentSetId)
        {
            using (var t = new AdjustmentOverClient())
            {
                await t.CreateOPS(filterExpression, perInvoice,  adjustmentType, asycudaDocumentSetId).ConfigureAwait(false);
            }
        }
    }
}