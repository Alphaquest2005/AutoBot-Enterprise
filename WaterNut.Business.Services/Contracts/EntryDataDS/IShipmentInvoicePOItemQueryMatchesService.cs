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
using EntryDataDS.Business.Entities;
using Core.Common.Business.Services;
using WaterNut.Interfaces;

namespace EntryDataDS.Business.Services
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IShipmentInvoicePOItemQueryMatchesService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatches(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<ShipmentInvoicePOItemQueryMatches> GetShipmentInvoicePOItemQueryMatchesByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<ShipmentInvoicePOItemQueryMatches> UpdateShipmentInvoicePOItemQueryMatches(ShipmentInvoicePOItemQueryMatches entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<ShipmentInvoicePOItemQueryMatches> CreateShipmentInvoicePOItemQueryMatches(ShipmentInvoicePOItemQueryMatches entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteShipmentInvoicePOItemQueryMatches(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedShipmentInvoicePOItemQueryMatches(IEnumerable<string> selectedShipmentInvoicePOItemQueryMatches);
	
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
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> LoadRangeNav(int startIndex, int count, string exp,
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
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByPOId(string POId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByINVId(string INVId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByINVInventoryItemId(string INVInventoryItemId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<ShipmentInvoicePOItemQueryMatches>> GetShipmentInvoicePOItemQueryMatchesByPOInventoryItemId(string POInventoryItemId, List<string> includesLst = null);
  



    }
}
