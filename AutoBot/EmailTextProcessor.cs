namespace AutoBotUtilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CoreEntities.Business.Entities;

// Serilog usings
using Serilog;
using Serilog.Context;

// ReSharper disable once HollowTypeName
public class EmailTextProcessor
{
    private readonly ILogger _logger;
    
    public EmailTextProcessor(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<FileInfo[]> Execute(FileInfo[] csvFiles, FileTypes fileType)
    {
        string operationName = nameof(Execute);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Use System.Diagnostics.Stopwatch

        // Assuming InvocationId is pushed to LogContext by the caller of Execute
        // If not, it should be generated here and pushed to context.
        // For now, assuming caller handles InvocationId context.

        _logger.Information("ACTION_START: {ActionName}. FileTypeId: {FileTypeId}, FileCount: {FileCount}",
            operationName, fileType?.Id, csvFiles?.Length ?? 0);

        string dbStatement = null;
        try
        {
            var res = new List<FileInfo>();

            _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Processing {FileCount} CSV files.", operationName, "FileProcessing", csvFiles?.Length ?? 0);

            foreach (var file in csvFiles)
            {
                
                    _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Reading lines from file {FileName}.", operationName, "ReadFile", file.Name);
                    // INVOKING_OPERATION for File.ReadAllLines
                    _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "File.ReadAllLines", "SYNC_EXPECTED");
                    var lines = File.ReadAllLines(file.FullName);
                    _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "File.ReadAllLines", "Sync call returned.");
                    _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Read {LineCount} lines from file {FileName}.", operationName, "ReadFile", lines.Length, file.Name);

                    _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Getting email mappings from lines.", operationName, "GetMappings");
                    var emailMappings = lines
                        .Where(line => !line.ToLower().Contains("Not Found".ToLower()))
                        .Select(line => GetEmailMappings(_logger, fileType, line)) // Pass _logger
                        .Where(x => !string.IsNullOrEmpty(x.line))
                        .ToList();

                    if (!emailMappings.Any())
                    {
                         // Empty Collection Warning
                         _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection 'emailMappings'. Item count: 0. {EmptyCollectionExpectation}",
                             operationName, "GetMappings", "Expected mappings but found none after filtering.");
                    }


                    if (emailMappings.Any())
                    {
                        res.Add(file);
                        _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Found {MappingCount} email mappings in file {FileName}.", operationName, "GetMappings", emailMappings.Count, file.Name);
                    }
                    else
                    {
                         _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): No email mappings found in file {FileName}.", operationName, "GetMappings", file.Name);
                    }


