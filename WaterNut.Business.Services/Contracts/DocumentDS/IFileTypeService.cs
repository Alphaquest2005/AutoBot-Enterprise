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
using DocumentDS.Business.Entities;
using Core.Common.Business.Services;
using WaterNut.Interfaces;

namespace DocumentDS.Business.Services
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IFileTypeService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypes(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<FileType> GetFileTypeByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypesByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypesByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<FileType>> GetFileTypesByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypesByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypesByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<FileType> UpdateFileType(FileType entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<FileType> CreateFileType(FileType entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteFileType(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedFileType(IEnumerable<string> selectedFileType);
	
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
        Task<IEnumerable<FileType>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<FileType>> LoadRangeNav(int startIndex, int count, string exp,
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
        Task<IEnumerable<FileType>> GetFileTypeByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypeByFileGroupId(string FileGroupId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypeByParentFileTypeId(string ParentFileTypeId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypeByOldFileTypeId(string OldFileTypeId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<FileType>> GetFileTypeByFileInfoId(string FileInfoId, List<string> includesLst = null);
  



    }
}

