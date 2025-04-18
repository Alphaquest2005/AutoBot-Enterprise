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
using AdjustmentQS.Client.DTO;


namespace AdjustmentQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface ITODO_AdjustmentOversToXMLService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXML(List<string> includesLst = null);

        [OperationContract]
        Task<TODO_AdjustmentOversToXML> GetTODO_AdjustmentOversToXMLByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TODO_AdjustmentOversToXML> UpdateTODO_AdjustmentOversToXML(TODO_AdjustmentOversToXML entity);

        [OperationContract]
        Task<TODO_AdjustmentOversToXML> CreateTODO_AdjustmentOversToXML(TODO_AdjustmentOversToXML entity);

        [OperationContract]
        Task<bool> DeleteTODO_AdjustmentOversToXML(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTODO_AdjustmentOversToXML(IEnumerable<string> selectedTODO_AdjustmentOversToXML);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TODO_AdjustmentOversToXML>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TODO_AdjustmentOversToXML>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByEntryDataId(string EntryDataId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByEmailId(string EmailId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<TODO_AdjustmentOversToXML>> GetTODO_AdjustmentOversToXMLByFileTypeId(string FileTypeId, List<string> includesLst = null);
        
  		
    }
}

