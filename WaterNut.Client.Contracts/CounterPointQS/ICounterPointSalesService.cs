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
using CounterPointQS.Client.DTO;


namespace CounterPointQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface ICounterPointSalesService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<CounterPointSales>> GetCounterPointSales(List<string> includesLst = null);

        [OperationContract]
        Task<CounterPointSales> GetCounterPointSalesByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<CounterPointSales>> GetCounterPointSalesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<CounterPointSales>> GetCounterPointSalesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<CounterPointSales>> GetCounterPointSalesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<CounterPointSales>> GetCounterPointSalesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<CounterPointSales>> GetCounterPointSalesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<CounterPointSales> UpdateCounterPointSales(CounterPointSales entity);

        [OperationContract]
        Task<CounterPointSales> CreateCounterPointSales(CounterPointSales entity);

        [OperationContract]
        Task<bool> DeleteCounterPointSales(string id);

        [OperationContract]
        Task<bool> RemoveSelectedCounterPointSales(IEnumerable<string> selectedCounterPointSales);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<CounterPointSales>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<CounterPointSales>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}

