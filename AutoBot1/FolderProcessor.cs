using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoBotUtilities;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Enums; // Added for ImportStatus
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils; // Added for FileTypeManager
using FileTypes = CoreEntities.Business.Entities.FileTypes; // Alias to resolve ambiguity
using WaterNut.DataSpace; // Assuming PDFUtils, ShipmentUtils etc. are accessible via this
using DocumentDS.Business.Entities; // Added for Attachments - Assuming this is the correct namespace
using CoreEntities.Business.Entities; // Added for Emails
using TrackableEntities; // Added for TrackingState


namespace AutoBot
{
    public class FolderProcessor
    {
        // Constructor could potentially take dependencies like ILogger, IEmailService etc. in a fuller refactoring
        public FolderProcessor()
        {
            // Initialize dependencies if needed
        }

        public async Task ProcessDownloadFolder(ApplicationSettings appSetting)
        {
            var downloadFolder = new DirectoryInfo(Path.Combine(appSetting.DataFolder, "Downloads"));

            if (!downloadFolder.Exists) downloadFolder.Create();

            foreach (var file in downloadFolder.GetFiles("*.pdf").ToList())
            {
                try
                {
                    await ProcessFile(appSetting, file).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    // Consider logging the exception instead of just writing to console
                    Console.WriteLine($"Error processing file {file.Name}: {e.Message}");
                    // Decide if processing should continue or stop on error
                    continue;
                }
            }
        }

        private async Task ProcessFile(ApplicationSettings appSetting, FileInfo file)
        {
            var documentsFolder = CreateDocumentsFolder(appSetting, file);
            var destFileName = CopyFileToDocumentsFolder(file, documentsFolder);

            // Assuming FileTypeManager is static or accessible
            var fileTypes = GetUnknownFileTypes(file);

            fileTypes.ForEach(x => x.EmailId = file.Name); // Set EmailId based on filename for context

            var allgood = await ProcessFileTypes(fileTypes, destFileName, file).ConfigureAwait(false);

            if (allgood)
            {
                try
                {
                    file.Delete();
                }
                catch (IOException ex)
                {
                     Console.WriteLine($"Error deleting processed file {file.Name}: {ex.Message}");
                     // Log error, maybe retry later?
                }
            }
        }

        private DirectoryInfo CreateDocumentsFolder(ApplicationSettings appSetting, FileInfo file)
        {
            // Ensure filename characters are valid for a directory path
            var safeFolderName = string.Join("_", file.Name.Replace(file.Extension, "").Split(Path.GetInvalidFileNameChars()));
            var documentsFolderPath = Path.Combine(appSetting.DataFolder, "Documents", safeFolderName);
            var documentsFolder = new DirectoryInfo(documentsFolderPath);

            if (!documentsFolder.Exists)
            {
                try
                {
                     documentsFolder.Create();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating directory {documentsFolderPath}: {ex.Message}");
                    // Handle error appropriately, maybe throw or return null/indicator
                    throw; // Rethrowing for now
                }
            }
            return documentsFolder;
        }

        private string CopyFileToDocumentsFolder(FileInfo file, DirectoryInfo documentsFolder)
        {
            var destFileName = Path.Combine(documentsFolder.FullName, file.Name);
            try
            {
                if (!File.Exists(destFileName))
                {
                    file.CopyTo(destFileName);
                }
                // Else: Decide on overwrite logic if needed
            }
            catch (IOException ex)
            {
                 Console.WriteLine($"Error copying file {file.Name} to {destFileName}: {ex.Message}");
                 // Handle error, maybe throw
                 throw; // Rethrowing for now
            }
            return destFileName;
        }

        private List<FileTypes> GetUnknownFileTypes(FileInfo file)
        {
            // Assuming FileTypeManager is static or accessible
            // Consider injecting IFileTypeManager if refactoring further
            return FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, file.FullName)
                .Where(x => x.Description == "Unknown") // Filter specifically for "Unknown" type
                .ToList();
        }

