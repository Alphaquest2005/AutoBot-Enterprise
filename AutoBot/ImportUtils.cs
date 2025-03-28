﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoBot;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;
using FileInfo = System.IO.FileInfo;

namespace AutoBotUtilities
{
    public class EmailTextProcessor
    {
        public static FileInfo[] Execute(FileInfo[] csvFiles, FileTypes fileType)
        {
            try
            {

                var res = new List<FileInfo>();

                foreach (var file in csvFiles)
                {
                    var emailMappings = File.ReadAllLines(file.FullName)
                        .Where(line => !line.ToLower().Contains("Not Found".ToLower()))
                        .Select(line => GetEmailMappings(fileType, line))
                        .Where(x => !string.IsNullOrEmpty(x.line))
                        .ToList();

                    if (emailMappings.Any()) res.Add(file);
                    var dbStatement = emailMappings.Select(linex =>
                        {

                            var str = linex.im.Select(im => GetMappingData(im, linex.line))
                                .Select(kp =>
                                {
                                    fileType.Data.Add(kp.InfoData);

                                    // --- BEGIN ADDED LOGGING ---
                                        Console.WriteLine($"--- EmailTextProcessor: Added to fileType.Data - Key: '{kp.InfoData.Key}', Value: '{kp.InfoData.Value}'");
                                        // --- END ADDED LOGGING ---

                                    return kp;
                                })
                                .Select(kp => GetDbStatement(fileType, kp))
                                .DefaultIfEmpty("")
                                .Aggregate((o, n) => $"{o}\r\n{n}");
                            return str;


                        })
                        .DefaultIfEmpty("")
                        .Aggregate((o, n) => $"{o}\r\n{n}").Trim();


                    if (!string.IsNullOrEmpty(dbStatement))
                    {
                        new CoreEntitiesContext().Database.ExecuteSqlCommand(dbStatement);
                    }

                }

                return res.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static (string line, List<(EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field)> im) GetEmailMappings(FileTypes fileType, string line)
        {
            var im = fileType.EmailInfoMappings.SelectMany(x => x.InfoMapping.InfoMappingRegEx.Select(z =>
                (
                    EmailMapping: x,
                    RegEx: z,
                    Key: Regex.Match(line, z.KeyRegX, RegexOptions.IgnoreCase | RegexOptions.Multiline),
                    Field: Regex.Match(line, z.FieldRx, RegexOptions.IgnoreCase | RegexOptions.Multiline)
                )))
                .Where(z => z.Key.Success && z.Field.Success).ToList();
            return (line,im);
        }

        private static ((EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field) InfoMapping, KeyValuePair<string, string> InfoData) GetMappingData((EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field) x, string line)
        {
                try
                {
                    var key = string.IsNullOrEmpty(x.Key.Groups["Key"].Value.Trim())
                        ? x.RegEx.InfoMapping.Key
                        : x.RegEx.KeyReplaceRx == null
                            ? x.Key.Groups["Key"].Value.Trim()
                            : Regex.Match(
                                    Regex.Replace(line, x.RegEx.KeyRegX, x.RegEx.KeyReplaceRx,
                                        RegexOptions.IgnoreCase), x.RegEx.KeyRegX,
                                    RegexOptions.IgnoreCase)
                                .Value.Trim();

                    var value = string.IsNullOrEmpty(x.Field.Groups["Value"].Value.Trim())
                        ? x.Field.Groups[0].Value.Trim()
                        : x.RegEx.FieldReplaceRx == null
                            ? x.Field.Groups["Value"].Value.Trim()
                            : Regex.Match(
                                    Regex.Replace(line, x.RegEx.FieldRx, x.RegEx.FieldReplaceRx,
                                        RegexOptions.IgnoreCase), x.RegEx.FieldRx,
                                    RegexOptions.IgnoreCase)
                                .Value.Trim();

                    return (InfoMapping: x,InfoData:  new KeyValuePair<string, string>(key, value));


                }
                catch (Exception)
                {
                    throw;
                }
            
            
        }

        private static string GetDbStatement(FileTypes fileType, ((EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field) InfoMapping, KeyValuePair<string, string> InfoData) ikp) =>
            $@" Update {ikp.InfoMapping.RegEx.InfoMapping.EntityType} Set {ikp.InfoMapping.RegEx.InfoMapping.Field} = '{ReplaceSpecialChar(ikp.InfoData.Value,
                "")}' Where {ikp.InfoMapping.RegEx.InfoMapping.EntityKeyField} = '{fileType.Data.First(z => z.Key == ikp.InfoMapping.RegEx.InfoMapping.EntityKeyField).Value}';";

