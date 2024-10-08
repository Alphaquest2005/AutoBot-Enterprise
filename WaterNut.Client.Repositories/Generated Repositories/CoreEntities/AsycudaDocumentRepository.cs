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

using AsycudaDocument = CoreEntities.Client.Entities.AsycudaDocument;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class AsycudaDocumentRepository : BaseRepository<AsycudaDocumentRepository>
    {

       private static readonly AsycudaDocumentRepository instance;
       static AsycudaDocumentRepository()
        {
            instance = new AsycudaDocumentRepository();
        }

       public static AsycudaDocumentRepository Instance
        {
            get { return instance; }
        }
        
        public async Task<IEnumerable<AsycudaDocument>> AsycudaDocuments(List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<AsycudaDocument>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentClient())
                    {
                        var res = await t.GetAsycudaDocuments(includesLst).ConfigureAwait(continueOnCapturedContext: false);
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentsByExpression(string exp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaDocument>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentClient())
                    {
					    IEnumerable<DTO.AsycudaDocument> res = null;
                        if(exp == "All")
                        {                       
						    res = await t.GetAsycudaDocuments(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaDocumentsByExpression(exp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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

		 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentsByExpressionLst(List<string> expLst, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || expLst.Count == 0 || expLst.FirstOrDefault() == "None") return new List<AsycudaDocument>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentClient())
                    {
					    IEnumerable<DTO.AsycudaDocument> res = null;
                       
                        res = await t.GetAsycudaDocumentsByExpressionLst(expLst, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                      
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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


		 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentsByExpressionNav(string exp, Dictionary<string, string> navExp, List<string> includesLst = null)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaDocument>().AsEnumerable();
            try
            {
                using (var t = new AsycudaDocumentClient())
                    {
					    IEnumerable<DTO.AsycudaDocument> res = null;
                        if(exp == "All" && navExp.Count == 0)
                        {                       
						    res = await t.GetAsycudaDocuments(includesLst).ConfigureAwait(continueOnCapturedContext: false);					
                        }
                        else
                        {
                             res = await t.GetAsycudaDocumentsByExpressionNav(exp, navExp, includesLst).ConfigureAwait(continueOnCapturedContext: false);	                         
                        }
                    
                        if (res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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


        public async Task<AsycudaDocument> GetAsycudaDocument(string id, List<string> includesLst = null)
        {
             try
             {   
                 using (var t = new AsycudaDocumentClient())
                    {
                        var res = await t.GetAsycudaDocumentByKey(id,includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return new AsycudaDocument(res)
                    {
                     // AsycudaDocumentItems = new System.Collections.ObjectModel.ObservableCollection<AsycudaDocumentItem>(res.AsycudaDocumentItems.Select(y => new AsycudaDocumentItem(y))),    
                  // AsycudaDocumentSetEx = (res.AsycudaDocumentSetEx != null?new AsycudaDocumentSetEx(res.AsycudaDocumentSetEx): null),    
                  // ApplicationSettings = (res.ApplicationSettings != null?new ApplicationSettings(res.ApplicationSettings): null),    
                  // Customs_Procedure = (res.Customs_Procedure != null?new Customs_Procedure(res.Customs_Procedure): null)    
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

        public async Task<AsycudaDocument> UpdateAsycudaDocument(AsycudaDocument entity)
        {
            if (entity == null) return entity;
            var entitychanges = entity.ChangeTracker.GetChanges().FirstOrDefault();
            if (entitychanges != null)
            {
                try
                {
                    using (var t = new AsycudaDocumentClient())
                    {
     
                        var updatedEntity =  await t.UpdateAsycudaDocument(entitychanges).ConfigureAwait(false);
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

        public async Task<AsycudaDocument> CreateAsycudaDocument(AsycudaDocument entity)
        {
            try
            {   
                using (var t = new AsycudaDocumentClient())
                    {
                        return new AsycudaDocument(await t.CreateAsycudaDocument(entity.DTO).ConfigureAwait(continueOnCapturedContext: false));
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

        public async Task<bool> DeleteAsycudaDocument(string id)
        {
            try
            {
             using (var t = new AsycudaDocumentClient())
                {
                    return await t.DeleteAsycudaDocument(id).ConfigureAwait(continueOnCapturedContext: false);
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

        public async Task<bool> RemoveSelectedAsycudaDocument(IEnumerable<string> selectedAsycudaDocument)
        {
            try
            {
                using (var ctx = new AsycudaDocumentClient())
                {
                    return await ctx.RemoveSelectedAsycudaDocument(selectedAsycudaDocument).ConfigureAwait(false);
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

		public async Task<Tuple<IEnumerable<AsycudaDocument>, int>> LoadRange(int startIndex, int count, string exp, Dictionary<string, string> navExp, IEnumerable<string> includeLst = null)
        {
			var overallCount = 0;
            if (System.ComponentModel.LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None")
            {
                
                return new Tuple<IEnumerable<AsycudaDocument>, int>(new List<AsycudaDocument>().AsEnumerable(), overallCount);
            }
            
            try
            {
                using (var t = new AsycudaDocumentClient())
                {

                    IEnumerable<DTO.AsycudaDocument> res = null;
                                         
						    res = await t.LoadRangeNav(startIndex, count, exp, navExp, includeLst).ConfigureAwait(continueOnCapturedContext: false);
						    overallCount = await t.CountNav(exp, navExp).ConfigureAwait(continueOnCapturedContext: false);
                   
                   
                                
                    if (res != null)
                    {
                        return new Tuple<IEnumerable<AsycudaDocument>, int>(res.Select(x => new AsycudaDocument(x)).AsEnumerable(), overallCount);
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

	 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentByAsycudaDocumentSetId(string AsycudaDocumentSetId, List<string> includesLst = null)
        {
             if (AsycudaDocumentSetId == "0") return null;
            try
            {
                 using (AsycudaDocumentClient t = new AsycudaDocumentClient())
                    {
                        var res = await t.GetAsycudaDocumentByAsycudaDocumentSetId(AsycudaDocumentSetId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentByCustoms_ProcedureId(string Customs_ProcedureId, List<string> includesLst = null)
        {
             if (Customs_ProcedureId == "0") return null;
            try
            {
                 using (AsycudaDocumentClient t = new AsycudaDocumentClient())
                    {
                        var res = await t.GetAsycudaDocumentByCustoms_ProcedureId(Customs_ProcedureId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentByDocument_TypeId(string Document_TypeId, List<string> includesLst = null)
        {
             if (Document_TypeId == "0") return null;
            try
            {
                 using (AsycudaDocumentClient t = new AsycudaDocumentClient())
                    {
                        var res = await t.GetAsycudaDocumentByDocument_TypeId(Document_TypeId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentByApplicationSettingsId(string ApplicationSettingsId, List<string> includesLst = null)
        {
             if (ApplicationSettingsId == "0") return null;
            try
            {
                 using (AsycudaDocumentClient t = new AsycudaDocumentClient())
                    {
                        var res = await t.GetAsycudaDocumentByApplicationSettingsId(ApplicationSettingsId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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
 	 public async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocumentByCustomsOperationId(string CustomsOperationId, List<string> includesLst = null)
        {
             if (CustomsOperationId == "0") return null;
            try
            {
                 using (AsycudaDocumentClient t = new AsycudaDocumentClient())
                    {
                        var res = await t.GetAsycudaDocumentByCustomsOperationId(CustomsOperationId, includesLst).ConfigureAwait(continueOnCapturedContext: false);
                         if(res != null)
                        {
                            return res.Select(x => new AsycudaDocument(x)).AsEnumerable();
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
                using (var t = new AsycudaDocumentClient())
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
                using (var t = new AsycudaDocumentClient())
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

