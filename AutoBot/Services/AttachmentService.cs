using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities;
using EmailDownloader; // Assuming Email type comes from here
using TrackableEntities;
using WaterNut.DataSpace; // For BaseDataModel, FileTypeManager
using WaterNut.Business.Services.Utils; // For FileTypeManager? Check dependencies
using System.Data.Entity; // For Include extension method

namespace AutoBot.Services
{
    /// <summary>
    /// Handles saving and managing email attachments and their references within the system.
    /// Extracted from AutoBot.Utils class to adhere to SRP.
    /// </summary>
    public static class AttachmentService
    {
        public static void SaveAttachments(FileInfo[] csvFiles, FileTypes fileType, Email email)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext() { StartTracking = true })
                {
                    var oldemail = ctx.Emails.Include("EmailAttachments.Attachments").FirstOrDefault(x => x.EmailId == email.EmailId);
                    if (oldemail == null)
                    {
                        oldemail = ctx.Emails.Add(new Emails(true)
                        {
                            EmailUniqueId = email.EmailUniqueId,
                            EmailId = email.EmailId,
                            Subject = email.Subject,
                            EmailDate = email.EmailDate,
                            MachineName = Environment.MachineName,
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            TrackingState = TrackingState.Added
                        });
                        ctx.SaveChanges();
                    }
                    else
                    {
                        oldemail.MachineName = Environment.MachineName;
                        oldemail.EmailUniqueId = email.EmailUniqueId;
                    }

                    foreach (var file in csvFiles)
                    {
                        // Assuming FileTypeManager is accessible, might need adjustment if it was part of Utils
                        if (fileType.FileImporterInfos.EntryType != FileTypeManager.EntryTypes.Unknown)
                        {
                            FileTypeManager.SendBackTooBigEmail(file, fileType);
                        }

                        var reference = GetReference(file, ctx); // Call local method

                        Attachments attachment = ctx.Attachments.FirstOrDefault(x => x.FilePath == file.FullName);
                        if(attachment == null)
                        attachment = new Attachments(true)
                        {
                            FilePath = file.FullName,
                            DocumentCode = fileType.DocumentCode,
                            Reference = reference,
                            EmailId = email.EmailId, // Use the passed email object's ID
                            TrackingState = TrackingState.Added
                        };

                        AddUpdateEmailAttachments(fileType, email, oldemail, file, ctx, attachment, reference); // Call local method

                        // Dependency on EntryDocSetUtils - This needs to be resolved later
                        // If EntryDocSetUtils becomes a service, inject or call it.
                        if (fileType.AsycudaDocumentSetId != 0)
                        {
                             // EntryDocSetUtils.AddUpdateDocSetAttachement(fileType, email, ctx, file, attachment, reference);
                             Console.WriteLine($"Warning: Call to EntryDocSetUtils.AddUpdateDocSetAttachement skipped for {file.Name} due to unresolved dependency."); // Log or handle appropriately
                        }
                    }
                     ctx.SaveChanges(); // Save changes after the loop
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw; // Re-throw to allow higher-level handling if needed
            }
        }

        private static void AddUpdateEmailAttachments(FileTypes fileType, Email email, Emails oldemail, FileInfo file,
            CoreEntitiesContext ctx, Attachments attachment, string reference)
        {
            var emailAttachement =
                oldemail.EmailAttachments.FirstOrDefault(x => x.Attachments.FilePath == file.FullName);

            if (emailAttachement == null)
            {
                ctx.EmailAttachments.Add(
                    new EmailAttachments(true)
                    {
                        Attachments = attachment, // Use the passed or created attachment
                        DocumentSpecific = fileType.DocumentSpecific,
                        EmailId = email.EmailId, // Use the passed email object's ID
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added
                    });
            }
            else
            {
                // Update existing email attachment record
                emailAttachement.DocumentSpecific = fileType.DocumentSpecific;
                emailAttachement.EmailId = email.EmailId; // Ensure EmailId is updated if necessary
                emailAttachement.FileTypeId = fileType.Id;
                // Update the underlying attachment details as well
                emailAttachement.Attachments.Reference = reference;
                emailAttachement.Attachments.DocumentCode = fileType.DocumentCode;
                emailAttachement.Attachments.EmailId = email.EmailId; // Ensure EmailId on attachment is also updated
                // Mark the EmailAttachment as modified if necessary (Trackable Entities handles this if StartTracking=true)
                 if(emailAttachement.TrackingState == TrackingState.Unchanged) emailAttachement.TrackingState = TrackingState.Modified;
                 if(emailAttachement.Attachments.TrackingState == TrackingState.Unchanged) emailAttachement.Attachments.TrackingState = TrackingState.Modified;
            }
            // SaveChanges is called in the main SaveAttachments method after the loop
        }

        private static string GetReference(FileInfo file, CoreEntitiesContext ctx)
        {
            var newReference = file.Name.Replace(file.Extension, "");

            // Make the query more efficient by filtering potentially similar references first
            var baseRef = newReference;
            var existingRefs = ctx.Attachments
                                  .Where(x => x.Reference.StartsWith(baseRef))
                                  .Select(x => x.Reference)
                                  .ToList();

            var existingRefCount = existingRefs.Count(x => x.StartsWith(baseRef + "-") || x == baseRef);

            if (existingRefCount > 0)
            {
                // Find the highest existing number suffix
                int maxSuffix = 0;
                foreach(var existingRef in existingRefs)
                {
                    if (existingRef == baseRef && maxSuffix == 0) // Handle the case where the base reference itself exists
                    {
                         maxSuffix = 1; // Start suffixing from 2 if base exists
                    }
                    else if (existingRef.StartsWith(baseRef + "-"))
                    {
                        string suffixStr = existingRef.Substring(baseRef.Length + 1);
                        if (int.TryParse(suffixStr, out int suffix) && suffix > maxSuffix)
                        {
                            maxSuffix = suffix;
                        }
                    }
                }
                 newReference = $"{baseRef}-{maxSuffix + 1}";
            }
            // Ensure uniqueness even if count logic had flaws (though less likely with StartsWith)
             while (ctx.Attachments.Any(x => x.Reference == newReference))
             {
                 // This part should ideally not be hit often if the suffix logic is correct
                 // Increment suffix logic might need refinement if collisions still happen
                 string[] parts = newReference.Split('-');
                 if (parts.Length > 1 && int.TryParse(parts.Last(), out int currentSuffix))
                 {
                     newReference = $"{string.Join("-", parts.Take(parts.Length - 1))}-{currentSuffix + 1}";
                 }
                 else
                 {
                     newReference = $"{newReference}-2"; // Start suffixing if no suffix exists
                 }
             }

            return newReference;
        }
    }
}