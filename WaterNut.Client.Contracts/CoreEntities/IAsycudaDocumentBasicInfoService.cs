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
    public partial interface IAsycudaDocumentBasicInfoService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfo(List<string> includesLst = null);

        [OperationContract]
        Task<AsycudaDocumentBasicInfo> GetAsycudaDocumentBasicInfoByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<AsycudaDocumentBasicInfo> UpdateAsycudaDocumentBasicInfo(AsycudaDocumentBasicInfo entity);

        [OperationContract]
        Task<AsycudaDocumentBasicInfo> CreateAsycudaDocumentBasicInfo(AsycudaDocumentBasicInfo entity);

        [OperationContract]
        Task<bool> DeleteAsycudaDocumentBasicInfo(string id);

        [OperationContract]
        Task<bool> RemoveSelectedAsycudaDocumentBasicInfo(IEnumerable<string> selectedAsycudaDocumentBasicInfo);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<AsycudaDocumentBasicInfo>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<AsycudaDocumentBasicInfo>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AsycudaDocumentBasicInfo>> GetAsycudaDocumentBasicInfoByCustoms_ProcedureId(string Customs_ProcedureId, List<string> includesLst = null);
        
  		
    }
}
