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

using AsycudaDocument_Attachments = CoreEntities.Client.Entities.AsycudaDocument_Attachments;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class AsycudaDocument_AttachmentsRepository : BaseRepository<AsycudaDocument_AttachmentsRepository>
    {

       private static readonly AsycudaDocument_AttachmentsRepository instance;
       static AsycudaDocument_AttachmentsRepository()
        {
            instance = new AsycudaDocument_AttachmentsRepository();
        }

       public static AsycudaDocument_AttachmentsRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<AsycudaDocument_Attachments>> AsycudaDocument_Attachments(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<AsycudaDocument_Attachments>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocument_AttachmentsClient())
                    {
                        var res = await t.GetAsycudaDocument_Attachments(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument_Attachments(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaDocument_Attachments>> GetAsycudaDocument_AttachmentsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaDocument_Attachments>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocument_AttachmentsClient())
                    {
					    IEnumerable<DTO.AsycudaDocument_Attachments> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetAsycudaDocument_Attachments(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaDocument_AttachmentsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument_Attachments(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaDocument_Attachments>> GetAsycudaDocument_AttachmentsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<AsycudaDocument_Attachments>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocument_AttachmentsClient())
                    {
					    IEnumerable<DTO.AsycudaDocument_Attachments> res = null;
                       
                        res = await t.GetAsycudaDocument_AttachmentsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument_Attachments(x)).AsEnumerable();
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


		 public async Task<IEnumerable<AsycudaDocument_Attachments>> GetAsycudaDocument_AttachmentsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaDocument_Attachments>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocument_AttachmentsClient())
                    {
					    IEnumerable<DTO.AsycudaDocument_Attachments> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetAsycudaDocument_Attachments(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaDocument_AttachmentsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument_Attachments(x)).AsEnumerable();
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


        public async Task<AsycudaDocument_Attachments> GetAsycudaDocument_Attachments(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new AsycudaDocument_AttachmentsClient())
                    {
                        var res = await t.GetAsycudaDocument_AttachmentsByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new AsycudaDocument_Attachments(res)
                    {
                  // Attachments = (res.Attachments != null?new Attachments(res.Attachments): null)    
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

        public async Task<AsycudaDocument_Attachments> UpdateAsycudaDocument_Attachments(AsycudaDocument_Attachments entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new AsycudaDocument_AttachmentsClient())
                    {
     
                        var updatedEntity =  await t.UpdateAsycudaDocument_Attachments(entitychanges).ConfigureAwait(false);
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

        public async Task<AsycudaDocument_Attachments> CreateAsycudaDocument_Attachments(AsycudaDocument_Attachments entity)
        {
            try
            {   
                using (var t = new AsycudaDocument_AttachmentsClient())
                    {
                        return new AsycudaDocument_Attachments(await t.CreateAsycudaDocument_Attachments(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteAsycudaDocument_Attachments(string id)
        {
            try
            {
             using (var t = new AsycudaDocument_AttachmentsClient())
                {
                    return await t.DeleteAsycudaDocument_Attachments(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedAsycudaDocument_Attachments(IEnumerable<string> selectedAsycudaDocument_Attachments)
        {
            try
            {
                using (var ctx = new AsycudaDocument_AttachmentsClient())
                {
                    return await ctx.RemoveSelectedAsycudaDocument_Attachments(selectedAsycudaDocument_Attachments).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<AsycudaDocument_Attachments>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<AsycudaDocument_Attachments>, int>(new List<AsycudaDocument_Attachments>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new AsycudaDocument_AttachmentsClient())
                {

                    IEnumerable<DTO.AsycudaDocument_Attachments> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<AsycudaDocument_Attachments>, int>(res.Select(x => new AsycudaDocument_Attachments(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<AsycudaDocument_Attachments>> GetAsycudaDocument_AttachmentsByAttachmentId(string AttachmentId, List<string> includesLst = null)
        {
             if (AttachmentId == "0") return null;
            try
            {
                 using (AsycudaDocument_AttachmentsClient t = new AsycudaDocument_AttachmentsClient())
                    {
                        var res = await t.GetAsycudaDocument_AttachmentsByAttachmentId(AttachmentId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaDocument_Attachments(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<AsycudaDocument_Attachments>> GetAsycudaDocument_AttachmentsByAsycudaDocumentId(string AsycudaDocumentId, List<string> includesLst = null)
        {
             if (AsycudaDocumentId == "0") return null;
            try
            {
                 using (AsycudaDocument_AttachmentsClient t = new AsycudaDocument_AttachmentsClient())
                    {
                        var res = await t.GetAsycudaDocument_AttachmentsByAsycudaDocumentId(AsycudaDocumentId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaDocument_Attachments(x)).AsEnumerable();
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
                using (var t = new AsycudaDocument_AttachmentsClient())
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
                using (var t = new AsycudaDocument_AttachmentsClient())
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
