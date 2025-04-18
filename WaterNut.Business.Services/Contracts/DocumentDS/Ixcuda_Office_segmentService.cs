﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;
using System.Threading.Tasks;

using Core.Common.Contracts;
using DocumentDS.Business.Entities;
using Core.Common.Business.Services;
using WaterNut.Interfaces;

namespace DocumentDS.Business.Services
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface Ixcuda_Office_segmentService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<xcuda_Office_segment>> Getxcuda_Office_segment(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<xcuda_Office_segment> Getxcuda_Office_segmentByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<xcuda_Office_segment>> Getxcuda_Office_segmentByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<xcuda_Office_segment>> Getxcuda_Office_segmentByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<xcuda_Office_segment>> Getxcuda_Office_segmentByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<xcuda_Office_segment>> Getxcuda_Office_segmentByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<xcuda_Office_segment>> Getxcuda_Office_segmentByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<xcuda_Office_segment> Updatexcuda_Office_segment(xcuda_Office_segment entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<xcuda_Office_segment> Createxcuda_Office_segment(xcuda_Office_segment entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> Deletexcuda_Office_segment(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedxcuda_Office_segment(IEnumerable<string> selectedxcuda_Office_segment);
	
		//Virtural list implementation
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<int> CountByExpressionLst(List<string> expLst);
    
		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<int> Count(string exp);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<int> CountNav(string exp, Dictionary<string, string> navExp);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<xcuda_Office_segment>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<xcuda_Office_segment>> LoadRangeNav(int startIndex, int count, string exp,
                                                                                 Dictionary<string, string> navExp, IEnumerable<string> includeLst = null);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		decimal SumField(string whereExp, string field);
        
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<decimal> SumNav( string exp, Dictionary<string, string> navExp, string field);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		string MinField(string whereExp, string field);

		



    }
}

