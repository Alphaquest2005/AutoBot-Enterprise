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
using OCR.Client.DTO;


namespace OCR.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IPartService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<Part>> GetPartExs(List<string> includesLst = null);

        [OperationContract]
        Task<Part> GetPartByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<Part>> GetPartExsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<Part>> GetPartExsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<Part>> GetPartExsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<Part>> GetPartExsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<Part>> GetPartExsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<Part> UpdatePart(Part entity);

        [OperationContract]
        Task<Part> CreatePart(Part entity);

        [OperationContract]
        Task<bool> DeletePart(string id);

        [OperationContract]
        Task<bool> RemoveSelectedPart(IEnumerable<string> selectedPart);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<Part>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<Part>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<Part>> GetPartByStartRegExId(string StartRegExId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<Part>> GetPartByInvoiceId(string InvoiceId, List<string> includesLst = null);
        
  		
    }
}
