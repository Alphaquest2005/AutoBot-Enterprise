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

using TODO_AssessDiscrepancyEntries = CoreEntities.Client.Entities.TODO_AssessDiscrepancyEntries;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class TODO_AssessDiscrepancyEntriesRepository : BaseRepository<TODO_AssessDiscrepancyEntriesRepository>
    {

       private static readonly TODO_AssessDiscrepancyEntriesRepository instance;
       static TODO_AssessDiscrepancyEntriesRepository()
        {
            instance = new TODO_AssessDiscrepancyEntriesRepository();
        }

       public static TODO_AssessDiscrepancyEntriesRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TODO_AssessDiscrepancyEntries>> TODO_AssessDiscrepancyEntries(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TODO_AssessDiscrepancyEntries>().AsEnumerable();
            try
            {
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
                    {
                        var res = await t.GetTODO_AssessDiscrepancyEntries(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TODO_AssessDiscrepancyEntries(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_AssessDiscrepancyEntries>().AsEnumerable();
            try
            {
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
                    {
					    IEnumerable<DTO.TODO_AssessDiscrepancyEntries> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTODO_AssessDiscrepancyEntries(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_AssessDiscrepancyEntriesByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_AssessDiscrepancyEntries(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TODO_AssessDiscrepancyEntries>().AsEnumerable();
            try
            {
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
                    {
					    IEnumerable<DTO.TODO_AssessDiscrepancyEntries> res = null;
                       
                        res = await t.GetTODO_AssessDiscrepancyEntriesByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_AssessDiscrepancyEntries(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TODO_AssessDiscrepancyEntries>> GetTODO_AssessDiscrepancyEntriesByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_AssessDiscrepancyEntries>().AsEnumerable();
            try
            {
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
                    {
					    IEnumerable<DTO.TODO_AssessDiscrepancyEntries> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTODO_AssessDiscrepancyEntries(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_AssessDiscrepancyEntriesByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_AssessDiscrepancyEntries(x)).AsEnumerable();
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


        public async Task<TODO_AssessDiscrepancyEntries> GetTODO_AssessDiscrepancyEntries(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TODO_AssessDiscrepancyEntriesClient())
                    {
                        var res = await t.GetTODO_AssessDiscrepancyEntriesByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TODO_AssessDiscrepancyEntries(res);
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

        public async Task<TODO_AssessDiscrepancyEntries> UpdateTODO_AssessDiscrepancyEntries(TODO_AssessDiscrepancyEntries entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TODO_AssessDiscrepancyEntriesClient())
                    {
     
                        var updatedEntity =  await t.UpdateTODO_AssessDiscrepancyEntries(entitychanges).ConfigureAwait(false);
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

        public async Task<TODO_AssessDiscrepancyEntries> CreateTODO_AssessDiscrepancyEntries(TODO_AssessDiscrepancyEntries entity)
        {
            try
            {   
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
                    {
                        return new TODO_AssessDiscrepancyEntries(await t.CreateTODO_AssessDiscrepancyEntries(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTODO_AssessDiscrepancyEntries(string id)
        {
            try
            {
             using (var t = new TODO_AssessDiscrepancyEntriesClient())
                {
                    return await t.DeleteTODO_AssessDiscrepancyEntries(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTODO_AssessDiscrepancyEntries(IEnumerable<string> selectedTODO_AssessDiscrepancyEntries)
        {
            try
            {
                using (var ctx = new TODO_AssessDiscrepancyEntriesClient())
                {
                    return await ctx.RemoveSelectedTODO_AssessDiscrepancyEntries(selectedTODO_AssessDiscrepancyEntries).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TODO_AssessDiscrepancyEntries>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TODO_AssessDiscrepancyEntries>, int>(new List<TODO_AssessDiscrepancyEntries>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
                {

                    IEnumerable<DTO.TODO_AssessDiscrepancyEntries> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TODO_AssessDiscrepancyEntries>, int>(res.Select(x => new TODO_AssessDiscrepancyEntries(x)).AsEnumerable(), overallCount);
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
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
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
                using (var t = new TODO_AssessDiscrepancyEntriesClient())
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
