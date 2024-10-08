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
    public partial interface IInventoryItemAliasExService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExes(List<string> includesLst = null);

        [OperationContract]
        Task<InventoryItemAliasEx> GetInventoryItemAliasExByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<InventoryItemAliasEx> UpdateInventoryItemAliasEx(InventoryItemAliasEx entity);

        [OperationContract]
        Task<InventoryItemAliasEx> CreateInventoryItemAliasEx(InventoryItemAliasEx entity);

        [OperationContract]
        Task<bool> DeleteInventoryItemAliasEx(string id);

        [OperationContract]
        Task<bool> RemoveSelectedInventoryItemAliasEx(IEnumerable<string> selectedInventoryItemAliasEx);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<InventoryItemAliasEx>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<InventoryItemAliasEx>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<InventoryItemAliasEx>> GetInventoryItemAliasExByInventoryItemId(string InventoryItemId, List<string> includesLst = null);
        
  		
    }
}

