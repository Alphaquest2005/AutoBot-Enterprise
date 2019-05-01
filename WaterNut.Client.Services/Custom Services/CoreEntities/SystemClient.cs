
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationQS.Client.Contracts;
using AllocationQS.Client.Services;
using Core.Common.Contracts;
using CoreEntities.Client.Contracts;
using CoreEntities.Client.DTO;
using WaterNut.Client.Services;

namespace CoreEntities.Client.Services
{
   [Export(typeof(AsycudaSalesAllocationsExClient))]
        [Export(typeof(IAsycudaSalesAllocationsExService))]
        [Export(typeof(IClientService))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SystemClient : ClientService<ISystemService>, ISystemService, IDisposable
    {
        
        public bool ValidateInstallation()
        {
            return Channel.ValidateInstallation();
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

