


using System;
using System.Threading.Tasks;
using SalesDataQS.Client.DTO;

namespace SalesDataQS.Client.Services
{
  
    public partial class SalesDataDetailClient 
    {
        public async Task SaveSalesDataDetail(SalesDataDetail salesDataDetail)
        {
            await Channel.SaveSalesDataDetail(salesDataDetail).ConfigureAwait(false);
        }
          
    }
}

