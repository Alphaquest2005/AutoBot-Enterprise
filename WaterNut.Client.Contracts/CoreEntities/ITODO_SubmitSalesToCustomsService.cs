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
    public partial interface ITODO_SubmitSalesToCustomsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustoms(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_SubmitSalesToCustoms> GetTODO_SubmitSalesToCustomsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_SubmitSalesToCustoms> UpdateTODO_SubmitSalesToCustoms(TODO_SubmitSalesToCustoms entity);

        [OperationContract]
        Task<TODO_SubmitSalesToCustoms> CreateTODO_SubmitSalesToCustoms(TODO_SubmitSalesToCustoms entity);

        [OperationContract]
        Task<bool> DeleteTODO_SubmitSalesToCustoms(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_SubmitSalesToCustoms(IEnumerable<string> selectedTODO_SubmitSalesToCustoms);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_SubmitSalesToCustoms>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_SubmitSalesToCustoms>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_SubmitSalesToCustoms>> GetTODO_SubmitSalesToCustomsByEmailId(string EmailId, List<string> includesLst = null);
        
  		
    }
}

