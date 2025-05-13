namespace AutoBotUtilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CoreEntities.Business.Entities;

// ReSharper disable once HollowTypeName
public class EmailTextProcessor
{
    public static async Task<FileInfo[]> Execute(FileInfo[] csvFiles, FileTypes fileType)
    {
        string dbStatement = null;
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
                dbStatement = emailMappings.Select(linex =>
                        {

                            var str = linex.im.Select(im => GetMappingData(im, linex.line))
                                .Select(kp =>
                                    {
                                        if(kp.InfoData.Key == "Currency" || kp.InfoData.Key == "FreightCurrency")
                                        {
                                            if(kp.InfoData.Value == "US")
                                            {
                                                kp.InfoData = new KeyValuePair<string, string>(kp.InfoData.Key, "USD");
                                            }
                                        }
                                    
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
                    await AutoBot.EntryDocSetUtils.SyncConsigneeInDB(fileType, csvFiles).ConfigureAwait(false);
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
        var im = fileType.EmailInfoMappings
            //.Where(x => x.UpdateDatabase == true) took out because consignee address and code not going into database so have to be set to false
            .SelectMany(x => x.InfoMapping.InfoMappingRegEx.Select(z =>
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

    private static string GetDbStatement(FileTypes fileType,
                                         ((EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field) InfoMapping,
                                             KeyValuePair<string, string> InfoData) ikp) =>
        ikp.InfoMapping.EmailMapping.UpdateDatabase == true
            ? $@" Update {ikp.InfoMapping.RegEx.InfoMapping.EntityType} Set {ikp.InfoMapping.RegEx.InfoMapping.Field} = '{ReplaceSpecialChar(ikp.InfoData.Value,
                "")}' Where {ikp.InfoMapping.RegEx.InfoMapping.EntityKeyField} = '{fileType.Data.First(z => z.Key == ikp.InfoMapping.RegEx.InfoMapping.EntityKeyField).Value}';"
            : null;

    public static string ReplaceSpecialChar(string msgSubject, string rstring)
    {
        return Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s\-]+", rstring);
    }
}