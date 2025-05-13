using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks; // Added for Task
using CoreEntities.Business.Entities;
using MoreLinq;
using OCR.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class UpdateInvoice
    {
        public static async Task UpdateRegEx(FileTypes fileTypes, FileInfo[] files)
        {
            
            var regExCommands = RegExCommands(fileTypes);

            foreach (var info in files.Where(x => x.Extension == ".txt"))
            {
                var infoTxt = File.ReadAllText(info.FullName);
                var commands = Regex.Matches(infoTxt, @"(?<Command>\w+):\s(?<Params>.+?)($|\r)",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

                foreach (Match cmdinfo in commands)
                {
                    if (InvoiceReader.InvoiceReader.CommandsTxt.Contains(cmdinfo.Value)) continue;
                    var cmdName = cmdinfo.Groups["Command"].Value.Trim();

                    if (!regExCommands.ContainsKey(cmdName)) continue;

                    var cmdParamInfo = cmdinfo.Groups["Params"].Value;
                    var cmdParams = Regex.Matches(cmdParamInfo, @"(?<Param>\w+):\s?(?<Value>.*?)((, )|($|\r))",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                   
                    var cmdparamDic = new Dictionary<string, string>(WaterNut.DataSpace.Utils.ignoreCase);
                    foreach (Match m in cmdParams)
                    {
                        cmdparamDic.Add(m.Groups["Param"].Value.Trim(',', ' ', '\''), m.Groups["Value"].Value.Trim(',',' ','\''));
                    }

                    ValidateParams(cmdparamDic, regExCommands[cmdName].Params);
                    await Task.Run(() => regExCommands[cmdName].Action.Invoke(cmdparamDic)).ConfigureAwait(false);

                }

            }
        }

        private static Dictionary<string, (Action<Dictionary<string, string>> Action, string[] Params)> RegExCommands(FileTypes fileTypes)
        {
            var regExCommands = new Dictionary<string, (Action<Dictionary<string, string>> Action, string[] Params)>(WaterNut.DataSpace.Utils.ignoreCase)
            {
                {
                    "demo",
                    ((paramInfo) =>
                        {
                            try
                            {
                                using (var ctx = new OCRContext())
                                {
                                    ctx.SaveChanges();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        },
                        new string[] { "RegId", "Regex" }
                    )
                },
                {
                    "RequestInvoice",
                    (async (paramInfo) => { await RequestInvoice(paramInfo, fileTypes).ConfigureAwait(false); },
                        new string[] { "Name" }
                    )
                },
                {
                    "UpdateRegex",
                    ((paramInfo) =>
                        {
                            try
                            {
                                using (var ctx = new OCRContext())
                                {
                                    var regId = int.Parse(paramInfo["RegId"]);
                                    var reg = ctx.RegularExpressions.First(x => x.Id == regId);
                                    reg.RegEx = paramInfo["Regex"];
                                    if (paramInfo.ContainsKey("IsMultiline"))
                                    {
                                        reg.MultiLine = (paramInfo["IsMultiline"] == "True");
                                    }

                                    ctx.SaveChanges();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        },
                        new string[] { "RegId", "Regex" }
                    )
                },
                {
                    "AddFieldRegEx",
                    ((paramInfo) => { AddFieldRegEx(paramInfo); },
                        new string[] { "RegId", "Field", "Regex", "ReplaceRegex" }
                    )
                },
                {
                    "AddInvoice",
                    ((paramInfo) => { AddInvoice(paramInfo); },
                        new string[] { "Name", "IDRegex" }
                    )
                },
                {
                    "AddPart",
                    ((paramInfo) => { AddPart(paramInfo); },
                        new string[] { "Invoice", "Name", "StartRegex", "IsRecurring", "IsComposite" }
                    )
                },
                {
                    "AddLine",
                    ((paramInfo) => { AddLine(paramInfo); },
                        new string[] { "Invoice", "Part", "Name", "Regex" }
                    )
                },
                {
                    "UpdateLine",
                    ((paramInfo) => { UpdateLine(paramInfo); },
                        new string[] { "Invoice", "Part", "Name", "Regex" }
                    )
                },
                {
                    "AddFieldFormatRegex",
                    ((paramInfo) => { AddFieldFormatRegex(paramInfo); },
                        new string[] { "RegexId", "Keyword", "Regex", "ReplacementRegex" }
                    )
                },
            };
            return regExCommands;
        }

        private static void AddFieldFormatRegex(Dictionary<string, string> paramInfo)
        {
            try
            {

                using (var ctx = new OCRContext() { StartTracking = true })
                {
                    var pRegexId = int.Parse(paramInfo["RegexId"]);
                    var pKeyword = paramInfo["Keyword"];
                    var pRegex = paramInfo["Regex"];
                    var pRepRegex = paramInfo["ReplacementRegex"];
                    var pIsMultiLine = paramInfo.ContainsKey("RegexIsMultiLine")
                        ? bool.Parse(paramInfo["RegexIsMultiLine"])
                        : (bool?)null;

                    var pRepIsMultiLine = paramInfo.ContainsKey("ReplacementRegexIsMultiLine")
                        ? bool.Parse(paramInfo["ReplacementRegexIsMultiLine"])
                        : (bool?)null;

                 

                    var regex = GetRegex(ctx, pRegex, pIsMultiLine);
                    var repRegex = GetRegex(ctx, pRepRegex, pRepIsMultiLine);

                    var fields = ctx.Fields.Include(x => x.FormatRegEx).Where(x => x.Key == pKeyword && x.Lines.RegExId == pRegexId && !x.FormatRegEx.Any(z => z.RegExId == regex.Id && z.ReplacementRegExId == repRegex.Id)).ToList();

                    foreach (var field in fields)
                    {
                        field.FormatRegEx.Add(new FieldFormatRegEx(true){TrackingState = TrackingState.Added, RegEx = regex, ReplacementRegEx = repRegex});
                    }

                    ctx.SaveChanges();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static RegularExpressions GetRegex(OCRContext ctx, string pRegex, bool? pIsMultiLine)
        {
            var regex = ctx.RegularExpressions.FirstOrDefault(x => x.RegEx == pRegex);
            if (regex == null)
                regex = new RegularExpressions(true)
                    { TrackingState = TrackingState.Added, RegEx = pRegex, MultiLine = pIsMultiLine };
            return regex;
        }


        private static void AddFieldRegEx(Dictionary<string, string> paramInfo)
        {
            try
            {
                using (var ctx = new OCRContext())
                {
                    var regId = int.Parse(paramInfo["RegId"]);

                    var f = paramInfo["Field"];
                    var fields = ctx.Fields.Where(x =>
                            x.Key == f &&
                            x.Lines.RegExId == regId)
                        .ToList();
                    var reg = GetRegularExpressions(ctx, paramInfo["Regex"]);
                    var regRep = GetRegularExpressions(ctx, paramInfo["ReplaceRegex"]);
                    foreach (var field in fields)
                    {
                        var fr = Queryable.FirstOrDefault(ctx.OCR_FieldFormatRegEx, x => x.FieldId == field.Id
                                                                                                        && x.RegExId == reg.Id
                                                                                                        && x.ReplacementRegExId == regRep.Id);
                        if (fr == null)
                        {
                            fr = new FieldFormatRegEx()
                            {
                                Fields = field,
                                RegEx = reg, ReplacementRegEx = regRep,
                                TrackingState = TrackingState.Added
                            };
                            ctx.OCR_FieldFormatRegEx.Add(fr);
                        }
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void UpdateLine(Dictionary<string, string> paramInfo)
        {
            try
            {
                using (var ctx = new OCRContext() { StartTracking = true })
                {
                    var pInvoice = paramInfo["Invoice"];
                    var pPart = paramInfo["Part"];
                    var pLine = paramInfo["Name"];
                    var pRegex = paramInfo["Regex"];
                    var pIsMultiLine = paramInfo.ContainsKey("IsMultiLine") ? bool.Parse(paramInfo["IsMultiLine"]) : (bool?)null;

                    var line = ctx.Lines.Include(x => x.Fields).FirstOrDefault(x =>
                        x.Parts.Invoices.Name == pInvoice && x.Parts.PartTypes.Name == pPart && x.Name == pLine);
                    if (line == null) return;
                    var part = ctx.Parts.FirstOrDefault(x => x.Invoices.Name == pInvoice && x.PartTypes.Name == pPart);
                    var invoice = ctx.Invoices.FirstOrDefault(x => x.Name == pInvoice);

                    if (part == null) return;
                    if (invoice == null) return;

                    var regex = ctx.RegularExpressions.FirstOrDefault(x => x.RegEx == pRegex);
                    if (regex == null)
                        regex = new RegularExpressions(true)
                        { TrackingState = TrackingState.Added, RegEx = pRegex, MultiLine = pIsMultiLine };
                    line.RegularExpressions = regex;
                   

                    ctx.SaveChanges();

                    var fields = Regex.Matches(pRegex, @"\<(?<Keyword>\w+)\>",
                        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                    var fieldLst = new List<Fields>();
                    foreach (Match fieldMatch in fields)
                    {
                        var keyWord = fieldMatch.Groups["Keyword"].Value;
                        var fieldMappingsList = ctx.OCR_FieldMappings.Where(x => x.Key == keyWord).ToList();
                        
                        foreach (var fieldMap in fieldMappingsList)
                        {
                            Fields field = line.Fields.FirstOrDefault(x => x.Key == fieldMap.Key);
                            if (field == null)
                            {
                                field = new Fields(true)
                                {
                                    Key = keyWord,
                                    Field = fieldMap.Field,
                                    AppendValues = fieldMap.AppendValues,
                                    EntityType = fieldMap.EntityType,
                                    DataType = fieldMap.DataType,
                                    IsRequired = fieldMap.IsRequired,
                                    LineId = line.Id,
                                    TrackingState = TrackingState.Added
                                };
                                ctx.Fields.Add(field);
                            }
                            else
                            {
                                field.Field = fieldMap.Field;
                                field.AppendValues = fieldMap.AppendValues;
                                field.EntityType = fieldMap.EntityType;
                                field.DataType = fieldMap.DataType;
                                field.IsRequired = fieldMap.IsRequired;
                            }
                           

                            
                            ctx.SaveChanges();
                            fieldLst.Add(field);

                        }
                    }

                    var fieldIdLst = fieldLst.Select(x => x.Id).ToList();
                    var oldFields = ctx.Fields.Where(x => x.LineId == line.Id && fieldIdLst.All(z => z != x.Id))
                        .ToList();
                    ctx.Fields.RemoveRange(oldFields);
                    ctx.SaveChanges();



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddLine(Dictionary<string, string> paramInfo)
        {
            try
            {
                using (var ctx = new OCRContext() { StartTracking = true })
                {
                    var pInvoice = paramInfo["Invoice"];
                    var pPart = paramInfo["Part"];
                    var pLine = paramInfo["Name"];
                    var pRegex = paramInfo["Regex"];
                    var pIsMultiLine = paramInfo.ContainsKey("IsMultiLine")
                        ? bool.Parse(paramInfo["IsMultiLine"])
                        : (bool?)null;

                    var line = ctx.Lines.FirstOrDefault(x =>
                        x.Parts.Invoices.Name == pInvoice && x.Parts.PartTypes.Name == pPart && x.Name == pLine);


                    var part = ctx.Parts.FirstOrDefault(x => x.Invoices.Name == pInvoice && x.PartTypes.Name == pPart);
                    var invoice = ctx.Invoices.FirstOrDefault(x => x.Name == pInvoice);

                    if (part == null) return;
                    if (invoice == null) return;

                    var regex = ctx.RegularExpressions.FirstOrDefault(x => x.RegEx == pRegex);
                    if (regex == null)
                        regex = new RegularExpressions(true)
                            { TrackingState = TrackingState.Added, RegEx = pRegex, MultiLine = pIsMultiLine };
                    if (line == null)
                    {
                        line = new Lines(true)
                        {
                            TrackingState = TrackingState.Added, Parts = part, Name = pLine, RegularExpressions = regex,
                            Fields = new List<Fields>()
                        };
                        ctx.Lines.Add(line);
                    }
                    else
                    {
                        line.Parts = part;
                        line.RegularExpressions = regex;
                    }



                    ctx.SaveChanges();

                    var fields = Regex.Matches(pRegex, @"\<(?<Keyword>\w+)\>",
                        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

                    foreach (Match fieldMatch in fields)
                    {
                        var keyWord = fieldMatch.Groups["Keyword"].Value;
                        var fieldMappingsList = ctx.OCR_FieldMappings.Where(x => x.Key == keyWord && x.FileTypeId == invoice.FileTypeId).ToList();
                        foreach (var fieldMap in fieldMappingsList)
                        {
                            var field = ctx.Fields.FirstOrDefault(x =>
                                x.Key == fieldMap.Key && x.Field == fieldMap.Field &&
                                x.EntityType == fieldMap.EntityType && x.LineId == line.Id);
                            if (field != null) continue;
                            field = new Fields(true)
                            {
                                Key = keyWord,
                                Field = fieldMap.Field,
                                AppendValues = fieldMap.AppendValues,
                                EntityType = fieldMap.EntityType,
                                DataType = fieldMap.DataType,
                                IsRequired = fieldMap.IsRequired,
                                LineId = line.Id,
                                TrackingState = TrackingState.Added
                            };
                            ctx.Fields.Add(field);
                            ctx.SaveChanges();

                        }

                    }




                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddPart(Dictionary<string, string> paramInfo)
        {
            try
            {
                using (var ctx = new OCRContext())
                {
                    var pInvoice = paramInfo["Invoice"];
                    var pPart = paramInfo["Name"];
                    var pStartRegex = paramInfo["StartRegex"];
                    var pIsRecurring = bool.Parse(paramInfo["IsRecurring"]);
                    var pIsComposite = bool.Parse(paramInfo["IsComposite"]);
                    var pIsMultiLine = paramInfo.ContainsKey("IsMultiLine")? bool.Parse(paramInfo["IsMultiLine"]):(bool?)null;
                    var pParentPart = paramInfo.ContainsKey("ParentPart") ? paramInfo["ParentPart"] : null;

                    var part = ctx.Parts.Include(x => x.Start)
                        .FirstOrDefault(x => x.Invoices.Name == pInvoice && x.PartTypes.Name == pPart);
                    var invoice = ctx.Invoices.FirstOrDefault(x => x.Name == pInvoice);
                    
                    if (part != null) return;
                    if (invoice == null) return;
                    var parentPart = ctx.Parts.FirstOrDefault(x =>
                        x.PartTypes.Name == pParentPart && x.Invoices.Name == pInvoice);

                    if (pParentPart != null && parentPart == null) return;

                    var startRegex = ctx.RegularExpressions.FirstOrDefault(x => x.RegEx == pStartRegex);
                    if (startRegex == null)
                        startRegex = new RegularExpressions(true)
                            { TrackingState = TrackingState.Added, RegEx = pStartRegex, MultiLine = pIsMultiLine };
                    var partType = ctx.PartTypes.FirstOrDefault(x => x.Name == pPart);
                    if (partType == null)
                        partType = new PartTypes(true) { TrackingState = TrackingState.Added, Name = pPart };
                    part = new Parts(true) {
                        TrackingState = TrackingState.Added,
                        PartTypes = partType,
                        Invoices =invoice,
                        Start = new List<Start>(){new Start(true){TrackingState = TrackingState.Added, RegularExpressions = startRegex}},
                       
                    };

                   
                    if (pIsRecurring == true) part.RecuringPart = new RecuringPart(true) { IsComposite = pIsComposite , TrackingState = TrackingState.Added};
                    ctx.Parts.Add(part);
                    ctx.SaveChanges();
                    
                    if (pParentPart != null && parentPart != null)
                        ctx.ChildParts.Add(new ChildParts(true){ChildPart = part, ParentPart = parentPart, TrackingState = TrackingState.Added});

                    ctx.SaveChanges();

                    

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddInvoice(Dictionary<string, string> paramInfo)
        {
            try
            {
                using (var ctx = new OCRContext())
                {
                    var pInvoice = paramInfo["Name"];
                    var pIDRegex = paramInfo["IDRegex"];
                    var pDocumentType = paramInfo.ContainsKey("DocumentType") ? paramInfo["DocumentType"]: null;
                    var invoice = ctx.Invoices.Include(x => x.InvoiceIdentificatonRegEx)
                        .FirstOrDefault(x => x.Name == pInvoice);
                    if (invoice != null) return;
                    invoice = new Invoices() {Name = pInvoice,
                        InvoiceIdentificatonRegEx = new List<InvoiceIdentificatonRegEx>()
                        {
                            new InvoiceIdentificatonRegEx(true)
                            {
                                OCR_RegularExpressions = new RegularExpressions(true)
                                {
                                    RegEx = pIDRegex ,TrackingState = TrackingState.Added,
                                    
                                },
                                TrackingState = TrackingState.Added
                            }

                        }
                        , IsActive = true
                        , ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                        , TrackingState = TrackingState.Added
                        , FileTypeId = new CoreEntitiesContext().FileTypes.First(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId 
                            && x.FileImporterInfos.EntryType == (pDocumentType??FileTypeManager.EntryTypes.ShipmentInvoice) && x.FileImporterInfos.Format == FileTypeManager.FileFormats.PDF).Id

                    };
                    ctx.Invoices.Add(invoice);
                    ctx.SaveChanges();

                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Change signature to async Task
        private static async Task RequestInvoice(Dictionary<string, string> paramInfo, FileTypes fileTypes)
        {
            try
            {
                var lines = new List<RegularExpressions>();
                using (var ctx = new OCRContext())
                {
                    var iname = paramInfo["Name"];
                    var invoices = ctx.Invoices.Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions").Where(x => x.Name.Contains(iname)).ToList();

                    var pdfs = new DirectoryInfo(fileTypes.FilePath).GetFiles("*.pdf");
                    var files = new Dictionary<string, string>();
                    foreach (var pdf in pdfs)
                    {
                        // Await the async call
                        var str = await InvoiceReader.InvoiceReader.GetPdftxt(pdf.FullName).ConfigureAwait(false);
                        if(str.Length > 0) files.Add(pdf.FullName, str.ToString());
                    }
                    foreach (var invoice in invoices)
                    {
                        lines.AddRange(ctx.Parts.Where(x => x.Invoices.Name == invoice.Name).SelectMany(z => z.Start)
                            .Select(q => q.RegularExpressions).ToList());

                        lines.AddRange(ctx.Parts.Where(x => x.Invoices.Name == invoice.Name).SelectMany(z => z.ChildParts)
                            .SelectMany(q => q.ChildPart.Start.Select(p => p.RegularExpressions)).ToList());

                        lines.AddRange(ctx.Parts.Where(x => x.Invoices.Name == invoice.Name).SelectMany(z => z.Lines)
                            .Select(q => q.RegularExpressions).ToList());

                        lines.AddRange(ctx.Parts.Where(x => x.Invoices.Name == invoice.Name).SelectMany(z => z.ChildParts)
                            .SelectMany(q => q.ChildPart.Lines.Select(p => p.RegularExpressions)).ToList());


                        var body = $"Hey,\r\n\r\n Regex for {invoice.Name}'.\r\n\r\n\r\n" +
                                   $"{lines.DistinctBy(x => x.Id).Select(x => $"RegId: {x.Id} - Regex: {x.RegEx}").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n" + c)}\r\n\r\n" +
                                   "Thanks\r\n" +
                                   $"AutoBot" +
                                   $"\r\n" +
                                   $"\r\n" +
                                   InvoiceReader.InvoiceReader.CommandsTxt;

                        var res = files.Where(x => InvoiceReader.InvoiceReader.IsInvoiceDocument(invoice, x.Value, x.Key)).ToList();
                        
                        res.ForEach(x => File.WriteAllText(x.Key + ".txt", x.Value));
                        var res1 = res.Select(x => x.Key + ".txt").ToList().Union(res.Select(x => x.Key).ToList()).ToArray();
                       

                        await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client,null, "Invoice Template Not found!",
                             EmailDownloader.EmailDownloader.GetContacts("Developer"), body, res1).ConfigureAwait(false);

                        fileTypes.ProcessNextStep.Add("Kill");

                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static RegularExpressions GetRegularExpressions(OCRContext ctx, string paramInfo)
        {
            var reg = ctx.RegularExpressions.FirstOrDefault(x => x.RegEx == paramInfo);
            if (reg == null)
            {
                reg = new RegularExpressions() { RegEx = paramInfo, TrackingState = TrackingState.Added };
                ctx.RegularExpressions.Add(reg);
            }

            return reg;
        }

        private static void ValidateParams(Dictionary<string, string> dictionary, string[] paramInfo)
        {
            var missing = paramInfo.Where(x => !dictionary.ContainsKey(x)).ToList();
            if (missing.Any())
            {
                throw new ApplicationException(
                    $"Update Regex Params Missing: {missing.Aggregate((o, n) => o + ", " + n)}");
            }
        }
    }
}