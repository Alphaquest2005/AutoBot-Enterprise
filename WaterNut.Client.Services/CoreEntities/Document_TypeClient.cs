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
    [Export (typeof(Document_TypeClient))]
    [Export (typeof(IDocument_TypeService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Document_TypeClient :  ClientService<IDocument_TypeService>, IDocument_TypeService, IDisposable
    {
        
        public async Task<IEnumerable<Document_Type>> GetDocument_Type(List<string> includesLst = null)
        {
            return await Channel.GetDocument_Type(includesLst).ConfigureAwait(false);
        }

        public async Task<Document_Type> GetDocument_TypeByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetDocument_TypeByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Document_Type>> GetDocument_TypeByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetDocument_TypeByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Document_Type>> GetDocument_TypeByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetDocument_TypeByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Document_Type>> GetDocument_TypeByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetDocument_TypeByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<Document_Type>> GetDocument_TypeByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetDocument_TypeByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Document_Type>> GetDocument_TypeByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetDocument_TypeByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<Document_Type> UpdateDocument_Type(Document_Type entity)
        {
           return await Channel.UpdateDocument_Type(entity).ConfigureAwait(false);
        }

        public async Task<Document_Type> CreateDocument_Type(Document_Type entity)
        {
           return await Channel.CreateDocument_Type(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteDocument_Type(string id)
        {
            return await Channel.DeleteDocument_Type(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedDocument_Type(IEnumerable<string> selectedDocument_Type)
        {
           return await Channel.RemoveSelectedDocument_Type(selectedDocument_Type).ConfigureAwait(false);
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

        public async Task<IEnumerable<Document_Type>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Document_Type>>  LoadRangeNav(int startIndex, int count, string exp,
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

