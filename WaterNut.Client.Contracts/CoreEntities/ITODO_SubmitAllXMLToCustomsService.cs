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
    public partial interface ITODO_SubmitAllXMLToCustomsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustoms(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_SubmitAllXMLToCustoms> GetTODO_SubmitAllXMLToCustomsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_SubmitAllXMLToCustoms> UpdateTODO_SubmitAllXMLToCustoms(TODO_SubmitAllXMLToCustoms entity);

        [OperationContract]
        Task<TODO_SubmitAllXMLToCustoms> CreateTODO_SubmitAllXMLToCustoms(TODO_SubmitAllXMLToCustoms entity);

        [OperationContract]
        Task<bool> DeleteTODO_SubmitAllXMLToCustoms(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_SubmitAllXMLToCustoms(IEnumerable<string> selectedTODO_SubmitAllXMLToCustoms);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsByEmailId(string EmailId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_SubmitAllXMLToCustoms>> GetTODO_SubmitAllXMLToCustomsBySystemDocumentSetId(string SystemDocumentSetId, List<string> includesLst = null);
        
  		
    }
}
