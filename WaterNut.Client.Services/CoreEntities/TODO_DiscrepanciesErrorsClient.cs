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
    [Export (typeof(TODO_DiscrepanciesErrorsClient))]
    [Export (typeof(ITODO_DiscrepanciesErrorsService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_DiscrepanciesErrorsClient :  ClientService<ITODO_DiscrepanciesErrorsService>, ITODO_DiscrepanciesErrorsService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrors(List<string> includesLst = null)
        {
            return await Channel.GetTODO_DiscrepanciesErrors(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_DiscrepanciesErrors> GetTODO_DiscrepanciesErrorsByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_DiscrepanciesErrorsByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_DiscrepanciesErrorsByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_DiscrepanciesErrorsByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_DiscrepanciesErrorsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_DiscrepanciesErrorsByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_DiscrepanciesErrorsByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_DiscrepanciesErrors> UpdateTODO_DiscrepanciesErrors(TODO_DiscrepanciesErrors entity)
        {
           return await Channel.UpdateTODO_DiscrepanciesErrors(entity).ConfigureAwait(false);
        }

        public async Task<TODO_DiscrepanciesErrors> CreateTODO_DiscrepanciesErrors(TODO_DiscrepanciesErrors entity)
        {
           return await Channel.CreateTODO_DiscrepanciesErrors(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_DiscrepanciesErrors(string id)
        {
            return await Channel.DeleteTODO_DiscrepanciesErrors(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_DiscrepanciesErrors(IEnumerable<string> selectedTODO_DiscrepanciesErrors)
        {
           return await Channel.RemoveSelectedTODO_DiscrepanciesErrors(selectedTODO_DiscrepanciesErrors).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_DiscrepanciesErrors>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_DiscrepanciesErrors>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_DiscrepanciesErrorsByAsycudaDocumentSetId(AsycudaDocumentSetId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByEmailId(string EmailId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_DiscrepanciesErrorsByEmailId(EmailId, includesLst).ConfigureAwait(false);
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
