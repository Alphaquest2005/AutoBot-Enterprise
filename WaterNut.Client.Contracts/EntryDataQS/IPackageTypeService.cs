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
using EntryDataQS.Client.DTO;


namespace EntryDataQS.Client.Contracts
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface IPackageTypeService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<PackageType>> GetPackageTypes(List<string> includesLst = null);

        [OperationContract]
        Task<PackageType> GetPackageTypeByKey(string id, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<PackageType>> GetPackageTypesByExpression(string exp, List<string> includesLst = null);

		[OperationContract]
        Task<IEnumerable<PackageType>> GetPackageTypesByExpressionLst(List<string> expLst, List<string> includesLst = null);

		[OperationContract]
		Task<IEnumerable<PackageType>> GetPackageTypesByExpressionNav(string exp,
														 Dictionary<string, string> navExp, List<string> includesLst = null);        
        [OperationContract]
        Task<IEnumerable<PackageType>> GetPackageTypesByBatch(string exp,
                                                                        int totalrow, List<string> includesLst = null);
        [OperationContract]
        Task<IEnumerable<PackageType>> GetPackageTypesByBatchExpressionLst(List<string> expLst,
                                                                        int totalrow, List<string> includesLst = null);

		[OperationContract]
        Task<PackageType> UpdatePackageType(PackageType entity);

        [OperationContract]
        Task<PackageType> CreatePackageType(PackageType entity);

        [OperationContract]
        Task<bool> DeletePackageType(string id);

        [OperationContract]
        Task<bool> RemoveSelectedPackageType(IEnumerable<string> selectedPackageType);

		// Virtural List Implementation

        [OperationContract]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        Task<int> Count(string exp);

		[OperationContract]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        Task<IEnumerable<PackageType>> LoadRange(int startIndex, int count, string exp);

		[OperationContract]
		Task<IEnumerable<PackageType>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
		string MinField(string whereExp, string field);

				
    }
}

