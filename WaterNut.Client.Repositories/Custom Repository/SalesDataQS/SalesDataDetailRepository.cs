

using System;
using System.Threading.Tasks;
using SalesDataQS.Client.Entities;
using SalesDataQS.Client.Services;

namespace SalesDataQS.Client.Repositories 
{
   
    public partial class SalesDataDetailRepository
    {

        public async Task SaveSalesDataDetail(SalesDataDetail salesDataDetail)
        {
            using (var ctx = new SalesDataDetailClient())
            {
                await ctx.SaveSalesDataDetail(salesDataDetail.DTO).ConfigureAwait(false);
            }
        }

    }
}

