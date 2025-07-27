﻿using AdjustmentQS.Business.Services;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataQS.Business.Entities;
using ExcelDataReader;
using InventoryDS.Business.Entities;
using LicenseDS.Business.Entities;
using MoreLinq;
using SalesDataQS.Business.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration; // Added for ConfigurationManager
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using AdjustmentQS.Business.Entities;
using AllocationDS.Business.Entities;
using Asycuda421;
using AutoBotUtilities;
using DocumentItemDS.Business.Entities;
using EmailDownloader;
using OCR.Business.Entities;
using Org.BouncyCastle.Ocsp;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using SimpleMvvmToolkit.ModelExtensions;
using TrackableEntities;
using TrackableEntities.Client;
using ValuationDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.DataSpace.Asycuda;
using ApplicationException = System.ApplicationException;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using AsycudaDocument_Attachments = CoreEntities.Business.Entities.AsycudaDocument_Attachments;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using Contacts = CoreEntities.Business.Entities.Contacts;
using CoreEntitiesContext = CoreEntities.Business.Entities.CoreEntitiesContext;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using InventoryItemAlias = EntryDataDS.Business.Entities.InventoryItemAlias;
using xBondAllocations = AllocationDS.Business.Entities.xBondAllocations;
using xcuda_Tarification = DocumentItemDS.Business.Entities.xcuda_Tarification;

namespace AutoBot
{
    using ExcelDataReader.Log;
    using Serilog;

    public partial class Utils
    {
       
        public static Client Client { get; set; } = new Client
        {
            CompanyName = BaseDataModel.Instance.CurrentApplicationSettings.CompanyName,
            DataFolder = BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
            Password = BaseDataModel.Instance.CurrentApplicationSettings.EmailPassword,
            Email = BaseDataModel.Instance.CurrentApplicationSettings.Email,
            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
            EmailMappings = BaseDataModel.Instance.CurrentApplicationSettings.EmailMapping.ToList(),
            DevMode = true
        };


    public static Task Kill(FileTypes arg1, FileInfo[] arg2)
    {
        return Task.Run(() => Application.Exit());
    }


    public static void SetCurrentApplicationSettings(int id)
    {
        // Explicitly load the connection string from configuration
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreEntities"].ConnectionString;

        // Add logging to show the connection string being used
        Console.WriteLine($"[DEBUG] Using connection string: {connectionString}");

        using (var ctx = new CoreEntitiesContext())
        {


            var appSetting = ctx.ApplicationSettings.AsNoTracking()
                    .Include(x => x.FileTypes)
                    .Include(x => x.Declarants)
                    .Include("FileTypes.FileTypeContacts.Contacts")
                    .Include("FileTypes.FileTypeActions.Actions")
                    .Include(x => x.EmailMapping)
                    .Include("FileTypes.FileTypeMappings")
                    .Where(x => x.IsActive)
                    .First(x => x.ApplicationSettingsId == id);

                // set BaseDataModel CurrentAppSettings
                BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                //check emails

            }
        }



      


       // Change signature to async Task
       public static async Task RunAllocationTestCases()
       {
           try
           {
               Console.WriteLine("Running Test Cases");
               List<KeyValuePair<int, string>> lst;
               using (var ctx = new AllocationDSContext())
               {
                   ctx.Database.CommandTimeout = 10;

                   lst = ctx
                       .AllocationsTestCases
                       .Select(x => new { x.EntryDataDetailsId, x.ItemNumber })
                       .Distinct()
                       .ToList()
                       .Select(x => new KeyValuePair<int, string>(x.EntryDataDetailsId, x.ItemNumber))
                       .ToList();
               }

               if (!lst.Any()) return;
               // Replace Wait() with await ConfigureAwait(false)
               await AllocationsModel.Instance.ClearDocSetAllocations(lst.Select(x => $"'{x.Value}'").Aggregate((o, n) => $"{o},{n}")).ConfigureAwait(false);

               AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);



               var strLst = lst.Select(x => $"{x.Key.ToString()}-{x.Value}").Aggregate((o, n) => $"{o},{n}");


               // Replace Wait() with await ConfigureAwait(false)
               await new AdjustmentShortService().AutoMatchUtils.AutoMatchItems(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, strLst).ConfigureAwait(false);


               // Replace Wait() with await ConfigureAwait(false)
               // ProcessDisErrorsForAllocation.Execute is synchronous, remove await and ConfigureAwait
               await new AdjustmentShortService().AutoMatchUtils.AutoMatchProcessor.ProcessDisErrorsForAllocation
                   .Execute(
                       BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                       strLst
                   ).ConfigureAwait(false); // Removed await and ConfigureAwait

               // Replace Wait() with await ConfigureAwait(false)
               await new OldSalesAllocator()
                   .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                       BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false, false, strLst).ConfigureAwait(false);

