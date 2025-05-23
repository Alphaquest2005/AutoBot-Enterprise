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
    [Export (typeof(FileImporterInfoClient))]
    [Export (typeof(IFileImporterInfoService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class FileImporterInfoClient :  ClientService<IFileImporterInfoService>, IFileImporterInfoService, IDisposable
    {
        
        public async Task<IEnumerable<FileImporterInfo>> GetFileImporterInfos(List<string> includesLst = null)
        {
            return await Channel.GetFileImporterInfos(includesLst).ConfigureAwait(false);
        }

        public async Task<FileImporterInfo> GetFileImporterInfoByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetFileImporterInfoByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileImporterInfo>> GetFileImporterInfosByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetFileImporterInfosByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileImporterInfo>> GetFileImporterInfosByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetFileImporterInfosByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileImporterInfo>> GetFileImporterInfosByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetFileImporterInfosByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<FileImporterInfo>> GetFileImporterInfosByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetFileImporterInfosByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FileImporterInfo>> GetFileImporterInfosByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetFileImporterInfosByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<FileImporterInfo> UpdateFileImporterInfo(FileImporterInfo entity)
        {
           return await Channel.UpdateFileImporterInfo(entity).ConfigureAwait(false);
        }

        public async Task<FileImporterInfo> CreateFileImporterInfo(FileImporterInfo entity)
        {
           return await Channel.CreateFileImporterInfo(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteFileImporterInfo(string id)
        {
            return await Channel.DeleteFileImporterInfo(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedFileImporterInfo(IEnumerable<string> selectedFileImporterInfo)
        {
           return await Channel.RemoveSelectedFileImporterInfo(selectedFileImporterInfo).ConfigureAwait(false);
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

        public async Task<IEnumerable<FileImporterInfo>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileImporterInfo>>  LoadRangeNav(int startIndex, int count, string exp,
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

