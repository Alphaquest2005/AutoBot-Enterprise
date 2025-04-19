
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using WaterNut.Client.Services;

using CoreEntities.Client.Services;
using CoreEntities.Client.Entities;
using CoreEntities.Client.DTO;

using System.Linq;
using Core.Common;
using System.ComponentModel;
using System.Collections.Generic;
using ApplicationSettings = CoreEntities.Client.Entities.ApplicationSettings;
using AsycudaDocumentSetEx = CoreEntities.Client.Entities.AsycudaDocumentSetEx;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class AsycudaDocumentSetExRepository 
    {
		 public IEnumerable<AsycudaDocumentSetEx> GetAsycudaDocumentSetExsByExpression(string exp, Dictionary<string, string> navExp)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || exp == null || exp == "None") return new List<AsycudaDocumentSetEx>().AsEnumerable();
            using (var t = new AsycudaDocumentSetExClient())
                {
					IEnumerable<DTO.AsycudaDocumentSetEx> res = null;
                    if(exp == "All" && navExp.Count == 0)
                    {                       
						res = t.GetAsycudaDocumentSetExs().Result;					
                    }
                    else
                    {
                        res = t.GetAsycudaDocumentSetExsByExpressionNav(exp, navExp).Result;	                         
                    }
                    
                    if (res != null)
                    {
                        return res.Select(x => new AsycudaDocumentSetEx(x)).AsEnumerable();
                    }
                    else
                    {
                        return null;
                    }                    
                }
        }

         public async Task DeleteDocuments(int docSetId)
         {
             try
             {
                 using (var t = new DocumentSetClient())
                 {
                     await t.DeleteDocuments(docSetId).ConfigureAwait(false);
                 }
             }
             catch (FaultException<ValidationFault> e)
             {
                 throw new Exception(e.Detail.Message, e.InnerException);
             }
             catch (Exception ex)
             {
                 Debugger.Break();
                 throw;
             }         
         }

        public async Task DeleteDocumentSet(int docSetId)
        {
            using (var t = new DocumentSetClient())
            {
                await t.DeleteDocumentSet(docSetId).ConfigureAwait(false);
            }

        }

        public async Task ImportDocuments(int asycudaDocumentSetId ,bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages,
            bool overwriteExisting, bool linkPi, List<string> fileNames)
        {
            try
            {
                using (var t = new DocumentSetClient())
                {
                   await t.ImportDocuments(asycudaDocumentSetId,fileNames,onlyRegisteredDocuments,importTariffCodes, noMessages,overwriteExisting, linkPi).ConfigureAwait(false);
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }   
        }

        public async Task ExportDocument(string fileName, int docId)
        {
            try
            {
                using (var t = new DocumentSetClient())
                {
                    await t.ExportDocument(fileName, docId).ConfigureAwait(false);
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            } 
        }

        public async Task ExportDocSet(int docSetId, string directoryName)
        {
            try
            {
                using (var t = new DocumentSetClient())
                {
                    await t.ExportDocSet(docSetId, directoryName).ConfigureAwait(false);
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            } 
        }

        public async Task SaveAsycudaDocumentSetEx(AsycudaDocumentSetEx asycudaDocumentSetEx)
        {
            try
            {
                using (var t = new DocumentSetClient())
                {
                    await t.SaveAsycudaDocumentSetEx(asycudaDocumentSetEx.DTO).ConfigureAwait(false);
                    
                }
            }
            catch (FaultException<ValidationFault> e)
            {
                throw new Exception(e.Detail.Message, e.InnerException);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            } 
        }

        public async Task<AsycudaDocumentSetEx> NewDocumentSet(int applicationSettingsId)
        {
            using (var t = new DocumentSetClient())
            {
                var dto = await t.NewDocumentSet(applicationSettingsId).ConfigureAwait(false);
                return new AsycudaDocumentSetEx(dto);
            }
        }

        public async  Task CleanBond(int docSetId, bool perIM7)
        {
            using (var t = new DocumentSetClient())
            {
               await t.CleanBond(docSetId, perIM7).ConfigureAwait(false);
            }
        }

        public async Task CleanEntries(int docSetId, IEnumerable<int> lst, bool perIM7)
        {
            using (var t = new DocumentSetClient())
            {
                await t.CleanEntries(docSetId, lst, perIM7).ConfigureAwait(false);
            }
        }

        public async Task BaseDataModelInitialize()
        {
            using (var t = new DocumentSetClient())
            {
                await t.BaseDataModelInitialize().ConfigureAwait(false);
            }
        }


        public async Task CleanLines(int docSetId, IEnumerable<int> lst, bool perIM7)
        {
            using (var t = new DocumentSetClient())
            {
                await t.CleanLines(docSetId, lst, perIM7).ConfigureAwait(false);
            }
        }


        public async Task AttachDocuments(int asycudaDocumentSetId, List<string> files)
        {
            using (var t = new DocumentSetClient())
            {
                await t.AttachDocuments(asycudaDocumentSetId, files).ConfigureAwait(false);
            }
        }
    }
}

