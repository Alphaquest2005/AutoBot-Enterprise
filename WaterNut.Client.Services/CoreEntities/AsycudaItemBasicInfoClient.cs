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
    [Export (typeof(AsycudaItemBasicInfoClient))]
    [Export (typeof(IAsycudaItemBasicInfoService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AsycudaItemBasicInfoClient :  ClientService<IAsycudaItemBasicInfoService>, IAsycudaItemBasicInfoService, IDisposable
    {
        
        public async Task<IEnumerable<AsycudaItemBasicInfo>> GetAsycudaItemBasicInfo(List<string> includesLst = null)
        {
            return await Channel.GetAsycudaItemBasicInfo(includesLst).ConfigureAwait(false);
        }

        public async Task<AsycudaItemBasicInfo> GetAsycudaItemBasicInfoByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaItemBasicInfoByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaItemBasicInfo>> GetAsycudaItemBasicInfoByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaItemBasicInfoByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaItemBasicInfo>> GetAsycudaItemBasicInfoByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaItemBasicInfoByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaItemBasicInfo>> GetAsycudaItemBasicInfoByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetAsycudaItemBasicInfoByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<AsycudaItemBasicInfo>> GetAsycudaItemBasicInfoByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaItemBasicInfoByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AsycudaItemBasicInfo>> GetAsycudaItemBasicInfoByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetAsycudaItemBasicInfoByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<AsycudaItemBasicInfo> UpdateAsycudaItemBasicInfo(AsycudaItemBasicInfo entity)
        {
           return await Channel.UpdateAsycudaItemBasicInfo(entity).ConfigureAwait(false);
        }

        public async Task<AsycudaItemBasicInfo> CreateAsycudaItemBasicInfo(AsycudaItemBasicInfo entity)
        {
           return await Channel.CreateAsycudaItemBasicInfo(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsycudaItemBasicInfo(string id)
        {
            return await Channel.DeleteAsycudaItemBasicInfo(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedAsycudaItemBasicInfo(IEnumerable<string> selectedAsycudaItemBasicInfo)
        {
           return await Channel.RemoveSelectedAsycudaItemBasicInfo(selectedAsycudaItemBasicInfo).ConfigureAwait(false);
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

        public async Task<IEnumerable<AsycudaItemBasicInfo>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<AsycudaItemBasicInfo>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<AsycudaItemBasicInfo>> GetAsycudaItemBasicInfoByASYCUDA_Id(string ASYCUDA_Id, List<string> includesLst = null)
        {
            return  await Channel.GetAsycudaItemBasicInfoByASYCUDA_Id(ASYCUDA_Id, includesLst).ConfigureAwait(false);
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
