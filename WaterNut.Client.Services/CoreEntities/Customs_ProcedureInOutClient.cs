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
    [Export (typeof(Customs_ProcedureInOutClient))]
    [Export (typeof(ICustoms_ProcedureInOutService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Customs_ProcedureInOutClient :  ClientService<ICustoms_ProcedureInOutService>, ICustoms_ProcedureInOutService, IDisposable
    {
        
        public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOut(List<string> includesLst = null)
        {
            return await Channel.GetCustoms_ProcedureInOut(includesLst).ConfigureAwait(false);
        }

        public async Task<Customs_ProcedureInOut> GetCustoms_ProcedureInOutByKey(string id, List<string> includesLst = null)
        {
            return await Channel.GetCustoms_ProcedureInOutByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOutByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.GetCustoms_ProcedureInOutByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOutByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.GetCustoms_ProcedureInOutByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOutByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.GetCustoms_ProcedureInOutByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOutByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetCustoms_ProcedureInOutByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOutByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.GetCustoms_ProcedureInOutByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<Customs_ProcedureInOut> UpdateCustoms_ProcedureInOut(Customs_ProcedureInOut entity)
        {
           return await Channel.UpdateCustoms_ProcedureInOut(entity).ConfigureAwait(false);
        }

        public async Task<Customs_ProcedureInOut> CreateCustoms_ProcedureInOut(Customs_ProcedureInOut entity)
        {
           return await Channel.CreateCustoms_ProcedureInOut(entity).ConfigureAwait(false);
        }

        public async Task<bool> DeleteCustoms_ProcedureInOut(string id)
        {
            return await Channel.DeleteCustoms_ProcedureInOut(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedCustoms_ProcedureInOut(IEnumerable<string> selectedCustoms_ProcedureInOut)
        {
           return await Channel.RemoveSelectedCustoms_ProcedureInOut(selectedCustoms_ProcedureInOut).ConfigureAwait(false);
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

        public async Task<IEnumerable<Customs_ProcedureInOut>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<Customs_ProcedureInOut>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOutByWarehouseCustomsProcedureId(string WarehouseCustomsProcedureId, List<string> includesLst = null)
        {
            return  await Channel.GetCustoms_ProcedureInOutByWarehouseCustomsProcedureId(WarehouseCustomsProcedureId, includesLst).ConfigureAwait(false);
        }
			 
  		public async Task<IEnumerable<Customs_ProcedureInOut>> GetCustoms_ProcedureInOutByExwarehouseCustomsProcedureId(string ExwarehouseCustomsProcedureId, List<string> includesLst = null)
        {
            return  await Channel.GetCustoms_ProcedureInOutByExwarehouseCustomsProcedureId(ExwarehouseCustomsProcedureId, includesLst).ConfigureAwait(false);
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
