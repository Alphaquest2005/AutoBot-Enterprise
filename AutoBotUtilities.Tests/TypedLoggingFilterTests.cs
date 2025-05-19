using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.NUnit; // Or use a StringWriter/TestOutputHelper approach
using System;
using System.IO;
using Core.Common.Extensions; // For LogCategory, TypedLoggerExtensions, LogFilterState


namespace AutoBotUtilities.Tests
{
    using Serilog.Context;
    using System.Collections.Generic;
    using System.Linq;

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

        [Test]
        public void Test04_MixedMode_ProofForStandard_DetailForTarget()
        {
            // Default: MethodBoundary=Info, InternalStep=Warning, DiagnosticDetail=Warning

            // Target only MethodA of TestDummyClass for full verbose detail
            LogFilterState.TargetSourceContextForDetails = typeof(TestDummyClass).FullName;
            LogFilterState.TargetMethodNameForDetails = nameof(TestDummyClass.MethodA);
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;

            var dummy = new TestDummyClass(_testLogger);
            var anotherDummy = new AnotherTestDummyClass(_testLogger);
            string invId = "test-inv-04";

            // Act
            anotherDummy.DoWork(invId);
            dummy.MethodA(invId);

            string output = _logOutput.ToString();
            Console.WriteLine("--- Test04 Output ---");
            Console.WriteLine(output);

            // Assertions for anotherDummy (non-target, should get default category filtering)
            Assert.That(output, Does.Match($@".*\[{LogCategory.MethodBoundary}\].*METHOD_ENTRY: MethodName: DoWork.*"), "AnotherDummy DoWork Entry line with correct LogCategory not found");
            Assert.That(output, Does.Match($@".*\[{LogCategory.StateChange}\].*State changed in AnotherTestDummyClass.*"), "AnotherDummy StateChange line with correct LogCategory not found");
            Assert.That(output, Does.Match($@".*\[{LogCategory.MethodBoundary}\].*METHOD_EXIT_SUCCESS: MethodName: DoWork.*"), "AnotherDummy DoWork Exit line with correct LogCategory not found");

            // Assertions for dummy.MethodA (targeted for full detail - Info, Debug, Verbose should all pass DetailTargetMinimumLevel = Verbose)
            Assert.That(output, Does.Match($@".*\[{LogCategory.MethodBoundary}\].*METHOD_ENTRY: MethodName: MethodA.*"));
            Assert.That(output, Does.Match($@".*\[{LogCategory.InternalStep}\].*Inside MethodA - step 1 \(debug\).*"));
            Assert.That(output, Does.Match($@".*\[{LogCategory.InternalStep}\].*Inside MethodA - step 2 \(verbose\).*"));

            // MethodB called by MethodA is NOT the specific target for *detail*, so it adheres to general category rules
            Assert.That(output, Does.Match($@".*\[{LogCategory.MethodBoundary}\].*METHOD_ENTRY: MethodName: MethodB.*"));
            Assert.That(output, Does.Not.Contain("Detail from MethodB")); // Debug, DiagnosticDetail, default level is Warning, so it should be filtered out
            Assert.That(output, Does.Match($@".*\[{LogCategory.MethodBoundary}\].*METHOD_EXIT_SUCCESS: MethodName: MethodB.*"));

            Assert.That(output, Does.Match($@".*\[{LogCategory.MethodBoundary}\].*METHOD_EXIT_SUCCESS: MethodName: MethodA.*"));
        }

        // In TypedLoggingFilterTests.cs

        // ... (Existing Setup, TearDown, Test01, Test02, Test03, Test04 if you added it) ...

        [Test]
        public void Test05_CategoryFiltering_EnableSpecificCategoryOnly()
        {
            // Disable all default categories, enable only Performance at Debug level
            foreach (var key in LogFilterState.EnabledCategoryLevels.Keys.ToList())
            {
                LogFilterState.EnabledCategoryLevels[key] = LogEventLevel.Fatal; // Effectively disable by setting above most log levels
            }
            LogFilterState.EnabledCategoryLevels[LogCategory.Performance] = LogEventLevel.Debug;
            LogFilterState.EnabledCategoryLevels[LogCategory.MethodBoundary] = LogEventLevel.Information; // Keep method boundaries for structure

            // Ensure no specific target is set
            LogFilterState.TargetSourceContextForDetails = null;
            LogFilterState.TargetMethodNameForDetails = null;

            var dummy = new TestDummyClass(_testLogger);
            var perfLogger = _testLogger.ForContext<TypedLoggingFilterTests>(); // Use a different source context for performance log
            string invId = "test-inv-05";

            // Act
            _testLogger.LogMethodEntry(invId, memberName: "Test05_Entry"); // Should appear

            // This log should appear
            perfLogger.LogDebugCategorized(LogCategory.Performance, "High performance operation details. Value: {Value}", invId, propertyValues: new object[] { 123 });

            // This log should NOT appear (InternalStep default is Warning, but we set it to Fatal)
            dummy.MethodA(invId); // MethodA has InternalSteps at Debug/Verbose

            _testLogger.LogMethodExitSuccess(invId, 1, memberName: "Test05_Exit"); // Should appear


            string output = _logOutput.ToString();
            Console.WriteLine("--- Test05 Output ---");
            Console.WriteLine(output);

            Assert.That(output, Does.Contain("Test05_Entry"));
            Assert.That(output, Does.Contain("High performance operation details. Value: 123"));
            Assert.That(output, Does.Contain($"[{LogCategory.Performance}]"));
            Assert.That(output, Does.Not.Contain("Inside MethodA - step 1 (debug)"));
            Assert.That(output, Does.Not.Contain("Inside MethodA - step 2 (verbose)"));
            Assert.That(output, Does.Contain("Test05_Exit"));
            // MethodBoundary for MethodA and MethodB should still appear due to its own EnabledCategoryLevel
            Assert.That(output, Does.Contain("METHOD_ENTRY: MethodName: MethodA"));
        }

