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
    public partial interface ITODO_LicenceAvailableQtyService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<TODO_LicenceAvailableQty>> GetTODO_LicenceAvailableQty(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<TODO_LicenceAvailableQty> GetTODO_LicenceAvailableQtyByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<TODO_LicenceAvailableQty>> GetTODO_LicenceAvailableQtyByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<TODO_LicenceAvailableQty>> GetTODO_LicenceAvailableQtyByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<TODO_LicenceAvailableQty>> GetTODO_LicenceAvailableQtyByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<TODO_LicenceAvailableQty>> GetTODO_LicenceAvailableQtyByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<TODO_LicenceAvailableQty>> GetTODO_LicenceAvailableQtyByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<TODO_LicenceAvailableQty> UpdateTODO_LicenceAvailableQty(TODO_LicenceAvailableQty entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<TODO_LicenceAvailableQty> CreateTODO_LicenceAvailableQty(TODO_LicenceAvailableQty entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteTODO_LicenceAvailableQty(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedTODO_LicenceAvailableQty(IEnumerable<string> selectedTODO_LicenceAvailableQty);
	
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
        Task<IEnumerable<TODO_LicenceAvailableQty>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<TODO_LicenceAvailableQty>> LoadRangeNav(int startIndex, int count, string exp,
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
        Task<IEnumerable<TODO_LicenceAvailableQty>> GetTODO_LicenceAvailableQtyByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
  



    }
}
