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
using AdjustmentQS.Client.DTO;


namespace AdjustmentQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface ISystemDocumentSetService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<SystemDocumentSet>> GetSystemDocumentSets(List<string> includesLst = null);

        [OperationContract]
        Task<SystemDocumentSet> GetSystemDocumentSetByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<SystemDocumentSet>> GetSystemDocumentSetsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<SystemDocumentSet>> GetSystemDocumentSetsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<SystemDocumentSet>> GetSystemDocumentSetsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<SystemDocumentSet>> GetSystemDocumentSetsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<SystemDocumentSet>> GetSystemDocumentSetsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<SystemDocumentSet> UpdateSystemDocumentSet(SystemDocumentSet entity);

        [OperationContract]
        Task<SystemDocumentSet> CreateSystemDocumentSet(SystemDocumentSet entity);

        [OperationContract]
        Task<bool> DeleteSystemDocumentSet(string id);

        [OperationContract]
        Task<bool> RemoveSelectedSystemDocumentSet(IEnumerable<string> selectedSystemDocumentSet);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<SystemDocumentSet>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<SystemDocumentSet>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}
