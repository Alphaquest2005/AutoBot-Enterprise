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
    [Export (typeof(TODO_SubmitPOInfoClient))]
    [Export (typeof(ITODO_SubmitPOInfoService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_SubmitPOInfoClient :  ClientService<ITODO_SubmitPOInfoService>, ITODO_SubmitPOInfoService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfo(List<string> includesLst = null)
        {
            return await Channel.GetTODO_SubmitPOInfo(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_SubmitPOInfo> GetTODO_SubmitPOInfoByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_SubmitPOInfoByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_SubmitPOInfoByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_SubmitPOInfoByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_SubmitPOInfoByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_SubmitPOInfoByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_SubmitPOInfoByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_SubmitPOInfo> UpdateTODO_SubmitPOInfo(TODO_SubmitPOInfo entity)
        {
           return await Channel.UpdateTODO_SubmitPOInfo(entity).ConfigureAwait(false);
        }

        public async Task<TODO_SubmitPOInfo> CreateTODO_SubmitPOInfo(TODO_SubmitPOInfo entity)
        {
           return await Channel.CreateTODO_SubmitPOInfo(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_SubmitPOInfo(string id)
        {
            return await Channel.DeleteTODO_SubmitPOInfo(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_SubmitPOInfo(IEnumerable<string> selectedTODO_SubmitPOInfo)
        {
           return await Channel.RemoveSelectedTODO_SubmitPOInfo(selectedTODO_SubmitPOInfo).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_SubmitPOInfo>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_SubmitPOInfo>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_SubmitPOInfoByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByEmailId(string EmailId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_SubmitPOInfoByEmailId(EmailId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_SubmitPOInfo>> GetTODO_SubmitPOInfoByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_SubmitPOInfoByFileTypeId(FileTypeId, includesLst).ConfigureAwait(false);
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
