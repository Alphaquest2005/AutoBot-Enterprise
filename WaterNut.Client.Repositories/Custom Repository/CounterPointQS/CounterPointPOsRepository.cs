
using CounterPointQS.Client.Services;
using Core.Common.Business.Services;
using System.Diagnostics;


using System.Threading.Tasks;
using System;
using System.ServiceModel;
using CounterPointPOs = CounterPointQS.Client.Entities.CounterPointPOs;

namespace CounterPointQS.Client.Repositories 
{

    public partial class CounterPointPOsRepository
    {
        public async Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId)
        {
            try
            {
                using (var t = new CounterPointPOsClient())
                {
                    await t.DownloadCPO(c.DTO, asycudaDocumentSetId).ConfigureAwait(false);
                }
            }


            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }


        }
    }
}

