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
    public partial interface IInvoiceRegExService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<InvoiceRegEx>> GetOCR_InvoiceRegEx(List<string> includesLst = null);

        [OperationContract]
        Task<InvoiceRegEx> GetInvoiceRegExByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InvoiceRegEx>> GetOCR_InvoiceRegExByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InvoiceRegEx>> GetOCR_InvoiceRegExByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<InvoiceRegEx>> GetOCR_InvoiceRegExByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<InvoiceRegEx>> GetOCR_InvoiceRegExByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<InvoiceRegEx>> GetOCR_InvoiceRegExByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<InvoiceRegEx> UpdateInvoiceRegEx(InvoiceRegEx entity);

        [OperationContract]
        Task<InvoiceRegEx> CreateInvoiceRegEx(InvoiceRegEx entity);

        [OperationContract]
        Task<bool> DeleteInvoiceRegEx(string id);

        [OperationContract]
        Task<bool> RemoveSelectedInvoiceRegEx(IEnumerable<string> selectedInvoiceRegEx);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<InvoiceRegEx>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<InvoiceRegEx>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<InvoiceRegEx>> GetInvoiceRegExByInvoiceId(string InvoiceId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<InvoiceRegEx>> GetInvoiceRegExByRegExId(string RegExId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<InvoiceRegEx>> GetInvoiceRegExByReplacementRegExId(string ReplacementRegExId, List<string> includesLst = null);
        
  		
    }
}

