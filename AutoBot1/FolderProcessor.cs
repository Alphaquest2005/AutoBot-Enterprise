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
                var res = await PDFUtils.ImportPDF(fileInfos, fileType).ConfigureAwait(false);
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
                        ShipmentUtils.CreateShipmentEmail(fileType, fileInfos);
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
    }
}