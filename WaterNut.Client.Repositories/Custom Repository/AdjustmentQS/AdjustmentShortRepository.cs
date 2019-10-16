using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdjustmentQS.Client.Services;

namespace AdjustmentQS.Client.Repositories
{

    public partial class AdjustmentShortRepository 
    {
        public async Task AutoMatch(int applicationSettingsId)
        {
            using (var t = new AdjustmentShortClient())
            {
                await t.AutoMatch(applicationSettingsId).ConfigureAwait(false);
            }
        }

        public async Task MatchToAsycudaItem(int entryDataDetailId, int itemId)
        {
            using (var t = new AdjustmentShortClient())
            {
                await t.MatchToAsycudaItem(entryDataDetailId, itemId).ConfigureAwait(false);
            }
        }

        public async Task CreateIM9(string filterExpression, bool perInvoice, bool process7100, int asycudaDocumentSetId, string ex9Type, string dutyFreePaid)
        {
            using (var t = new AdjustmentShortClient())
            {
                await t.CreateIM9(filterExpression, perInvoice, process7100, asycudaDocumentSetId, ex9Type, dutyFreePaid ).ConfigureAwait(false);
            }
        }

      
    }
}
