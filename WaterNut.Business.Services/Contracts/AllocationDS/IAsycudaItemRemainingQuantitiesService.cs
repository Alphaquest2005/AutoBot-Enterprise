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
using AllocationDS.Business.Entities;
using Core.Common.Business.Services;
using WaterNut.Interfaces;

namespace AllocationDS.Business.Services
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IAsycudaItemRemainingQuantitiesService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaItemRemainingQuantities>> GetAsycudaItemRemainingQuantities(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<AsycudaItemRemainingQuantities> GetAsycudaItemRemainingQuantitiesByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaItemRemainingQuantities>> GetAsycudaItemRemainingQuantitiesByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaItemRemainingQuantities>> GetAsycudaItemRemainingQuantitiesByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<AsycudaItemRemainingQuantities>> GetAsycudaItemRemainingQuantitiesByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaItemRemainingQuantities>> GetAsycudaItemRemainingQuantitiesByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaItemRemainingQuantities>> GetAsycudaItemRemainingQuantitiesByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<AsycudaItemRemainingQuantities> UpdateAsycudaItemRemainingQuantities(AsycudaItemRemainingQuantities entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<AsycudaItemRemainingQuantities> CreateAsycudaItemRemainingQuantities(AsycudaItemRemainingQuantities entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteAsycudaItemRemainingQuantities(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedAsycudaItemRemainingQuantities(IEnumerable<string> selectedAsycudaItemRemainingQuantities);
	
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
        Task<IEnumerable<AsycudaItemRemainingQuantities>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<AsycudaItemRemainingQuantities>> LoadRangeNav(int startIndex, int count, string exp,
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
        Task<IEnumerable<AsycudaItemRemainingQuantities>> GetAsycudaItemRemainingQuantitiesByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
  



    }
}