                    _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Generating DB statement from mappings.", operationName, "GenerateDbStatement");
                    dbStatement = emailMappings.Select(linex =>
                            {

                                var str = linex.im.Select(im => GetMappingData(_logger, im, linex.line)) // Pass _logger
                                    .Select(kp =>
                                        {
                                            if(kp.InfoData.Key == "Currency" || kp.InfoData.Key == "FreightCurrency")
                                            {
                                                if(kp.InfoData.Value == "US")
                                                {
                                                    kp.InfoData = new KeyValuePair<string, string>(kp.InfoData.Key, "USD");
                                                    _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Corrected currency code from 'US' to 'USD' for key '{Key}'.", operationName, "CurrencyCorrection", kp.InfoData.Key);
                                                }
                                            }

                                            fileType.Data.Add(kp.InfoData);

                                            // Replaced Console.WriteLine with Serilog
                                            _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Added to fileType.Data - Key: '{Key}', Value: '{Value}'.",
                                                operationName, "AddFileData", kp.InfoData.Key, kp.InfoData.Value);

                                            return kp;

                                        })
                                    .Select(kp => GetDbStatement(_logger, fileType, kp)) // Pass _logger
                                    .DefaultIfEmpty("")
                                    .Aggregate((o, n) => $"{o}\r\n{n}");
                                return str;


                            })
                        .DefaultIfEmpty("")
                        .Aggregate((o, n) => $"{o}\r\n{n}").Trim();

                    _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Generated DB statement (length: {StatementLength}).", operationName, "GenerateDbStatement", dbStatement.Length);


                    if (!string.IsNullOrEmpty(dbStatement))
                    {
                        _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Executing DB statement.", operationName, "ExecuteDbStatement");
                        // INVOKING_OPERATION for SyncConsigneeInDB
                        _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.SyncConsigneeInDB", "ASYNC_EXPECTED");
                        await AutoBot.EntryDocSetUtils.SyncConsigneeInDB(fileType, csvFiles, _logger).ConfigureAwait(false);
                        _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "EntryDocSetUtils.SyncConsigneeInDB", "Async call completed (await).");

                        // INVOKING_OPERATION for ExecuteSqlCommand
                        _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "CoreEntitiesContext.Database.ExecuteSqlCommand", "SYNC_EXPECTED");
                        new CoreEntitiesContext().Database.ExecuteSqlCommand(dbStatement);
                        _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "CoreEntitiesContext.Database.ExecuteSqlCommand", "Sync call returned.");
                        _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): DB statement executed successfully.", operationName, "ExecuteDbStatement");
                    }
                    else
                    {
                         _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): No DB statement to execute for file {FileName}.", operationName, "ExecuteDbStatement", file.Name);
                    }
                
            } // End foreach file

            stopwatch.Stop();
            _logger.Information("ACTION_END_SUCCESS: {ActionName}. Duration: {TotalObservedDurationMs}ms", operationName, stopwatch.ElapsedMilliseconds);
            return res.ToArray();
        }
        catch (Exception ex) // Catch specific exception variable
        {
            stopwatch.Stop();
            // Log the exception before re-throwing
            _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                operationName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw; // Re-throw the original exception
        }
    }

    private static (string line, List<(EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field)> im) GetEmailMappings(ILogger log, FileTypes fileType, string line)
    {
        string methodName = nameof(GetEmailMappings);
        log.Debug("METHOD_ENTRY: {MethodName}. Context: {{FileTypeId: {FileTypeId}, LineLength: {LineLength}}}",
            methodName, fileType?.Id, line?.Length ?? 0);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // INVOKING_OPERATION for LINQ/Regex operations
            log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "LINQ/Regex operations to find email mappings", "SYNC_EXPECTED");
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
            log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "LINQ/Regex operations to find email mappings", "Sync call returned.");

            stopwatch.Stop();
            log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Found {MappingCount} mappings.",
                methodName, stopwatch.ElapsedMilliseconds, im.Count);
            return (line,im);
        }
        catch (Exception ex) // Catch specific exception variable
        {
            stopwatch.Stop();
            // Log the exception before re-throwing
            log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw; // Re-throw the original exception
        }
    }

    private static ((EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field) InfoMapping, KeyValuePair<string, string> InfoData) GetMappingData(ILogger log, (EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field) x, string line)
    {
        string methodName = nameof(GetMappingData);
        log.Debug("METHOD_ENTRY: {MethodName}. Context: {{LineLength: {LineLength}}}",
            methodName, line?.Length ?? 0);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // INVOKING_OPERATION for Regex operations
            log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Regex operations to extract key and value", "SYNC_EXPECTED");
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
            log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "Regex operations to extract key and value", "Sync call returned.");

            stopwatch.Stop();
            log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Extracted Key: '{Key}', Value: '{Value}'.",
                methodName, stopwatch.ElapsedMilliseconds, key, value);
            return (InfoMapping: x,InfoData:  new KeyValuePair<string, string>(key, value));


        }
        catch (Exception ex) // Catch specific exception variable
        {
            stopwatch.Stop();
            // Log the exception before re-throwing
            log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw; // Re-throw the original exception
        }


    }

    private static string GetDbStatement(ILogger log, FileTypes fileType,
                                         ((EmailInfoMappings EmailMapping, InfoMappingRegEx RegEx, Match Key, Match Field) InfoMapping,
                                             KeyValuePair<string, string> InfoData) ikp)
    {
        string methodName = nameof(GetDbStatement);
        log.Debug("METHOD_ENTRY: {MethodName}. Context: {{EntityType: {EntityType}, Field: {Field}}}",
            methodName, ikp.InfoMapping.RegEx.InfoMapping.EntityType, ikp.InfoMapping.RegEx.InfoMapping.Field);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // INVOKING_OPERATION for string formatting and ReplaceSpecialChar
            log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Generate DB statement string", "SYNC_EXPECTED");
            var result = ikp.InfoMapping.EmailMapping.UpdateDatabase == true
                ? $@" Update {ikp.InfoMapping.RegEx.InfoMapping.EntityType} Set {ikp.InfoMapping.RegEx.InfoMapping.Field} = '{ReplaceSpecialChar(ikp.InfoData.Value,
                    "")}' Where {ikp.InfoMapping.RegEx.InfoMapping.EntityKeyField} = '{fileType.Data.First(z => z.Key == ikp.InfoMapping.RegEx.InfoMapping.EntityKeyField).Value}';"
                : null;
            log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "Generate DB statement string", "Sync call returned.");

            stopwatch.Stop();
            log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms. Statement generated: {StatementGenerated}",
                methodName, stopwatch.ElapsedMilliseconds, result != null);
            return result;
        }
        catch (Exception ex) // Catch specific exception variable
        {
            stopwatch.Stop();
            // Log the exception before re-throwing
            log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw; // Re-throw the original exception
        }
    }

    public static string ReplaceSpecialChar(string msgSubject, string rstring)
    {
        // This is a simple utility method, maybe not worth full METHOD_ENTRY/EXIT logs
        // but can add a debug log if needed.
        // string methodName = nameof(ReplaceSpecialChar);
        // __logger.Debug("METHOD_ENTRY: {MethodName}. SubjectLength: {SubjectLength}", methodName, msgSubject?.Length ?? 0);
        // var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // INVOKING_OPERATION for Regex.Replace
        // __logger.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Regex.Replace in ReplaceSpecialChar", "SYNC_EXPECTED");
        var result = Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s\-]+", rstring);
        // __logger.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. ({AsyncGuidance})", "Regex.Replace in ReplaceSpecialChar", "Sync call returned.");

        // stopwatch.Stop();
        // __logger.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.", methodName, stopwatch.ElapsedMilliseconds);
        return result;
    }
}