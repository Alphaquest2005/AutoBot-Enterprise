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

using TODO_DiscrepanciesToSubmit = CoreEntities.Client.Entities.TODO_DiscrepanciesToSubmit;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class TODO_DiscrepanciesToSubmitRepository : BaseRepository<TODO_DiscrepanciesToSubmitRepository>
    {

       private static readonly TODO_DiscrepanciesToSubmitRepository instance;
       static TODO_DiscrepanciesToSubmitRepository()
        {
            instance = new TODO_DiscrepanciesToSubmitRepository();
        }

       public static TODO_DiscrepanciesToSubmitRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TODO_DiscrepanciesToSubmit>> TODO_DiscrepanciesToSubmit(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TODO_DiscrepanciesToSubmit>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesToSubmitClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesToSubmit(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesToSubmit(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_DiscrepanciesToSubmit>> GetTODO_DiscrepanciesToSubmitByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_DiscrepanciesToSubmit>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesToSubmitClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesToSubmit> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTODO_DiscrepanciesToSubmit(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_DiscrepanciesToSubmitByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesToSubmit(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_DiscrepanciesToSubmit>> GetTODO_DiscrepanciesToSubmitByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TODO_DiscrepanciesToSubmit>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesToSubmitClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesToSubmit> res = null;
                       
                        res = await t.GetTODO_DiscrepanciesToSubmitByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesToSubmit(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TODO_DiscrepanciesToSubmit>> GetTODO_DiscrepanciesToSubmitByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_DiscrepanciesToSubmit>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesToSubmitClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesToSubmit> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTODO_DiscrepanciesToSubmit(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_DiscrepanciesToSubmitByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesToSubmit(x)).AsEnumerable();
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


        public async Task<TODO_DiscrepanciesToSubmit> GetTODO_DiscrepanciesToSubmit(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TODO_DiscrepanciesToSubmitClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesToSubmitByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TODO_DiscrepanciesToSubmit(res);
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

        public async Task<TODO_DiscrepanciesToSubmit> UpdateTODO_DiscrepanciesToSubmit(TODO_DiscrepanciesToSubmit entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TODO_DiscrepanciesToSubmitClient())
                    {
     
                        var updatedEntity =  await t.UpdateTODO_DiscrepanciesToSubmit(entitychanges).ConfigureAwait(false);
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

        public async Task<TODO_DiscrepanciesToSubmit> CreateTODO_DiscrepanciesToSubmit(TODO_DiscrepanciesToSubmit entity)
        {
            try
            {   
                using (var t = new TODO_DiscrepanciesToSubmitClient())
                    {
                        return new TODO_DiscrepanciesToSubmit(await t.CreateTODO_DiscrepanciesToSubmit(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTODO_DiscrepanciesToSubmit(string id)
        {
            try
            {
             using (var t = new TODO_DiscrepanciesToSubmitClient())
                {
                    return await t.DeleteTODO_DiscrepanciesToSubmit(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTODO_DiscrepanciesToSubmit(IEnumerable<string> selectedTODO_DiscrepanciesToSubmit)
        {
            try
            {
                using (var ctx = new TODO_DiscrepanciesToSubmitClient())
                {
                    return await ctx.RemoveSelectedTODO_DiscrepanciesToSubmit(selectedTODO_DiscrepanciesToSubmit).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TODO_DiscrepanciesToSubmit>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TODO_DiscrepanciesToSubmit>, int>(new List<TODO_DiscrepanciesToSubmit>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TODO_DiscrepanciesToSubmitClient())
                {

                    IEnumerable<DTO.TODO_DiscrepanciesToSubmit> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TODO_DiscrepanciesToSubmit>, int>(res.Select(x => new TODO_DiscrepanciesToSubmit(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<TODO_DiscrepanciesToSubmit>> GetTODO_DiscrepanciesToSubmitByEmailId(string EmailId, List<string> includesLst = null)
        {
             if (EmailId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesToSubmitClient t = new TODO_DiscrepanciesToSubmitClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesToSubmitByEmailId(EmailId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesToSubmit(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<TODO_DiscrepanciesToSubmit>> GetTODO_DiscrepanciesToSubmitByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
             if (FileTypeId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesToSubmitClient t = new TODO_DiscrepanciesToSubmitClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesToSubmitByFileTypeId(FileTypeId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesToSubmit(x)).AsEnumerable();
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
                using (var t = new TODO_DiscrepanciesToSubmitClient())
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
                using (var t = new TODO_DiscrepanciesToSubmitClient())
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
