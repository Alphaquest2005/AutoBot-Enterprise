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

using TODO_ERRReport_ByItemNumber = CoreEntities.Client.Entities.TODO_ERRReport_ByItemNumber;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class TODO_ERRReport_ByItemNumberRepository : BaseRepository<TODO_ERRReport_ByItemNumberRepository>
    {

       private static readonly TODO_ERRReport_ByItemNumberRepository instance;
       static TODO_ERRReport_ByItemNumberRepository()
        {
            instance = new TODO_ERRReport_ByItemNumberRepository();
        }

       public static TODO_ERRReport_ByItemNumberRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TODO_ERRReport_ByItemNumber>> TODO_ERRReport_ByItemNumber(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TODO_ERRReport_ByItemNumber>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_ByItemNumberClient())
                    {
                        var res = await t.GetTODO_ERRReport_ByItemNumber(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_ByItemNumber(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_ERRReport_ByItemNumber>> GetTODO_ERRReport_ByItemNumberByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_ERRReport_ByItemNumber>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_ByItemNumberClient())
                    {
					    IEnumerable<DTO.TODO_ERRReport_ByItemNumber> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTODO_ERRReport_ByItemNumber(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_ERRReport_ByItemNumberByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_ByItemNumber(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_ERRReport_ByItemNumber>> GetTODO_ERRReport_ByItemNumberByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TODO_ERRReport_ByItemNumber>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_ByItemNumberClient())
                    {
					    IEnumerable<DTO.TODO_ERRReport_ByItemNumber> res = null;
                       
                        res = await t.GetTODO_ERRReport_ByItemNumberByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_ByItemNumber(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TODO_ERRReport_ByItemNumber>> GetTODO_ERRReport_ByItemNumberByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_ERRReport_ByItemNumber>().AsEnumerable();
            try
            {
                using (var t = new TODO_ERRReport_ByItemNumberClient())
                    {
					    IEnumerable<DTO.TODO_ERRReport_ByItemNumber> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTODO_ERRReport_ByItemNumber(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_ERRReport_ByItemNumberByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_ByItemNumber(x)).AsEnumerable();
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


        public async Task<TODO_ERRReport_ByItemNumber> GetTODO_ERRReport_ByItemNumber(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TODO_ERRReport_ByItemNumberClient())
                    {
                        var res = await t.GetTODO_ERRReport_ByItemNumberByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TODO_ERRReport_ByItemNumber(res);
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

        public async Task<TODO_ERRReport_ByItemNumber> UpdateTODO_ERRReport_ByItemNumber(TODO_ERRReport_ByItemNumber entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TODO_ERRReport_ByItemNumberClient())
                    {
     
                        var updatedEntity =  await t.UpdateTODO_ERRReport_ByItemNumber(entitychanges).ConfigureAwait(false);
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

        public async Task<TODO_ERRReport_ByItemNumber> CreateTODO_ERRReport_ByItemNumber(TODO_ERRReport_ByItemNumber entity)
        {
            try
            {   
                using (var t = new TODO_ERRReport_ByItemNumberClient())
                    {
                        return new TODO_ERRReport_ByItemNumber(await t.CreateTODO_ERRReport_ByItemNumber(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTODO_ERRReport_ByItemNumber(string id)
        {
            try
            {
             using (var t = new TODO_ERRReport_ByItemNumberClient())
                {
                    return await t.DeleteTODO_ERRReport_ByItemNumber(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTODO_ERRReport_ByItemNumber(IEnumerable<string> selectedTODO_ERRReport_ByItemNumber)
        {
            try
            {
                using (var ctx = new TODO_ERRReport_ByItemNumberClient())
                {
                    return await ctx.RemoveSelectedTODO_ERRReport_ByItemNumber(selectedTODO_ERRReport_ByItemNumber).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TODO_ERRReport_ByItemNumber>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TODO_ERRReport_ByItemNumber>, int>(new List<TODO_ERRReport_ByItemNumber>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TODO_ERRReport_ByItemNumberClient())
                {

                    IEnumerable<DTO.TODO_ERRReport_ByItemNumber> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TODO_ERRReport_ByItemNumber>, int>(res.Select(x => new TODO_ERRReport_ByItemNumber(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<TODO_ERRReport_ByItemNumber>> GetTODO_ERRReport_ByItemNumberByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
             if (ApplicationSettingsId == "0") return null;
            try
            {
                 using (TODO_ERRReport_ByItemNumberClient t = new TODO_ERRReport_ByItemNumberClient())
                    {
                        var res = await t.GetTODO_ERRReport_ByItemNumberByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_ERRReport_ByItemNumber(x)).AsEnumerable();
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
                using (var t = new TODO_ERRReport_ByItemNumberClient())
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
                using (var t = new TODO_ERRReport_ByItemNumberClient())
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
