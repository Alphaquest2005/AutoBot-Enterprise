﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;
using System.Threading.Tasks;

using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using Core.Common.Business.Services;
using WaterNut.Interfaces;

namespace CoreEntities.Business.Services
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IFileTypeContactsService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContacts(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<FileTypeContacts> GetFileTypeContactsByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<FileTypeContacts> UpdateFileTypeContacts(FileTypeContacts entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<FileTypeContacts> CreateFileTypeContacts(FileTypeContacts entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteFileTypeContacts(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedFileTypeContacts(IEnumerable<string> selectedFileTypeContacts);
	
		//Virtural list implementation
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<int> Count(string exp);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<FileTypeContacts>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		string MinField(string whereExp, string field);

				[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByFileTypeId(string FileTypeId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileTypeContacts>> GetFileTypeContactsByContactId(string ContactId, List<string> includesLst = null);
  



    }
}

