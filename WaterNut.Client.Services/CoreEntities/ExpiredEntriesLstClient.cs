﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CoreEntities.Client.DTO;
using CoreEntities.Client.Contracts;
using Core.Common.Client.Services;


using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace CoreEntities.Client.Services
{
    [Export (typeof(ExpiredEntriesLstClient))]
    [Export (typeof(IExpiredEntriesLstService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ExpiredEntriesLstClient :  ClientService<IExpiredEntriesLstService>, IExpiredEntriesLstService, IDisposable
    {
        
        public async Task<IEnumerable<ExpiredEntriesLst>> GetExpiredEntriesLst(List<string> includesLst = null)
        {
            return await Channel.GetExpiredEntriesLst(includesLst).ConfigureAwait(false);
        }

        public async Task<ExpiredEntriesLst> GetExpiredEntriesLstByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetExpiredEntriesLstByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ExpiredEntriesLst>> GetExpiredEntriesLstByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetExpiredEntriesLstByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ExpiredEntriesLst>> GetExpiredEntriesLstByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetExpiredEntriesLstByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ExpiredEntriesLst>> GetExpiredEntriesLstByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetExpiredEntriesLstByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<ExpiredEntriesLst>> GetExpiredEntriesLstByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetExpiredEntriesLstByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ExpiredEntriesLst>> GetExpiredEntriesLstByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetExpiredEntriesLstByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<ExpiredEntriesLst> UpdateExpiredEntriesLst(ExpiredEntriesLst entity)
        {
           return await Channel.UpdateExpiredEntriesLst(entity).ConfigureAwait(false);
        }

        public async Task<ExpiredEntriesLst> CreateExpiredEntriesLst(ExpiredEntriesLst entity)
        {
           return await Channel.CreateExpiredEntriesLst(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteExpiredEntriesLst(string id)
        {
            return await Channel.DeleteExpiredEntriesLst(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedExpiredEntriesLst(IEnumerable<string> selectedExpiredEntriesLst)
        {
           return await Channel.RemoveSelectedExpiredEntriesLst(selectedExpiredEntriesLst).ConfigureAwait(false);
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

        public async Task<IEnumerable<ExpiredEntriesLst>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ExpiredEntriesLst>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<ExpiredEntriesLst>> GetExpiredEntriesLstByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
            return  await Channel.GetExpiredEntriesLstByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(false);
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
