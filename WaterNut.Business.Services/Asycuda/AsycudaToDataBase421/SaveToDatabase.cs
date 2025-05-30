using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421;
using CoreEntities.Business.Entities;
using TrackableEntities.Common;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        public async Task SaveToDatabase(ASYCUDA adoc, AsycudaDocumentSet docSet, FileInfo file)
        {
            try
            {
                a = adoc; // Assuming 'a' is a field in this partial class or another partial class
                // This calls DeleteExistingDocument, which needs to be in its own partial class
                if (await DeleteExistingDocument().ConfigureAwait(false)) return;

                var ads = docSet;
                // This calls CreateDocumentCt, which needs to be in its own partial class
                da = await CreateDocumentCt(ads, file).ConfigureAwait(false); // Assuming 'da' is a field

                // These call methods which need to be in their own partial classes
                await SaveGeneralInformation().ConfigureAwait(false);
                await SaveDeclarant().ConfigureAwait(false);
                await SaveTraders().ConfigureAwait(false);
                await SaveProperty().ConfigureAwait(false);
                await SaveIdentification().ConfigureAwait(false);
                await SaveTransport().ConfigureAwait(false);
                await SaveFinancial().ConfigureAwait(false);
                await Save_Warehouse().ConfigureAwait(false);
                await Save_Valuation().ConfigureAwait(false);
                await SaveContainer().ConfigureAwait(false);
                await Save_Items().ConfigureAwait(false);

                if (da.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure == null) // Potential NullReferenceException
                    return;

                if (!da.DocumentItems.Any()) // Potential NullReferenceException
                {
                    await BaseDataModel.Instance.DeleteAsycudaDocument(da.Document.ASYCUDA_Id).ConfigureAwait(false); // Assuming DeleteAsycudaDocument exists
                    return;
                }

                if (LinkPi) await BaseDataModel.Instance.LinkExistingPreviousItems(da.Document, da.DocumentItems, false).ConfigureAwait(false); // Assuming LinkExistingPreviousItems exists

                if (da.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure?.CustomsOperationId == (int)CoreEntities.Business.Enums.CustomsOperations.Exwarehouse) // Potential NullReferenceException
                     // This calls SavePreviousItem, which needs to be in its own partial class
                     await SavePreviousItem().ConfigureAwait(false);

                // This calls Save_Suppliers_Documents, which needs to be in its own partial class
                await Save_Suppliers_Documents().ConfigureAwait(false);

                if (!da.DocumentItems.Any(x => x.ImportComplete == false)) // Potential NullReferenceException
                    da.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete = true; // Potential NullReferenceException

                await BaseDataModel.Instance.SaveDocumentCt.Execute(da).ConfigureAwait(false); // Assuming SaveDocumentCt exists
                using (var ctx = new CoreEntitiesContext())
                {
                    var res = new AsycudaDocumentSet_Attachments(true)
                    {
                        AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                        DocumentSpecific = true,
                        FileDate = file.LastWriteTime,
                        EmailId = null,
                        FileTypeId = ctx.FileTypes.FirstOrDefault(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.FileImporterInfos.Format == FileTypeManager.FileFormats.XML)?.Id, // Assuming FileTypeManager exists
                        TrackingState = TrackingState.Added,
                        Attachments = new Attachments(true)
                        {
                            FilePath = file.FullName,
                            DocumentCode = "NA",
                            Reference = file.Name.Replace(file.Extension, ""),
                            TrackingState = TrackingState.Added,
                        }
                    };
                    ctx.AsycudaDocumentSet_Attachments.Add(res);
                    ctx.SaveChanges();
                }

                Debug.WriteLine($"{a.Identification.Registration.Number}"); // Potential NullReferenceException
                // BuildSalesReportClass.Instance.ReBuildSalesReports(da.Document.id);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (da != null && da.Document != null && !da.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete) // Potential NullReferenceException
                {
                    da.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties // Potential NullReferenceException
                        .Remove(da.Document.xcuda_ASYCUDA_ExtendedProperties);
                    await BaseDataModel.Instance.DeleteDocumentCt(da).ConfigureAwait(false); // Assuming DeleteDocumentCt exists
                }
            }
        }
    }
}