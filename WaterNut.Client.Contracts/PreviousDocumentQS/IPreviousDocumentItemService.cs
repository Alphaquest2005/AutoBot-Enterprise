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
using PreviousDocumentQS.Client.DTO;


namespace PreviousDocumentQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IPreviousDocumentItemService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItems(List<string> includesLst = null);

        [OperationContract]
        Task<PreviousDocumentItem> GetPreviousDocumentItemByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<PreviousDocumentItem> UpdatePreviousDocumentItem(PreviousDocumentItem entity);

        [OperationContract]
        Task<PreviousDocumentItem> CreatePreviousDocumentItem(PreviousDocumentItem entity);

        [OperationContract]
        Task<bool> DeletePreviousDocumentItem(string id);

        [OperationContract]
        Task<bool> RemoveSelectedPreviousDocumentItem(IEnumerable<string> selectedPreviousDocumentItem);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<PreviousDocumentItem>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<PreviousDocumentItem>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemByASYCUDA_Id(string ASYCUDA_Id, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<PreviousDocumentItem>> GetPreviousDocumentItemByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		
    }
}

