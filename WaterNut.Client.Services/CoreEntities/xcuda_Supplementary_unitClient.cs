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
    [Export (typeof(xcuda_Supplementary_unitClient))]
    [Export (typeof(Ixcuda_Supplementary_unitService))]
    [Export(typeof(IClientService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class xcuda_Supplementary_unitClient :  ClientService<Ixcuda_Supplementary_unitService>, Ixcuda_Supplementary_unitService, IDisposable
    {
        
        public async Task<IEnumerable<xcuda_Supplementary_unit>> Getxcuda_Supplementary_unit(List<string> includesLst = null)
        {
            return await Channel.Getxcuda_Supplementary_unit(includesLst).ConfigureAwait(false);
        }

        public async Task<xcuda_Supplementary_unit> Getxcuda_Supplementary_unitByKey(string id, List<string> includesLst = null)
        {
            return await Channel.Getxcuda_Supplementary_unitByKey(id, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<xcuda_Supplementary_unit>> Getxcuda_Supplementary_unitByExpression(string exp, List<string> includesLst = null)
        {
            return await Channel.Getxcuda_Supplementary_unitByExpression(exp, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<xcuda_Supplementary_unit>> Getxcuda_Supplementary_unitByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            return await Channel.Getxcuda_Supplementary_unitByExpressionLst(expLst, includesLst).ConfigureAwait(false);
        }

		public async Task<IEnumerable<xcuda_Supplementary_unit>> Getxcuda_Supplementary_unitByExpressionNav(string exp,
															 Dictionary<string, string> navExp, List<string> includesLst = null)
		{
			return await Channel.Getxcuda_Supplementary_unitByExpressionNav(exp, navExp, includesLst).ConfigureAwait(false);
		}

        public async Task<IEnumerable<xcuda_Supplementary_unit>> Getxcuda_Supplementary_unitByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.Getxcuda_Supplementary_unitByBatch(exp, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<IEnumerable<xcuda_Supplementary_unit>> Getxcuda_Supplementary_unitByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null)
        {
            return await Channel.Getxcuda_Supplementary_unitByBatchExpressionLst(expLst, totalrow, includesLst).ConfigureAwait(false);
        }

        public async Task<xcuda_Supplementary_unit> Updatexcuda_Supplementary_unit(xcuda_Supplementary_unit entity)
        {
           return await Channel.Updatexcuda_Supplementary_unit(entity).ConfigureAwait(false);
        }

        public async Task<xcuda_Supplementary_unit> Createxcuda_Supplementary_unit(xcuda_Supplementary_unit entity)
        {
           return await Channel.Createxcuda_Supplementary_unit(entity).ConfigureAwait(false);
        }

        public async Task<bool> Deletexcuda_Supplementary_unit(string id)
        {
            return await Channel.Deletexcuda_Supplementary_unit(id).ConfigureAwait(false);
        }

        public async Task<bool> RemoveSelectedxcuda_Supplementary_unit(IEnumerable<string> selectedxcuda_Supplementary_unit)
        {
           return await Channel.RemoveSelectedxcuda_Supplementary_unit(selectedxcuda_Supplementary_unit).ConfigureAwait(false);
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

        public async Task<IEnumerable<xcuda_Supplementary_unit>> LoadRange(int startIndex, int count, string exp)
        {
            return await Channel.LoadRange(startIndex,count,exp).ConfigureAwait(false);
        }

		public async Task<IEnumerable<xcuda_Supplementary_unit>>  LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
            return await Channel.LoadRangeNav(startIndex,count,exp, navExp, includeLst).ConfigureAwait(false);
        }
		public async Task<IEnumerable<xcuda_Supplementary_unit>> Getxcuda_Supplementary_unitByTarification_Id(string Tarification_Id, List<string> includesLst = null)
        {
            return  await Channel.Getxcuda_Supplementary_unitByTarification_Id(Tarification_Id, includesLst).ConfigureAwait(false);
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

