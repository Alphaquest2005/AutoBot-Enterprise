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
    [Export (typeof(TODO_Error_DuplicateEntryClient))]
    [Export (typeof(ITODO_Error_DuplicateEntryService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_Error_DuplicateEntryClient :  ClientService<ITODO_Error_DuplicateEntryService>, ITODO_Error_DuplicateEntryService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_Error_DuplicateEntry>> GetTODO_Error_DuplicateEntry(List<string> includesLst = null)
        {
            return await Channel.GetTODO_Error_DuplicateEntry(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_Error_DuplicateEntry> GetTODO_Error_DuplicateEntryByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_Error_DuplicateEntryByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_Error_DuplicateEntry>> GetTODO_Error_DuplicateEntryByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_Error_DuplicateEntryByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_Error_DuplicateEntry>> GetTODO_Error_DuplicateEntryByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_Error_DuplicateEntryByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_Error_DuplicateEntry>> GetTODO_Error_DuplicateEntryByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_Error_DuplicateEntryByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_Error_DuplicateEntry>> GetTODO_Error_DuplicateEntryByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_Error_DuplicateEntryByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_Error_DuplicateEntry>> GetTODO_Error_DuplicateEntryByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_Error_DuplicateEntryByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_Error_DuplicateEntry> UpdateTODO_Error_DuplicateEntry(TODO_Error_DuplicateEntry entity)
        {
           return await Channel.UpdateTODO_Error_DuplicateEntry(entity).ConfigureAwait(false);
        }

        public async Task<TODO_Error_DuplicateEntry> CreateTODO_Error_DuplicateEntry(TODO_Error_DuplicateEntry entity)
        {
           return await Channel.CreateTODO_Error_DuplicateEntry(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_Error_DuplicateEntry(string id)
        {
            return await Channel.DeleteTODO_Error_DuplicateEntry(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_Error_DuplicateEntry(IEnumerable<string> selectedTODO_Error_DuplicateEntry)
        {
           return await Channel.RemoveSelectedTODO_Error_DuplicateEntry(selectedTODO_Error_DuplicateEntry).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_Error_DuplicateEntry>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_Error_DuplicateEntry>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
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
