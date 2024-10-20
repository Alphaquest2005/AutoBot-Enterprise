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

using TariffCategoryCodeSuppUnit = InventoryQS.Client.Entities.TariffCategoryCodeSuppUnit;

namespace InventoryQS.Client.Repositories 
{
   
    public partial class TariffCategoryCodeSuppUnitRepository : BaseRepository<TariffCategoryCodeSuppUnitRepository>
    {

       private static readonly TariffCategoryCodeSuppUnitRepository instance;
       static TariffCategoryCodeSuppUnitRepository()
        {
            instance = new TariffCategoryCodeSuppUnitRepository();
        }

       public static TariffCategoryCodeSuppUnitRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TariffCategoryCodeSuppUnit>> TariffCategoryCodeSuppUnit(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TariffCategoryCodeSuppUnit>().AsEnumerable();
            try
            {
                using (var t = new TariffCategoryCodeSuppUnitClient())
                    {
                        var res = await t.GetTariffCategoryCodeSuppUnit(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TariffCategoryCodeSuppUnit(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TariffCategoryCodeSuppUnit>> GetTariffCategoryCodeSuppUnitByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TariffCategoryCodeSuppUnit>().AsEnumerable();
            try
            {
                using (var t = new TariffCategoryCodeSuppUnitClient())
                    {
					    IEnumerable<DTO.TariffCategoryCodeSuppUnit> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTariffCategoryCodeSuppUnit(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTariffCategoryCodeSuppUnitByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TariffCategoryCodeSuppUnit(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TariffCategoryCodeSuppUnit>> GetTariffCategoryCodeSuppUnitByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TariffCategoryCodeSuppUnit>().AsEnumerable();
            try
            {
                using (var t = new TariffCategoryCodeSuppUnitClient())
                    {
					    IEnumerable<DTO.TariffCategoryCodeSuppUnit> res = null;
                       
                        res = await t.GetTariffCategoryCodeSuppUnitByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TariffCategoryCodeSuppUnit(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TariffCategoryCodeSuppUnit>> GetTariffCategoryCodeSuppUnitByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TariffCategoryCodeSuppUnit>().AsEnumerable();
            try
            {
                using (var t = new TariffCategoryCodeSuppUnitClient())
                    {
					    IEnumerable<DTO.TariffCategoryCodeSuppUnit> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTariffCategoryCodeSuppUnit(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTariffCategoryCodeSuppUnitByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TariffCategoryCodeSuppUnit(x)).AsEnumerable();
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


        public async Task<TariffCategoryCodeSuppUnit> GetTariffCategoryCodeSuppUnit(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TariffCategoryCodeSuppUnitClient())
                    {
                        var res = await t.GetTariffCategoryCodeSuppUnitByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TariffCategoryCodeSuppUnit(res)
                    {
                  // TariffCategory = (res.TariffCategory != null?new TariffCategory(res.TariffCategory): null),    
                  // TariffSupUnitLkps = (res.TariffSupUnitLkps != null?new TariffSupUnitLkps(res.TariffSupUnitLkps): null)    
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

        public async Task<TariffCategoryCodeSuppUnit> UpdateTariffCategoryCodeSuppUnit(TariffCategoryCodeSuppUnit entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TariffCategoryCodeSuppUnitClient())
                    {
     
                        var updatedEntity =  await t.UpdateTariffCategoryCodeSuppUnit(entitychanges).ConfigureAwait(false);
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

        public async Task<TariffCategoryCodeSuppUnit> CreateTariffCategoryCodeSuppUnit(TariffCategoryCodeSuppUnit entity)
        {
            try
            {   
                using (var t = new TariffCategoryCodeSuppUnitClient())
                    {
                        return new TariffCategoryCodeSuppUnit(await t.CreateTariffCategoryCodeSuppUnit(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTariffCategoryCodeSuppUnit(string id)
        {
            try
            {
             using (var t = new TariffCategoryCodeSuppUnitClient())
                {
                    return await t.DeleteTariffCategoryCodeSuppUnit(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTariffCategoryCodeSuppUnit(IEnumerable<string> selectedTariffCategoryCodeSuppUnit)
        {
            try
            {
                using (var ctx = new TariffCategoryCodeSuppUnitClient())
                {
                    return await ctx.RemoveSelectedTariffCategoryCodeSuppUnit(selectedTariffCategoryCodeSuppUnit).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TariffCategoryCodeSuppUnit>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TariffCategoryCodeSuppUnit>, int>(new List<TariffCategoryCodeSuppUnit>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TariffCategoryCodeSuppUnitClient())
                {

                    IEnumerable<DTO.TariffCategoryCodeSuppUnit> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TariffCategoryCodeSuppUnit>, int>(res.Select(x => new TariffCategoryCodeSuppUnit(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<TariffCategoryCodeSuppUnit>> GetTariffCategoryCodeSuppUnitByTariffSupUnitId(string TariffSupUnitId, List<string> includesLst = null)
        {
             if (TariffSupUnitId == "0") return null;
            try
            {
                 using (TariffCategoryCodeSuppUnitClient t = new TariffCategoryCodeSuppUnitClient())
                    {
                        var res = await t.GetTariffCategoryCodeSuppUnitByTariffSupUnitId(TariffSupUnitId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TariffCategoryCodeSuppUnit(x)).AsEnumerable();
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
                using (var t = new TariffCategoryCodeSuppUnitClient())
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
                using (var t = new TariffCategoryCodeSuppUnitClient())
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

