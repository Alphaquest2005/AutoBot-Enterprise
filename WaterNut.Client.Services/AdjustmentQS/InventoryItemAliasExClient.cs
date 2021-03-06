﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using AdjustmentQS.Client.DTO;
using AdjustmentQS.Client.Contracts;
using Core.Common.Client.Services;


using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace AdjustmentQS.Client.Services
{
    [Export (typeof(InventoryItemAliasExClient))]
    [Export (typeof(IInventoryItemAliasExService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class InventoryItemAliasExClient :  ClientService<IInventoryItemAliasExService>, IInventoryItemAliasExService, IDisposable
    {
        
        public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExes(List<string> includesLst = null)
        {
            return await Channel.GetInventoryItemAliasExes(includesLst).ConfigureAwait(false);
        }

        public async Task<InventoryItemAliasEx> GetInventoryItemAliasExByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetInventoryItemAliasExByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetInventoryItemAliasExesByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetInventoryItemAliasExesByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetInventoryItemAliasExesByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetInventoryItemAliasExesByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetInventoryItemAliasExesByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<InventoryItemAliasEx> UpdateInventoryItemAliasEx(InventoryItemAliasEx entity)
        {
           return await Channel.UpdateInventoryItemAliasEx(entity).ConfigureAwait(false);
        }

        public async Task<InventoryItemAliasEx> CreateInventoryItemAliasEx(InventoryItemAliasEx entity)
        {
           return await Channel.CreateInventoryItemAliasEx(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteInventoryItemAliasEx(string id)
        {
            return await Channel.DeleteInventoryItemAliasEx(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedInventoryItemAliasEx(IEnumerable<string> selectedInventoryItemAliasEx)
        {
           return await Channel.RemoveSelectedInventoryItemAliasEx(selectedInventoryItemAliasEx).ConfigureAwait(false);
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

        public async Task<IEnumerable<InventoryItemAliasEx>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<InventoryItemAliasEx>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
            return  await Channel.GetInventoryItemAliasExByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExByInventoryItemId(string InventoryItemId, List<string> includesLst = null)
        {
            return  await Channel.GetInventoryItemAliasExByInventoryItemId(InventoryItemId, includesLst).ConfigureAwait(false);
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

