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
using CoreEntities.Business.Entities;
using Core.Common.Business.Services;
using WaterNut.Interfaces;

namespace CoreEntities.Business.Services
{
    [ServiceContract (Namespace="http://www.insight-software.com/WaterNut")]
    public partial interface ISessionActionsService : IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<SessionActions>> GetSessionActions(List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<SessionActions> GetSessionActionsByKey(string id, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<SessionActions>> GetSessionActionsByExpression(string exp, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<SessionActions>> GetSessionActionsByExpressionLst(List<string> expLst, List<string> includesLst = null, bool tracking = true);

		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<SessionActions>> GetSessionActionsByExpressionNav(string exp,
            Dictionary<string, string> navExp, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<SessionActions>> GetSessionActionsByBatch(string exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<SessionActions>> GetSessionActionsByBatchExpressionLst(List<string> exp,
            int totalrow, List<string> includesLst = null, bool tracking = true);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<SessionActions> UpdateSessionActions(SessionActions entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<SessionActions> CreateSessionActions(SessionActions entity);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> DeleteSessionActions(string id);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> RemoveSelectedSessionActions(IEnumerable<string> selectedSessionActions);
	
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
        Task<IEnumerable<SessionActions>> LoadRange(int startIndex, int count, string exp);



		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
		Task<IEnumerable<SessionActions>> LoadRangeNav(int startIndex, int count, string exp,
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

				[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<SessionActions>> GetSessionActionsBySessionId(string SessionId, List<string> includesLst = null);
  		[OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<SessionActions>> GetSessionActionsByActionId(string ActionId, List<string> includesLst = null);
  



    }
}
