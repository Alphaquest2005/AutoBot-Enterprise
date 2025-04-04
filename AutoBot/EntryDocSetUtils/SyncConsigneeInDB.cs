using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
// using TrackableEntities; // Removed duplicate using
using CoreAsycudaDocumentSet = CoreEntities.Business.Entities.AsycudaDocumentSet; // Alias for CoreEntities version
using CoreConsignees = CoreEntities.Business.Entities.Consignees; // Alias for CoreEntities version
using AutoBot.Services;

namespace AutoBot
{
    public partial class EntryDocSetUtils
    {
        public static void SyncConsigneeInDB(FileTypes ft, FileInfo[] fs)
        {
            //   --- BEGIN ADDED LOGGING ---
            Console.WriteLine($"--- Logging FileType.Data at start of SyncConsigneeInDB (DocSetId: {ft.AsycudaDocumentSetId}) ---");
            if (ft.Data == null || !ft.Data.Any())
            {
                Console.WriteLine("        FileType.Data is null or empty.");
            }
            else
            {
                foreach (var kvp in ft.Data)
                {
                    Console.WriteLine($"        Key: '{kvp.Key}', Value: '{kvp.Value}'");
                }
            }
            Console.WriteLine($"--- End Logging ---");
            //   --- END ADDED LOGGING ---

            Console.WriteLine($"Executing --> SyncConsigneeInDB for DocSetId: {ft.AsycudaDocumentSetId}");
            try
            {
                // Ensure we have a valid DocSetId
                if (ft.AsycudaDocumentSetId == 0)
                {
                    Console.WriteLine("  - AsycudaDocumentSetId is 0. Skipping.");
                    return;
                }

                // Try to get the extracted consignee name from the List
                var consigneeKvp = ft.Data.FirstOrDefault(kvp => kvp.Key == "Consignee Name");
                var consigneeName = consigneeKvp.Value; // Will be null if KeyValuePair is default

                if (string.IsNullOrWhiteSpace(consigneeName))
                {
                    Console.WriteLine("  - ConsigneeName not found in FileType.Data or is empty. Skipping.");
                    return;
                }

                consigneeName = consigneeName.Trim(); // Clean up the name

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false; // Improve performance

                    // 1. Find or Create Consignee
                    CoreConsignees consignee = ctx.Consignees
                            .FirstOrDefault(c => c.ConsigneeName == consigneeName && c.ApplicationSettingsId == ft.ApplicationSettingsId);

                    if (consignee == null)
                    {
                        Console.WriteLine($"  - Consignee '{consigneeName}' not found. Creating...");
                        // Assuming ConsigneeCode might be same as name or needs generation? Using name for now.
                        var newConsignee = new CoreConsignees
                        {
                            ConsigneeName = consigneeName,
                            ConsigneeCode = consigneeName, // Or generate a unique code if required
                            ApplicationSettingsId = ft.ApplicationSettingsId,
                            TrackingState = TrackingState.Added // Mark as new
                        };
                        ctx.Consignees.Add(newConsignee);
                        ctx.SaveChanges(); // Save to get the ID if needed and ensure it exists for the relationship
                        consignee = newConsignee;
                        Console.WriteLine($"  - Created Consignee with Code: {consignee.ConsigneeCode}");
                    }
                    else
                    {
                        Console.WriteLine($"  - Found existing Consignee with Code: {consignee.ConsigneeCode}");
                    }

                    // 2. Find and Update AsycudaDocumentSet
                    var docSet = ctx.AsycudaDocumentSet
                                                    .Include(d => d.Consignees) // Include existing relationship
                                                    .FirstOrDefault(d => d.AsycudaDocumentSetId == ft.AsycudaDocumentSetId);

                    if (docSet != null)
                    {
                        bool changed = false;
                        if (docSet.ConsigneeName != consignee.ConsigneeName)
                        {
                            Console.WriteLine($"  - Updating DocSet {docSet.AsycudaDocumentSetId} ConsigneeName from '{docSet.ConsigneeName}' to '{consignee.ConsigneeName}'");
                            docSet.ConsigneeName = consignee.ConsigneeName;
                            changed = true;
                        }

                        // Check if the relationship needs updating
                        // Assuming Consignees navigation property links via ConsigneeCode FK implicitly or explicitly
                        // If there's an explicit FK like ConsigneeCode or ConsigneeId, update that too.
                        // For simplicity, setting the navigation property often handles the FK update.
                        if (docSet.Consignees?.ConsigneeCode != consignee.ConsigneeCode) // Check if relationship is different
                        {
                            Console.WriteLine($"  - Updating DocSet {docSet.AsycudaDocumentSetId} Consignees relationship.");
                            docSet.Consignees = consignee;
                            changed = true;
                        }

                        if (changed)
                        {
                            // Mark docSet as modified if EF doesn't track automatically (depends on context setup)
                            // ctx.Entry(docSet).State = EntityState.Modified; // Usually not needed if fetched from context
                            ctx.SaveChanges();
                            Console.WriteLine($"  - Successfully updated DocSet {docSet.AsycudaDocumentSetId}.");
                        }
                        else
                        {
                            Console.WriteLine($"  - DocSet {docSet.AsycudaDocumentSetId} already up-to-date.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  - ERROR: AsycudaDocumentSet with Id {ft.AsycudaDocumentSetId} not found in CoreEntitiesContext.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in SyncConsigneeInDB: {ex.Message}");
                // Optionally re-throw or log more details
                BaseDataModel.EmailExceptionHandler(ex); // Use existing error handling
                // throw; // Uncomment if the process should stop on error
            }
        }
    }
}