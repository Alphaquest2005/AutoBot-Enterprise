using System;
using System.Diagnostics;
using System.Reflection;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Standardized exception logging for LLM-friendly debugging
    /// Provides comprehensive context including stack traces, method information, and structured data
    /// </summary>
    public static class LLMExceptionLogger
    {
        /// <summary>
        /// Logs an exception with comprehensive context for LLM debugging
        /// </summary>
        /// <param name="logger">The logger instance to use</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="contextDescription">Human-readable description of what operation failed</param>
        /// <param name="additionalData">Any additional structured data relevant to the failure</param>
        /// <param name="memberName">Automatically captured calling method name</param>
        /// <param name="sourceFilePath">Automatically captured source file path</param>
        /// <param name="sourceLineNumber">Automatically captured source line number</param>
        public static void LogComprehensiveException(
            ILogger logger, 
            Exception ex, 
            string contextDescription,
            object additionalData = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                // Extract key exception details
                var exceptionType = ex.GetType().Name;
                var message = ex.Message;
                var stackTrace = ex.StackTrace ?? "No stack trace available";
                var innerException = ex.InnerException?.ToString() ?? "None";
                
                // Get current method context
                var currentMethod = new StackTrace()?.GetFrame(1)?.GetMethod();
                var methodFullName = currentMethod != null ? 
                    $"{currentMethod.DeclaringType?.FullName}.{currentMethod.Name}" : 
                    memberName;
                
                // Extract source file name from full path
                var sourceFileName = !string.IsNullOrEmpty(sourceFilePath) ? 
                    System.IO.Path.GetFileName(sourceFilePath) : 
                    "Unknown";

                // Log with comprehensive structured data
                logger.Error("🚨 **LLM_EXCEPTION_COMPLETE_CONTEXT**: " +
                    "Context={ContextDescription}, " +
                    "ExceptionType={ExceptionType}, " +
                    "Message={Message}, " +
                    "Method={MethodFullName}, " +
                    "SourceFile={SourceFileName}, " +
                    "LineNumber={SourceLineNumber}, " +
                    "StackTrace={StackTrace}, " +
                    "InnerException={InnerException}, " +
                    "AdditionalData={AdditionalData}",
                    contextDescription,
                    exceptionType,
                    message,
                    methodFullName,
                    sourceFileName,
                    sourceLineNumber,
                    stackTrace,
                    innerException,
                    additionalData ?? "None");

                // Also log the raw exception object for completeness
                logger.Error(ex, "🚨 **EXCEPTION_OBJECT**: {ContextDescription}", contextDescription);
            }
            catch (Exception loggingEx)
            {
                // Fallback logging if the comprehensive logging itself fails
                logger.Error(loggingEx, "🚨 **LOGGING_EXCEPTION**: Failed to log exception comprehensively");
                logger.Error(ex, "🚨 **ORIGINAL_EXCEPTION**: {ContextDescription}", contextDescription);
            }
        }

        /// <summary>
        /// Logs a CriticalValidationException with additional validation-specific context
        /// </summary>
        /// <param name="logger">The logger instance to use</param>
        /// <param name="ex">The CriticalValidationException to log</param>
        /// <param name="memberName">Automatically captured calling method name</param>
        /// <param name="sourceFilePath">Automatically captured source file path</param>
        /// <param name="sourceLineNumber">Automatically captured source line number</param>
        public static void LogCriticalValidationException(
            ILogger logger,
            CriticalValidationException ex,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            var validationContext = new
            {
                Layer = ex.Layer,
                Evidence = ex.Evidence,
                DocumentType = ex.DocumentType,
                ValidationContext = ex.ValidationContext
            };

            LogComprehensiveException(
                logger, 
                ex, 
                $"Critical validation failure: {ex.Layer}",
                validationContext,
                memberName,
                sourceFilePath,
                sourceLineNumber);

            // Also log the LLM-friendly description
            logger.Error("🎯 **VALIDATION_FAILURE_SUMMARY**: {LLMDescription}", ex.GetLLMFriendlyDescription());
        }

        /// <summary>
        /// Creates a structured exception context object for logging
        /// </summary>
        /// <param name="operation">The operation that was being performed</param>
        /// <param name="input">Input parameters or data</param>
        /// <param name="expectedOutcome">What the operation was supposed to achieve</param>
        /// <param name="actualOutcome">What actually happened</param>
        /// <returns>Structured context object</returns>
        public static object CreateExceptionContext(string operation, object input = null, string expectedOutcome = null, string actualOutcome = null)
        {
            return new
            {
                Operation = operation,
                Input = input?.ToString() ?? "None",
                ExpectedOutcome = expectedOutcome ?? "Not specified",
                ActualOutcome = actualOutcome ?? "Exception occurred",
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff UTC")
            };
        }
    }
}