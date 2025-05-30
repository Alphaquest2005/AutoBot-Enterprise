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
using AdjustmentQS.Client.Services;
using AdjustmentQS.Client.Entities;
using AdjustmentQS.Client.DTO;
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

using AsycudaSalesAllocation = AdjustmentQS.Client.Entities.AsycudaSalesAllocation;

namespace AdjustmentQS.Client.Repositories 
{
   
    public partial class AsycudaSalesAllocationRepository : BaseRepository<AsycudaSalesAllocationRepository>
    {

       private static readonly AsycudaSalesAllocationRepository instance;
       static AsycudaSalesAllocationRepository()
        {
            instance = new AsycudaSalesAllocationRepository();
        }

       public static AsycudaSalesAllocationRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<AsycudaSalesAllocation>> AsycudaSalesAllocations(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<AsycudaSalesAllocation>().AsEnumerable();
            try
            {
                using (var t = new AsycudaSalesAllocationClient())
                    {
                        var res = await t.GetAsycudaSalesAllocations(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaSalesAllocation>> GetAsycudaSalesAllocationsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaSalesAllocation>().AsEnumerable();
            try
            {
                using (var t = new AsycudaSalesAllocationClient())
                    {
					    IEnumerable<DTO.AsycudaSalesAllocation> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetAsycudaSalesAllocations(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaSalesAllocationsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaSalesAllocation>> GetAsycudaSalesAllocationsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<AsycudaSalesAllocation>().AsEnumerable();
            try
            {
                using (var t = new AsycudaSalesAllocationClient())
                    {
					    IEnumerable<DTO.AsycudaSalesAllocation> res = null;
                       
                        res = await t.GetAsycudaSalesAllocationsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable();
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


		 public async Task<IEnumerable<AsycudaSalesAllocation>> GetAsycudaSalesAllocationsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaSalesAllocation>().AsEnumerable();
            try
            {
                using (var t = new AsycudaSalesAllocationClient())
                    {
					    IEnumerable<DTO.AsycudaSalesAllocation> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetAsycudaSalesAllocations(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaSalesAllocationsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable();
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


        public async Task<AsycudaSalesAllocation> GetAsycudaSalesAllocation(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new AsycudaSalesAllocationClient())
                    {
                        var res = await t.GetAsycudaSalesAllocationByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new AsycudaSalesAllocation(res)
                    {
                  // EntryDataDetail = (res.EntryDataDetail != null?new EntryDataDetail(res.EntryDataDetail): null),    
                  // xcuda_Item = (res.xcuda_Item != null?new xcuda_Item(res.xcuda_Item): null)    
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

        public async Task<AsycudaSalesAllocation> UpdateAsycudaSalesAllocation(AsycudaSalesAllocation entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new AsycudaSalesAllocationClient())
                    {
     
                        var updatedEntity =  await t.UpdateAsycudaSalesAllocation(entitychanges).ConfigureAwait(false);
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

        public async Task<AsycudaSalesAllocation> CreateAsycudaSalesAllocation(AsycudaSalesAllocation entity)
        {
            try
            {   
                using (var t = new AsycudaSalesAllocationClient())
                    {
                        return new AsycudaSalesAllocation(await t.CreateAsycudaSalesAllocation(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteAsycudaSalesAllocation(string id)
        {
            try
            {
             using (var t = new AsycudaSalesAllocationClient())
                {
                    return await t.DeleteAsycudaSalesAllocation(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedAsycudaSalesAllocation(IEnumerable<string> selectedAsycudaSalesAllocation)
        {
            try
            {
                using (var ctx = new AsycudaSalesAllocationClient())
                {
                    return await ctx.RemoveSelectedAsycudaSalesAllocation(selectedAsycudaSalesAllocation).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<AsycudaSalesAllocation>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<AsycudaSalesAllocation>, int>(new List<AsycudaSalesAllocation>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new AsycudaSalesAllocationClient())
                {

                    IEnumerable<DTO.AsycudaSalesAllocation> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<AsycudaSalesAllocation>, int>(res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<AsycudaSalesAllocation>> GetAsycudaSalesAllocationByEntryDataDetailsId(string EntryDataDetailsId, List<string> includesLst = null)
        {
             if (EntryDataDetailsId == "0") return null;
            try
            {
                 using (AsycudaSalesAllocationClient t = new AsycudaSalesAllocationClient())
                    {
                        var res = await t.GetAsycudaSalesAllocationByEntryDataDetailsId(EntryDataDetailsId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<AsycudaSalesAllocation>> GetAsycudaSalesAllocationByPreviousItem_Id(string PreviousItem_Id, List<string> includesLst = null)
        {
             if (PreviousItem_Id == "0") return null;
            try
            {
                 using (AsycudaSalesAllocationClient t = new AsycudaSalesAllocationClient())
                    {
                        var res = await t.GetAsycudaSalesAllocationByPreviousItem_Id(PreviousItem_Id, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<AsycudaSalesAllocation>> GetAsycudaSalesAllocationByxEntryItem_Id(string xEntryItem_Id, List<string> includesLst = null)
        {
             if (xEntryItem_Id == "0") return null;
            try
            {
                 using (AsycudaSalesAllocationClient t = new AsycudaSalesAllocationClient())
                    {
                        var res = await t.GetAsycudaSalesAllocationByxEntryItem_Id(xEntryItem_Id, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaSalesAllocation(x)).AsEnumerable();
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
                using (var t = new AsycudaSalesAllocationClient())
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
                using (var t = new AsycudaSalesAllocationClient())
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

