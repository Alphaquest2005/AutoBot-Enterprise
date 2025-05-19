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

            // ... (LogFilterState setup remains the same) ...

            _testLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                // Use the custom StringWriterSink
                .WriteTo.Sink(new StringWriterSink(_logOutput,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] {Message:lj}{NewLine}{Properties}{NewLine}"))
                .WriteTo.NUnitOutput(restrictedToMinimumLevel: LogEventLevel.Verbose)
                .Filter.ByIncludingOnly(evt =>
                {
                    LogCategory category = LogCategory.Undefined;
                    // ... (category extraction) ...
                    string sourceContext = evt.Properties.ContainsKey("SourceContext") ? evt.Properties["SourceContext"].ToString().Trim('"') : "NULL_SC";
                    string memberName = evt.Properties.ContainsKey("MemberName") ? evt.Properties["MemberName"].ToString().Trim('"') : "NULL_MN";
                    string message = evt.RenderMessage(); // Get the rendered message for easier identification

                    Console.WriteLine($"FILTER CHECK: SC='{sourceContext}', MN='{memberName}', Cat='{category}', Lvl='{evt.Level}', Msg='{message}'");
                    Console.WriteLine($"  TargetSC='{LogFilterState.TargetSourceContextForDetails}', TargetMN='{LogFilterState.TargetMethodNameForDetails}', DetailLvl='{LogFilterState.DetailTargetMinimumLevel}'");

                    bool decision = false;
                    // 1. Targeted Troubleshooting Mode
                    if (!string.IsNullOrEmpty(LogFilterState.TargetSourceContextForDetails) &&
                        sourceContext != null && // Check sourceContext is not null before Contains
                        sourceContext.Contains(LogFilterState.TargetSourceContextForDetails))
                    {
                        Console.WriteLine("  TARGET_SC_MATCH");
                        bool methodMatch = string.IsNullOrEmpty(LogFilterState.TargetMethodNameForDetails) ||
                                           (memberName != null && memberName.Equals(LogFilterState.TargetMethodNameForDetails, StringComparison.OrdinalIgnoreCase));

                        if (methodMatch)
                        {
                            Console.WriteLine("  TARGET_MN_MATCH (or no MN target)");
                            decision = evt.Level >= LogFilterState.DetailTargetMinimumLevel;
                            Console.WriteLine($"    DETAIL_FILTER_DECISION: {decision} (EvtLvl: {evt.Level} >= DetailLvl: {LogFilterState.DetailTargetMinimumLevel})");
                            return decision;
                        }
                        else
                        {
                            Console.WriteLine("  TARGET_MN_MISMATCH");
                        }
                    }

                    // 2. General Category/Level Check
                    if (LogFilterState.EnabledCategoryLevels.TryGetValue(category, out var enabledMinLevel))
                    {
                        decision = evt.Level >= enabledMinLevel;
                        Console.WriteLine($"  CATEGORY_FILTER_DECISION: {decision} (Cat: {category}, EvtLvl: {evt.Level} >= EnabledLvl: {enabledMinLevel})");
                        return decision;
                    }

                    Console.WriteLine("  NO_MATCH_DEFAULT_EXCLUDE");
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
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose; // Target set to Verbose

            var dummy = new TestDummyClass(_testLogger);
            var anotherDummy = new AnotherTestDummyClass(_testLogger);
            string invId = "test-inv-02";

            // Act
            dummy.MethodA(invId);
            anotherDummy.DoWork(invId);

            string output = _logOutput.ToString();
            Console.WriteLine("--- Test02 Output (Strict DetailTargetMinimumLevel) ---");
            Console.WriteLine(output);

            // For MethodA (Targeted, DetailTargetMinimumLevel = Verbose):
            // Only Verbose level and higher logs from MethodA should appear.
            Assert.That(output, Does.Not.Contain("METHOD_ENTRY: MethodName: MethodA"), "MethodA Entry (Info) should be filtered out by DetailTargetMinLevel=Verbose");
            Assert.That(output, Does.Not.Contain("Inside MethodA - step 1 (debug)"), "MethodA Debug step should be filtered out by DetailTargetMinLevel=Verbose");
            Assert.That(output, Does.Contain("Inside MethodA - step 2 (verbose)"), "MethodA Verbose step should be present"); // This is Verbose >= Verbose
            Assert.That(output, Does.Not.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodA"), "MethodA Exit (Info) should be filtered out by DetailTargetMinLevel=Verbose");

            // For MethodB (called by MethodA, SourceContext is TestDummyClass, but MemberName is MethodB, so it's NOT the specific method target):
            // MethodB logs fall back to the general LogFilterState.EnabledCategoryLevels.
            // MethodBoundary category is Information by default.
            // DiagnosticDetail category (for its debug log) is Warning by default.
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: MethodB"), "MethodB Entry (Info) should appear based on EnabledCategoryLevels");
            Assert.That(output, Does.Not.Contain("Detail from MethodB"), "MethodB Debug detail (DiagnosticDetail) should be filtered out by EnabledCategoryLevels default of Warning");
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_EXIT_SUCCESS: MethodName: MethodB"), "MethodB Exit (Info) should appear based on EnabledCategoryLevels");

            // For AnotherTestDummyClass (Not targeted):
            // Governed by LogFilterState.EnabledCategoryLevels.
            Assert.That(output, Does.Contain($"[{LogCategory.MethodBoundary}]").And.Contain("METHOD_ENTRY: MethodName: DoWork"));
            Assert.That(output, Does.Contain($"[{LogCategory.StateChange}]").And.Contain("State changed in AnotherTestDummyClass")); // StateChange is Information by default
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