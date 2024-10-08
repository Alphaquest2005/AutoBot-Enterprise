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
    public partial interface IDeclarantService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<Declarant>> GetDeclarants(List<string> includesLst = null);

        [OperationContract]
        Task<Declarant> GetDeclarantByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<Declarant>> GetDeclarantsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<Declarant>> GetDeclarantsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<Declarant>> GetDeclarantsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<Declarant>> GetDeclarantsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<Declarant>> GetDeclarantsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<Declarant> UpdateDeclarant(Declarant entity);

        [OperationContract]
        Task<Declarant> CreateDeclarant(Declarant entity);

        [OperationContract]
        Task<bool> DeleteDeclarant(string id);

        [OperationContract]
        Task<bool> RemoveSelectedDeclarant(IEnumerable<string> selectedDeclarant);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<Declarant>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<Declarant>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<Declarant>> GetDeclarantByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		
    }
}

