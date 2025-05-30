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
    [Export (typeof(TODO_LICToCreateClient))]
    [Export (typeof(ITODO_LICToCreateService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_LICToCreateClient :  ClientService<ITODO_LICToCreateService>, ITODO_LICToCreateService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_LICToCreate>> GetTODO_LICToCreate(List<string> includesLst = null)
        {
            return await Channel.GetTODO_LICToCreate(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_LICToCreate> GetTODO_LICToCreateByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_LICToCreateByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_LICToCreate>> GetTODO_LICToCreateByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_LICToCreateByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_LICToCreate>> GetTODO_LICToCreateByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_LICToCreateByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_LICToCreate>> GetTODO_LICToCreateByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_LICToCreateByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_LICToCreate>> GetTODO_LICToCreateByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_LICToCreateByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_LICToCreate>> GetTODO_LICToCreateByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_LICToCreateByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_LICToCreate> UpdateTODO_LICToCreate(TODO_LICToCreate entity)
        {
           return await Channel.UpdateTODO_LICToCreate(entity).ConfigureAwait(false);
        }

        public async Task<TODO_LICToCreate> CreateTODO_LICToCreate(TODO_LICToCreate entity)
        {
           return await Channel.CreateTODO_LICToCreate(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_LICToCreate(string id)
        {
            return await Channel.DeleteTODO_LICToCreate(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_LICToCreate(IEnumerable<string> selectedTODO_LICToCreate)
        {
           return await Channel.RemoveSelectedTODO_LICToCreate(selectedTODO_LICToCreate).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_LICToCreate>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_LICToCreate>>  LoadRangeNav(int startIndex, int count, string exp,
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

