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
using InventoryQS.Client.DTO;


namespace InventoryQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IInventoryItemsExService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<InventoryItemsEx>> GetInventoryItemsEx(List<string> includesLst = null);

        [OperationContract]
        Task<InventoryItemsEx> GetInventoryItemsExByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InventoryItemsEx>> GetInventoryItemsExByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InventoryItemsEx>> GetInventoryItemsExByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<InventoryItemsEx>> GetInventoryItemsExByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<InventoryItemsEx>> GetInventoryItemsExByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<InventoryItemsEx>> GetInventoryItemsExByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<InventoryItemsEx> UpdateInventoryItemsEx(InventoryItemsEx entity);

        [OperationContract]
        Task<InventoryItemsEx> CreateInventoryItemsEx(InventoryItemsEx entity);

        [OperationContract]
        Task<bool> DeleteInventoryItemsEx(string id);

        [OperationContract]
        Task<bool> RemoveSelectedInventoryItemsEx(IEnumerable<string> selectedInventoryItemsEx);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<InventoryItemsEx>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<InventoryItemsEx>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<InventoryItemsEx>> GetInventoryItemsExByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		
    }
}

