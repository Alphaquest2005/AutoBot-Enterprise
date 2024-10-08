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
    [Export (typeof(TODO_ImportCompleteEntriesClient))]
    [Export (typeof(ITODO_ImportCompleteEntriesService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_ImportCompleteEntriesClient :  ClientService<ITODO_ImportCompleteEntriesService>, ITODO_ImportCompleteEntriesService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntries(List<string> includesLst = null)
        {
            return await Channel.GetTODO_ImportCompleteEntries(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_ImportCompleteEntries> GetTODO_ImportCompleteEntriesByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ImportCompleteEntriesByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ImportCompleteEntriesByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ImportCompleteEntriesByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_ImportCompleteEntriesByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ImportCompleteEntriesByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ImportCompleteEntriesByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_ImportCompleteEntries> UpdateTODO_ImportCompleteEntries(TODO_ImportCompleteEntries entity)
        {
           return await Channel.UpdateTODO_ImportCompleteEntries(entity).ConfigureAwait(false);
        }

        public async Task<TODO_ImportCompleteEntries> CreateTODO_ImportCompleteEntries(TODO_ImportCompleteEntries entity)
        {
           return await Channel.CreateTODO_ImportCompleteEntries(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_ImportCompleteEntries(string id)
        {
            return await Channel.DeleteTODO_ImportCompleteEntries(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_ImportCompleteEntries(IEnumerable<string> selectedTODO_ImportCompleteEntries)
        {
           return await Channel.RemoveSelectedTODO_ImportCompleteEntries(selectedTODO_ImportCompleteEntries).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_ImportCompleteEntries>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ImportCompleteEntries>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_ImportCompleteEntriesByAsycudaDocumentSetId(AsycudaDocumentSetId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_ImportCompleteEntriesByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByEmailId(string EmailId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_ImportCompleteEntriesByEmailId(EmailId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_ImportCompleteEntriesByFileTypeId(FileTypeId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_ImportCompleteEntries>> GetTODO_ImportCompleteEntriesByEntryDataId(string EntryDataId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_ImportCompleteEntriesByEntryDataId(EntryDataId, includesLst).ConfigureAwait(false);
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

