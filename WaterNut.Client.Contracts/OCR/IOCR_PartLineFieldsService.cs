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
    public partial interface IOCR_PartLineFieldsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<OCR_PartLineFields>> GetOCR_PartLineFields(List<string> includesLst = null);

        [OperationContract]
        Task<OCR_PartLineFields> GetOCR_PartLineFieldsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<OCR_PartLineFields>> GetOCR_PartLineFieldsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<OCR_PartLineFields>> GetOCR_PartLineFieldsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<OCR_PartLineFields>> GetOCR_PartLineFieldsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<OCR_PartLineFields>> GetOCR_PartLineFieldsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<OCR_PartLineFields>> GetOCR_PartLineFieldsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<OCR_PartLineFields> UpdateOCR_PartLineFields(OCR_PartLineFields entity);

        [OperationContract]
        Task<OCR_PartLineFields> CreateOCR_PartLineFields(OCR_PartLineFields entity);

        [OperationContract]
        Task<bool> DeleteOCR_PartLineFields(string id);

        [OperationContract]
        Task<bool> RemoveSelectedOCR_PartLineFields(IEnumerable<string> selectedOCR_PartLineFields);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<OCR_PartLineFields>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<OCR_PartLineFields>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<OCR_PartLineFields>> GetOCR_PartLineFieldsByParentId(string ParentId, List<string> includesLst = null);
        
  		
    }
}

