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
using EntryDataQS.Client.Services;
using EntryDataQS.Client.Entities;
using EntryDataQS.Client.DTO;
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

using AsycudaDocumentItemEntryDataDetail = EntryDataQS.Client.Entities.AsycudaDocumentItemEntryDataDetail;

namespace EntryDataQS.Client.Repositories 
{
   
    public partial class AsycudaDocumentItemEntryDataDetailRepository : BaseRepository<AsycudaDocumentItemEntryDataDetailRepository>
    {

       private static readonly AsycudaDocumentItemEntryDataDetailRepository instance;
       static AsycudaDocumentItemEntryDataDetailRepository()
        {
            instance = new AsycudaDocumentItemEntryDataDetailRepository();
        }

       public static AsycudaDocumentItemEntryDataDetailRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> AsycudaDocumentItemEntryDataDetails(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<AsycudaDocumentItemEntryDataDetail>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                    {
                        var res = await t.GetAsycudaDocumentItemEntryDataDetails(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocumentItemEntryDataDetail(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaDocumentItemEntryDataDetail>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                    {
					    IEnumerable<DTO.AsycudaDocumentItemEntryDataDetail> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetAsycudaDocumentItemEntryDataDetails(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaDocumentItemEntryDataDetailsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocumentItemEntryDataDetail(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<AsycudaDocumentItemEntryDataDetail>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                    {
					    IEnumerable<DTO.AsycudaDocumentItemEntryDataDetail> res = null;
                       
                        res = await t.GetAsycudaDocumentItemEntryDataDetailsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocumentItemEntryDataDetail(x)).AsEnumerable();
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


		 public async Task<IEnumerable<AsycudaDocumentItemEntryDataDetail>> GetAsycudaDocumentItemEntryDataDetailsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaDocumentItemEntryDataDetail>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                    {
					    IEnumerable<DTO.AsycudaDocumentItemEntryDataDetail> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetAsycudaDocumentItemEntryDataDetails(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaDocumentItemEntryDataDetailsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocumentItemEntryDataDetail(x)).AsEnumerable();
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


        public async Task<AsycudaDocumentItemEntryDataDetail> GetAsycudaDocumentItemEntryDataDetail(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                    {
                        var res = await t.GetAsycudaDocumentItemEntryDataDetailByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new AsycudaDocumentItemEntryDataDetail(res);
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

        public async Task<AsycudaDocumentItemEntryDataDetail> UpdateAsycudaDocumentItemEntryDataDetail(AsycudaDocumentItemEntryDataDetail entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                    {
     
                        var updatedEntity =  await t.UpdateAsycudaDocumentItemEntryDataDetail(entitychanges).ConfigureAwait(false);
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

        public async Task<AsycudaDocumentItemEntryDataDetail> CreateAsycudaDocumentItemEntryDataDetail(AsycudaDocumentItemEntryDataDetail entity)
        {
            try
            {   
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                    {
                        return new AsycudaDocumentItemEntryDataDetail(await t.CreateAsycudaDocumentItemEntryDataDetail(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteAsycudaDocumentItemEntryDataDetail(string id)
        {
            try
            {
             using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                {
                    return await t.DeleteAsycudaDocumentItemEntryDataDetail(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedAsycudaDocumentItemEntryDataDetail(IEnumerable<string> selectedAsycudaDocumentItemEntryDataDetail)
        {
            try
            {
                using (var ctx = new AsycudaDocumentItemEntryDataDetailClient())
                {
                    return await ctx.RemoveSelectedAsycudaDocumentItemEntryDataDetail(selectedAsycudaDocumentItemEntryDataDetail).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<AsycudaDocumentItemEntryDataDetail>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<AsycudaDocumentItemEntryDataDetail>, int>(new List<AsycudaDocumentItemEntryDataDetail>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
                {

                    IEnumerable<DTO.AsycudaDocumentItemEntryDataDetail> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<AsycudaDocumentItemEntryDataDetail>, int>(res.Select(x => new AsycudaDocumentItemEntryDataDetail(x)).AsEnumerable(), overallCount);
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
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
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
                using (var t = new AsycudaDocumentItemEntryDataDetailClient())
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
