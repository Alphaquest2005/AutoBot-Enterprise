using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.NUnit; // Or use a StringWriter/TestOutputHelper approach
using System;
using System.IO;
using Core.Common.Extensions; // For LogCategory, TypedLoggerExtensions, LogFilterState


namespace AutoBotUtilities.Tests
{
    using System.Collections.Generic;

    [TestFixture]
    public class TypedLoggingFilterTests
    {
        private ILogger _testLogger;
        private StringWriter _logOutput; // To capture log output

        // Dummy class and methods for testing logging contexts
        private class TestDummyClass
        {
            private readonly ILogger _logger;
            public TestDummyClass(ILogger logger) { _logger = logger.ForContext<TestDummyClass>(); }

            public void MethodA(string invocationId)
            {
                _logger.LogMethodEntry(invocationId);
                _logger.LogInternalStep(invocationId, "Inside MethodA - step 1 (debug)");
                _logger.LogInternalStepVerbose(invocationId, "Inside MethodA - step 2 (verbose)");
                MethodB(invocationId);
                _logger.LogMethodExitSuccess(invocationId, 10);
            }

            public void MethodB(string invocationId)
            {
                _logger.LogMethodEntry(invocationId);
                _logger.LogDebugCategorized(LogCategory.DiagnosticDetail, "Detail from MethodB", invocationId);
                _logger.LogMethodExitSuccess(invocationId, 5);
            }
        }
        private class AnotherTestDummyClass
        {
            private readonly ILogger _logger;
            public AnotherTestDummyClass(ILogger logger) { _logger = logger.ForContext<AnotherTestDummyClass>(); }

            public void DoWork(string invocationId)
            {
                _logger.LogMethodEntry(invocationId);
                _logger.LogInfoCategorized(LogCategory.StateChange, "State changed in AnotherTestDummyClass", invocationId);
                _logger.LogMethodExitSuccess(invocationId, 3);
            }
        }


        [SetUp]
        public void Setup()
        {
            _logOutput = new StringWriter();

            // Reset LogFilterState to defaults before each test
            LogFilterState.TargetSourceContextForDetails = null;
            LogFilterState.TargetMethodNameForDetails = null;
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;

            LogFilterState.EnabledCategoryLevels = new Dictionary<LogCategory, LogEventLevel>
            {
                { LogCategory.MethodBoundary, LogEventLevel.Information },
                { LogCategory.ActionBoundary, LogEventLevel.Information },
                { LogCategory.ExternalCall, LogEventLevel.Information },
                { LogCategory.StateChange, LogEventLevel.Information },
                { LogCategory.Security, LogEventLevel.Information },
                { LogCategory.MetaLog, LogEventLevel.Warning },
                { LogCategory.InternalStep, LogEventLevel.Warning },
                { LogCategory.DiagnosticDetail, LogEventLevel.Warning },
                { LogCategory.Performance, LogEventLevel.Warning },
                { LogCategory.Undefined, LogEventLevel.Information }
            };

            _testLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("TestName", TestContext.CurrentContext.Test.Name)
                .WriteTo.Sink(new StringWriterSink(_logOutput,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}{Properties}{NewLine}"))
                .WriteTo.NUnitOutput(restrictedToMinimumLevel: LogEventLevel.Verbose,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}")
                .Filter.ByIncludingOnly(evt =>
                {
                    LogCategory category = LogCategory.Undefined;
                    string sourceContext = evt.Properties.TryGetValue("SourceContext", out var scP) && scP is ScalarValue scV && scV.Value != null ? scV.Value.ToString().Trim('"') : "";
                    string memberName = evt.Properties.TryGetValue("MemberName", out var mnP) && mnP is ScalarValue mnV && mnV.Value != null ? mnV.Value.ToString().Trim('"') : "";

                    if (evt.Properties.TryGetValue("LogCategory", out var categoryProp) &&
                        categoryProp is ScalarValue svCat &&
                        svCat.Value is LogCategory catVal)
                    {
                        category = catVal;
                    }
                    else if (evt.Properties.TryGetValue("LogCategory", out var categoryPropStr) &&
                               categoryPropStr is ScalarValue svCatStr &&
                               svCatStr.Value != null &&
                               Enum.TryParse<LogCategory>(svCatStr.Value.ToString().Trim('"'), out var catValStr))
                    {
                        category = catValStr;
                    }

                    if (!string.IsNullOrEmpty(LogFilterState.TargetSourceContextForDetails) &&
                        !string.IsNullOrEmpty(sourceContext) && // Ensure sourceContext is not null before Contains
                        sourceContext.Contains(LogFilterState.TargetSourceContextForDetails))
                    {
                        bool methodMatch = string.IsNullOrEmpty(LogFilterState.TargetMethodNameForDetails) ||
                                           (!string.IsNullOrEmpty(memberName) && memberName.Equals(LogFilterState.TargetMethodNameForDetails, StringComparison.OrdinalIgnoreCase));

                        if (methodMatch)
                        {
                            return evt.Level >= LogFilterState.DetailTargetMinimumLevel;
                        }
                    }

                    if (LogFilterState.EnabledCategoryLevels.TryGetValue(category, out var enabledMinLevelForCategory))
                    {
                        return evt.Level >= enabledMinLevelForCategory;
                    }

                    return false;
                })
                .CreateLogger();
        }

        [TearDown]
        public void TearDown()
        {
            _logOutput?.Dispose();
        }

        [Test]
        public void Test01_DefaultFiltering_ShowsProofOfExecutionOnly()
        {
            var dummy = new TestDummyClass(_testLogger);
            var anotherDummy = new AnotherTestDummyClass(_testLogger);
            string invId = "test-inv-01";

            // Act
            dummy.MethodA(invId);
            anotherDummy.DoWork(invId);

            string output = _logOutput.ToString();
            Console.WriteLine("--- Test01 Output ---");
            Console.WriteLine(output);

            // Assert "Proof of Execution" (MethodBoundary at Information)
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: MethodA"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodA"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: MethodB"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodB"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: DoWork"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: DoWork"));

