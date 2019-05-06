
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationQS.Client.DTO;
using AllocationQS.Client.Contracts;
using CoreEntities.Client.DTO;
using PreviousDocumentQS.Client.DTO;
using WaterNut.Client.Services;
using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace AllocationQS.Client.Services
{
    [Export(typeof(AsycudaSalesAllocationsExClient))]
    [Export(typeof(IAsycudaSalesAllocationsExService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AllocationsClient : ClientService<IAllocationsService>, IAllocationsService, IDisposable
    {
        
        public Task<IEnumerable<AsycudaSalesAllocationsEx>> GetAsycudaSalesAllocationsExesByDates(DateTime startDate, DateTime endDate)
        {
            return Channel.GetAsycudaSalesAllocationsExesByDates(startDate,endDate);
        }

        public async Task CreateEx9(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks, int AsycudaDocumentSetId)
        {
             await Channel.CreateEx9(filterExpression, perIM7, process7100, applyCurrentChecks, AsycudaDocumentSetId).ConfigureAwait(false);
        }

        public async Task CreateOPS(string filterExpression, int AsycudaDocumentSetId)
        {
             await Channel.CreateOPS(filterExpression, AsycudaDocumentSetId).ConfigureAwait(false);
        }

        public async Task ManuallyAllocate(int AllocationId, int PreviousItem_Id)
        {
           await Channel.ManuallyAllocate(AllocationId, PreviousItem_Id).ConfigureAwait(false);
        }

        public async Task ClearAllocations(IEnumerable<int> alst)
        {
            await Channel.ClearAllocations(alst).ConfigureAwait(false);
        }

        public async Task ClearAllocationsByFilter(string filterExpression)
        {
            await Channel.ClearAllocationsByFilter(filterExpression).ConfigureAwait(false);
        }

        public async Task CreateIncompOPS(string filterExpression, int AsycudaDocumentSetId)
        {
            await Channel.CreateIncompOPS(filterExpression, AsycudaDocumentSetId).ConfigureAwait(false);
        }

        public async Task AllocateSales(ApplicationSettings applicationSettings,
            bool allocateToLastAdjustment)
        {
            await Channel.AllocateSales(applicationSettings, allocateToLastAdjustment).ConfigureAwait(false);
        }

        public async Task ReBuildSalesReports()
        {
            await Channel.ReBuildSalesReports().ConfigureAwait(false);
        }

        public async Task ClearAllAllocations(int applicationSettingsId)
        {
            await Channel.ClearAllAllocations(applicationSettingsId).ConfigureAwait(false);
        }

        public async Task ReBuildSalesReports(int asycuda_Id)
        {
            await Channel.ReBuildSalesReports().ConfigureAwait(false);
        }

        #region IDisposable implementation

        /// <summary>
        /// IDisposable.Dispose implementation, calls Dispose(true).
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose worker method. Handles graceful shutdown of the
        /// client even if it is an faulted state.
        /// </summary>
        /// <param name="disposing">Are we disposing (alternative
        /// is to be finalizing)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (State != CommunicationState.Faulted)
                    {
                        Close();
                    }
                }
                finally
                {
                    if (State != CommunicationState.Closed)
                    {
                        Abort();
                    }
                    GC.SuppressFinalize(this);
                }
            }
        }





        #endregion
    }
}

