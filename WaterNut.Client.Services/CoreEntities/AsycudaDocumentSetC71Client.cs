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
    [Export (typeof(AsycudaDocumentSetC71Client))]
    [Export (typeof(IAsycudaDocumentSetC71Service))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AsycudaDocumentSetC71Client :  ClientService<IAsycudaDocumentSetC71Service>, IAsycudaDocumentSetC71Service, IDisposable
    {
        
        public async Task<IEnumerable<AsycudaDocumentSetC71>> GetAsycudaDocumentSetC71(List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetC71(includesLst).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetC71> GetAsycudaDocumentSetC71ByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetC71ByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetC71>> GetAsycudaDocumentSetC71ByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetC71ByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetC71>> GetAsycudaDocumentSetC71ByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetC71ByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetC71>> GetAsycudaDocumentSetC71ByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetAsycudaDocumentSetC71ByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<AsycudaDocumentSetC71>> GetAsycudaDocumentSetC71ByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetC71ByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AsycudaDocumentSetC71>> GetAsycudaDocumentSetC71ByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetC71ByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetC71> UpdateAsycudaDocumentSetC71(AsycudaDocumentSetC71 entity)
        {
           return await Channel.UpdateAsycudaDocumentSetC71(entity).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetC71> CreateAsycudaDocumentSetC71(AsycudaDocumentSetC71 entity)
        {
           return await Channel.CreateAsycudaDocumentSetC71(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsycudaDocumentSetC71(string id)
        {
            return await Channel.DeleteAsycudaDocumentSetC71(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedAsycudaDocumentSetC71(IEnumerable<string> selectedAsycudaDocumentSetC71)
        {
           return await Channel.RemoveSelectedAsycudaDocumentSetC71(selectedAsycudaDocumentSetC71).ConfigureAwait(false);
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

        public async Task<IEnumerable<AsycudaDocumentSetC71>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetC71>>  LoadRangeNav(int startIndex, int count, string exp,
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
