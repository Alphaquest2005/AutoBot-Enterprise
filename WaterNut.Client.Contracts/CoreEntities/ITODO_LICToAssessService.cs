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
    public partial interface ITODO_LICToAssessService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_LICToAssess>> GetTODO_LICToAssess(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_LICToAssess> GetTODO_LICToAssessByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_LICToAssess>> GetTODO_LICToAssessByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_LICToAssess>> GetTODO_LICToAssessByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_LICToAssess>> GetTODO_LICToAssessByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_LICToAssess>> GetTODO_LICToAssessByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_LICToAssess>> GetTODO_LICToAssessByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_LICToAssess> UpdateTODO_LICToAssess(TODO_LICToAssess entity);

        [OperationContract]
        Task<TODO_LICToAssess> CreateTODO_LICToAssess(TODO_LICToAssess entity);

        [OperationContract]
        Task<bool> DeleteTODO_LICToAssess(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_LICToAssess(IEnumerable<string> selectedTODO_LICToAssess);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_LICToAssess>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_LICToAssess>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}
