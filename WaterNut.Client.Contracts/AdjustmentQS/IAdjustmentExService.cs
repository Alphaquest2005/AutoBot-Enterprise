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
    public partial interface IAdjustmentExService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<AdjustmentEx>> GetAdjustmentExes(List<string> includesLst = null);

        [OperationContract]
        Task<AdjustmentEx> GetAdjustmentExByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AdjustmentEx>> GetAdjustmentExesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<AdjustmentEx>> GetAdjustmentExesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<AdjustmentEx>> GetAdjustmentExesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<AdjustmentEx>> GetAdjustmentExesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<AdjustmentEx>> GetAdjustmentExesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<AdjustmentEx> UpdateAdjustmentEx(AdjustmentEx entity);

        [OperationContract]
        Task<AdjustmentEx> CreateAdjustmentEx(AdjustmentEx entity);

        [OperationContract]
        Task<bool> DeleteAdjustmentEx(string id);

        [OperationContract]
        Task<bool> RemoveSelectedAdjustmentEx(IEnumerable<string> selectedAdjustmentEx);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<AdjustmentEx>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<AdjustmentEx>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				[OperationContract]
		Task<IEnumerable<AdjustmentEx>> GetAdjustmentExByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentEx>> GetAdjustmentExByEmailId(string EmailId, List<string> includesLst = null);
        
  		[OperationContract]
		Task<IEnumerable<AdjustmentEx>> GetAdjustmentExByFileTypeId(string FileTypeId, List<string> includesLst = null);
        
  		
    }
}

