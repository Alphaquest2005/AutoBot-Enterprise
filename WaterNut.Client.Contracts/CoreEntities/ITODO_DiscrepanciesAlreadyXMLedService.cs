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
    public partial interface ITODO_DiscrepanciesAlreadyXMLedService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLed(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_DiscrepanciesAlreadyXMLed> GetTODO_DiscrepanciesAlreadyXMLedByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_DiscrepanciesAlreadyXMLed> UpdateTODO_DiscrepanciesAlreadyXMLed(TODO_DiscrepanciesAlreadyXMLed entity);

        [OperationContract]
        Task<TODO_DiscrepanciesAlreadyXMLed> CreateTODO_DiscrepanciesAlreadyXMLed(TODO_DiscrepanciesAlreadyXMLed entity);

        [OperationContract]
        Task<bool> DeleteTODO_DiscrepanciesAlreadyXMLed(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_DiscrepanciesAlreadyXMLed(IEnumerable<string> selectedTODO_DiscrepanciesAlreadyXMLed);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByEmailId(string EmailId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByFileTypeId(string FileTypeId, List<string> includesLst = null);
        
  		
    }
}