               // Replace Wait() with await ConfigureAwait(false)
               await new MarkErrors()
                   .Execute(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);

           }
           catch (Exception e)
           {
               Console.WriteLine(e);
               throw;
           }

       }


        public static async Task SaveAttachments(FileInfo[] csvFiles, FileTypes fileType, Email email, ILogger log) // Added ILogger
        {
            string operationName = nameof(SaveAttachments);
            var stopwatch = Stopwatch.StartNew(); // Start stopwatch
            log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}", // METHOD_ENTRY log
                operationName, new { FileCount = csvFiles?.Length ?? 0, FileTypeId = fileType?.Id, EmailId = email?.EmailId });

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Starting attachment saving process.", operationName, "Start"); // INTERNAL_STEP

                using (var ctx = new CoreEntitiesContext() { StartTracking = true })
                {
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                        "ctx.Emails.Include(\"EmailAttachments.Attachments\").FirstOrDefault", "SYNC_EXPECTED"); // This is not async, so SYNC_EXPECTED
                    var oldemail = ctx.Emails.Include("EmailAttachments.Attachments").FirstOrDefault(x => x.EmailId == email.EmailId);
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) EmailFound: {EmailFound}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                        "ctx.Emails.Include(\"EmailAttachments.Attachments\").FirstOrDefault", "Sync call returned.", oldemail != null);

                    if (oldemail == null)
                    {
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Email record not found, creating new one.", operationName, "CreateEmail"); // INTERNAL_STEP
                        oldemail = ctx.Emails.Add(new Emails(true)
                        {
EmailId = email.EmailId, // Added EmailId
                            EmailUniqueId = email.EmailUniqueId,
                            Subject = email.Subject,
                            EmailDate = email.EmailDate,
                            MachineName = Environment.MachineName,
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            TrackingState = TrackingState.Added
                        });
                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                            "ctx.SaveChanges (for new email)", "SYNC_EXPECTED");
                        ctx.SaveChanges(); // This is not async, so SYNC_EXPECTED
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                            "ctx.SaveChanges (for new email)", "Sync call returned.");
                    }
                    else
                    {
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Email record found, updating existing one.", operationName, "UpdateEmail"); // INTERNAL_STEP
                        oldemail.MachineName = Environment.MachineName;
                        oldemail.EmailUniqueId = email.EmailUniqueId;
                        oldemail.TrackingState = TrackingState.Modified; // Set TrackingState to Modified
                    }

                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Processing {FileCount} attached files.", operationName, "ProcessFiles", csvFiles?.Length ?? 0); // INTERNAL_STEP
                    if (csvFiles == null || csvFiles.Length == 0)
                    {
                         log.Warning("INTERNAL_STEP ({MethodName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                             operationName, "ProcessFiles", "csvFiles", "No files provided to save.");
                    }


                    foreach (var file in csvFiles)
                    {
                        log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Processing file: {FileName}", operationName, "ProcessSingleFile", file.Name); // INTERNAL_STEP

                        if (fileType.FileImporterInfos.EntryType != FileTypeManager.EntryTypes.Unknown)
                        {
                            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}", // INVOKING_OPERATION
                                "FileTypeManager.SendBackTooBigEmail", "ASYNC_EXPECTED", file.Name);
                            await FileTypeManager.SendBackTooBigEmail(file, fileType, log).ConfigureAwait(false);
                            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for file {FileName}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                                "FileTypeManager.SendBackTooBigEmail", "Async call completed (await).", file.Name);
                        }

                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}", // INVOKING_OPERATION
                            "GetReference", "SYNC_EXPECTED", file.Name); // This is not async, so SYNC_EXPECTED
                        var reference = GetReference(file, ctx, log);
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) Reference: {Reference} for file {FileName}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                            "GetReference", "Sync call returned.", reference, file.Name);


                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}", // INVOKING_OPERATION
                            "ctx.Attachments.FirstOrDefault", "SYNC_EXPECTED", file.Name); // This is not async, so SYNC_EXPECTED
                        Attachments attachment = ctx.Attachments.FirstOrDefault(x => x.FilePath == file.FullName);
                         log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) AttachmentFound: {AttachmentFound} for file {FileName}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                            "ctx.Attachments.FirstOrDefault", "Sync call returned.", attachment != null, file.Name);

                        if(attachment == null)
                        {
                            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Attachment record not found, creating new one for file {FileName}.", operationName, "CreateAttachment", file.Name); // INTERNAL_STEP
                            attachment = new Attachments(true)
                            {
                                FilePath = file.FullName,
                                DocumentCode = fileType.DocumentCode,
                                Reference = reference,
                                EmailId = email.EmailId,
                                TrackingState = TrackingState.Added
                            };
                            ctx.Attachments.Add(attachment); // Explicitly add new attachment to context
                        }
                        else
                        {
                             log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Attachment record found, updating existing one for file {FileName}.", operationName, "UpdateAttachment", file.Name); // INTERNAL_STEP
                        }


                        log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}", // INVOKING_OPERATION
                            "AddUpdateEmailAttachments", "SYNC_EXPECTED", file.Name); // This is not async, so SYNC_EXPECTED
                        AddUpdateEmailAttachments(fileType, email, oldemail, file, ctx, attachment, reference, log);
                        log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for file {FileName}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                            "AddUpdateEmailAttachments", "Sync call returned.", file.Name);


                        if (fileType.AsycudaDocumentSetId != 0)
                        {
                            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName} and DocSetId {DocSetId}", // INVOKING_OPERATION
                                "EntryDocSetUtils.AddUpdateDocSetAttachement", "SYNC_EXPECTED", file.Name, fileType.AsycudaDocumentSetId); // This is not async, so SYNC_EXPECTED
                            EntryDocSetUtils.AddUpdateDocSetAttachement(fileType, email, ctx, file, attachment, reference, log);
                            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for file {FileName} and DocSetId {DocSetId}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                                "EntryDocSetUtils.AddUpdateDocSetAttachement", "Sync call returned.", file.Name, fileType.AsycudaDocumentSetId);
                        }
                         else
                        {
                             log.Information("INTERNAL_STEP ({MethodName} - {Stage}): AsycudaDocumentSetId is 0. Skipping EntryDocSetUtils.AddUpdateDocSetAttachement for file {FileName}.", operationName, "SkipDocSetAttachment", file.Name); // INTERNAL_STEP
                        }

                    }

                }
                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", // METHOD_EXIT_SUCCESS
                    operationName, stopwatch.ElapsedMilliseconds);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // META_LOG_DIRECTIVE: Type: Instrumentation; Context: Method:SaveAttachments, Catch:DbEntityValidationException; Directive: Adding diagnostic log at the start of the catch block to verify execution flow.; ExpectedChange: A new debug log will appear in the logs if code execution proceeds past the initial error log.; SourceIteration: LLM_Iter_0.2
                log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Entered DbEntityValidationException catch block.", operationName, "CatchEntry"); // Diagnostic log

                log.Warning("META_LOG_DIRECTIVE: Type: {MetaType}; Context: {MetaContext}; Directive: {MetaDirective}; ExpectedChange: {ExpectedBehavioralChange}; SourceIteration: {SourceLLMIterationId}", "ErrorAnalysis", "Method:SaveAttachments, Exception:DbEntityValidationException", "Logging detailed EntityValidationErrors to diagnose save failure.", "Logs will contain specific entity and property validation errors.", "LLM_Iter_3.1");

                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    log.Error("INTERNAL_STEP ({MethodName} - {Stage}): Entity of type \"{EntityType}\" in state \"{EntityState}\" has validation errors.",
                        operationName, "EntityValidationDetails", validationErrors.Entry.Entity.GetType().Name, validationErrors.Entry.State);

                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        log.Error("INTERNAL_STEP ({MethodName} - {Stage}): Property: \"{PropertyName}\", Error: \"{ErrorMessage}\"",
                            operationName, "EntityValidationError", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                throw; // Re-throw the exception
            }
            catch (Exception e)
            {
                 stopwatch.Stop(); // Stop stopwatch on error
                 log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", // METHOD_EXIT_FAILURE
                     operationName, stopwatch.ElapsedMilliseconds, e.Message);
                 throw; // Re-throw the exception
             }
         }



        private static void AddUpdateEmailAttachments(FileTypes fileType, Email email, Emails oldemail, FileInfo file,
            CoreEntitiesContext ctx, Attachments attachment, string reference, ILogger log) // Added ILogger
        {
            string operationName = nameof(AddUpdateEmailAttachments);
            var stopwatch = Stopwatch.StartNew(); // Start stopwatch
            log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}", // METHOD_ENTRY log
                operationName, new { FileTypeId = fileType?.Id, EmailId = email?.EmailId, FileName = file?.Name, AttachmentId = attachment?.Id, Reference = reference });

            try
            {
                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}", // INVOKING_OPERATION
                    "oldemail.EmailAttachments.FirstOrDefault", "SYNC_EXPECTED", file?.Name); // This is not async, so SYNC_EXPECTED
                var emailAttachement =
                    oldemail.EmailAttachments.FirstOrDefault(x => x.Attachments.FilePath == file.FullName);
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) EmailAttachmentFound: {EmailAttachmentFound} for file {FileName}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "oldemail.EmailAttachments.FirstOrDefault", "Sync call returned.", emailAttachement != null, file?.Name);


                if (emailAttachement == null)
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Email attachment record not found, creating new one for file {FileName}.", operationName, "CreateEmailAttachment", file?.Name); // INTERNAL_STEP
                    log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for file {FileName}", // INVOKING_OPERATION
                        "ctx.EmailAttachments.Add", "SYNC_EXPECTED", file?.Name); // This is not async, so SYNC_EXPECTED
                    ctx.EmailAttachments.Add(
                        new EmailAttachments(true)
                        {
                            Attachments = attachment,
                            DocumentSpecific = fileType.DocumentSpecific,
                            EmailId = email.EmailId,
                            FileTypeId = fileType.Id,
                            TrackingState = TrackingState.Added
                        });
                    log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) for file {FileName}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                        "ctx.EmailAttachments.Add", "Sync call returned.", file?.Name);
                }
                else
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Email attachment record found, updating existing one for file {FileName}.", operationName, "UpdateEmailAttachment", file?.Name); // INTERNAL_STEP
                    emailAttachement.DocumentSpecific = fileType.DocumentSpecific;
                    emailAttachement.EmailId = email.EmailId;
                    emailAttachement.FileTypeId = fileType.Id;
                    emailAttachement.Attachments.Reference = reference;
                    emailAttachement.Attachments.DocumentCode = fileType.DocumentCode;
                    emailAttachement.Attachments.EmailId = email.EmailId;
                }

                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", // INVOKING_OPERATION
                    "ctx.SaveChanges (for email attachment)", "SYNC_EXPECTED"); // This is not async, so SYNC_EXPECTED
                ctx.SaveChanges();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "ctx.SaveChanges (for email attachment)", "Sync call returned.");

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", // METHOD_EXIT_SUCCESS
                    operationName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                stopwatch.Stop(); // Stop stopwatch on error
                log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", // METHOD_EXIT_FAILURE
                    operationName, stopwatch.ElapsedMilliseconds, e.Message);
                throw; // Re-throw the exception
            }
        }

        private static string GetReference(FileInfo file, CoreEntitiesContext ctx, ILogger log) // Added ILogger
        {
            string operationName = nameof(GetReference);
            var stopwatch = Stopwatch.StartNew(); // Start stopwatch
            log.Information("METHOD_ENTRY: {MethodName}. Context: {Context}", // METHOD_ENTRY log
                operationName, new { FileName = file?.Name });

            try
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Generating initial reference from file name {FileName}.", operationName, "GenerateInitialReference", file?.Name); // INTERNAL_STEP
                var newReference = file.Name.Replace(file.Extension, "");
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Initial reference generated: {InitialReference}", operationName, "InitialReferenceGenerated", newReference); // INTERNAL_STEP


                log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation}) for reference {Reference}", // INVOKING_OPERATION
                    "ctx.Attachments.Select(x => x.Reference).Where(x => x.Contains(newReference)).Count()", "SYNC_EXPECTED", newReference); // This is not async, so SYNC_EXPECTED
                var existingRefCount = ctx.Attachments.Select(x => x.Reference)
                    .Where(x => x.Contains(newReference)).Count();
                log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance}) ExistingReferenceCount: {ExistingReferenceCount} for reference {Reference}", // OPERATION_INVOKED_AND_CONTROL_RETURNED
                    "ctx.Attachments.Select(x => x.Reference).Where(x => x.Contains(newReference)).Count()", "Sync call returned.", existingRefCount, newReference);


                if (existingRefCount > 0)
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Existing references found. Appending count to reference.", operationName, "AppendCount"); // INTERNAL_STEP
                    newReference = $"{existingRefCount + 1}-{newReference}";
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Updated reference: {UpdatedReference}", operationName, "UpdatedReference", newReference); // INTERNAL_STEP
                }
                else
                {
                    log.Information("INTERNAL_STEP ({MethodName} - {Stage}): No existing references found. Using initial reference.", operationName, "NoExistingReferences"); // INTERNAL_STEP
                }

                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Generated Reference: {GeneratedReference}", // METHOD_EXIT_SUCCESS
                    operationName, stopwatch.ElapsedMilliseconds, newReference);
                return newReference;
            }
            catch (Exception e)
            {
                stopwatch.Stop(); // Stop stopwatch on error
                log.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", // METHOD_EXIT_FAILURE
                    operationName, stopwatch.ElapsedMilliseconds, e.Message);
                throw; // Re-throw the exception
            }
        }


        public static async Task<(bool success, int lcontValue)> AssessComplete(string instrFile, string resultsFile, ILogger log)
        {
            int lcontValue = 0;
            try
            {
                var rcount = 0;

                if (File.Exists(instrFile))
                {
                    if (!File.Exists(resultsFile)) return (false, lcontValue);
                    var lines = File.ReadAllLines(instrFile).Where(x => x.StartsWith("File\t")).ToArray();
                    var res = File.ReadAllLines(resultsFile).Where(x => x.StartsWith("File\t")).ToArray();
                    if (res.Length == 0)
                    {
                        return (false, lcontValue);
                    }

                    foreach (var line in lines)
                    {
                        var p = line.Split('\t');
                        if (lcontValue + 1 >= res.Length) return (false, lcontValue);
                        if (string.IsNullOrEmpty(res[lcontValue])) return (false, lcontValue);
                        rcount += 1;
                        var isSuccess = false;
                        foreach (var rline in res)
                        {
                            var r = rline.Split('\t');

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Success")
                            {
                                if (r[0] == "File") lcontValue = rcount - 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] == r[0] && r[2] == "Success") // for file
                            {
                                lcontValue += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 4 && p[2] == r[2] && r[3] == "Success") // for file
                            {
                                lcontValue += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] != r[1] && r[2] == "Error")
                            {
                                if (r[0] == "Screenshot")
                                {
                                    await SubmitScriptErrors(r[1], log).ConfigureAwait(false);
                                    // Assuming true indicates an action was taken, even on error path,
                                    // or that the process should be considered "complete" for this iteration.
                                    // If AssessComplete's "true" means "overall success", this might need adjustment.
                                    // For now, mirroring the original logic's "return true" path.
                                    return (true, lcontValue);
                                }
                                //isSuccess = true; // Original commented out
                                //break; // Original commented out
                            }

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Error")
                            {
                                // email error
                                //if (r[0] == "File") lcontValue = rcount - 1; // Original commented out
                                //isSuccess = true; // Original commented out
                                //break; // Original commented out
                            }
                        }

                        if (isSuccess == true) continue;
                        return (false, lcontValue);
                    }
                    return (true, lcontValue);
                }
                // If instrFile does not exist, original logic returns true.
                return (true, lcontValue);
            }
            catch (Exception)
            {
                // Preserving original stack trace
                throw;
            }
        }

        public static async Task SubmitScriptErrors(string file, ILogger log)
        {
            try
            {


                Console.WriteLine("Submit Script Errors");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                    && (x.Role == "Developer" || x.Role == "Broker")).Select(x => x.EmailAddress).ToArray();


                    var body = $"Please see attached.\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";

                    var msg =  EmailDownloader.EmailDownloader.CreateMessage(Client, "AutoBot Script Error", contacts, body, new string[]
                    {
                        file
                    }, log);
                    await EmailDownloader.EmailDownloader.SendEmailInternalAsync(Client, msg).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public static void RetryImport(int trytimes, string script, bool redownload, string directoryName)
        {
            int lcont;
            for (int i = 0; i < trytimes; i++)
            {
                if (Utils.ImportComplete(directoryName, redownload, out lcont, DateTime.Now.Year))
                    break; //ImportComplete(directoryName,false, out lcont);
                Utils.RunSiKuLi(directoryName, script, lcont.ToString());
                if (Utils.ImportComplete(directoryName, redownload, out lcont, DateTime.Now.Year)) break;
            }
        }

        public static async Task RetryAssess(string instrFile, string resultsFile, string directoryName, int trytimes, ILogger log)
        {
            var lcont = 0;
            for (int i = 0; i < trytimes; i++)
            {
                var assessmentResult1 = await Utils.AssessComplete(instrFile, resultsFile, log).ConfigureAwait(false);
                lcont = assessmentResult1.lcontValue;
                if (assessmentResult1.success == true) break;
            
                // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                Utils.RunSiKuLi(directoryName, "AssessIM7", lcont.ToString()); //SaveIM7
                var assessmentResult2 = await Utils.AssessComplete(instrFile, resultsFile, log).ConfigureAwait(false);
                lcont = assessmentResult2.lcontValue;
                if(assessmentResult2.success == true) break;
            }
        }

        public static async Task Assess(string instrFile, string resultsFile, string directoryName, ILogger log)
        {
            var lcont = 0;
            var assessmentResult = await Utils.AssessComplete(instrFile, resultsFile, log).ConfigureAwait(false);
            lcont = assessmentResult.lcontValue;
            while (assessmentResult.success == false)
            {
                // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                Utils.RunSiKuLi(directoryName, "AssessIM7", lcont.ToString()); //SaveIM7
                assessmentResult = await Utils.AssessComplete(instrFile, resultsFile, log).ConfigureAwait(false);
                lcont = assessmentResult.lcontValue;
            }
        }

        private static int oldProcessId = 0;
        public static void RunSiKuLi(string directoryName, string scriptName, string lastCNumber = "", int sMonths = 0, int sYears = 0, int eMonths = 0, int eYears = 0, bool enableDebugging = false) // Added enableDebugging parameter
        {
            try
            {

                if (string.IsNullOrEmpty(directoryName)) return;

                Serilog.Log.Information("Executing {ScriptName}", scriptName);
                var script =
                    $"C:\\Users\\{Environment.UserName}\\OneDrive\\Clients\\AutoBot\\Scripts\\{scriptName}.sikuli";
                if (!Directory.Exists(script)) throw new ApplicationException($"Script not found: '{script}'");

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "java.exe",
                    // Construct arguments string
                    Arguments = $@"-jar C:\Users\{Environment.UserName}\OneDrive\Clients\AutoBot\sikulixide-2.0.5.jar -r {script} --args {
                        BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin} {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} ""{directoryName + "\\\\"}"" {
                        (string.IsNullOrEmpty(lastCNumber) ? "" : lastCNumber + "")
                    }{(sMonths + sYears + eMonths + eYears == 0 ? "" : $" {sMonths} {sYears} {eMonths} {eYears}")}{(enableDebugging ? " --debug-screenshots" : "")}", // Conditionally add debug flag
                    UseShellExecute = false
                };
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;

                /// wait if instance already running 
                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA"))
                             .ToList())
                {
                    Thread.Sleep(1000 * 60);
                }

                foreach (var process1 in Process.GetProcesses().Where(x => x.ProcessName.Contains("java") || x.ProcessName.Contains("Photo"))
                             .ToList())
                {
                    process1.Kill();
                }

                if (oldProcessId != 0)
                {
                    try
                    {
                        var oldProcess = Process.GetProcessById(oldProcessId);
                        oldProcess.Kill();
                    }
                    catch (Exception)
                    {

                    }

                }


                process.Start();
                oldProcessId = process.Id;
                var timeoutCycles = 0;
                while (!process.HasExited && process.Responding)
                {
                    //var rmo = Process.GetProcesses().Select(x => x).ToList();
                    if (timeoutCycles > 1 && !Process.GetProcesses().Where(x =>
                                x.MainWindowTitle.Contains("ASYCUDA"))
                            .ToList().Any()) break;
                    if (timeoutCycles > WaterNut.DataSpace.Utils._noOfCyclesBeforeHardExit) break;
                    //Console.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Debug.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Thread.Sleep(1000 * 60);
                    timeoutCycles += 1;
                }

                if (!process.HasExited) process.Kill();

                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA") 
                                                                           || x.MainWindowTitle.Contains("Acrobat Reader")
                                                                           || x.MainWindowTitle.Contains("Photo")
                                                                           )
                             .ToList())
                {
                    process1.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool ImportComplete(string directoryName, bool redownload, out int lcont, int startYear, bool retryOnblankFile = false)
        {
            try
            {

                lcont = 0;

                var desFolder = directoryName + (directoryName.EndsWith(@"\") ? "" : "\\");
                var overviewFile = Path.Combine(desFolder, "OverView.txt");
                if (File.Exists(overviewFile) || File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-2))
                {


                    if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-1)) return false;
                    var readAllText = File.ReadAllText(overviewFile);

                    if (readAllText == "No Files Found") return !retryOnblankFile;

                    var lines = readAllText
                        .Split(new[] {$"\r\n{startYear}\t"}, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length == 0)
                    {

                        return false;
                    }



                    var existingfiles = 0;

                    foreach (var line in lines)
                    {
                        lcont += 1;

                        //var p = line.StartsWith($"{DateTime.Now.Year}\t")
                        //    ? line.Replace($"{DateTime.Now.Year}\t", "").Split('\t')
                        //    : line.Split('\t');
                        var p = line.Split('\t').ToList();
                        if (p[0] == startYear.ToString())
                        {
                            p.RemoveAt(0);
                        }


                        if (p.Count < 8) continue;
                        if (!DateTime.TryParse(p[6], out var regDate)) continue;

                        var fileName = Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{regDate.Year}-{p[5]}.xml");
                        if (File.Exists(fileName))
                        {
                            existingfiles += 1;
                            continue;
                        }

                        return false;
                    }

                    if (redownload && (DateTime.Now - new FileInfo(overviewFile).LastWriteTime).Minutes > 5)
                        return false;
                    return existingfiles != 0;
                }
                else
                {

                    return false;
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                lcont = 0;
                return false;
                //throw;
            }
           
        }

     
        public static async Task SubmitMissingInvoices(FileTypes ft, ILogger log)
        {
            try
            {


                Console.WriteLine("Submit Missing Invoices");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitDocSetWithIncompleteInvoices
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList()
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                        if (Utils.GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices").Any<ActionDocSetLogs>()) continue;


                        var body = $"The {emailIds.FirstOrDefault().Declarant_Reference_Number} is missing Invoices. {emailIds.FirstOrDefault().ImportedInvoices} were Imported out of {emailIds.FirstOrDefault().TotalInvoices} . \r\n" +
                                   $"\t{"Template No.".FormatedSpace(20)}{"Template Date".FormatedSpace(20)}{"Template Value".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.InvoiceDate.ToShortDateString().FormatedSpace(20)}{current.InvoiceTotal.Value.ToString().FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                           await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "", "Error:Missing Invoices",
                               contacts, body, attlst.ToArray(), log).ConfigureAwait(false);
                        }
                        else
                        {
                           await EmailDownloader.EmailDownloader.ForwardMsgAsync(emailIds.Key.EmailId, Utils.Client, "Error:Missing Invoices", body, contacts, attlst.ToArray(), log).ConfigureAwait(false);
                        }


                        ctx.SaveChanges();


                        //LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices");


                    }

                }
                await SubmitMissingInvoicePDFs(ft, log).ConfigureAwait(false);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static List<ActionDocSetLogs> GetDocSetActions(int docSetId, string actionName)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                return ctx.ActionDocSetLogs.Where(x => x.Actions.Name == actionName && x.AsycudaDocumentSetId == docSetId).ToList();

            }
        }

        public static async Task SubmitMissingInvoicePDFs(FileTypes ft, ILogger log)
        {
            try
            {


                Console.WriteLine("Submit Missing Template PDFs");


                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitMissingInvoicePDFs
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId) // use the more precise filter
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                        if (Enumerable.Any<ActionDocSetLogs>(Utils.GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs"))) continue;


                        var body = $"The {emailIds.FirstOrDefault().Declarant_Reference_Number} is missing Template PDF Attachments. \r\n" +
                                   $"\t{"Template No.".FormatedSpace(20)}{"Source File".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SourceFile.FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please email CSV with Coresponding PDF to prevent this error.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                           await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "", "Error:Missing Invoices PDF Attachments",
                               contacts, body, attlst.ToArray(), log).ConfigureAwait(false);
                        }
                        else
                        {
                           await EmailDownloader.EmailDownloader.ForwardMsgAsync(emailIds.Key.EmailId, Utils.Client, "Error:Missing Invoices PDF Attachments", body, contacts, attlst.ToArray(), log).ConfigureAwait(false);
                        }


                        ctx.SaveChanges();


                        //LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs");


                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static async Task SubmitIncompleteEntryData(FileTypes ft, ILogger log)
        {
            try
            {


                Console.WriteLine("Submit Incomplete Entry Data");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts
                        .Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitIncompleteEntryData
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)// ft.AsycudaDocumentSetId == 0 ||
                        .ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {



                        var body = "The Following Invoices Total do not match Imported Totals . \r\n" +
                                   $"\t{"Template No.".FormatedSpace(20)}{"Supplier Code".FormatedSpace(20)}{"Template Total".FormatedSpace(20)}{"Imported Total".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SupplierCode.FormatedSpace(20)}{current.InvoiceTotal.Value.ToString("C").FormatedSpace(20)}{current.ImportedTotal.Value.ToString("C").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                           await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "", "Error:Incomplete Template Data",
                               contacts, body, attlst.ToArray(), log).ConfigureAwait(false);
                        }
                        else
                        {
                           await EmailDownloader.EmailDownloader.ForwardMsgAsync(emailIds.Key, Utils.Client, "Error:Incomplete Template Data", body, contacts, attlst.ToArray(), log).ConfigureAwait(false);
                        }


                        await ctx.SaveChangesAsync().ConfigureAwait(false);

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public static Task<List<FileTypes>> GetFileType(string entryType, string format, string fileName)
        {
            return FileTypeManager.GetImportableFileType(entryType, format, fileName);
        }

        public static void RetryImport(int trytimes, string script, bool redownload, string directoryName, int sMonth,
            int sYear, int eMonth, int eYear, int startYear, bool retryOnblankFile = false)
        {
            int lcont;
            for (int i = 0; i < trytimes; i++)
            {
                if (Utils.ImportComplete(directoryName, redownload, out lcont, startYear, retryOnblankFile))
                    break; //ImportComplete(directoryName,false, out lcont);
                Utils.RunSiKuLi(directoryName, script, lcont.ToString(), sMonth, sYear, eMonth, eYear);
                if (Utils.ImportComplete(directoryName, redownload, out lcont, startYear, retryOnblankFile)) break;
            }
        }
    }
}
