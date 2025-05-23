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
using AllocationQS.Client.DTO;


namespace AllocationQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IAdjustmentShortAllocationService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocations(List<string> includesLst = null);

        [OperationContract]
        Task<AdjustmentShortAllocation> GetAdjustmentShortAllocationByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<AdjustmentShortAllocation> UpdateAdjustmentShortAllocation(AdjustmentShortAllocation entity);

        [OperationContract]
        Task<AdjustmentShortAllocation> CreateAdjustmentShortAllocation(AdjustmentShortAllocation entity);

        [OperationContract]
        Task<bool> DeleteAdjustmentShortAllocation(string id);

        [OperationContract]
        Task<bool> RemoveSelectedAdjustmentShortAllocation(IEnumerable<string> selectedAdjustmentShortAllocation);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<AdjustmentShortAllocation>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByPreviousItem_Id(string PreviousItem_Id, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByxBond_Item_Id(string xBond_Item_Id, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByxASYCUDA_Id(string xASYCUDA_Id, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationBypASYCUDA_Id(string pASYCUDA_Id, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByFileTypeId(string FileTypeId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentShortAllocation>> GetAdjustmentShortAllocationByEmailId(string EmailId, List<string> includesLst = null);
        
  		
    }
}

