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
using OCR.Client.Services;
using OCR.Client.Entities;
using OCR.Client.DTO;
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

using RecuringPart = OCR.Client.Entities.RecuringPart;

namespace OCR.Client.Repositories 
{
   
    public partial class RecuringPartRepository : BaseRepository<RecuringPartRepository>
    {

       private static readonly RecuringPartRepository instance;
       static RecuringPartRepository()
        {
            instance = new RecuringPartRepository();
        }

       public static RecuringPartRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<RecuringPart>> RecuringPart(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<RecuringPart>().AsEnumerable();
            try
            {
                using (var t = new RecuringPartClient())
                    {
                        var res = await t.GetRecuringPart(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new RecuringPart(x)).AsEnumerable();
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

		 public async Task<IEnumerable<RecuringPart>> GetRecuringPartByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<RecuringPart>().AsEnumerable();
            try
            {
                using (var t = new RecuringPartClient())
                    {
					    IEnumerable<DTO.RecuringPart> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetRecuringPart(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetRecuringPartByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new RecuringPart(x)).AsEnumerable();
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

		 public async Task<IEnumerable<RecuringPart>> GetRecuringPartByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<RecuringPart>().AsEnumerable();
            try
            {
                using (var t = new RecuringPartClient())
                    {
					    IEnumerable<DTO.RecuringPart> res = null;
                       
                        res = await t.GetRecuringPartByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new RecuringPart(x)).AsEnumerable();
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


		 public async Task<IEnumerable<RecuringPart>> GetRecuringPartByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<RecuringPart>().AsEnumerable();
            try
            {
                using (var t = new RecuringPartClient())
                    {
					    IEnumerable<DTO.RecuringPart> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetRecuringPart(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetRecuringPartByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new RecuringPart(x)).AsEnumerable();
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


        public async Task<RecuringPart> GetRecuringPart(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new RecuringPartClient())
                    {
                        var res = await t.GetRecuringPartByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new RecuringPart(res)
                    {
                  // Parts = (res.Parts != null?new Parts(res.Parts): null)    
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

        public async Task<RecuringPart> UpdateRecuringPart(RecuringPart entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new RecuringPartClient())
                    {
     
                        var updatedEntity =  await t.UpdateRecuringPart(entitychanges).ConfigureAwait(false);
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

        public async Task<RecuringPart> CreateRecuringPart(RecuringPart entity)
        {
            try
            {   
                using (var t = new RecuringPartClient())
                    {
                        return new RecuringPart(await t.CreateRecuringPart(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteRecuringPart(string id)
        {
            try
            {
             using (var t = new RecuringPartClient())
                {
                    return await t.DeleteRecuringPart(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedRecuringPart(IEnumerable<string> selectedRecuringPart)
        {
            try
            {
                using (var ctx = new RecuringPartClient())
                {
                    return await ctx.RemoveSelectedRecuringPart(selectedRecuringPart).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<RecuringPart>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<RecuringPart>, int>(new List<RecuringPart>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new RecuringPartClient())
                {

                    IEnumerable<DTO.RecuringPart> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<RecuringPart>, int>(res.Select(x => new RecuringPart(x)).AsEnumerable(), overallCount);
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
                using (var t = new RecuringPartClient())
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
                using (var t = new RecuringPartClient())
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