        [Test]
        public void Test06_TargetedTroubleshooting_DifferentDetailLevel_Debug()
        {
            LogFilterState.TargetSourceContextForDetails = typeof(TestDummyClass).FullName;
            LogFilterState.TargetMethodNameForDetails = nameof(TestDummyClass.MethodA);
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Debug; // Target set to Debug

            var dummy = new TestDummyClass(_testLogger);
            string invId = "test-inv-06";

            // Act
            dummy.MethodA(invId); // Contains Info, Debug, and Verbose logs

            string output = _logOutput.ToString();
            Console.WriteLine("--- Test06 Output ---");
            Console.WriteLine(output);

            // For MethodA (Targeted, DetailTargetMinimumLevel = Debug):
            // Information and Debug logs from MethodA should appear.
            // Verbose logs from MethodA should NOT appear.
            Assert.That(output, Does.Contain("METHOD_ENTRY: MethodName: MethodA"), "MethodA Entry (Info) should be present (Info >= Debug)");
            Assert.That(output, Does.Contain("Inside MethodA - step 1 (debug)"), "MethodA Debug step should be present (Debug >= Debug)");
            Assert.That(output, Does.Not.Contain("Inside MethodA - step 2 (verbose)"), "MethodA Verbose step should be filtered out (Verbose < Debug)");
            Assert.That(output, Does.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodA"), "MethodA Exit (Info) should be present (Info >= Debug)");

            // For MethodB (called by MethodA, but not the explicitly targeted method for detail):
            // Governed by general LogFilterState.EnabledCategoryLevels
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: MethodB"));
            Assert.That(output, Does.Not.Contain("Detail from MethodB"), "MethodB Debug detail (DiagnosticDetail) should be filtered by EnabledCategoryLevels default of Warning");
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodB"));
        }

        [Test]
        public void Test07_InvocationIdPropagationAndFiltering_Simulated()
        {
            string invId1 = "INV-001";
            string invId2 = "INV-002";

            LogFilterState.EnabledCategoryLevels[LogCategory.MethodBoundary] = LogEventLevel.Information;
            LogFilterState.EnabledCategoryLevels[LogCategory.InternalStep] = LogEventLevel.Debug; // Ensure these pass for the test

            var dummy = new TestDummyClass(_testLogger); // Not used, but fine

            // Act
            using (LogContext.PushProperty("InvocationId", invId1))
            {
                _testLogger.LogMethodEntry(invId1, memberName: "Operation1");
                _testLogger.LogInternalStep(invId1, "Step A in Operation1"); // Simpler params for this test
            }

            using (LogContext.PushProperty("InvocationId", invId2))
            {
                _testLogger.LogMethodEntry(invId2, memberName: "Operation2");
                _testLogger.LogInternalStep(invId2, "Step X in Operation2"); // Simpler params
            }

            string output = _logOutput.ToString();
            Console.WriteLine("--- Test07 Output ---");
            Console.WriteLine(output);

            // Assertions for Operation1 with invId1
            string op1EntryPattern = $@"\[MethodBoundary\].*\[Operation1\].*METHOD_ENTRY: MethodName: Operation1, InvocationId: {invId1}";
            string op1EntryPropertiesPattern = $@"{{.*InvocationId: ""{invId1}"".*TestName: ""{TestContext.CurrentContext.Test.Name}"""; // Check for InvocationId in properties

            Assert.That(output, Does.Match(op1EntryPattern), "Operation1 Entry log line incorrect.");
            Assert.That(output, Does.Match(op1EntryPropertiesPattern), "Operation1 Entry properties incorrect or InvocationId missing.");


            string op1StepPattern = $@"\[InternalStep\].*\[{nameof(Test07_InvocationIdPropagationAndFiltering_Simulated)}\].*Step A in Operation1";
            string op1StepPropertiesPattern = $@"{{.*InvocationId: ""{invId1}"".*TestName: ""{TestContext.CurrentContext.Test.Name}""";

            Assert.That(output, Does.Match(op1StepPattern), "Operation1 Step A log line incorrect.");
            Assert.That(output, Does.Match(op1StepPropertiesPattern), "Operation1 Step A properties incorrect or InvocationId missing.");


            // Assertions for Operation2 with invId2
            string op2EntryPattern = $@"\[MethodBoundary\].*\[Operation2\].*METHOD_ENTRY: MethodName: Operation2, InvocationId: {invId2}";
            string op2EntryPropertiesPattern = $@"{{.*InvocationId: ""{invId2}"".*TestName: ""{TestContext.CurrentContext.Test.Name}""";

            Assert.That(output, Does.Match(op2EntryPattern), "Operation2 Entry log line incorrect.");
            Assert.That(output, Does.Match(op2EntryPropertiesPattern), "Operation2 Entry properties incorrect or InvocationId missing.");

            string op2StepPattern = $@"\[InternalStep\].*\[{nameof(Test07_InvocationIdPropagationAndFiltering_Simulated)}\].*Step X in Operation2";
            string op2StepPropertiesPattern = $@"{{.*InvocationId: ""{invId2}"".*TestName: ""{TestContext.CurrentContext.Test.Name}""";

            Assert.That(output, Does.Match(op2StepPattern), "Operation2 Step X log line incorrect.");
            Assert.That(output, Does.Match(op2StepPropertiesPattern), "Operation2 Step X properties incorrect or InvocationId missing.");
        }
    }
}