        private async Task<bool> ProcessFileTypes(List<FileTypes> fileTypes, string destFileName, FileInfo originalFile)
        {
            var allgood = true;
            foreach (var fileType in fileTypes)
            {
                var fileInfos = new FileInfo[] { new FileInfo(destFileName) };
                // Assuming PDFUtils and ShipmentUtils are static or accessible
                // Consider injecting IPdfUtils, IShipmentUtils if refactoring further
                var res = await InvoiceReader.InvoiceReader.ImportPDF(fileInfos, fileType).ConfigureAwait(false);
                if (!res.Any(x => x.Value.DocumentType.ToString() == FileTypeManager.EntryTypes.ShipmentInvoice && x.Value.Status == ImportStatus.Success))
                {
                    var res2 = await PDFUtils.ImportPDFDeepSeek(fileInfos, fileType).ConfigureAwait(false);
                    if (!res2.Any()
                        || res2.Any(x => x.Value.status != ImportStatus.Success))
                    {
                        NotifyUnknownPDF(originalFile, res2);
                        allgood = false;
                        continue; // Skip to next fileType if this one failed
                    }
                     // If DeepSeek succeeded, potentially update status or log
                }

                // Only create shipment if initial PDF import or DeepSeek was successful for this fileType
                 if (allgood) // Check if still good after potential DeepSeek
                 {
                    try
                    {
                        allgood = ShipmentUtils.CreateShipmentEmail(fileType, fileInfos);
                    }
                    catch(Exception ex)
                    {
                         Console.WriteLine($"Error creating shipment email for {originalFile.Name}: {ex.Message}");
                         // Decide if this error should mark 'allgood' as false
                         allgood = false;
                    }
                 }
            }

            return allgood; // Returns true only if all fileTypes processed successfully
        }