        public static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s\-]+", rstring);
        }
    }

    public class ImportUtils
    {
        public static void ExecuteDataSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            try
            {
                var missingActions = fileType.FileTypeActions.Where(x => x.Actions.IsDataSpecific == true
                                                                         && !FileUtils.FileActions.ContainsKey(x.Actions.Name)).ToList();

                if (missingActions.Any())
                {
                    throw new ApplicationException(
                        $"The following actions were missing: {missingActions.Select(x => x.Actions.Name).Aggregate((old, current) => old + ", " + current)}");
                }
                // --- MODIFICATION START: Query DB directly for ordered actions ---
                using (var ctx = new CoreEntitiesContext()) // Use a new context for fresh data
                {
                    var orderedActions = ctx.FileTypeActions
                        .Include(fta => fta.Actions) // Eager load Actions
                        .Where(fta => fta.FileTypeId == fileType.Id)
                        .Where(fta => fta.Actions.IsDataSpecific == true) // Original filter
                        .Where(fta => (fta.AssessIM7.Equals(null) && fta.AssessEX.Equals(null)) || // Original filter
                                      (appSetting.AssessIM7 == fta.AssessIM7 ||
                                       appSetting.AssessEX == fta.AssessEX))
                        .Where(fta => fta.Actions.TestMode == // Original filter
                                      (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                        .OrderBy(fta => fta.Id)
                        .Include(fileTypeActions => fileTypeActions.Actions) // Order by ID from DB
                        .ToList(); // Execute query

                    // Now iterate through 'orderedActions'
                    orderedActions
                        .Where(fta => FileUtils.FileActions.ContainsKey(fta.Actions.Name)) // Ensure action exists in dictionary
                        .Select(fta => (fta.Actions.Name, FileUtils.FileActions[fta.Actions.Name])) // Get action delegate
                        .ToList()
                        .ForEach(actionTuple => { ExecuteActions(fileType, files, actionTuple); }); // Execute
                }
                // --- MODIFICATION END ---
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId,BaseDataModel.GetClient(), $"Bug Found",
                    $"{e.Message}\r\n{e.StackTrace}", EmailDownloader.EmailDownloader.GetContacts("Developer"),
                    Array.Empty<string>());
            }
        }


        public static void ExecuteEmailMappingActions(EmailMapping emailMapping, FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
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
                    .ForEach(x => { ExecuteActions(fileType, files, x); });

            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId, BaseDataModel.GetClient(), $"Bug Found",
                    $"{e.Message}\r\n{e.StackTrace}", EmailDownloader.EmailDownloader.GetContacts("Developer"),
                    Array.Empty<string>());
            }
        }

        private static void ExecuteActions(FileTypes fileType, FileInfo[] files,
            (string Name, Action<FileTypes, FileInfo[]> Action) x)
        {
            try
            {
                if (fileType.ProcessNextStep != null && fileType.ProcessNextStep.Any())
                {
                    var isContinue = false;
                    while (fileType.ProcessNextStep.Any())
                    {
                        var res = fileType.ProcessNextStep.Select(z => new {Name = z, Action = FileUtils.FileActions[z]})
                            .ToList();
                        if (res.First().Name == "Continue")
                        {
                            isContinue = true;
                            break;
                        }
                        res.First().Action.Invoke(fileType, files);
                        fileType.ProcessNextStep.RemoveAt(0);
                    }
                    if(!isContinue) return;
                }

                Console.WriteLine($"Executing -->> {x.Name}");
                x.Action.Invoke(fileType, files);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void ExecuteNonSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)
        {
            try
            {
                // Check for missing action implementations first (using cached FileTypeActions is fine here)
                var missingActionsCheck = fileType.FileTypeActions
                                           .Where(x => (x.Actions.IsDataSpecific == null || x.Actions.IsDataSpecific != true) && !FileUtils.FileActions.ContainsKey(x.Actions.Name))
                                           .ToList();
                if (missingActionsCheck.Any())
                {
                    // Log or handle missing non-specific actions if necessary, maybe less critical than specific ones
                    Console.WriteLine($"WARNING: Non-specific actions missing implementation: {missingActionsCheck.Select(x => x.Actions.Name).Aggregate((old, current) => old + ", " + current)}");
                }

                // --- MODIFICATION START: Query DB directly for ordered actions ---
                using (var ctx = new CoreEntitiesContext()) // Use a new context for fresh data
                {
                    var orderedActions = ctx.FileTypeActions
                                            .Include(fta => fta.Actions) // Eager load Actions
                                            .Where(fta => fta.FileTypeId == fileType.Id)
                                            .Where(fta => fta.Actions.IsDataSpecific == null || fta.Actions.IsDataSpecific != true) // Original filter
                                            .Where(fta => (fta.AssessIM7.Equals(null) && fta.AssessEX.Equals(null)) || // Original filter
                                                        (appSetting.AssessIM7 == fta.AssessIM7 ||
                                                         appSetting.AssessEX == fta.AssessEX))
                                            .Where(fta => fta.Actions.TestMode == // Original filter
                                                        (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                                            .OrderBy(fta => fta.Id) // Order by ID from DB
                                            .ToList(); // Execute query

                    // Now iterate through 'orderedActions'
                    orderedActions
                       .Where(fta => FileUtils.FileActions.ContainsKey(fta.Actions.Name)) // Ensure action exists in dictionary
                       .Select(fta => (fta.Actions.Name, FileUtils.FileActions[fta.Actions.Name])) // Get action delegate
                       .ToList()
                       .ForEach(actionTuple => { ExecuteActions(fileType, files, actionTuple); }); // Execute
                }
                // --- MODIFICATION END ---
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(BaseDataModel.GetClient(), null, $"Bug Found in ExecuteNonSpecificFileActions",
                     EmailDownloader.EmailDownloader.GetContacts("Developer"), $"{e.Message}\r\n{e.StackTrace}",
                    Array.Empty<string>());
                // Consider re-throwing if needed: throw;
            }
        }
    }
}