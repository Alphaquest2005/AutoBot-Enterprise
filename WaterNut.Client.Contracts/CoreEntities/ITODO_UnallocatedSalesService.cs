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
using CoreEntities.Client.DTO;


namespace CoreEntities.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface ITODO_UnallocatedSalesService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSales(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_UnallocatedSales> GetTODO_UnallocatedSalesByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_UnallocatedSales> UpdateTODO_UnallocatedSales(TODO_UnallocatedSales entity);

        [OperationContract]
        Task<TODO_UnallocatedSales> CreateTODO_UnallocatedSales(TODO_UnallocatedSales entity);

        [OperationContract]
        Task<bool> DeleteTODO_UnallocatedSales(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_UnallocatedSales(IEnumerable<string> selectedTODO_UnallocatedSales);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_UnallocatedSales>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_UnallocatedSales>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}
