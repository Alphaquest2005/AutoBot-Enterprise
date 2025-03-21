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
    [Export (typeof(AsycudaDocumentSetAttachmentsClient))]
    [Export (typeof(IAsycudaDocumentSetAttachmentsService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AsycudaDocumentSetAttachmentsClient :  ClientService<IAsycudaDocumentSetAttachmentsService>, IAsycudaDocumentSetAttachmentsService, IDisposable
    {
        
        public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachments(List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetAttachments(includesLst).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetAttachments> GetAsycudaDocumentSetAttachmentsByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetAttachmentsByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachmentsByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetAttachmentsByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachmentsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetAttachmentsByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachmentsByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetAsycudaDocumentSetAttachmentsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachmentsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetAttachmentsByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachmentsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaDocumentSetAttachmentsByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetAttachments> UpdateAsycudaDocumentSetAttachments(AsycudaDocumentSetAttachments entity)
        {
           return await Channel.UpdateAsycudaDocumentSetAttachments(entity).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetAttachments> CreateAsycudaDocumentSetAttachments(AsycudaDocumentSetAttachments entity)
        {
           return await Channel.CreateAsycudaDocumentSetAttachments(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsycudaDocumentSetAttachments(string id)
        {
            return await Channel.DeleteAsycudaDocumentSetAttachments(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedAsycudaDocumentSetAttachments(IEnumerable<string> selectedAsycudaDocumentSetAttachments)
        {
           return await Channel.RemoveSelectedAsycudaDocumentSetAttachments(selectedAsycudaDocumentSetAttachments).ConfigureAwait(false);
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

        public async Task<IEnumerable<AsycudaDocumentSetAttachments>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaDocumentSetAttachments>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachmentsByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null)
        {
            return  await Channel.GetAsycudaDocumentSetAttachmentsByAsycudaDocumentSetId(AsycudaDocumentSetId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<AsycudaDocumentSetAttachments>> GetAsycudaDocumentSetAttachmentsByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
            return  await Channel.GetAsycudaDocumentSetAttachmentsByFileTypeId(FileTypeId, includesLst).ConfigureAwait(false);
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

