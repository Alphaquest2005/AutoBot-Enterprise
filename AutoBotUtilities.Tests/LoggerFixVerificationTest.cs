using System;
using System.Collections.Generic;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Serilog.Core;
using Core.Common.Extensions;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class LoggerFixVerificationTest
    {
        private static ILogger _testLog;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Configure LogFilterState for test logging
            LogFilterState.EnabledCategoryLevels = new Dictionary<LogCategory, LogEventLevel>
            {
                { LogCategory.MethodBoundary, LogEventLevel.Information },
                { LogCategory.InternalStep, LogEventLevel.Information },
                { LogCategory.DiagnosticDetail, LogEventLevel.Information },
                { LogCategory.Performance, LogEventLevel.Warning },
                { LogCategory.Undefined, LogEventLevel.Information }
            };

            _testLog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext().Enrich.WithProperty("TestFixture", nameof(LoggerFixVerificationTest))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}{Exception}")
                .Filter.ByIncludingOnly(evt =>
                {
                    var category = evt.Properties.TryGetValue("LogCategory", out var categoryValue) && categoryValue is ScalarValue sv && sv.Value is LogCategory lc ? lc : LogCategory.Undefined;
                    if (!string.IsNullOrEmpty(LogFilterState.TargetSourceContextForDetails))
                    {
                        var sourceContext = evt.Properties.TryGetValue("SourceContext", out var sourceContextValue) && sourceContextValue is ScalarValue scv ? scv.Value?.ToString() : "";
                        var memberName = evt.Properties.TryGetValue("MemberName", out var memberNameValue) && memberNameValue is ScalarValue mnv ? mnv.Value?.ToString() : "";
                        var contextMatch = sourceContext?.Contains(LogFilterState.TargetSourceContextForDetails) == true;
                        var methodMatch = string.IsNullOrEmpty(LogFilterState.TargetMethodNameForDetails) || memberName?.Contains(LogFilterState.TargetMethodNameForDetails) == true;
                        if (contextMatch && methodMatch) { return evt.Level >= LogFilterState.DetailTargetMinimumLevel; }
                    }
                    if (LogFilterState.EnabledCategoryLevels.TryGetValue(category, out var enabledMinLevelForCategory)) { return evt.Level >= enabledMinLevelForCategory; }
                    return false;
                })
                .CreateLogger();
        }

        [Test]
        public void Logger_ShouldNotThrowNullReferenceException()
        {
            var testInvocationId = Guid.NewGuid().ToString();

            // This should not throw a NullReferenceException
            Assert.DoesNotThrow(() =>
            {
                _testLog.LogMethodEntry(testInvocationId);
                _testLog.LogInfoCategorized(LogCategory.DiagnosticDetail, "Test message", invocationId: testInvocationId);
                _testLog.LogMethodExitSuccess(testInvocationId, 0);
            });
        }

        [Test]
        public void Logger_ShouldHandleParameterizedMessages()
        {
            var testInvocationId = Guid.NewGuid().ToString();

            // This should not throw a NullReferenceException
            Assert.DoesNotThrow(() =>
            {
                _testLog.LogInfoCategorized(LogCategory.DiagnosticDetail, "Test message with {Count} parameters", testInvocationId, propertyValues: new object[] { 2 });
                _testLog.LogDebugCategorized(LogCategory.DiagnosticDetail, "Debug message with {Key} = {Value}", testInvocationId, propertyValues: new object[] { "TestKey", "TestValue" });
            });
        }
    }
}
