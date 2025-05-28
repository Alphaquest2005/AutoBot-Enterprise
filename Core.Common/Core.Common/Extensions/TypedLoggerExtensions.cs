using Serilog;
using Serilog.Context;
using Serilog.Events; // Required for LogEventLevel
using System;
using System.Collections.Generic; // Required for List
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Core.Common.Extensions // Your specified namespace
{
    public static class TypedLoggerExtensions
    {
        // Helper to push InvocationId to LogContext if provided and not null/empty
        private static IDisposable PushInvocationId(string invocationId) =>
            !string.IsNullOrEmpty(invocationId) ? LogContext.PushProperty("InvocationId", invocationId) : null;

        // Centralized helper to add common context properties (CallerMemberName, etc.)
        private static ILogger GetConfiguredLogger(ILogger logger, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            return logger.ForContext("MemberName", memberName)
                         .ForContext("SourceFilePath", sourceFilePath)
                         .ForContext("SourceLineNumber", sourceLineNumber);
        }

        // --- METHOD BOUNDARY ---
        public static void LogMethodEntry(this ILogger logger, string invocationId,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", LogCategory.MethodBoundary)
                    .Information("METHOD_ENTRY: MethodName: {MethodName}, InvocationId: {StoredInvocationId}", memberName, invocationId);
            }
        }

        public static void LogMethodExitSuccess(this ILogger logger, string invocationId, long durationMs,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", LogCategory.MethodBoundary)
                    .ForContext("DurationMs", durationMs)
                    .Information("METHOD_EXIT_SUCCESS: MethodName: {MethodName}, InvocationId: {StoredInvocationId}, DurationMs: {DurationMs}", memberName, invocationId, durationMs);
            }
        }

        public static void LogMethodExitFailure(this ILogger logger, string invocationId, long durationMs, Exception ex,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", LogCategory.MethodBoundary)
                    .ForContext("DurationMs", durationMs)
                    .ForContext("ExceptionType", ex?.GetType().FullName ?? "Unknown")
                    .Error(ex, "METHOD_EXIT_FAILURE: MethodName: {MethodName}, InvocationId: {StoredInvocationId}, DurationMs: {DurationMs}, ExceptionType: {ExceptionType}",
                           memberName, invocationId, durationMs, ex?.GetType().FullName ?? "Unknown");
            }
        }

        // --- ACTION BOUNDARY ---
        public static void LogActionStart(this ILogger logger, string invocationId, string actionName, string parentAction = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                var log = GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                            .ForContext("LogCategory", LogCategory.ActionBoundary)
                            .ForContext("ActionName", actionName);

                string messageTemplate = "ACTION_START: ActionName: {ActionName}, InvocationId: {StoredInvocationId}";
                var args = new List<object> { actionName, invocationId };

                if (!string.IsNullOrEmpty(parentAction))
                {
                    log = log.ForContext("ParentAction", parentAction);
                    messageTemplate += ", ParentAction: {ParentAction}";
                    args.Add(parentAction);
                }
                log.Information(messageTemplate, args.ToArray());
            }
        }

        public static void LogActionEndSuccess(this ILogger logger, string invocationId, string actionName, long durationMs,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", LogCategory.ActionBoundary)
                    .ForContext("ActionName", actionName)
                    .ForContext("DurationMs", durationMs)
                    .Information("ACTION_END_SUCCESS: ActionName: {ActionName}, InvocationId: {StoredInvocationId}, DurationMs: {DurationMs}",
                                 actionName, invocationId, durationMs);
            }
        }

        public static void LogActionEndFailure(this ILogger logger, string invocationId, string actionName, long durationMs, Exception ex,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", LogCategory.ActionBoundary)
                    .ForContext("ActionName", actionName)
                    .ForContext("DurationMs", durationMs)
                    .ForContext("ExceptionType", ex?.GetType().FullName ?? "Unknown")
                    .Error(ex, "ACTION_END_FAILURE: ActionName: {ActionName}, InvocationId: {StoredInvocationId}, DurationMs: {DurationMs}, ExceptionType: {ExceptionType}",
                           actionName, invocationId, durationMs, ex?.GetType().FullName ?? "Unknown");
            }
        }

        // --- INTERNAL STEP ---
        public static void LogInternalStep(this ILogger logger, string invocationId, string messageTemplate,
                                           [CallerMemberName] string memberName = "",
                                           [CallerFilePath] string sourceFilePath = "",
                                           [CallerLineNumber] int sourceLineNumber = 0,
                                           params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", LogCategory.InternalStep)
                    .Debug(messageTemplate, propertyValues);
            }
        }

        public static void LogInternalStepVerbose(this ILogger logger, string invocationId, string messageTemplate,
                                                  [CallerMemberName] string memberName = "",
                                                  [CallerFilePath] string sourceFilePath = "",
                                                  [CallerLineNumber] int sourceLineNumber = 0,
                                                  params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", LogCategory.InternalStep)
                    .Verbose(messageTemplate, propertyValues);
            }
        }

        // --- EXTERNAL CALL ---
        public static void LogExternalCallStart(this ILogger logger, string invocationId, string externalSystem, string operation, object parameters = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                var log = GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                            .ForContext("LogCategory", LogCategory.ExternalCall)
                            .ForContext("ExternalSystem", externalSystem)
                            .ForContext("Operation", operation);

                string messageTemplate = "EXTERNAL_CALL_START: ExternalSystem: {ExternalSystem}, Operation: {Operation}, InvocationId: {StoredInvocationId}";
                var args = new List<object> { externalSystem, operation, invocationId };

                if (parameters != null)
                {
                    log = log.ForContext("Parameters", parameters, destructureObjects: true);
                    messageTemplate += ", Parameters: {@Parameters}";
                }
                log.Information(messageTemplate, args.ToArray());
            }
        }

        public static void LogExternalCallEnd(this ILogger logger, string invocationId, string externalSystem, string operation, bool success, long durationMs, object result = null, Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                var log = GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                            .ForContext("LogCategory", LogCategory.ExternalCall)
                            .ForContext("ExternalSystem", externalSystem)
                            .ForContext("Operation", operation)
                            .ForContext("Success", success)
                            .ForContext("DurationMs", durationMs);

                string messageTemplate = "EXTERNAL_CALL_END: ExternalSystem: {ExternalSystem}, Operation: {Operation}, InvocationId: {StoredInvocationId}, Success: {Success}, DurationMs: {DurationMs}";
                var args = new List<object> { externalSystem, operation, invocationId, success, durationMs };

                if (success)
                {
                    if (result != null)
                    {
                        log = log.ForContext("Result", result, destructureObjects: true);
                        messageTemplate += ", Result: {@Result}";
                    }
                    log.Information(messageTemplate, args.ToArray());
                }
                else
                {
                    string exceptionTypeFullName = ex?.GetType().FullName ?? "Unknown";
                    log = log.ForContext("ExceptionType", exceptionTypeFullName);
                    messageTemplate += ", ExceptionType: {ExceptionType}";
                    args.Add(exceptionTypeFullName);
                    log.Error(ex, messageTemplate, args.ToArray());
                }
            }
        }

        // --- META LOG DIRECTIVE ---
        public static void LogMetaDirective(
            this ILogger logger,
            LogEventLevel level,
            string type, string context, string directive, string sourceIteration,
            string expectedChange = null, object details = null, string invocationId = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            using (PushInvocationId(invocationId))
            {
                var log = GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                            .ForContext("LogCategory", LogCategory.MetaLog)
                            .ForContext("MetaType", type)
                            .ForContext("MetaContext", context)
                            .ForContext("MetaDirective", directive)
                            .ForContext("SourceIteration", sourceIteration);

                var messageTemplateParts = new List<string> { "META_LOG_DIRECTIVE: Type: {MetaType}", "Context: {MetaContext}", "Directive: {MetaDirective}", "SourceIteration: {SourceIteration}" };
                var logArgs = new List<object> { type, context, directive, sourceIteration };

                if (!string.IsNullOrEmpty(expectedChange))
                {
                    log = log.ForContext("ExpectedChange", expectedChange);
                    messageTemplateParts.Add("ExpectedChange: {ExpectedChange}");
                    logArgs.Add(expectedChange);
                }
                if (details != null)
                {
                    log = log.ForContext("MetaDetails", details, destructureObjects: true);
                    messageTemplateParts.Add("MetaDetails: {@MetaDetails}");
                }

                log.Write(level, string.Join(", ", messageTemplateParts), logArgs.ToArray());
            }
        }
        public static void LogMetaDirective(this ILogger logger, string type, string context, string directive, string sourceIteration, string expectedChange = null, object details = null, string invocationId = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            logger.LogMetaDirective(LogEventLevel.Warning, type, context, directive, sourceIteration, expectedChange, details, invocationId, memberName, sourceFilePath, sourceLineNumber);
        }

        // --- GENERIC CATEGORIZED LOGGING ---
        public static void LogInfoCategorized(this ILogger logger, LogCategory category, string messageTemplate, string invocationId = null,
                                              [CallerMemberName] string memberName = "",
                                              [CallerFilePath] string sourceFilePath = "",
                                              [CallerLineNumber] int sourceLineNumber = 0,
                                              params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", category).Information(messageTemplate, propertyValues);
            }
        }
        public static void LogDebugCategorized(this ILogger logger, LogCategory category, string messageTemplate, string invocationId = null,
                                               [CallerMemberName] string memberName = "",
                                               [CallerFilePath] string sourceFilePath = "",
                                               [CallerLineNumber] int sourceLineNumber = 0,
                                               params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", category).Debug(messageTemplate, propertyValues);
            }
        }
        public static void LogVerboseCategorized(this ILogger logger, LogCategory category, string messageTemplate, string invocationId = null,
                                                 [CallerMemberName] string memberName = "",
                                                 [CallerFilePath] string sourceFilePath = "",
                                                 [CallerLineNumber] int sourceLineNumber = 0,
                                                 params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", category).Verbose(messageTemplate, propertyValues);
            }
        }
        public static void LogWarningCategorized(this ILogger logger, LogCategory category, string messageTemplate, string invocationId = null,
                                                 [CallerMemberName] string memberName = "",
                                                 [CallerFilePath] string sourceFilePath = "",
                                                 [CallerLineNumber] int sourceLineNumber = 0,
                                                 params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", category).Warning(messageTemplate, propertyValues);
            }
        }
        public static void LogWarningCategorized(this ILogger logger, LogCategory category, Exception ex, string messageTemplate, string invocationId = null,
                                                 [CallerMemberName] string memberName = "",
                                                 [CallerFilePath] string sourceFilePath = "",
                                                 [CallerLineNumber] int sourceLineNumber = 0,
                                                 params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", category).Warning(ex, messageTemplate, propertyValues);
            }
        }
        public static void LogErrorCategorized(this ILogger logger, LogCategory category, string messageTemplate, string invocationId = null,
                                               [CallerMemberName] string memberName = "",
                                               [CallerFilePath] string sourceFilePath = "",
                                               [CallerLineNumber] int sourceLineNumber = 0,
                                               params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", category).Error(messageTemplate, propertyValues);
            }
        }
        public static void LogErrorCategorized(this ILogger logger, LogCategory category, Exception ex, string messageTemplate, string invocationId = null,
                                               [CallerMemberName] string memberName = "",
                                               [CallerFilePath] string sourceFilePath = "",
                                               [CallerLineNumber] int sourceLineNumber = 0,
                                               params object[] propertyValues)
        {
            using (PushInvocationId(invocationId))
            {
                GetConfiguredLogger(logger, memberName, sourceFilePath, sourceLineNumber)
                    .ForContext("LogCategory", category).Error(ex, messageTemplate, propertyValues);
            }
        }
    }
}