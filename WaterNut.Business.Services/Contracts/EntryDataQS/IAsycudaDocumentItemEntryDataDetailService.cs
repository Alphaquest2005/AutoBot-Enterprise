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
using EntryDataQS.Business.Entities;
using Core.Common.Business.Services;
using WaterNut.Interfaces;

namespace EntryDataQS.Business.Services
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IAsycudaDocumentItemEntryDataDetailService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetails(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<AsycudaDocumentItemEntryDataDetail> GetAsycudaDocumentItemEntryDataDetailByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<AsycudaDocumentItemEntryDataDetail> UpdateAsycudaDocumentItemEntryDataDetail(AsycudaDocumentItemEntryDataDetail entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<AsycudaDocumentItemEntryDataDetail> CreateAsycudaDocumentItemEntryDataDetail(AsycudaDocumentItemEntryDataDetail entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteAsycudaDocumentItemEntryDataDetail(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedAsycudaDocumentItemEntryDataDetail(IEnumerable<string> selectedAsycudaDocumentItemEntryDataDetail);
	
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
        Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> LoadRangeNav(int startIndex, int count, string exp,
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

		



    }
}
