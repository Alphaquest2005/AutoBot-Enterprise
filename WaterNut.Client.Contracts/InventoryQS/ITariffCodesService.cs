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
using InventoryQS.Client.DTO;


namespace InventoryQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface ITariffCodesService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<TariffCodes>> GetTariffCodes(List<string> includesLst = null);

        [OperationContract]
        Task<TariffCodes> GetTariffCodesByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TariffCodes>> GetTariffCodesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<TariffCodes>> GetTariffCodesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<TariffCodes>> GetTariffCodesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<TariffCodes>> GetTariffCodesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<TariffCodes>> GetTariffCodesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<TariffCodes> UpdateTariffCodes(TariffCodes entity);

        [OperationContract]
        Task<TariffCodes> CreateTariffCodes(TariffCodes entity);

        [OperationContract]
        Task<bool> DeleteTariffCodes(string id);

        [OperationContract]
        Task<bool> RemoveSelectedTariffCodes(IEnumerable<string> selectedTariffCodes);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<TariffCodes>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<TariffCodes>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}

