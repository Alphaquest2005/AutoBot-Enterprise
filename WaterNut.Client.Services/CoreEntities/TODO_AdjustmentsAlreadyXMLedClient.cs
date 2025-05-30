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
    [Export (typeof(TODO_AdjustmentsAlreadyXMLedClient))]
    [Export (typeof(ITODO_AdjustmentsAlreadyXMLedService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TODO_AdjustmentsAlreadyXMLedClient :  ClientService<ITODO_AdjustmentsAlreadyXMLedService>, ITODO_AdjustmentsAlreadyXMLedService, IDisposable
    {
        
        public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLed(List<string> includesLst = null)
        {
            return await Channel.GetTODO_AdjustmentsAlreadyXMLed(includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_AdjustmentsAlreadyXMLed> GetTODO_AdjustmentsAlreadyXMLedByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetTODO_AdjustmentsAlreadyXMLedByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLedByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetTODO_AdjustmentsAlreadyXMLedByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLedByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetTODO_AdjustmentsAlreadyXMLedByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLedByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetTODO_AdjustmentsAlreadyXMLedByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLedByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_AdjustmentsAlreadyXMLedByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLedByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetTODO_AdjustmentsAlreadyXMLedByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<TODO_AdjustmentsAlreadyXMLed> UpdateTODO_AdjustmentsAlreadyXMLed(TODO_AdjustmentsAlreadyXMLed entity)
        {
           return await Channel.UpdateTODO_AdjustmentsAlreadyXMLed(entity).ConfigureAwait(false);
        }

        public async Task<TODO_AdjustmentsAlreadyXMLed> CreateTODO_AdjustmentsAlreadyXMLed(TODO_AdjustmentsAlreadyXMLed entity)
        {
           return await Channel.CreateTODO_AdjustmentsAlreadyXMLed(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTODO_AdjustmentsAlreadyXMLed(string id)
        {
            return await Channel.DeleteTODO_AdjustmentsAlreadyXMLed(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedTODO_AdjustmentsAlreadyXMLed(IEnumerable<string> selectedTODO_AdjustmentsAlreadyXMLed)
        {
           return await Channel.RemoveSelectedTODO_AdjustmentsAlreadyXMLed(selectedTODO_AdjustmentsAlreadyXMLed).ConfigureAwait(false);
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

        public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLedByEmailId(string EmailId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_AdjustmentsAlreadyXMLedByEmailId(EmailId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<TODO_AdjustmentsAlreadyXMLed>> GetTODO_AdjustmentsAlreadyXMLedByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
            return  await Channel.GetTODO_AdjustmentsAlreadyXMLedByFileTypeId(FileTypeId, includesLst).ConfigureAwait(false);
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

