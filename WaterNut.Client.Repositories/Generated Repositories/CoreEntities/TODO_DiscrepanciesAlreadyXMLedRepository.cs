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

using TODO_DiscrepanciesAlreadyXMLed = CoreEntities.Client.Entities.TODO_DiscrepanciesAlreadyXMLed;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class TODO_DiscrepanciesAlreadyXMLedRepository : BaseRepository<TODO_DiscrepanciesAlreadyXMLedRepository>
    {

       private static readonly TODO_DiscrepanciesAlreadyXMLedRepository instance;
       static TODO_DiscrepanciesAlreadyXMLedRepository()
        {
            instance = new TODO_DiscrepanciesAlreadyXMLedRepository();
        }

       public static TODO_DiscrepanciesAlreadyXMLedRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> TODO_DiscrepanciesAlreadyXMLed(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TODO_DiscrepanciesAlreadyXMLed>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesAlreadyXMLed(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesAlreadyXMLed(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_DiscrepanciesAlreadyXMLed>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesAlreadyXMLed> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTODO_DiscrepanciesAlreadyXMLed(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_DiscrepanciesAlreadyXMLedByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesAlreadyXMLed(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TODO_DiscrepanciesAlreadyXMLed>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesAlreadyXMLed> res = null;
                       
                        res = await t.GetTODO_DiscrepanciesAlreadyXMLedByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesAlreadyXMLed(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_DiscrepanciesAlreadyXMLed>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesAlreadyXMLed> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTODO_DiscrepanciesAlreadyXMLed(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_DiscrepanciesAlreadyXMLedByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesAlreadyXMLed(x)).AsEnumerable();
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


        public async Task<TODO_DiscrepanciesAlreadyXMLed> GetTODO_DiscrepanciesAlreadyXMLed(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesAlreadyXMLedByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TODO_DiscrepanciesAlreadyXMLed(res);
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

        public async Task<TODO_DiscrepanciesAlreadyXMLed> UpdateTODO_DiscrepanciesAlreadyXMLed(TODO_DiscrepanciesAlreadyXMLed entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
     
                        var updatedEntity =  await t.UpdateTODO_DiscrepanciesAlreadyXMLed(entitychanges).ConfigureAwait(false);
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

        public async Task<TODO_DiscrepanciesAlreadyXMLed> CreateTODO_DiscrepanciesAlreadyXMLed(TODO_DiscrepanciesAlreadyXMLed entity)
        {
            try
            {   
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
                        return new TODO_DiscrepanciesAlreadyXMLed(await t.CreateTODO_DiscrepanciesAlreadyXMLed(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTODO_DiscrepanciesAlreadyXMLed(string id)
        {
            try
            {
             using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                {
                    return await t.DeleteTODO_DiscrepanciesAlreadyXMLed(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTODO_DiscrepanciesAlreadyXMLed(IEnumerable<string> selectedTODO_DiscrepanciesAlreadyXMLed)
        {
            try
            {
                using (var ctx = new TODO_DiscrepanciesAlreadyXMLedClient())
                {
                    return await ctx.RemoveSelectedTODO_DiscrepanciesAlreadyXMLed(selectedTODO_DiscrepanciesAlreadyXMLed).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>, int>(new List<TODO_DiscrepanciesAlreadyXMLed>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
                {

                    IEnumerable<DTO.TODO_DiscrepanciesAlreadyXMLed> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>, int>(res.Select(x => new TODO_DiscrepanciesAlreadyXMLed(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByEmailId(string EmailId, List<string> includesLst = null)
        {
             if (EmailId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesAlreadyXMLedClient t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesAlreadyXMLedByEmailId(EmailId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesAlreadyXMLed(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<TODO_DiscrepanciesAlreadyXMLed>> GetTODO_DiscrepanciesAlreadyXMLedByFileTypeId(string FileTypeId, List<string> includesLst = null)
        {
             if (FileTypeId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesAlreadyXMLedClient t = new TODO_DiscrepanciesAlreadyXMLedClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesAlreadyXMLedByFileTypeId(FileTypeId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesAlreadyXMLed(x)).AsEnumerable();
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
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
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
                using (var t = new TODO_DiscrepanciesAlreadyXMLedClient())
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

