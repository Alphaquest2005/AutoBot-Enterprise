using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoBot;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;
using FileInfo = System.IO.FileInfo;

namespace AutoBotUtilities
{
    public class ImportUtils
    {
        public static FileInfo[] TrySaveFileInfo(FileInfo[] csvFiles, FileTypes fileType)
        {
            try
            {

                var res = new List<FileInfo>();
                foreach (var file in csvFiles)
                {
                    var fileTxt = File.ReadAllLines(file.FullName);
                    string dbStatement = "";
                    foreach (var line in fileTxt)
                    {
                        if (line.ToLower().Contains("Not Found".ToLower())) continue;
                        var im = fileType.EmailInfoMappings.SelectMany(x => x.InfoMapping.InfoMappingRegEx.Select(z => new
                            {
                                Em = x,
                                Rx = z,
                                Key = Regex.Match(line, z.KeyRegX, RegexOptions.IgnoreCase | RegexOptions.Multiline),
                                Field = Regex.Match(line, z.FieldRx, RegexOptions.IgnoreCase | RegexOptions.Multiline)
                            }))
                            .Where(z => z.Key.Success && z.Field.Success).ToList();

                        if (!im.Any()) continue;
                        if (!res.Contains(file)) res.Add(file);

                        im.ForEach(x =>
                        {
                            try
                            {
                                var key = string.IsNullOrEmpty(x.Key.Groups["Key"].Value.Trim())
                                    ? x.Rx.InfoMapping.Key
                                    : x.Rx.KeyReplaceRx == null
                                        ? x.Key.Groups["Key"].Value.Trim()
                                        : Regex.Match(
                                                Regex.Replace(line, x.Rx.KeyRegX, x.Rx.KeyReplaceRx,
                                                    RegexOptions.IgnoreCase), x.Rx.KeyRegX,
                                                RegexOptions.IgnoreCase)
                                            .Value.Trim();

                                var value = string.IsNullOrEmpty(x.Field.Groups["Value"].Value.Trim())
                                    ? x.Field.Groups[0].Value.Trim()
                                    : x.Rx.FieldReplaceRx == null
                                        ? x.Field.Groups["Value"].Value.Trim()
                                        : Regex.Match(
                                                Regex.Replace(line, x.Rx.FieldRx, x.Rx.FieldReplaceRx,
                                                    RegexOptions.IgnoreCase), x.Rx.FieldRx,
                                                RegexOptions.IgnoreCase)
                                            .Value.Trim();
                                fileType.Data.Add(
                                    new KeyValuePair<string, string>(key, value));

                                if (x.Em.UpdateDatabase == true)
                                {
                                    dbStatement +=
                                        $@" Update {x.Rx.InfoMapping.EntityType} Set {x.Rx.InfoMapping.Field} = '{
                                            ReplaceSpecialChar(value,
                                                "")
                                        }' Where {x.Rx.InfoMapping.EntityKeyField} = '{
                                            fileType.Data.First(z => z.Key == x.Rx.InfoMapping.EntityKeyField).Value}';";
                                }
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        });


                    }

                    if (!string.IsNullOrEmpty(dbStatement))
                        new CoreEntitiesContext().Database.ExecuteSqlCommand(dbStatement);
                }

                return res.ToArray();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s\-]+", rstring);
        }

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

                fileType.FileTypeActions.Where(x => x.Actions.IsDataSpecific == true).OrderBy(x => x.Id)
                    .Where(x => (x.AssessIM7.Equals(null) && x.AssessEX.Equals(null)) ||
                                (appSetting.AssessIM7 == x.AssessIM7 ||
                                 appSetting.AssessEX == x.AssessEX))
                    .Where(x => x.Actions.TestMode ==
                                (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                    .Select<FileTypeActions, (string Name, Action<FileTypes, FileInfo[]>)>(x =>  (x.Actions.Name, FileUtils.FileActions[x.Actions.Name])).ToList()
                    .ForEach(x => { ExecuteActions(fileType, files, x); });

            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId,BaseDataModel.GetClient(), $"Bug Found",
                    $"{e.Message}\r\n{e.StackTrace}",new[] { "Joseph@auto-brokerage.com" },
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
                    while (fileType.ProcessNextStep.Any())
                    {
                        var res = fileType.ProcessNextStep.Select(z => new {Name = z, Action = FileUtils.FileActions[z]})
                            .ToList();
                        res.First().Action.Invoke(fileType, files);
                        fileType.ProcessNextStep.RemoveAt(0);
                    }

                    return;
                }


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
                fileType.FileTypeActions.Where(x => x.Actions.IsDataSpecific == null || x.Actions.IsDataSpecific != true).OrderBy(x => x.Id)
                    .Where(x => (x.AssessIM7.Equals(null) && x.AssessEX.Equals(null)) ||
                                (appSetting.AssessIM7 == x.AssessIM7 ||
                                 appSetting.AssessEX == x.AssessEX))
                    .Where(x => x.Actions.TestMode ==
                                (BaseDataModel.Instance.CurrentApplicationSettings.TestMode))
                    .Select<FileTypeActions, (string Name, Action<FileTypes, FileInfo[]>)>(x => (x.Actions.Name , FileUtils.FileActions[x.Actions.Name])).ToList()
                    .ForEach(x =>  ExecuteActions(fileType, files, x));
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(BaseDataModel.GetClient(), null, $"Bug Found",
                    new[] { "Joseph@auto-brokerage.com" }, $"{e.Message}\r\n{e.StackTrace}",
                    Array.Empty<string>());
            }
        }
    }
}