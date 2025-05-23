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
    public partial interface IEX9AsycudaSalesAllocationsService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocations(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<EX9AsycudaSalesAllocations> GetEX9AsycudaSalesAllocationsByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<EX9AsycudaSalesAllocations> UpdateEX9AsycudaSalesAllocations(EX9AsycudaSalesAllocations entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<EX9AsycudaSalesAllocations> CreateEX9AsycudaSalesAllocations(EX9AsycudaSalesAllocations entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteEX9AsycudaSalesAllocations(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedEX9AsycudaSalesAllocations(IEnumerable<string> selectedEX9AsycudaSalesAllocations);
	
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
        Task<IEnumerable<EX9AsycudaSalesAllocations>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<EX9AsycudaSalesAllocations>> LoadRangeNav(int startIndex, int count, string exp,
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
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByPreviousItem_Id(string PreviousItem_Id, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsBypASYCUDA_Id(string pASYCUDA_Id, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByxBond_Item_Id(string xBond_Item_Id, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByEmailId(string EmailId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByFileTypeId(string FileTypeId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByEntryData_Id(string EntryData_Id, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByCustomsOperationId(string CustomsOperationId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByCustoms_ProcedureId(string Customs_ProcedureId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<EX9AsycudaSalesAllocations>> GetEX9AsycudaSalesAllocationsByInventoryItemId(string InventoryItemId, List<string> includesLst = null);
  



    }
}

