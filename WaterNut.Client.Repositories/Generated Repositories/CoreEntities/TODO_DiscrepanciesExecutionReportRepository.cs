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

using TODO_DiscrepanciesExecutionReport = CoreEntities.Client.Entities.TODO_DiscrepanciesExecutionReport;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class TODO_DiscrepanciesExecutionReportRepository : BaseRepository<TODO_DiscrepanciesExecutionReportRepository>
    {

       private static readonly TODO_DiscrepanciesExecutionReportRepository instance;
       static TODO_DiscrepanciesExecutionReportRepository()
        {
            instance = new TODO_DiscrepanciesExecutionReportRepository();
        }

       public static TODO_DiscrepanciesExecutionReportRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> TODO_DiscrepanciesExecutionReport(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<TODO_DiscrepanciesExecutionReport>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesExecutionReport(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_DiscrepanciesExecutionReport>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesExecutionReport> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetTODO_DiscrepanciesExecutionReport(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_DiscrepanciesExecutionReportByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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

		 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<TODO_DiscrepanciesExecutionReport>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesExecutionReport> res = null;
                       
                        res = await t.GetTODO_DiscrepanciesExecutionReportByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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


		 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<TODO_DiscrepanciesExecutionReport>().AsEnumerable();
            try
            {
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
                    {
					    IEnumerable<DTO.TODO_DiscrepanciesExecutionReport> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetTODO_DiscrepanciesExecutionReport(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetTODO_DiscrepanciesExecutionReportByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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


        public async Task<TODO_DiscrepanciesExecutionReport> GetTODO_DiscrepanciesExecutionReport(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesExecutionReportByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new TODO_DiscrepanciesExecutionReport(res);
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

        public async Task<TODO_DiscrepanciesExecutionReport> UpdateTODO_DiscrepanciesExecutionReport(TODO_DiscrepanciesExecutionReport entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new TODO_DiscrepanciesExecutionReportClient())
                    {
     
                        var updatedEntity =  await t.UpdateTODO_DiscrepanciesExecutionReport(entitychanges).ConfigureAwait(false);
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

        public async Task<TODO_DiscrepanciesExecutionReport> CreateTODO_DiscrepanciesExecutionReport(TODO_DiscrepanciesExecutionReport entity)
        {
            try
            {   
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        return new TODO_DiscrepanciesExecutionReport(await t.CreateTODO_DiscrepanciesExecutionReport(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteTODO_DiscrepanciesExecutionReport(string id)
        {
            try
            {
             using (var t = new TODO_DiscrepanciesExecutionReportClient())
                {
                    return await t.DeleteTODO_DiscrepanciesExecutionReport(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedTODO_DiscrepanciesExecutionReport(IEnumerable<string> selectedTODO_DiscrepanciesExecutionReport)
        {
            try
            {
                using (var ctx = new TODO_DiscrepanciesExecutionReportClient())
                {
                    return await ctx.RemoveSelectedTODO_DiscrepanciesExecutionReport(selectedTODO_DiscrepanciesExecutionReport).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<TODO_DiscrepanciesExecutionReport>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<TODO_DiscrepanciesExecutionReport>, int>(new List<TODO_DiscrepanciesExecutionReport>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
                {

                    IEnumerable<DTO.TODO_DiscrepanciesExecutionReport> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<TODO_DiscrepanciesExecutionReport>, int>(res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null)
        {
             if (EntryDataDetailsId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesExecutionReportClient t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesExecutionReportByEntryDataDetailsId(EntryDataDetailsId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
             if (ApplicationSettingsId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesExecutionReportClient t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesExecutionReportByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null)
        {
             if (AsycudaDocumentSetId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesExecutionReportClient t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesExecutionReportByAsycudaDocumentSetId(AsycudaDocumentSetId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByEmailId(string EmailId, List<string> includesLst = null)
        {
             if (EmailId == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesExecutionReportClient t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesExecutionReportByEmailId(EmailId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<TODO_DiscrepanciesExecutionReport>> GetTODO_DiscrepanciesExecutionReportByASYCUDA_Id(string ASYCUDA_Id, List<string> includesLst = null)
        {
             if (ASYCUDA_Id == "0") return null;
            try
            {
                 using (TODO_DiscrepanciesExecutionReportClient t = new TODO_DiscrepanciesExecutionReportClient())
                    {
                        var res = await t.GetTODO_DiscrepanciesExecutionReportByASYCUDA_Id(ASYCUDA_Id, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new TODO_DiscrepanciesExecutionReport(x)).AsEnumerable();
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
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
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
                using (var t = new TODO_DiscrepanciesExecutionReportClient())
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

