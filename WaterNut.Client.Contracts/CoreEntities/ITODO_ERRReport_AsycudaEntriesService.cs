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
    public partial interface ITODO_ERRReport_AsycudaEntriesService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> GetTODO_ERRReport_AsycudaEntries(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_ERRReport_AsycudaEntries> GetTODO_ERRReport_AsycudaEntriesByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> GetTODO_ERRReport_AsycudaEntriesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> GetTODO_ERRReport_AsycudaEntriesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> GetTODO_ERRReport_AsycudaEntriesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> GetTODO_ERRReport_AsycudaEntriesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> GetTODO_ERRReport_AsycudaEntriesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_ERRReport_AsycudaEntries> UpdateTODO_ERRReport_AsycudaEntries(TODO_ERRReport_AsycudaEntries entity);

        [OperationContract]
        Task<TODO_ERRReport_AsycudaEntries> CreateTODO_ERRReport_AsycudaEntries(TODO_ERRReport_AsycudaEntries entity);

        [OperationContract]
        Task<bool> DeleteTODO_ERRReport_AsycudaEntries(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_ERRReport_AsycudaEntries(IEnumerable<string> selectedTODO_ERRReport_AsycudaEntries);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_ERRReport_AsycudaEntries>> GetTODO_ERRReport_AsycudaEntriesByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		
    }
}

