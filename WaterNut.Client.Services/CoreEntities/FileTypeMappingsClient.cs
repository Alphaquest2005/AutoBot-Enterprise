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
    [Export (typeof(FileTypeMappingsClient))]
    [Export (typeof(IFileTypeMappingsService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class FileTypeMappingsClient :  ClientService<IFileTypeMappingsService>, IFileTypeMappingsService, IDisposable
    {
        
        public async Task<IEnumerable<FileTypeMappings>> GetFileTypeMappings(List<string> includesLst = null)
        {
            return await Channel.GetFileTypeMappings(includesLst).ConfigureAwait(false);
        }

        public async Task<FileTypeMappings> GetFileTypeMappingsByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetFileTypeMappingsByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileTypeMappings>> GetFileTypeMappingsByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetFileTypeMappingsByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileTypeMappings>> GetFileTypeMappingsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetFileTypeMappingsByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileTypeMappings>> GetFileTypeMappingsByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetFileTypeMappingsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<FileTypeMappings>> GetFileTypeMappingsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetFileTypeMappingsByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FileTypeMappings>> GetFileTypeMappingsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetFileTypeMappingsByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<FileTypeMappings> UpdateFileTypeMappings(FileTypeMappings entity)
        {
           return await Channel.UpdateFileTypeMappings(entity).ConfigureAwait(false);
        }

        public async Task<FileTypeMappings> CreateFileTypeMappings(FileTypeMappings entity)
        {
           return await Channel.CreateFileTypeMappings(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteFileTypeMappings(string id)
        {
            return await Channel.DeleteFileTypeMappings(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedFileTypeMappings(IEnumerable<string> selectedFileTypeMappings)
        {
           return await Channel.RemoveSelectedFileTypeMappings(selectedFileTypeMappings).ConfigureAwait(false);
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

        public async Task<IEnumerable<FileTypeMappings>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<FileTypeMappings>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<FileTypeMappings>> GetFileTypeMappingsByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
            return  await Channel.GetFileTypeMappingsByFileTypeId(FileTypeId, includesLst).ConfigureAwait(false);
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
