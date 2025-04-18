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
    [Export (typeof(TODO_PODocSetClient))]
    [Export (typeof(ITODO_PODocSetService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_PODocSetClient :  ClientService<ITODO_PODocSetService>, ITODO_PODocSetService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_PODocSet>> GetTODO_PODocSet(List<string> includesLst = null)
        {
            return await Channel.GetTODO_PODocSet(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_PODocSet> GetTODO_PODocSetByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_PODocSetByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_PODocSet>> GetTODO_PODocSetByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_PODocSetByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_PODocSet>> GetTODO_PODocSetByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_PODocSetByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_PODocSet>> GetTODO_PODocSetByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_PODocSetByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_PODocSet>> GetTODO_PODocSetByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_PODocSetByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_PODocSet>> GetTODO_PODocSetByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_PODocSetByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_PODocSet> UpdateTODO_PODocSet(TODO_PODocSet entity)
        {
           return await Channel.UpdateTODO_PODocSet(entity).ConfigureAwait(false);
        }

        public async Task<TODO_PODocSet> CreateTODO_PODocSet(TODO_PODocSet entity)
        {
           return await Channel.CreateTODO_PODocSet(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_PODocSet(string id)
        {
            return await Channel.DeleteTODO_PODocSet(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_PODocSet(IEnumerable<string> selectedTODO_PODocSet)
        {
           return await Channel.RemoveSelectedTODO_PODocSet(selectedTODO_PODocSet).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_PODocSet>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_PODocSet>>  LoadRangeNav(int startIndex, int count, string exp,
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

