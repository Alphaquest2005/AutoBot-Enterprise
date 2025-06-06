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
    [Export (typeof(AllocationsTestCasClient))]
    [Export (typeof(IAllocationsTestCasService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AllocationsTestCasClient :  ClientService<IAllocationsTestCasService>, IAllocationsTestCasService, IDisposable
    {
        
        public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCases(List<string> includesLst = null)
        {
            return await Channel.GetAllocationsTestCases(includesLst).ConfigureAwait(false);
        }

        public async Task<AllocationsTestCas> GetAllocationsTestCasByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetAllocationsTestCasByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCasesByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetAllocationsTestCasesByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCasesByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetAllocationsTestCasesByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCasesByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetAllocationsTestCasesByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCasesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAllocationsTestCasesByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCasesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAllocationsTestCasesByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<AllocationsTestCas> UpdateAllocationsTestCas(AllocationsTestCas entity)
        {
           return await Channel.UpdateAllocationsTestCas(entity).ConfigureAwait(false);
        }

        public async Task<AllocationsTestCas> CreateAllocationsTestCas(AllocationsTestCas entity)
        {
           return await Channel.CreateAllocationsTestCas(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAllocationsTestCas(string id)
        {
            return await Channel.DeleteAllocationsTestCas(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedAllocationsTestCas(IEnumerable<string> selectedAllocationsTestCas)
        {
           return await Channel.RemoveSelectedAllocationsTestCas(selectedAllocationsTestCas).ConfigureAwait(false);
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

        public async Task<IEnumerable<AllocationsTestCas>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AllocationsTestCas>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCasByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null)
        {
            return  await Channel.GetAllocationsTestCasByEntryDataDetailsId(EntryDataDetailsId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AllocationsTestCas>> GetAllocationsTestCasByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
            return  await Channel.GetAllocationsTestCasByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(false);
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

