using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using Serilog.Events;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Global monitoring system that detects ❌ FAIL conditions in logging output
    /// and automatically throws CriticalValidationException to shortcircuit the pipeline
    /// 
    /// This implements the centralized specification object approach by monitoring
    /// all log messages for failure patterns and triggering immediate pipeline abort.
    /// </summary>
    public static class GlobalFailureMonitor
    {
        private static readonly object _lockObject = new object();
        private static readonly List<string> _recentLogMessages = new List<string>();
        private static readonly int MaxLogHistory = 50;
        private static bool _isMonitoringEnabled = true;
        
        // Patterns that indicate critical failures requiring pipeline abort
        private static readonly string[] FailurePatterns = {
            @"🏆 \*\*OVERALL_METHOD_SUCCESS\*\*: ❌ FAIL",
            @"🏆 \*\*FINAL_METHOD_SUCCESS_WITH_TEMPLATE_SPEC\*\*: ❌ FAIL",
            @"🏆 \*\*TEMPLATE_SPECIFICATION_SUCCESS\*\*: ❌ FAIL"
        };
        
        // Context patterns to extract method names and evidence
        private static readonly Regex MethodContextPattern = new Regex(
            @"🏆.*❌ FAIL.*?-\s*([^-]+?)\s*(?:failed|for)", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
            
        private static readonly Regex EvidencePattern = new Regex(
            @"failed\s+([^$]+)", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Enables or disables the global failure monitoring system
        /// </summary>
        /// <param name="enabled">True to enable monitoring, false to disable</param>
        public static void SetMonitoringEnabled(bool enabled)
        {
            lock (_lockObject)
            {
                _isMonitoringEnabled = enabled;
            }
        }

        /// <summary>
        /// Monitors a log message for critical failure patterns
        /// If a failure is detected, throws CriticalValidationException immediately
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="logMessage">The log message to monitor</param>
        /// <param name="logLevel">The log level of the message</param>
        public static void MonitorLogMessage(ILogger logger, string logMessage, LogEventLevel logLevel = LogEventLevel.Error)
        {
            if (!_isMonitoringEnabled || string.IsNullOrEmpty(logMessage))
                return;

            lock (_lockObject)
            {
                // Add to recent log history for context
                _recentLogMessages.Add($"[{DateTime.UtcNow:HH:mm:ss.fff}] {logMessage}");
                if (_recentLogMessages.Count > MaxLogHistory)
                {
                    _recentLogMessages.RemoveAt(0);
                }

                // Check for critical failure patterns
                foreach (var pattern in FailurePatterns)
                {
                    if (Regex.IsMatch(logMessage, pattern))
                    {
                        HandleCriticalFailure(logger, logMessage);
                        return; // Exception thrown, execution stops here
                    }
                }
            }
        }

        /// <summary>
        /// Handles detection of a critical failure by extracting context and throwing CriticalValidationException
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="failureMessage">The log message containing the failure</param>
        private static void HandleCriticalFailure(ILogger logger, string failureMessage)
        {
            // Extract method context and evidence
            var methodMatch = MethodContextPattern.Match(failureMessage);
            var evidenceMatch = EvidencePattern.Match(failureMessage);
            
            var methodContext = methodMatch.Success ? methodMatch.Groups[1].Value.Trim() : "Unknown method";
            var evidence = evidenceMatch.Success ? evidenceMatch.Groups[1].Value.Trim() : failureMessage;
            
            // Clean up method context
            if (methodContext.Contains(" "))
            {
                var words = methodContext.Split(' ');
                methodContext = words.Length > 0 ? words[0] : methodContext;
            }
            
            var layer = "GLOBAL_FAILURE_MONITOR";
            var fullEvidence = $"Detected critical failure in {methodContext}: {evidence}";
            
            logger.Error("🚨 **CRITICAL_VALIDATION_FAILURE**: {Layer} - {Evidence} - **ABORTING_PIPELINE**", layer, fullEvidence);
            logger.Error("📋 **FAILURE_CONTEXT**: Recent log history: {@RecentLogs}", _recentLogMessages.TakeLast(10).ToArray());
            
            // Create comprehensive exception
            var criticalException = new CriticalValidationException(layer, fullEvidence, "Unknown", methodContext);
            
            // Log comprehensive exception context for LLM debugging
            LLMExceptionLogger.LogCriticalValidationException(logger, criticalException);
            
            // Clear monitoring temporarily to prevent recursive exceptions
            SetMonitoringEnabled(false);
            
            // Throw to stop pipeline immediately
            throw criticalException;
        }

        /// <summary>
        /// Gets recent log history for debugging purposes
        /// </summary>
        /// <returns>Array of recent log messages</returns>
        public static string[] GetRecentLogHistory()
        {
            lock (_lockObject)
            {
                return _recentLogMessages.ToArray();
            }
        }

        /// <summary>
        /// Clears the log history buffer
        /// </summary>
        public static void ClearLogHistory()
        {
            lock (_lockObject)
            {
                _recentLogMessages.Clear();
            }
        }

        /// <summary>
        /// Re-enables monitoring after an exception (for recovery scenarios)
        /// </summary>
        public static void ResetMonitoring()
        {
            lock (_lockObject)
            {
                _isMonitoringEnabled = true;
                _recentLogMessages.Clear();
            }
        }
    }

    /// <summary>
    /// Serilog sink that integrates with GlobalFailureMonitor
    /// Automatically monitors all log messages for critical failure patterns
    /// </summary>
    public class FailureMonitoringSink : Serilog.Core.ILogEventSink
    {
        private readonly ILogger _logger;
        
        public FailureMonitoringSink(ILogger logger)
        {
            _logger = logger;
        }

        public void Emit(LogEvent logEvent)
        {
            // Only monitor Error level messages for performance
            if (logEvent.Level >= LogEventLevel.Error)
            {
                var message = logEvent.RenderMessage();
                GlobalFailureMonitor.MonitorLogMessage(_logger, message, logEvent.Level);
            }
        }
    }

    /// <summary>
    /// Extension methods for easy integration with existing logging
    /// </summary>
    public static class FailureMonitoringExtensions
    {
        /// <summary>
        /// Logs an error message and monitors it for critical failure patterns
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="messageTemplate">The message template</param>
        /// <param name="propertyValues">Property values for the template</param>
        public static void ErrorWithMonitoring(this ILogger logger, string messageTemplate, params object[] propertyValues)
        {
            logger.Error(messageTemplate, propertyValues);
            
            // Format the message for monitoring
            var formattedMessage = string.Format(messageTemplate, propertyValues);
            GlobalFailureMonitor.MonitorLogMessage(logger, formattedMessage, LogEventLevel.Error);
        }

        /// <summary>
        /// Logs a critical failure that should immediately abort the pipeline
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="methodName">The method where the failure occurred</param>
        /// <param name="evidence">Evidence of the failure</param>
        /// <param name="documentType">Document type being processed (optional)</param>
        public static void LogCriticalPipelineFailure(this ILogger logger, string methodName, string evidence, string documentType = "Unknown")
        {
            var criticalMessage = $"🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL - {methodName} failed {evidence}";
            logger.Error(criticalMessage);
            
            // This will trigger the monitoring and throw CriticalValidationException
            GlobalFailureMonitor.MonitorLogMessage(logger, criticalMessage, LogEventLevel.Error);
        }
    }
}