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
using InventoryQS.Client.Services;
using InventoryQS.Client.Entities;
using InventoryQS.Client.DTO;
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

using TariffSupUnitLkps = InventoryQS.Client.Entities.TariffSupUnitLkps;

namespace InventoryQS.Client.Repositories 
{
   
    public partial class TariffSupUnitLkpsRepository : BaseRepository<TariffSupUnitLkpsRepository>
    {

       private static readonly TariffSupUnitLkpsRepository instance;
       static TariffSupUnitLkpsRepository()
        {
            instance = new TariffSupUnitLkpsRepository();
        }

       public static TariffSupUnitLkpsRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TariffSupUnitLkps>> TariffSupUnitLkps(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TariffSupUnitLkps>().AsEnumerable();
            try
            {
                using (var t = new TariffSupUnitLkpsClient())
                    {
                        var res = await t.GetTariffSupUnitLkps(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TariffSupUnitLkps(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TariffSupUnitLkps>> GetTariffSupUnitLkpsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TariffSupUnitLkps>().AsEnumerable();
            try
            {
                using (var t = new TariffSupUnitLkpsClient())
                    {
					    IEnumerable<DTO.TariffSupUnitLkps> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTariffSupUnitLkps(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTariffSupUnitLkpsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TariffSupUnitLkps(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TariffSupUnitLkps>> GetTariffSupUnitLkpsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TariffSupUnitLkps>().AsEnumerable();
            try
            {
                using (var t = new TariffSupUnitLkpsClient())
                    {
					    IEnumerable<DTO.TariffSupUnitLkps> res = null;
                       
                        res = await t.GetTariffSupUnitLkpsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TariffSupUnitLkps(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TariffSupUnitLkps>> GetTariffSupUnitLkpsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TariffSupUnitLkps>().AsEnumerable();
            try
            {
                using (var t = new TariffSupUnitLkpsClient())
                    {
					    IEnumerable<DTO.TariffSupUnitLkps> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTariffSupUnitLkps(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTariffSupUnitLkpsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TariffSupUnitLkps(x)).AsEnumerable();
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


        public async Task<TariffSupUnitLkps> GetTariffSupUnitLkps(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TariffSupUnitLkpsClient())
                    {
                        var res = await t.GetTariffSupUnitLkpsByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TariffSupUnitLkps(res)
                    {
                     // TariffCategoryCodeSuppUnit = new System.Collections.ObjectModel.ObservableCollection<TariffCategoryCodeSuppUnit>(res.TariffCategoryCodeSuppUnit.Select(y => new TariffCategoryCodeSuppUnit(y)))    
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

        public async Task<TariffSupUnitLkps> UpdateTariffSupUnitLkps(TariffSupUnitLkps entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TariffSupUnitLkpsClient())
                    {
     
                        var updatedEntity =  await t.UpdateTariffSupUnitLkps(entitychanges).ConfigureAwait(false);
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

        public async Task<TariffSupUnitLkps> CreateTariffSupUnitLkps(TariffSupUnitLkps entity)
        {
            try
            {   
                using (var t = new TariffSupUnitLkpsClient())
                    {
                        return new TariffSupUnitLkps(await t.CreateTariffSupUnitLkps(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTariffSupUnitLkps(string id)
        {
            try
            {
             using (var t = new TariffSupUnitLkpsClient())
                {
                    return await t.DeleteTariffSupUnitLkps(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTariffSupUnitLkps(IEnumerable<string> selectedTariffSupUnitLkps)
        {
            try
            {
                using (var ctx = new TariffSupUnitLkpsClient())
                {
                    return await ctx.RemoveSelectedTariffSupUnitLkps(selectedTariffSupUnitLkps).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TariffSupUnitLkps>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TariffSupUnitLkps>, int>(new List<TariffSupUnitLkps>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TariffSupUnitLkpsClient())
                {

                    IEnumerable<DTO.TariffSupUnitLkps> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TariffSupUnitLkps>, int>(res.Select(x => new TariffSupUnitLkps(x)).AsEnumerable(), overallCount);
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
                using (var t = new TariffSupUnitLkpsClient())
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
                using (var t = new TariffSupUnitLkpsClient())
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

