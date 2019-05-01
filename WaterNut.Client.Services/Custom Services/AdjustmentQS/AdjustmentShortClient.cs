using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdjustmentQS.Client.Contracts;
using AdjustmentQS.Client.Services;
using Core.Common.Contracts;
using WaterNut.Client.Services;

namespace AdjustmentQS.Client.Services
{
    public partial class AdjustmentShortClient
    {
        public async Task AutoMatch()
        {
            await Channel.AutoMatch().ConfigureAwait(false);
        }

        public async Task CreateIM9(string filterExpression, bool perInvoice, bool process7100, int asycudaDocumentSetId, string ex9Type, string dutyFreePaid)
        {
            await Channel.CreateIM9(filterExpression, perInvoice, process7100, asycudaDocumentSetId, ex9Type, dutyFreePaid).ConfigureAwait(false);
        }
    }
}
