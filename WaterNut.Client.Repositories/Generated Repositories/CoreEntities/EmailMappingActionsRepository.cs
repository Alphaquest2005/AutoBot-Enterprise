﻿// <autogenerated>
//   This file was generated by T4 code generator AllRepository.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using Core.Common.Client.Services;
using Core.Common.Client.Repositories;
using CoreEntities.Client.Services;
using CoreEntities.Client.Entities;
using CoreEntities.Client.DTO;
using Core.Common.Business.Services;
using System.Diagnostics;


using System.Threading.Tasks;
using System.Linq;
using Core.Common;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.ServiceModel;
using TrackableEntities.Common;

using EmailMappingActions = CoreEntities.Client.Entities.EmailMappingActions;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class EmailMappingActionsRepository : BaseRepository<EmailMappingActionsRepository>
    {

       private static readonly EmailMappingActionsRepository instance;
       static EmailMappingActionsRepository()
        {
            instance = new EmailMappingActionsRepository();
        }

       public static EmailMappingActionsRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<EmailMappingActions>> EmailMappingActions(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<EmailMappingActions>().AsEnumerable();
            try
            {
                using (var t = new EmailMappingActionsClient())
                    {
                        var res = await t.GetEmailMappingActions(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new EmailMappingActions(x)).AsEnumerable();
                        }
                        else
                        {
                            return null;
                        }                    
                    }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

		 public async Task<IEnumerable<EmailMappingActions>> GetEmailMappingActionsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<EmailMappingActions>().AsEnumerable();
            try
            {
                using (var t = new EmailMappingActionsClient())
                    {
					    IEnumerable<DTO.EmailMappingActions> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetEmailMappingActions(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetEmailMappingActionsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new EmailMappingActions(x)).AsEnumerable();
                        }
                        else
                        {
                            return null;
                        }                    
                    }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

		 public async Task<IEnumerable<EmailMappingActions>> GetEmailMappingActionsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<EmailMappingActions>().AsEnumerable();
            try
            {
                using (var t = new EmailMappingActionsClient())
                    {
					    IEnumerable<DTO.EmailMappingActions> res = null;
                       
                        res = await t.GetEmailMappingActionsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new EmailMappingActions(x)).AsEnumerable();
                        }
                        else
                        {
                            return null;
                        }                    
                    }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }


		 public async Task<IEnumerable<EmailMappingActions>> GetEmailMappingActionsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<EmailMappingActions>().AsEnumerable();
            try
            {
                using (var t = new EmailMappingActionsClient())
                    {
					    IEnumerable<DTO.EmailMappingActions> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetEmailMappingActions(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetEmailMappingActionsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new EmailMappingActions(x)).AsEnumerable();
                        }
                        else
                        {
                            return null;
                        }                    
                    }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }


        public async Task<EmailMappingActions> GetEmailMappingActions(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new EmailMappingActionsClient())
                    {
                        var res = await t.GetEmailMappingActionsByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new EmailMappingActions(res)
                    {
                  // Actions = (res.Actions != null?new Actions(res.Actions): null),    
                  // EmailMapping = (res.EmailMapping != null?new EmailMapping(res.EmailMapping): null)    
                  };
                    }
                    else
                    {
                        return null;
                    }                    
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

        public async Task<EmailMappingActions> UpdateEmailMappingActions(EmailMappingActions entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new EmailMappingActionsClient())
                    {
     
                        var updatedEntity =  await t.UpdateEmailMappingActions(entitychanges).ConfigureAwait(false);
                        entity.EntityId = updatedEntity.EntityId;
                        entity.DTO.AcceptChanges();
                         //var  = entity.;
                        //entity.ChangeTracker.MergeChanges(,updatedEntity);
                        //entity. = ;
                        return entity;
                    }
                }
                catch (FaultException<ValidationFault> e)
                {
                    throw new Exception(e.Detail.Message, e.InnerException);
                }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
            }
            else
            {
                return entity;
            }

        }

        public async Task<EmailMappingActions> CreateEmailMappingActions(EmailMappingActions entity)
        {
            try
            {   
                using (var t = new EmailMappingActionsClient())
                    {
                        return new EmailMappingActions(await t.CreateEmailMappingActions(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
                    }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

        public async Task<bool> DeleteEmailMappingActions(string id)
        {
            try
            {
             using (var t = new EmailMappingActionsClient())
                {
                    return await t.DeleteEmailMappingActions(id).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }  
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }         
        }

        public async Task<bool> RemoveSelectedEmailMappingActions(IEnumerable<string> selectedEmailMappingActions)
        {
            try
            {
                using (var ctx = new EmailMappingActionsClient())
                {
                    return await ctx.RemoveSelectedEmailMappingActions(selectedEmailMappingActions).ConfigureAwait(false);
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }  
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }      
        }


		//Virtural List Implementation

		public async Task<Tuple<IEnumerable<EmailMappingActions>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<EmailMappingActions>, int>(new List<EmailMappingActions>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new EmailMappingActionsClient())
                {

                    IEnumerable<DTO.EmailMappingActions> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<EmailMappingActions>, int>(res.Select(x => new EmailMappingActions(x)).AsEnumerable(), overallCount);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        }

	 public async Task<IEnumerable<EmailMappingActions>> GetEmailMappingActionsByEmailMappingId(string EmailMappingId, List<string> includesLst = null)
        {
             if (EmailMappingId == "0") return null;
            try
            {
                 using (EmailMappingActionsClient t = new EmailMappingActionsClient())
                    {
                        var res = await t.GetEmailMappingActionsByEmailMappingId(EmailMappingId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new EmailMappingActions(x)).AsEnumerable();
					    }                
					    else
					    {
						    return null;
					    }                    
                    }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        } 
 	 public async Task<IEnumerable<EmailMappingActions>> GetEmailMappingActionsByActionId(string ActionId, List<string> includesLst = null)
        {
             if (ActionId == "0") return null;
            try
            {
                 using (EmailMappingActionsClient t = new EmailMappingActionsClient())
                    {
                        var res = await t.GetEmailMappingActionsByActionId(ActionId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new EmailMappingActions(x)).AsEnumerable();
					    }                
					    else
					    {
						    return null;
					    }                    
                    }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }
        } 
         
		public decimal SumField(string whereExp, string sumExp)
        {
            try
            {
                using (var t = new EmailMappingActionsClient())
                {
                    return t.SumField(whereExp,sumExp);
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }

        }

        public async Task<decimal> SumNav(string whereExp, Dictionary<string, string> navExp, string sumExp)
        {
            try
            {
                using (var t = new EmailMappingActionsClient())
                {
                    return await t.SumNav(whereExp,navExp,sumExp).ConfigureAwait(false);
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception)
            {
                Debugger.Break();
                throw;
            }

        }
    }
}
