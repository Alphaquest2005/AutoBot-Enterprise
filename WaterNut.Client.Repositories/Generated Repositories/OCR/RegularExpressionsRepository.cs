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

using RegularExpressions = OCR.Client.Entities.RegularExpressions;

namespace OCR.Client.Repositories 
{
   
    public partial class RegularExpressionsRepository : BaseRepository<RegularExpressionsRepository>
    {

       private static readonly RegularExpressionsRepository instance;
       static RegularExpressionsRepository()
        {
            instance = new RegularExpressionsRepository();
        }

       public static RegularExpressionsRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<RegularExpressions>> RegularExpressions(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<RegularExpressions>().AsEnumerable();
            try
            {
                using (var t = new RegularExpressionsClient())
                    {
                        var res = await t.GetRegularExpressions(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new RegularExpressions(x)).AsEnumerable();
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

		 public async Task<IEnumerable<RegularExpressions>> GetRegularExpressionsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<RegularExpressions>().AsEnumerable();
            try
            {
                using (var t = new RegularExpressionsClient())
                    {
					    IEnumerable<DTO.RegularExpressions> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetRegularExpressions(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetRegularExpressionsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new RegularExpressions(x)).AsEnumerable();
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

		 public async Task<IEnumerable<RegularExpressions>> GetRegularExpressionsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<RegularExpressions>().AsEnumerable();
            try
            {
                using (var t = new RegularExpressionsClient())
                    {
					    IEnumerable<DTO.RegularExpressions> res = null;
                       
                        res = await t.GetRegularExpressionsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new RegularExpressions(x)).AsEnumerable();
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


		 public async Task<IEnumerable<RegularExpressions>> GetRegularExpressionsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<RegularExpressions>().AsEnumerable();
            try
            {
                using (var t = new RegularExpressionsClient())
                    {
					    IEnumerable<DTO.RegularExpressions> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetRegularExpressions(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetRegularExpressionsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new RegularExpressions(x)).AsEnumerable();
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


        public async Task<RegularExpressions> GetRegularExpressions(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new RegularExpressionsClient())
                    {
                        var res = await t.GetRegularExpressionsByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new RegularExpressions(res)
                    {
                     // End = new System.Collections.ObjectModel.ObservableCollection<End>(res.End.Select(y => new End(y))),    
                     // Lines = new System.Collections.ObjectModel.ObservableCollection<Lines>(res.Lines.Select(y => new Lines(y))),    
                     // Start = new System.Collections.ObjectModel.ObservableCollection<Start>(res.Start.Select(y => new Start(y)))    
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

        public async Task<RegularExpressions> UpdateRegularExpressions(RegularExpressions entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new RegularExpressionsClient())
                    {
     
                        var updatedEntity =  await t.UpdateRegularExpressions(entitychanges).ConfigureAwait(false);
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

        public async Task<RegularExpressions> CreateRegularExpressions(RegularExpressions entity)
        {
            try
            {   
                using (var t = new RegularExpressionsClient())
                    {
                        return new RegularExpressions(await t.CreateRegularExpressions(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteRegularExpressions(string id)
        {
            try
            {
             using (var t = new RegularExpressionsClient())
                {
                    return await t.DeleteRegularExpressions(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedRegularExpressions(IEnumerable<string> selectedRegularExpressions)
        {
            try
            {
                using (var ctx = new RegularExpressionsClient())
                {
                    return await ctx.RemoveSelectedRegularExpressions(selectedRegularExpressions).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<RegularExpressions>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<RegularExpressions>, int>(new List<RegularExpressions>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new RegularExpressionsClient())
                {

                    IEnumerable<DTO.RegularExpressions> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<RegularExpressions>, int>(res.Select(x => new RegularExpressions(x)).AsEnumerable(), overallCount);
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
                using (var t = new RegularExpressionsClient())
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
                using (var t = new RegularExpressionsClient())
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
