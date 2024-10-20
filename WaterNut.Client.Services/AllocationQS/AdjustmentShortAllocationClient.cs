﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationQS.Client.DTO;
using AllocationQS.Client.Contracts;
using Core.Common.Client.Services;


using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace AllocationQS.Client.Services
{
    [Export (typeof(AdjustmentShortAllocationClient))]
    [Export (typeof(IAdjustmentShortAllocationService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdjustmentShortAllocationClient :  ClientService<IAdjustmentShortAllocationService>, IAdjustmentShortAllocationService, IDisposable
    {
        
        public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocations(List<string> includesLst = null)
        {
            return await Channel.GetAdjustmentShortAllocations(includesLst).ConfigureAwait(false);
        }

        public async Task<AdjustmentShortAllocation> GetAdjustmentShortAllocationByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetAdjustmentShortAllocationByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetAdjustmentShortAllocationsByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetAdjustmentShortAllocationsByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetAdjustmentShortAllocationsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAdjustmentShortAllocationsByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAdjustmentShortAllocationsByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<AdjustmentShortAllocation> UpdateAdjustmentShortAllocation(AdjustmentShortAllocation entity)
        {
           return await Channel.UpdateAdjustmentShortAllocation(entity).ConfigureAwait(false);
        }

        public async Task<AdjustmentShortAllocation> CreateAdjustmentShortAllocation(AdjustmentShortAllocation entity)
        {
           return await Channel.CreateAdjustmentShortAllocation(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAdjustmentShortAllocation(string id)
        {
            return await Channel.DeleteAdjustmentShortAllocation(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedAdjustmentShortAllocation(IEnumerable<string> selectedAdjustmentShortAllocation)
        {
           return await Channel.RemoveSelectedAdjustmentShortAllocation(selectedAdjustmentShortAllocation).ConfigureAwait(false);
        }

       // Virtural List implementation

        public async Task<int> CountByExpressionLst(List<string> expLst)
        {
            return await Channel.CountByExpressionLst(expLst).ConfigureAwait(continueOnCapturedContext: false);
        }
        
	    public async Task<int> Count(string exp)
        {
            return await Channel.Count(exp).ConfigureAwait(continueOnCapturedContext: false);
        }

		public async Task<int> CountNav(string exp, Dictionary<string, string> navExp)
        {
           return await Channel.CountNav(exp, navExp).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AdjustmentShortAllocation>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AdjustmentShortAllocation>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByPreviousItem_Id(string PreviousItem_Id, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationByPreviousItem_Id(PreviousItem_Id, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationByEntryDataDetailsId(EntryDataDetailsId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByxBond_Item_Id(string xBond_Item_Id, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationByxBond_Item_Id(xBond_Item_Id, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByxASYCUDA_Id(string xASYCUDA_Id, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationByxASYCUDA_Id(xASYCUDA_Id, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationBypASYCUDA_Id(string pASYCUDA_Id, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationBypASYCUDA_Id(pASYCUDA_Id, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationByFileTypeId(FileTypeId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByEmailId(string EmailId, List<string> includesLst = null)
        {
            return  await Channel.GetAdjustmentShortAllocationByEmailId(EmailId, includesLst).ConfigureAwait(false);
        }
			 
          public decimal SumField(string whereExp, string sumExp)
		{
			return Channel.SumField(whereExp,sumExp);
		}

        public async Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field)
        {
            return await Channel.SumNav(exp,navExp,field);
        }

		public string MinField(string whereExp, string sumExp)
		{
			return Channel.MinField(whereExp,sumExp);
		}

		#region IDisposable implementation

            /// <summary>
            /// IDisposable.Dispose implementation, calls Dispose(true).
            /// </summary>
            void IDisposable.Dispose()
            {
                Dispose(true);
            }

            /// <summary>
            /// Dispose worker method. Handles graceful shutdown of the
            /// client even if it is an faulted state.
            /// </summary>
            /// <param name="disposing">Are we disposing (alternative
            /// is to be finalizing)</param>
            protected new void Dispose(bool disposing)
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

