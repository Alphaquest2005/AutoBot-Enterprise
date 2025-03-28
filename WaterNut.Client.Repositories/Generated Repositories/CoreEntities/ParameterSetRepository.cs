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

using ParameterSet = CoreEntities.Client.Entities.ParameterSet;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class ParameterSetRepository : BaseRepository<ParameterSetRepository>
    {

       private static readonly ParameterSetRepository instance;
       static ParameterSetRepository()
        {
            instance = new ParameterSetRepository();
        }

       public static ParameterSetRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<ParameterSet>> ParameterSet(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<ParameterSet>().AsEnumerable();
            try
            {
                using (var t = new ParameterSetClient())
                    {
                        var res = await t.GetParameterSet(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new ParameterSet(x)).AsEnumerable();
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

		 public async Task<IEnumerable<ParameterSet>> GetParameterSetByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<ParameterSet>().AsEnumerable();
            try
            {
                using (var t = new ParameterSetClient())
                    {
					    IEnumerable<DTO.ParameterSet> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetParameterSet(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetParameterSetByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new ParameterSet(x)).AsEnumerable();
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

		 public async Task<IEnumerable<ParameterSet>> GetParameterSetByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<ParameterSet>().AsEnumerable();
            try
            {
                using (var t = new ParameterSetClient())
                    {
					    IEnumerable<DTO.ParameterSet> res = null;
                       
                        res = await t.GetParameterSetByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new ParameterSet(x)).AsEnumerable();
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


		 public async Task<IEnumerable<ParameterSet>> GetParameterSetByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<ParameterSet>().AsEnumerable();
            try
            {
                using (var t = new ParameterSetClient())
                    {
					    IEnumerable<DTO.ParameterSet> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetParameterSet(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetParameterSetByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new ParameterSet(x)).AsEnumerable();
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


        public async Task<ParameterSet> GetParameterSet(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new ParameterSetClient())
                    {
                        var res = await t.GetParameterSetByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new ParameterSet(res)
                    {
                     // ParameterSetParameters = new System.Collections.ObjectModel.ObservableCollection<ParameterSetParameters>(res.ParameterSetParameters.Select(y => new ParameterSetParameters(y))),    
                     // SessionSchedule = new System.Collections.ObjectModel.ObservableCollection<SessionSchedule>(res.SessionSchedule.Select(y => new SessionSchedule(y)))    
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

        public async Task<ParameterSet> UpdateParameterSet(ParameterSet entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new ParameterSetClient())
                    {
     
                        var updatedEntity =  await t.UpdateParameterSet(entitychanges).ConfigureAwait(false);
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

        public async Task<ParameterSet> CreateParameterSet(ParameterSet entity)
        {
            try
            {   
                using (var t = new ParameterSetClient())
                    {
                        return new ParameterSet(await t.CreateParameterSet(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteParameterSet(string id)
        {
            try
            {
             using (var t = new ParameterSetClient())
                {
                    return await t.DeleteParameterSet(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedParameterSet(IEnumerable<string> selectedParameterSet)
        {
            try
            {
                using (var ctx = new ParameterSetClient())
                {
                    return await ctx.RemoveSelectedParameterSet(selectedParameterSet).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<ParameterSet>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<ParameterSet>, int>(new List<ParameterSet>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new ParameterSetClient())
                {

                    IEnumerable<DTO.ParameterSet> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<ParameterSet>, int>(res.Select(x => new ParameterSet(x)).AsEnumerable(), overallCount);
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
                using (var t = new ParameterSetClient())
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
                using (var t = new ParameterSetClient())
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

