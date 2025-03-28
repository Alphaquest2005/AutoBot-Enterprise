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
    public partial interface ITODO_DiscrepanciesErrorsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrors(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_DiscrepanciesErrors> GetTODO_DiscrepanciesErrorsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_DiscrepanciesErrors> UpdateTODO_DiscrepanciesErrors(TODO_DiscrepanciesErrors entity);

        [OperationContract]
        Task<TODO_DiscrepanciesErrors> CreateTODO_DiscrepanciesErrors(TODO_DiscrepanciesErrors entity);

        [OperationContract]
        Task<bool> DeleteTODO_DiscrepanciesErrors(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_DiscrepanciesErrors(IEnumerable<string> selectedTODO_DiscrepanciesErrors);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesErrors>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesErrors>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesErrors>> GetTODO_DiscrepanciesErrorsByEmailId(string EmailId, List<string> includesLst = null);
        
  		
    }
}

