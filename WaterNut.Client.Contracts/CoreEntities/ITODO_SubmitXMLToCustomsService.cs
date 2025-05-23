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
    public partial interface ITODO_SubmitXMLToCustomsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustoms(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_SubmitXMLToCustoms> GetTODO_SubmitXMLToCustomsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_SubmitXMLToCustoms> UpdateTODO_SubmitXMLToCustoms(TODO_SubmitXMLToCustoms entity);

        [OperationContract]
        Task<TODO_SubmitXMLToCustoms> CreateTODO_SubmitXMLToCustoms(TODO_SubmitXMLToCustoms entity);

        [OperationContract]
        Task<bool> DeleteTODO_SubmitXMLToCustoms(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_SubmitXMLToCustoms(IEnumerable<string> selectedTODO_SubmitXMLToCustoms);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_SubmitXMLToCustoms>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_SubmitXMLToCustoms>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_SubmitXMLToCustoms>> GetTODO_SubmitXMLToCustomsByEmailId(string EmailId, List<string> includesLst = null);
        
  		
    }
}

