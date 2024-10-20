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
    public partial interface IAsycudaDocumentSetService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSet(List<string> includesLst = null);

        [OperationContract]
        Task<AsycudaDocumentSet> GetAsycudaDocumentSetByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSetByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSetByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSetByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSetByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSetByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<AsycudaDocumentSet> UpdateAsycudaDocumentSet(AsycudaDocumentSet entity);

        [OperationContract]
        Task<AsycudaDocumentSet> CreateAsycudaDocumentSet(AsycudaDocumentSet entity);

        [OperationContract]
        Task<bool> DeleteAsycudaDocumentSet(string id);

        [OperationContract]
        Task<bool> RemoveSelectedAsycudaDocumentSet(IEnumerable<string> selectedAsycudaDocumentSet);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<AsycudaDocumentSet>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<AsycudaDocumentSet>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSetByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AsycudaDocumentSet>> GetAsycudaDocumentSetByCustoms_ProcedureId(string Customs_ProcedureId, List<string> includesLst = null);
        
  		
    }
}