        private void NotifyUnknownPDF(FileInfo file, List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>> res2)
        {
            // Assuming Utils.Client and EmailDownloader are static or accessible
            // Consider injecting IEmailService if refactoring further
            try
            {
                var errorDetails = res2.FirstOrDefault(x => x.Value.status != ImportStatus.Success);
                var errorMessage = $"Unknown PDF Found: {file.Name}\r\n" +
                                   (errorDetails.Value.status != ImportStatus.Success ? $"Failed Step: {errorDetails.Value.DocumentType}" : "Details unavailable.");

                EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Unknown PDF Found",
                    EmailDownloader.EmailDownloader.GetContacts("Developer"),
                    errorMessage,
                    res2.Select(x => x.Value.FileName).Distinct().ToArray());
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error sending notification email for {file.Name}: {ex.Message}");
                 // Log error
            }
        }

        // --- New Method for Processing Shipment Folders ---
        public async Task ProcessShipmentFolders(ApplicationSettings appSetting)
        {
            var inputFolder = new DirectoryInfo(Path.Combine(appSetting.DataFolder, "ShipmentInput")); // Define input folder
            Console.WriteLine($"Checking for shipment folders in: {inputFolder.FullName}");

            if (!inputFolder.Exists)
            {
                Console.WriteLine($"Input folder '{inputFolder.FullName}' does not exist. Creating...");
                try
                {
                    inputFolder.Create();
                }
                catch (Exception ex)
                {
                     Console.WriteLine($"Error creating input folder: {ex.Message}");
                     return; // Cannot proceed
                }
            }

            foreach (var subfolder in inputFolder.GetDirectories())
            {
                Console.WriteLine($"Processing subfolder: {subfolder.Name}");
                var infoFilePath = Path.Combine(subfolder.FullName, "Info.txt");

                if (!File.Exists(infoFilePath))
                {
                    Console.WriteLine($"Skipping folder '{subfolder.Name}': Info.txt not found.");
                    continue;
                }

                string placeholderEmailId = null; // Use string for EmailId
                List<FileInfo> allFilesInFolder = null;

                try
                {
                    // --- Get FileType ---
                    FileTypes shipmentFolderType = null;
                    using (var coreCtx = new CoreEntitiesContext()) // Use Core context for FileTypes
                    {
                         // Assuming ID 1186 is stable, otherwise query by description
                         shipmentFolderType = coreCtx.FileTypes.Find(1186);
                    }

                    if (shipmentFolderType == null)
                    {
                        Console.WriteLine($"Error: FileType 'ShipmentFolder' (ID 1186) not found in database.");
                        continue; // Skip folder if FileType is missing
                    }

                    // --- Extract BL Number ---
                    string blNumber = null;
                    try
                    {
                        var lines = File.ReadAllLines(infoFilePath);
                        foreach (var line in lines)
                        {
                             if (string.IsNullOrWhiteSpace(line)) continue;
                             var parts = line.Split(new[] { ':' }, 2);
                             if (parts.Length == 2 && parts[0].Trim().Equals("BL", StringComparison.OrdinalIgnoreCase))
                             {
                                 blNumber = parts[1].Trim();
                                 break;
                             }
                        }
                        if (string.IsNullOrWhiteSpace(blNumber))
                        {
                            throw new ApplicationException("BL Number key not found or value is empty in Info.txt");
                        }
                         Console.WriteLine($"Extracted BL Number: {blNumber}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading BL number from Info.txt in folder '{subfolder.Name}': {ex.Message}");
                        continue; // Skip folder on error
                    }


                    // --- Create Placeholder Email & Attachments ---
                    allFilesInFolder = subfolder.GetFiles().ToList(); // Get all files for context and attachments

                    using (var coreCtx = new CoreEntitiesContext())
                    {
                        // Removed check for existing email

                        // Generate a unique string ID for the email
                        placeholderEmailId = Guid.NewGuid().ToString();

                        var email = new Emails(true) // Assuming constructor enables tracking
                        {
                            TrackingState = TrackingState.Added,
                            EmailId = placeholderEmailId, // Assign generated string ID
                            Subject = $"Shipment Folder: {blNumber}", // Set Subject
                            EmailDate = DateTime.Now, // Corrected: Use EmailDate property
                            ApplicationSettingsId = appSetting.ApplicationSettingsId
                        };
                        coreCtx.Emails.Add(email);
                        coreCtx.SaveChanges(); // Save Email first
                        Console.WriteLine($"Created placeholder Email record with ID: {placeholderEmailId}");

                        // Create Attachment records
                        foreach (var file in allFilesInFolder)
                        {
                            // Use correct Attachments entity and properties
                            var attachment = new CoreEntities.Business.Entities.Attachments(true)
                            {
                                TrackingState = TrackingState.Added,
                                EmailId = placeholderEmailId, // Link to the new Email record
                                FilePath = file.FullName
                                // DocumentCode and Reference are optional
                            };
                            coreCtx.Attachments.Add(attachment);
                        }
                        coreCtx.SaveChanges(); // Save Attachments
                        Console.WriteLine($"Created {allFilesInFolder.Count} Attachment records for EmailId: {placeholderEmailId}");
                    }

                    // --- Prepare Context and Execute Actions ---
                    shipmentFolderType.EmailId = placeholderEmailId; // Pass context (string)
                    shipmentFolderType.Data = new List<KeyValuePair<string, string>>(); // Initialize Data list

                    Console.WriteLine($"Executing actions for FileType {shipmentFolderType.Id}, EmailId {placeholderEmailId}...");
                    ImportUtils.ExecuteDataSpecificFileActions(shipmentFolderType, allFilesInFolder.ToArray(), appSetting);
                    ImportUtils.ExecuteNonSpecificFileActions(shipmentFolderType, allFilesInFolder.ToArray(), appSetting);
                    Console.WriteLine($"Finished actions for folder: {subfolder.Name}");

                    // --- Post-Processing (Example: Move to Archive) ---
                    try
                    {
                        var archiveFolder = new DirectoryInfo(Path.Combine(inputFolder.FullName, "Archive", subfolder.Name));
                        if (!archiveFolder.Parent.Exists) archiveFolder.Parent.Create();
                        if (archiveFolder.Exists)
                        {
                             // Handle potential name collision in archive
                             archiveFolder = new DirectoryInfo($"{archiveFolder.FullName}_{DateTime.Now:yyyyMMddHHmmss}");
                        }
                        subfolder.MoveTo(archiveFolder.FullName);
                        Console.WriteLine($"Moved processed folder '{subfolder.Name}' to archive.");
                    }
                    catch (Exception mvEx)
                    {
                         Console.WriteLine($"Error moving processed folder '{subfolder.Name}' to archive: {mvEx.Message}");
                         // Log error, maybe leave folder in place?
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Critical error processing folder '{subfolder.Name}': {ex.Message}\n{ex.StackTrace}");
                    // Log critical error, potentially move to an error folder
                    try
                    {
                         var errorFolder = new DirectoryInfo(Path.Combine(inputFolder.FullName, "Error", subfolder.Name));
                         if (!errorFolder.Parent.Exists) errorFolder.Parent.Create();
                          if (errorFolder.Exists) errorFolder = new DirectoryInfo($"{errorFolder.FullName}_{DateTime.Now:yyyyMMddHHmmss}");
                         subfolder.MoveTo(errorFolder.FullName);
                         Console.WriteLine($"Moved error folder '{subfolder.Name}' to error directory.");
                    }
                    catch(Exception mvErrEx)
                    {
                         Console.WriteLine($"Failed to move error folder '{subfolder.Name}': {mvErrEx.Message}");
                    }
                    continue; // Continue to next folder
                }
            } // End foreach subfolder
        } // End of ProcessShipmentFolders

    } // End of FolderProcessor class
} // End of namespace AutoBot