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
    [Export (typeof(SessionsClient))]
    [Export (typeof(ISessionsService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SessionsClient :  ClientService<ISessionsService>, ISessionsService, IDisposable
    {
        
        public async Task<IEnumerable<Sessions>> GetSessions(List<string> includesLst = null)
        {
            return await Channel.GetSessions(includesLst).ConfigureAwait(false);
        }

        public async Task<Sessions> GetSessionsByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetSessionsByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Sessions>> GetSessionsByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetSessionsByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Sessions>> GetSessionsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetSessionsByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Sessions>> GetSessionsByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetSessionsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<Sessions>> GetSessionsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetSessionsByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Sessions>> GetSessionsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetSessionsByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<Sessions> UpdateSessions(Sessions entity)
        {
           return await Channel.UpdateSessions(entity).ConfigureAwait(false);
        }

        public async Task<Sessions> CreateSessions(Sessions entity)
        {
           return await Channel.CreateSessions(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteSessions(string id)
        {
            return await Channel.DeleteSessions(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedSessions(IEnumerable<string> selectedSessions)
        {
           return await Channel.RemoveSelectedSessions(selectedSessions).ConfigureAwait(false);
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

        public async Task<IEnumerable<Sessions>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Sessions>>  LoadRangeNav(int startIndex, int count, string exp,
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
