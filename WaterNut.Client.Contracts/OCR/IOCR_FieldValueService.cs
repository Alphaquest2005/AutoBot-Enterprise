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
    public partial interface IOCR_FieldValueService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<OCR_FieldValue>> GetOCR_FieldValue(List<string> includesLst = null);

        [OperationContract]
        Task<OCR_FieldValue> GetOCR_FieldValueByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<OCR_FieldValue>> GetOCR_FieldValueByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<OCR_FieldValue>> GetOCR_FieldValueByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<OCR_FieldValue>> GetOCR_FieldValueByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<OCR_FieldValue>> GetOCR_FieldValueByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<OCR_FieldValue>> GetOCR_FieldValueByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<OCR_FieldValue> UpdateOCR_FieldValue(OCR_FieldValue entity);

        [OperationContract]
        Task<OCR_FieldValue> CreateOCR_FieldValue(OCR_FieldValue entity);

        [OperationContract]
        Task<bool> DeleteOCR_FieldValue(string id);

        [OperationContract]
        Task<bool> RemoveSelectedOCR_FieldValue(IEnumerable<string> selectedOCR_FieldValue);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<OCR_FieldValue>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<OCR_FieldValue>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}