            // Assert StateChange (Information by default)
            Assert.That(output, Does.Contain($"[{LogCategory.StateChange}]").And.Contain("State changed in AnotherTestDummyClass"));

            // Assert Detailed logs are NOT present
            Assert.That(output, Does.Not.Contain("Inside MethodA - step 1 (debug)"));
            Assert.That(output, Does.Not.Contain("Inside MethodA - step 2 (verbose)"));
            Assert.That(output, Does.Not.Contain("Detail from MethodB"));
        }

        [Test]
        public void Test02_TargetedTroubleshooting_ShowsAllDetailsForTargetMethod()
        {
            LogFilterState.TargetSourceContextForDetails = typeof(TestDummyClass).FullName;
            LogFilterState.TargetMethodNameForDetails = nameof(TestDummyClass.MethodA);
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;

            var dummy = new TestDummyClass(_testLogger);
            var anotherDummy = new AnotherTestDummyClass(_testLogger);
            string invId = "test-inv-02";

            dummy.MethodA(invId);
            anotherDummy.DoWork(invId);

            string output = _logOutput.ToString();
            Console.WriteLine("--- Test02 Output (After Extension Fix + Corrected Asserts) ---");
            Console.WriteLine(output);

            // For MethodA (Targeted, DetailTargetMinimumLevel = Verbose):
            // ALL logs from MethodA at Verbose level or numerically higher (i.e., less severe or equal) should appear.
            Assert.That(output, Does.Contain("METHOD_ENTRY: MethodName: MethodA"), "MethodA Entry (Info) should be present");
            Assert.That(output, Does.Contain("Inside MethodA - step 1 (debug)"), "MethodA Debug step should be present");
            Assert.That(output, Does.Contain("Inside MethodA - step 2 (verbose)"), "MethodA Verbose step should be present");
            Assert.That(output, Does.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodA"), "MethodA Exit (Info) should be present");

            // For MethodB (called by MethodA, SourceContext is TestDummyClass, but MemberName is MethodB, so it's NOT the specific method target):
            // Falls to general category filter.
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: MethodB"));
            Assert.That(output, Does.Not.Contain("Detail from MethodB"), "MethodB Debug detail (DiagnosticDetail) should be filtered by EnabledCategoryLevels default");
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodB"));

            // For AnotherTestDummyClass (Not targeted):
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: DoWork"));
            Assert.That(output, Does.Contain($"[{LogCategory.StateChange}]").And.Contain("State changed in AnotherTestDummyClass"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: DoWork"));
        }

        [Test]
        public void Test03_TargetedTroubleshooting_ClassLevel_ShowsAllDetailsForTargetClass()
        {
            LogFilterState.TargetSourceContextForDetails = typeof(TestDummyClass).FullName; // Target the specific class
            LogFilterState.TargetMethodNameForDetails = null;    // NO specific method, so all methods in TestDummyClass
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose; // Show everything from target class

            var dummy = new TestDummyClass(_testLogger);
            var anotherDummy = new AnotherTestDummyClass(_testLogger);
            string invId = "test-inv-03";

            // Act
            dummy.MethodA(invId); // This will call MethodB internally
            anotherDummy.DoWork(invId);

            string output = _logOutput.ToString();
            Console.WriteLine("--- Test03 Output ---");
            Console.WriteLine(output);

            // Assert ALL logs from TestDummyClass (MethodA and MethodB) are present
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: MethodA"));
            Assert.That(output, Does.Contain("Inside MethodA - step 1 (debug)"));
            Assert.That(output, Does.Contain("Inside MethodA - step 2 (verbose)"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: MethodB"));
            Assert.That(output, Does.Contain("Detail from MethodB")); // Now this SHOULD appear
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodB"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodA"));

            // Assert only proof of execution for AnotherTestDummyClass
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: DoWork"));
            Assert.That(output, Does.Contain($"[{LogCategory.StateChange}]").And.Contain("State changed in AnotherTestDummyClass"));
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: DoWork"));
        }

        // Add more tests:
        // - Test enabling/disabling specific categories via EnabledCategoryLevels
        // - Test different DetailTargetMinimumLevel (e.g., Debug instead of Verbose)
        // - Test invocationId propagation and filtering (if we add invocationId to filter state)
    }
}