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
    public partial interface IInvoiceIdentificatonRegExService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegEx(List<string> includesLst = null);

        [OperationContract]
        Task<InvoiceIdentificatonRegEx> GetInvoiceIdentificatonRegExByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegExByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegExByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegExByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegExByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegExByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<InvoiceIdentificatonRegEx> UpdateInvoiceIdentificatonRegEx(InvoiceIdentificatonRegEx entity);

        [OperationContract]
        Task<InvoiceIdentificatonRegEx> CreateInvoiceIdentificatonRegEx(InvoiceIdentificatonRegEx entity);

        [OperationContract]
        Task<bool> DeleteInvoiceIdentificatonRegEx(string id);

        [OperationContract]
        Task<bool> RemoveSelectedInvoiceIdentificatonRegEx(IEnumerable<string> selectedInvoiceIdentificatonRegEx);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<InvoiceIdentificatonRegEx>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<InvoiceIdentificatonRegEx>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegExByInvoiceId(string InvoiceId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<InvoiceIdentificatonRegEx>> GetInvoiceIdentificatonRegExByRegExId(string RegExId, List<string> includesLst = null);
        
  		
    }
}
