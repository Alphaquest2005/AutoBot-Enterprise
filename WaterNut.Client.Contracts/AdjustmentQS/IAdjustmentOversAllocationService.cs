﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using AdjustmentQS.Client.DTO;


namespace AdjustmentQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IAdjustmentOversAllocationService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocations(List<string> includesLst = null);

        [OperationContract]
        Task<AdjustmentOversAllocation> GetAdjustmentOversAllocationByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<AdjustmentOversAllocation> UpdateAdjustmentOversAllocation(AdjustmentOversAllocation entity);

        [OperationContract]
        Task<AdjustmentOversAllocation> CreateAdjustmentOversAllocation(AdjustmentOversAllocation entity);

        [OperationContract]
        Task<bool> DeleteAdjustmentOversAllocation(string id);

        [OperationContract]
        Task<bool> RemoveSelectedAdjustmentOversAllocation(IEnumerable<string> selectedAdjustmentOversAllocation);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<AdjustmentOversAllocation>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<AdjustmentOversAllocation>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationByPreviousItem_Id(string PreviousItem_Id, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentOversAllocation>> GetAdjustmentOversAllocationByAsycuda_Id(string Asycuda_Id, List<string> includesLst = null);
        
  		
    }
}

