﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using OCR.Client.DTO;
using OCR.Client.Contracts;
using Core.Common.Client.Services;


using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace OCR.Client.Services
{
    [Export (typeof(ChildPartsClient))]
    [Export (typeof(IChildPartsService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ChildPartsClient :  ClientService<IChildPartsService>, IChildPartsService, IDisposable
    {
        
        public async Task<IEnumerable<ChildParts>> GetChildParts(List<string> includesLst = null)
        {
            return await Channel.GetChildParts(includesLst).ConfigureAwait(false);
        }

        public async Task<ChildParts> GetChildPartsByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetChildPartsByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ChildParts>> GetChildPartsByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetChildPartsByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ChildParts>> GetChildPartsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetChildPartsByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ChildParts>> GetChildPartsByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetChildPartsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<ChildParts>> GetChildPartsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetChildPartsByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ChildParts>> GetChildPartsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetChildPartsByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<ChildParts> UpdateChildParts(ChildParts entity)
        {
           return await Channel.UpdateChildParts(entity).ConfigureAwait(false);
        }

        public async Task<ChildParts> CreateChildParts(ChildParts entity)
        {
           return await Channel.CreateChildParts(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteChildParts(string id)
        {
            return await Channel.DeleteChildParts(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedChildParts(IEnumerable<string> selectedChildParts)
        {
           return await Channel.RemoveSelectedChildParts(selectedChildParts).ConfigureAwait(false);
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

        public async Task<IEnumerable<ChildParts>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<ChildParts>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<ChildParts>> GetChildPartsByParentPartId(string ParentPartId, List<string> includesLst = null)
        {
            return  await Channel.GetChildPartsByParentPartId(ParentPartId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<ChildParts>> GetChildPartsByChildPartId(string ChildPartId, List<string> includesLst = null)
        {
            return  await Channel.GetChildPartsByChildPartId(ChildPartId, includesLst).ConfigureAwait(false);
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

