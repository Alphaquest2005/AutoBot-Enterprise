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
    public partial interface ITODO_DiscrepancyPreExecutionReportService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> GetTODO_DiscrepancyPreExecutionReport(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_DiscrepancyPreExecutionReport> GetTODO_DiscrepancyPreExecutionReportByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> GetTODO_DiscrepancyPreExecutionReportByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> GetTODO_DiscrepancyPreExecutionReportByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> GetTODO_DiscrepancyPreExecutionReportByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> GetTODO_DiscrepancyPreExecutionReportByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> GetTODO_DiscrepancyPreExecutionReportByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_DiscrepancyPreExecutionReport> UpdateTODO_DiscrepancyPreExecutionReport(TODO_DiscrepancyPreExecutionReport entity);

        [OperationContract]
        Task<TODO_DiscrepancyPreExecutionReport> CreateTODO_DiscrepancyPreExecutionReport(TODO_DiscrepancyPreExecutionReport entity);

        [OperationContract]
        Task<bool> DeleteTODO_DiscrepancyPreExecutionReport(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_DiscrepancyPreExecutionReport(IEnumerable<string> selectedTODO_DiscrepancyPreExecutionReport);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_DiscrepancyPreExecutionReport>> GetTODO_DiscrepancyPreExecutionReportByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null);
        
  		
    }
}
