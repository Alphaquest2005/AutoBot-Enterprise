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
    public partial interface IFileTypeContactsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContacts(List<string> includesLst = null);

        [OperationContract]
        Task<FileTypeContacts> GetFileTypeContactsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<FileTypeContacts> UpdateFileTypeContacts(FileTypeContacts entity);

        [OperationContract]
        Task<FileTypeContacts> CreateFileTypeContacts(FileTypeContacts entity);

        [OperationContract]
        Task<bool> DeleteFileTypeContacts(string id);

        [OperationContract]
        Task<bool> RemoveSelectedFileTypeContacts(IEnumerable<string> selectedFileTypeContacts);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<FileTypeContacts>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<FileTypeContacts>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByFileTypeId(string FileTypeId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByContactId(string ContactId, List<string> includesLst = null);
        
  		
    }
}
