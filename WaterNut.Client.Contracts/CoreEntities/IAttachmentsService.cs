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
    public partial interface IAttachmentsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<Attachments>> GetAttachments(List<string> includesLst = null);

        [OperationContract]
        Task<Attachments> GetAttachmentsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<Attachments>> GetAttachmentsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<Attachments>> GetAttachmentsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<Attachments>> GetAttachmentsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<Attachments>> GetAttachmentsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<Attachments>> GetAttachmentsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<Attachments> UpdateAttachments(Attachments entity);

        [OperationContract]
        Task<Attachments> CreateAttachments(Attachments entity);

        [OperationContract]
        Task<bool> DeleteAttachments(string id);

        [OperationContract]
        Task<bool> RemoveSelectedAttachments(IEnumerable<string> selectedAttachments);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<Attachments>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<Attachments>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<Attachments>> GetAttachmentsByEmailId(string EmailId, List<string> includesLst = null);
        
  		
    }
}

