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
    public partial interface IOCR_FieldMappingsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<OCR_FieldMappings>> GetOCR_FieldMappings(List<string> includesLst = null);

        [OperationContract]
        Task<OCR_FieldMappings> GetOCR_FieldMappingsByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<OCR_FieldMappings>> GetOCR_FieldMappingsByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<OCR_FieldMappings>> GetOCR_FieldMappingsByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<OCR_FieldMappings>> GetOCR_FieldMappingsByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<OCR_FieldMappings>> GetOCR_FieldMappingsByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<OCR_FieldMappings>> GetOCR_FieldMappingsByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<OCR_FieldMappings> UpdateOCR_FieldMappings(OCR_FieldMappings entity);

        [OperationContract]
        Task<OCR_FieldMappings> CreateOCR_FieldMappings(OCR_FieldMappings entity);

        [OperationContract]
        Task<bool> DeleteOCR_FieldMappings(string id);

        [OperationContract]
        Task<bool> RemoveSelectedOCR_FieldMappings(IEnumerable<string> selectedOCR_FieldMappings);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<OCR_FieldMappings>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<OCR_FieldMappings>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<OCR_FieldMappings>> GetOCR_FieldMappingsByFileTypeId(string FileTypeId, List<string> includesLst = null);
        
  		
    }
}

