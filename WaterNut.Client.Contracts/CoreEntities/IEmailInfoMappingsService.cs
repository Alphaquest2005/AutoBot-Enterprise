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
    public partial interface IEmailInfoMappingsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappings(List<string> includesLst = null);

        [OperationContract]
        Task<EmailInfoMappings> GetEmailInfoMappingsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappingsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappingsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappingsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappingsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappingsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<EmailInfoMappings> UpdateEmailInfoMappings(EmailInfoMappings entity);

        [OperationContract]
        Task<EmailInfoMappings> CreateEmailInfoMappings(EmailInfoMappings entity);

        [OperationContract]
        Task<bool> DeleteEmailInfoMappings(string id);

        [OperationContract]
        Task<bool> RemoveSelectedEmailInfoMappings(IEnumerable<string> selectedEmailInfoMappings);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<EmailInfoMappings>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<EmailInfoMappings>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappingsByEmailMappingId(string EmailMappingId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<EmailInfoMappings>> GetEmailInfoMappingsByInfoMappingId(string InfoMappingId, List<string> includesLst = null);
        
  		
    }
}
