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
    public partial interface ITODO_CreateEx9Service : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_CreateEx9>> GetTODO_CreateEx9(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_CreateEx9> GetTODO_CreateEx9ByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_CreateEx9>> GetTODO_CreateEx9ByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_CreateEx9>> GetTODO_CreateEx9ByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_CreateEx9>> GetTODO_CreateEx9ByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_CreateEx9>> GetTODO_CreateEx9ByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_CreateEx9>> GetTODO_CreateEx9ByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_CreateEx9> UpdateTODO_CreateEx9(TODO_CreateEx9 entity);

        [OperationContract]
        Task<TODO_CreateEx9> CreateTODO_CreateEx9(TODO_CreateEx9 entity);

        [OperationContract]
        Task<bool> DeleteTODO_CreateEx9(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_CreateEx9(IEnumerable<string> selectedTODO_CreateEx9);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_CreateEx9>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_CreateEx9>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}
