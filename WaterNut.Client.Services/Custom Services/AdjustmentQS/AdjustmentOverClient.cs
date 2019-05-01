using System.Threading.Tasks;

namespace AdjustmentQS.Client.Services
{
    public partial class AdjustmentOverClient
    {
        public async Task CreateOPS(string filterExpression, object perInvoice, int asycudaDocumentSetId)
        {
            await Channel.CreateOPS(filterExpression, perInvoice, asycudaDocumentSetId).ConfigureAwait(false);
        }
    }
}