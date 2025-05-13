using System;
using System.Data.Entity;
using System.Diagnostics; // Added for Stopwatch
using System.Linq;
using System.Threading.Tasks;
using AutoBot;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;
using FileInfo = System.IO.FileInfo;
namespace AutoBotUtilities
{
    public class ImportUtils
    {
        


        public static async Task ExecuteEmailMappingActions(EmailMapping emailMapping, FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            try
            {
                var missingActions = emailMapping.EmailMappingActions.Where(x => x.Actions.IsDataSpecific == true
                                                                             && !FileUtils.FileActions.ContainsKey(x.Actions.Name)).ToList();

                if (missingActions.Any())
                {
                    throw new ApplicationException(
                        $"The following actions were missing: {missingActions.Select(x => x.Actions.Name).Aggregate((old, current) => old + ", " + current)}");
                }

                emailMapping.EmailMappingActions.OrderBy(x => x.Id)
                    .Where(x => x.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                    .Select(x => (x.Actions.Name, FileUtils.FileActions[x.Actions.Name])).ToList()
                    .ForEach(async x => { await ExecuteActions(fileType, files, x).ConfigureAwait(false); });

            }
            catch (Exception e)
            {
                await EmailDownloader.EmailDownloader.ForwardMsgAsync(fileType.EmailId, BaseDataModel.GetClient(), $"Bug Found",
                    $"{e.Message}\r\n{e.StackTrace}", EmailDownloader.EmailDownloader.GetContacts("Developer"),
                    Array.Empty<string>()).ConfigureAwait(false);
            }
        }



        public static async Task ExecuteActions(FileTypes fileType, FileInfo[] files,
                                                (string Name, Func<FileTypes, FileInfo[], Task> Action) x)
        {
            var contextInfo = $"FTID: {fileType.Id}, EmailId: {fileType.EmailId ?? "N/A"}, DocSetId: {(fileType.AsycudaDocumentSetId == 0 ? "N/A" : fileType.AsycudaDocumentSetId.ToString())}, Ref: {fileType.DocSetRefernece ?? "N/A"}";
            Console.WriteLine($"Action START: {x.Name} | Context: [{contextInfo}]");
            var stopwatch = Stopwatch.StartNew();

            // Declare isContinue here so it's accessible for the final check
            bool isContinueProcessingMainAction = true; // Default to true, ProcessNextStep logic might set it to false

            try
            {
                // --- ProcessNextStep Logic ---
                if (fileType.ProcessNextStep != null && fileType.ProcessNextStep.Any())
                {
                    bool hitContinueInLoop = false; // Tracks if "Continue" action was encountered in the loop

                    while (fileType.ProcessNextStep.Any())
                    {
                        var nextActionName = fileType.ProcessNextStep.First();
                        if (!FileUtils.FileActions.TryGetValue(nextActionName, out var nextActionAsyncFunc))
                        {
                            Console.WriteLine($"Action SKIP (ProcessNextStep): Action '{nextActionName}' not found in FileUtils.FileActions. | Context: [{contextInfo}]");
                            // Remove to avoid infinite loop if action is permanently missing
                            // Place removal in finally block to ensure it's always removed after processing attempt
                        }
                        else // Action found, proceed to execute
                        {
                            Console.WriteLine($"Action START (ProcessNextStep): {nextActionName} | Context: [{contextInfo}]");
                            var nextStopwatch = Stopwatch.StartNew();
                            try
                            {
                                if (nextActionName == "Continue")
                                {
                                    hitContinueInLoop = true;
                                    nextStopwatch.Stop();
                                    Console.WriteLine($"Action CONTINUE (ProcessNextStep): '{nextActionName}' encountered. Proceeding to main action. | Context: [{contextInfo}]");
                                    // Remove "Continue" action from the list
                                    fileType.ProcessNextStep.RemoveAt(0);
                                    break; // Exit while loop to continue to main action
                                }

                                await nextActionAsyncFunc.Invoke(fileType, files).ConfigureAwait(false);

                                nextStopwatch.Stop();
                                Console.WriteLine($"Action SUCCESS (ProcessNextStep): {nextActionName} | Duration: {nextStopwatch.ElapsedMilliseconds}ms | Context: [{contextInfo}]");
                            }
                            catch (Exception nextEx)
                            {
                                nextStopwatch.Stop();
                                Console.WriteLine($"Action FAILED (ProcessNextStep): {nextActionName} | Duration: {nextStopwatch.ElapsedMilliseconds}ms | Error: {nextEx.Message} | Context: [{contextInfo}]");
                                // If a step in ProcessNextStep fails, we should not proceed to the main action
                                isContinueProcessingMainAction = false;
                                // Remove the failed action before rethrowing or exiting
                                if (fileType.ProcessNextStep.Any() && fileType.ProcessNextStep.First() == nextActionName)
                                {
                                    fileType.ProcessNextStep.RemoveAt(0);
                                }
                                throw; // Rethrowing to be caught by the outer try-catch, which will stop further processing for this x.Action
                            }
                        }
                        // Always remove the processed (or skipped) action from the head of the list
                        // This needs to be done carefully, especially with the 'Continue' break and potential exceptions
                        if (fileType.ProcessNextStep.Any() && fileType.ProcessNextStep.First() == nextActionName)
                        {
                            fileType.ProcessNextStep.RemoveAt(0);
                        }
                        else if (nextActionName == "Continue" && !fileType.ProcessNextStep.Any(pns => pns == "Continue"))
                        {
                            // This case is if "Continue" was the last item and already removed.
                            // No action needed here, the break took care of it.
                        }
                        else if (fileType.ProcessNextStep.Any())
                        { // If it wasn't the 'Continue' action that broke, or if action was skipped
                            fileType.ProcessNextStep.RemoveAt(0);
                        }


                    } // End While

                    // After the loop, if "Continue" was not hit AND there were items to process,
                    // it means the ProcessNextStep sequence finished without a 'Continue' signal.
                    // In this case, the main action (x.Action) should not be executed.
                    if (!hitContinueInLoop)
                    {
                        isContinueProcessingMainAction = false;
                        Console.WriteLine($"Action EXIT (ProcessNextStep): Sequence completed without 'Continue'. Main action '{x.Name}' will NOT run. | Context: [{contextInfo}]");
                    }
                }
                // --- End ProcessNextStep Logic ---

                if (isContinueProcessingMainAction)
                {
                    // Execute the main action passed into this method
                    await x.Action.Invoke(fileType, files).ConfigureAwait(false);
                    stopwatch.Stop();
                    Console.WriteLine($"Action SUCCESS: {x.Name} | Duration: {stopwatch.ElapsedMilliseconds}ms | Context: [{contextInfo}]");
                }
                else
                {
                    // If ProcessNextStep determined we shouldn't continue, log it and stop the timer.
                    stopwatch.Stop(); // Ensure stopwatch is stopped even if main action doesn't run
                    Console.WriteLine($"Action SKIPPED (Due to ProcessNextStep): {x.Name} | Duration: {stopwatch.ElapsedMilliseconds}ms | Context: [{contextInfo}]");
                    // Do not throw here, as this is a controlled exit from ProcessNextStep
                }
            }
            catch (Exception e)
            {
                stopwatch.Stop();
                Console.WriteLine($"Action FAILED: {x.Name} | Duration: {stopwatch.ElapsedMilliseconds}ms | Error: {e.Message} | Context: [{contextInfo}]");
                Console.WriteLine($"Stack Trace: {e.StackTrace}");
                await EmailDownloader.EmailDownloader.SendEmailAsync(BaseDataModel.GetClient(), null, $"Bug Found in Action: {x.Name}",
                    EmailDownloader.EmailDownloader.GetContacts("Developer"), $"{e.Message}\r\n{e.StackTrace}",
                    Array.Empty<string>()).ConfigureAwait(false);
                throw;
            }
        }

        public static async Task ExecuteDataSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            try
            {
                // Missing actions check (this part is fine, assuming FileUtils.FileActions keys are up-to-date)
                var missingActions = fileType.FileTypeActions
                    .Where(x => x.Actions.IsDataSpecific == true && !FileUtils.FileActions.ContainsKey(x.Actions.Name))
                    .ToList();

                if (missingActions.Any())
                {
                    throw new ApplicationException(
                        $"The following data-specific actions were missing: {string.Join(", ", missingActions.Select(x => x.Actions.Name))}");
                }

                using (var ctx = new CoreEntitiesContext())
                {
                    var orderedFileActions = ctx.FileTypeActions // Renamed for clarity
                        .Include(fta => fta.Actions)
                        .Where(fta => fta.FileTypeId == fileType.Id)
                        .Where(fta => fta.Actions.IsDataSpecific == true)
                        .Where(fta => (fta.AssessIM7 == null && fta.AssessEX == null) || // Corrected .Equals(null) to == null
                                      (appSetting.AssessIM7 == fta.AssessIM7 ||
                                       appSetting.AssessEX == fta.AssessEX))
                        .Where(fta => fta.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                        .OrderBy(fta => fta.Id)
                        // .Include(fileTypeActions => fileTypeActions.Actions) // Already included above, redundant
                        .ToList();

                    // Get the delegates from FileUtils.FileActions
                    var actionsToExecute = orderedFileActions
                        .Where(fta => FileUtils.FileActions.ContainsKey(fta.Actions.Name)) // Ensure action exists
                        .Select(fta => (Name: fta.Actions.Name, Action: FileUtils.FileActions[fta.Actions.Name])) // Action is Func<..., Task>
                        .ToList();

                    // Use a proper foreach loop to await async operations
                    foreach (var actionTuple in actionsToExecute)
                    {
                        await ExecuteActions(fileType, files, actionTuple).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                // ... error handling ...
                await EmailDownloader.EmailDownloader.ForwardMsgAsync(fileType.EmailId, BaseDataModel.GetClient(), $"Bug Found in ExecuteDataSpecificFileActions",
                    $"{e.Message}\r\n{e.StackTrace}", EmailDownloader.EmailDownloader.GetContacts("Developer"),
                    Array.Empty<string>()).ConfigureAwait(false);
                // Consider rethrowing if this is a critical path: throw;
            }
        }

        public static async Task ExecuteNonSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            try
            {
                // Missing actions check
                var missingActionsCheck = fileType.FileTypeActions
                   .Where(x => (x.Actions.IsDataSpecific == null || x.Actions.IsDataSpecific != true) && !FileUtils.FileActions.ContainsKey(x.Actions.Name))
                   .ToList();
                if (missingActionsCheck.Any())
                {
                    Console.WriteLine($"WARNING: Non-specific actions missing implementation: {string.Join(", ", missingActionsCheck.Select(x => x.Actions.Name))}");
                }

                using (var ctx = new CoreEntitiesContext())
                {
                    var orderedFileActions = ctx.FileTypeActions // Renamed for clarity
                        .Include(fta => fta.Actions)
                        .Where(fta => fta.FileTypeId == fileType.Id)
                        .Where(fta => fta.Actions.IsDataSpecific == null || fta.Actions.IsDataSpecific != true)
                        .Where(fta => (fta.AssessIM7 == null && fta.AssessEX == null) || // Corrected .Equals(null) to == null
                                      (appSetting.AssessIM7 == fta.AssessIM7 ||
                                       appSetting.AssessEX == fta.AssessEX))
                        .Where(fta => fta.Actions.TestMode == BaseDataModel.Instance.CurrentApplicationSettings.TestMode)
                        .OrderBy(fta => fta.Id)
                        .ToList();

                    var actionsToExecute = orderedFileActions
                       .Where(fta => FileUtils.FileActions.ContainsKey(fta.Actions.Name))
                       .Select(fta => (Name: fta.Actions.Name, Action: FileUtils.FileActions[fta.Actions.Name])) // Action is Func<..., Task>
                       .ToList();

                    // Use a proper foreach loop to await async operations
                    foreach (var actionTuple in actionsToExecute)
                    {
                        // ExecuteActions returns a Task, so await it.
                        await ExecuteActions(fileType, files, actionTuple).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                // ... error handling ...
                await EmailDownloader.EmailDownloader.SendEmailAsync(BaseDataModel.GetClient(), null, $"Bug Found in ExecuteNonSpecificFileActions",
                    EmailDownloader.EmailDownloader.GetContacts("Developer"), $"{e.Message}\r\n{e.StackTrace}",
                    Array.Empty<string>()).ConfigureAwait(false);
                // throw; // Consider re-throwing
            }
        }
    }
}