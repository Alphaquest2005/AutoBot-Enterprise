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
    [Export (typeof(TODO_ERRReport_SubmitWarehouseErrorsClient))]
    [Export (typeof(ITODO_ERRReport_SubmitWarehouseErrorsService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_ERRReport_SubmitWarehouseErrorsClient :  ClientService<ITODO_ERRReport_SubmitWarehouseErrorsService>, ITODO_ERRReport_SubmitWarehouseErrorsService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>> GetTODO_ERRReport_SubmitWarehouseErrors(List<string> includesLst = null)
        {
            return await Channel.GetTODO_ERRReport_SubmitWarehouseErrors(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_ERRReport_SubmitWarehouseErrors> GetTODO_ERRReport_SubmitWarehouseErrorsByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ERRReport_SubmitWarehouseErrorsByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>> GetTODO_ERRReport_SubmitWarehouseErrorsByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ERRReport_SubmitWarehouseErrorsByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>> GetTODO_ERRReport_SubmitWarehouseErrorsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ERRReport_SubmitWarehouseErrorsByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>> GetTODO_ERRReport_SubmitWarehouseErrorsByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_ERRReport_SubmitWarehouseErrorsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>> GetTODO_ERRReport_SubmitWarehouseErrorsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ERRReport_SubmitWarehouseErrorsByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>> GetTODO_ERRReport_SubmitWarehouseErrorsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_ERRReport_SubmitWarehouseErrorsByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_ERRReport_SubmitWarehouseErrors> UpdateTODO_ERRReport_SubmitWarehouseErrors(TODO_ERRReport_SubmitWarehouseErrors entity)
        {
           return await Channel.UpdateTODO_ERRReport_SubmitWarehouseErrors(entity).ConfigureAwait(false);
        }

        public async Task<TODO_ERRReport_SubmitWarehouseErrors> CreateTODO_ERRReport_SubmitWarehouseErrors(TODO_ERRReport_SubmitWarehouseErrors entity)
        {
           return await Channel.CreateTODO_ERRReport_SubmitWarehouseErrors(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_ERRReport_SubmitWarehouseErrors(string id)
        {
            return await Channel.DeleteTODO_ERRReport_SubmitWarehouseErrors(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_ERRReport_SubmitWarehouseErrors(IEnumerable<string> selectedTODO_ERRReport_SubmitWarehouseErrors)
        {
           return await Channel.RemoveSelectedTODO_ERRReport_SubmitWarehouseErrors(selectedTODO_ERRReport_SubmitWarehouseErrors).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_ERRReport_SubmitWarehouseErrors>>  LoadRangeNav(int startIndex, int count, string exp,
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
