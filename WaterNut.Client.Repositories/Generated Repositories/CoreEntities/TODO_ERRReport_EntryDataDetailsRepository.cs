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

using TODO_ERRReport_EntryDataDetails = CoreEntities.Client.Entities.TODO_ERRReport_EntryDataDetails;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class TODO_ERRReport_EntryDataDetailsRepository : BaseRepository<TODO_ERRReport_EntryDataDetailsRepository>
    {

       private static readonly TODO_ERRReport_EntryDataDetailsRepository instance;
       static TODO_ERRReport_EntryDataDetailsRepository()
        {
            instance = new TODO_ERRReport_EntryDataDetailsRepository();
        }

       public static TODO_ERRReport_EntryDataDetailsRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TODO_ERRReport_EntryDataDetails>> TODO_ERRReport_EntryDataDetails(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TODO_ERRReport_EntryDataDetails>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
                        var res = await t.GetTODO_ERRReport_EntryDataDetails(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_EntryDataDetails(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_ERRReport_EntryDataDetails>> GetTODO_ERRReport_EntryDataDetailsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_ERRReport_EntryDataDetails>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
					    IEnumerable<DTO.TODO_ERRReport_EntryDataDetails> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTODO_ERRReport_EntryDataDetails(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_ERRReport_EntryDataDetailsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_EntryDataDetails(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_ERRReport_EntryDataDetails>> GetTODO_ERRReport_EntryDataDetailsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TODO_ERRReport_EntryDataDetails>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
					    IEnumerable<DTO.TODO_ERRReport_EntryDataDetails> res = null;
                       
                        res = await t.GetTODO_ERRReport_EntryDataDetailsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_EntryDataDetails(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TODO_ERRReport_EntryDataDetails>> GetTODO_ERRReport_EntryDataDetailsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_ERRReport_EntryDataDetails>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
					    IEnumerable<DTO.TODO_ERRReport_EntryDataDetails> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTODO_ERRReport_EntryDataDetails(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_ERRReport_EntryDataDetailsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_EntryDataDetails(x)).AsEnumerable();
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


        public async Task<TODO_ERRReport_EntryDataDetails> GetTODO_ERRReport_EntryDataDetails(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
                        var res = await t.GetTODO_ERRReport_EntryDataDetailsByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TODO_ERRReport_EntryDataDetails(res);
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

        public async Task<TODO_ERRReport_EntryDataDetails> UpdateTODO_ERRReport_EntryDataDetails(TODO_ERRReport_EntryDataDetails entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
     
                        var updatedEntity =  await t.UpdateTODO_ERRReport_EntryDataDetails(entitychanges).ConfigureAwait(false);
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

        public async Task<TODO_ERRReport_EntryDataDetails> CreateTODO_ERRReport_EntryDataDetails(TODO_ERRReport_EntryDataDetails entity)
        {
            try
            {   
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
                        return new TODO_ERRReport_EntryDataDetails(await t.CreateTODO_ERRReport_EntryDataDetails(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTODO_ERRReport_EntryDataDetails(string id)
        {
            try
            {
             using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                {
                    return await t.DeleteTODO_ERRReport_EntryDataDetails(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTODO_ERRReport_EntryDataDetails(IEnumerable<string> selectedTODO_ERRReport_EntryDataDetails)
        {
            try
            {
                using (var ctx = new TODO_ERRReport_EntryDataDetailsClient())
                {
                    return await ctx.RemoveSelectedTODO_ERRReport_EntryDataDetails(selectedTODO_ERRReport_EntryDataDetails).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TODO_ERRReport_EntryDataDetails>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TODO_ERRReport_EntryDataDetails>, int>(new List<TODO_ERRReport_EntryDataDetails>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
                {

                    IEnumerable<DTO.TODO_ERRReport_EntryDataDetails> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TODO_ERRReport_EntryDataDetails>, int>(res.Select(x => new TODO_ERRReport_EntryDataDetails(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<TODO_ERRReport_EntryDataDetails>> GetTODO_ERRReport_EntryDataDetailsByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
             if (ApplicationSettingsId == "0") return null;
            try
            {
                 using (TODO_ERRReport_EntryDataDetailsClient t = new TODO_ERRReport_EntryDataDetailsClient())
                    {
                        var res = await t.GetTODO_ERRReport_EntryDataDetailsByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_EntryDataDetails(x)).AsEnumerable();
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
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
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
                using (var t = new TODO_ERRReport_EntryDataDetailsClient())
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
