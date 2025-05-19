using Core.Common.Extensions; // Assuming LogCategory is in this namespace
using Serilog.Events;       // Required for LogEventLevel
using System.Collections.Generic;

namespace Core.Common.Extensions
{
    public static class LogFilterState
    {
        /// <summary>
        /// Defines the minimum Serilog LogEventLevel for each LogCategory to be generally included in the output.
        /// This is used when no specific TargetSourceContextForDetails is active, or for logs outside the targeted context.
        /// </summary>
        public static Dictionary<LogCategory, LogEventLevel> EnabledCategoryLevels { get; set; } =
            new Dictionary<LogCategory, LogEventLevel>
        {
            // Default "Proof of Execution" and important events
            { LogCategory.MethodBoundary, LogEventLevel.Information },
            { LogCategory.ActionBoundary, LogEventLevel.Information },
            { LogCategory.ExternalCall, LogEventLevel.Information },
            { LogCategory.StateChange, LogEventLevel.Information },
            { LogCategory.Security, LogEventLevel.Information }, // Security events are usually important
            { LogCategory.MetaLog, LogEventLevel.Warning },       // LLM Meta Directives, default to Warning

            // Categories typically suppressed unless specific troubleshooting is active
            // or their level is explicitly raised here for broader debugging.
            { LogCategory.InternalStep, LogEventLevel.Warning },      // Suppress Debug/Verbose InternalSteps by default
            { LogCategory.DiagnosticDetail, LogEventLevel.Warning },  // Suppress Debug/Verbose DiagnosticDetails by default
            { LogCategory.Performance, LogEventLevel.Warning },       // Performance logs off by default

            { LogCategory.Undefined, LogEventLevel.Information }      // For generic logs not yet categorized
        };

        /// <summary>
        /// When set, specifies the SourceContext (e.g., "MyNamespace.MyClass") to get detailed logs from.
        /// If null or empty, no specific source context is targeted for full details.
        /// </summary>
        public static string TargetSourceContextForDetails { get; set; } = null;

        /// <summary>
        /// When TargetSourceContextForDetails is set, this can optionally specify a method name
        /// within that source context for even more granular detailed logging.
        /// If null or empty, all methods within TargetSourceContextForDetails get detailed logs.
        /// </summary>
        public static string TargetMethodNameForDetails { get; set; } = null;

        /// <summary>
        /// If TargetSourceContextForDetails is set, this is the minimum LogEventLevel
        /// (e.g., Verbose, Debug) that will be output from that targeted context/method.
        /// </summary>
        public static LogEventLevel DetailTargetMinimumLevel { get; set; } = LogEventLevel.Verbose;
    }
}