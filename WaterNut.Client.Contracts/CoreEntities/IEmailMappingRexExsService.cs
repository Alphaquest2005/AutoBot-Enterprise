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
    public partial interface IEmailMappingRexExsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<EmailMappingRexExs>> GetEmailMappingRexExs(List<string> includesLst = null);

        [OperationContract]
        Task<EmailMappingRexExs> GetEmailMappingRexExsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<EmailMappingRexExs>> GetEmailMappingRexExsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<EmailMappingRexExs>> GetEmailMappingRexExsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<EmailMappingRexExs>> GetEmailMappingRexExsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<EmailMappingRexExs>> GetEmailMappingRexExsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<EmailMappingRexExs>> GetEmailMappingRexExsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<EmailMappingRexExs> UpdateEmailMappingRexExs(EmailMappingRexExs entity);

        [OperationContract]
        Task<EmailMappingRexExs> CreateEmailMappingRexExs(EmailMappingRexExs entity);

        [OperationContract]
        Task<bool> DeleteEmailMappingRexExs(string id);

        [OperationContract]
        Task<bool> RemoveSelectedEmailMappingRexExs(IEnumerable<string> selectedEmailMappingRexExs);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<EmailMappingRexExs>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<EmailMappingRexExs>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<EmailMappingRexExs>> GetEmailMappingRexExsByEmailMappingId(string EmailMappingId, List<string> includesLst = null);
        
  		
    }
}

