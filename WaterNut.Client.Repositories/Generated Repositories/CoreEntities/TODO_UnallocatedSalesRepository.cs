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

using TODO_UnallocatedSales = CoreEntities.Client.Entities.TODO_UnallocatedSales;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class TODO_UnallocatedSalesRepository : BaseRepository<TODO_UnallocatedSalesRepository>
    {

       private static readonly TODO_UnallocatedSalesRepository instance;
       static TODO_UnallocatedSalesRepository()
        {
            instance = new TODO_UnallocatedSalesRepository();
        }

       public static TODO_UnallocatedSalesRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TODO_UnallocatedSales>> TODO_UnallocatedSales(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TODO_UnallocatedSales>().AsEnumerable();
            try
            {
                using (var t = new TODO_UnallocatedSalesClient())
                    {
                        var res = await t.GetTODO_UnallocatedSales(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TODO_UnallocatedSales(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_UnallocatedSales>().AsEnumerable();
            try
            {
                using (var t = new TODO_UnallocatedSalesClient())
                    {
					    IEnumerable<DTO.TODO_UnallocatedSales> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTODO_UnallocatedSales(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_UnallocatedSalesByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_UnallocatedSales(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TODO_UnallocatedSales>().AsEnumerable();
            try
            {
                using (var t = new TODO_UnallocatedSalesClient())
                    {
					    IEnumerable<DTO.TODO_UnallocatedSales> res = null;
                       
                        res = await t.GetTODO_UnallocatedSalesByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_UnallocatedSales(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TODO_UnallocatedSales>> GetTODO_UnallocatedSalesByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_UnallocatedSales>().AsEnumerable();
            try
            {
                using (var t = new TODO_UnallocatedSalesClient())
                    {
					    IEnumerable<DTO.TODO_UnallocatedSales> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTODO_UnallocatedSales(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_UnallocatedSalesByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_UnallocatedSales(x)).AsEnumerable();
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


        public async Task<TODO_UnallocatedSales> GetTODO_UnallocatedSales(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TODO_UnallocatedSalesClient())
                    {
                        var res = await t.GetTODO_UnallocatedSalesByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TODO_UnallocatedSales(res);
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

        public async Task<TODO_UnallocatedSales> UpdateTODO_UnallocatedSales(TODO_UnallocatedSales entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TODO_UnallocatedSalesClient())
                    {
     
                        var updatedEntity =  await t.UpdateTODO_UnallocatedSales(entitychanges).ConfigureAwait(false);
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

        public async Task<TODO_UnallocatedSales> CreateTODO_UnallocatedSales(TODO_UnallocatedSales entity)
        {
            try
            {   
                using (var t = new TODO_UnallocatedSalesClient())
                    {
                        return new TODO_UnallocatedSales(await t.CreateTODO_UnallocatedSales(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTODO_UnallocatedSales(string id)
        {
            try
            {
             using (var t = new TODO_UnallocatedSalesClient())
                {
                    return await t.DeleteTODO_UnallocatedSales(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTODO_UnallocatedSales(IEnumerable<string> selectedTODO_UnallocatedSales)
        {
            try
            {
                using (var ctx = new TODO_UnallocatedSalesClient())
                {
                    return await ctx.RemoveSelectedTODO_UnallocatedSales(selectedTODO_UnallocatedSales).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TODO_UnallocatedSales>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TODO_UnallocatedSales>, int>(new List<TODO_UnallocatedSales>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TODO_UnallocatedSalesClient())
                {

                    IEnumerable<DTO.TODO_UnallocatedSales> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TODO_UnallocatedSales>, int>(res.Select(x => new TODO_UnallocatedSales(x)).AsEnumerable(), overallCount);
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
                using (var t = new TODO_UnallocatedSalesClient())
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
                using (var t = new TODO_UnallocatedSalesClient())
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
