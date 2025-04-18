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
    public partial interface ITODO_AssessDiscrepancyEntriesService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntries(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_AssessDiscrepancyEntries> GetTODO_AssessDiscrepancyEntriesByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_AssessDiscrepancyEntries> UpdateTODO_AssessDiscrepancyEntries(TODO_AssessDiscrepancyEntries entity);

        [OperationContract]
        Task<TODO_AssessDiscrepancyEntries> CreateTODO_AssessDiscrepancyEntries(TODO_AssessDiscrepancyEntries entity);

        [OperationContract]
        Task<bool> DeleteTODO_AssessDiscrepancyEntries(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_AssessDiscrepancyEntries(IEnumerable<string> selectedTODO_AssessDiscrepancyEntries);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_AssessDiscrepancyEntries>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_AssessDiscrepancyEntries>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}

