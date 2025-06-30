// --- Namespace for your tests ---
namespace MyLoggingUtils.Tests // Or your test project's namespace
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Moq;

    using Core.Common.Extensions; // Changed from MyLoggingUtils;

    using NUnit.Framework; // To access the classes defined above
    using NUnit.Framework.Legacy;

    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Parsing;
    using Serilog.Sinks.SystemConsole.Themes;
    using Serilog.Sinks.TestCorrelator;

    [TestFixture]
    public class LevelOverridableLoggerTests
    {
        private Serilog.Core.Logger _innerSerilogLogger; // Serilog's concrete Logger
        private LevelOverridableLogger _sut; // System Under Test

        [SetUp]
        public void SetUp()
        {
            _innerSerilogLogger = new LoggerConfiguration()
                .MinimumLevel.Information() // Default level for the *actual* inner logger
                .WriteTo.Console( // ADD THIS LINE FOR CONSOLE OUTPUT
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Literate) // Optional: for colored/themed output
                .WriteTo.TestCorrelator()
                .CreateLogger();

            _sut = new LevelOverridableLogger(_innerSerilogLogger);
           
        }

        [TearDown]
        public void TearDown()
        {
            _innerSerilogLogger.Dispose();
            LogLevelOverride.EmergencyReset(); // Ensure override is cleared
        }

        private static IEnumerable<LogEvent> GetLoggedEvents()
        {
            // TestCorrelator automatically captures from the current async context
            return TestCorrelator.GetLogEventsFromCurrentContext();
        }

        [Test]
        public void WhenNoOverride_UsesInnerLoggerLevel()
        {
            using (TestCorrelator.CreateContext())
            {
                _sut.Information("Info message");
                _sut.Debug("Debug message"); // Should not be logged (SUT respects override, inner logger is Info)

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1), "Only one message should be logged.");
                Assert.That(logs[0].Level, Is.EqualTo(Serilog.Events.LogEventLevel.Information));
                StringAssert.Contains("Info message", logs[0].RenderMessage());
            }
        }

        [Test]
        public void WhenOverrideToMoreVerbose_LogsAtOverriddenLevel()
        {
            using (TestCorrelator.CreateContext())
            {
                _sut.Debug("Debug message before override"); // Should not log

                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    _sut.Information("Info message in override");
                    _sut.Debug("Debug message in override"); // Should now log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(2), "Two messages should be logged from within override.");
                Assert.That(logs.Any(l => l.Level == Serilog.Events.LogEventLevel.Information && l.RenderMessage().Contains("Info message in override")), Is.True);
                Assert.That(logs.Any(l => l.Level == Serilog.Events.LogEventLevel.Debug && l.RenderMessage().Contains("Debug message in override")), Is.True);
            }
        }

        [Test]
        public void WhenOverrideToLessVerbose_LogsAtOverriddenLevel()
        {
            using (TestCorrelator.CreateContext())
            {
                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Warning))
                {
                    _sut.Information("Info message in override"); // Should NOT log
                    _sut.Debug("Debug message in override");     // Should NOT log
                    _sut.Warning("Warning message in override"); // Should log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1), "Only warning should be logged.");
                Assert.That(logs[0].Level, Is.EqualTo(Serilog.Events.LogEventLevel.Warning));
                StringAssert.Contains("Warning message in override", logs[0].RenderMessage());
            }
        }

        [Test]
        public void OverrideIsReset_AfterUsingBlock()
        {
            using (TestCorrelator.CreateContext())
            {
                _sut.Debug("Debug before"); // No

                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    _sut.Debug("Debug during"); // Yes
                }

                _sut.Debug("Debug after"); // No, should revert to Information default of inner logger via SUT

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1), "Only one debug message (during override) should be logged.");
                Assert.That(logs[0].Level, Is.EqualTo(Serilog.Events.LogEventLevel.Debug));
                StringAssert.Contains("Debug during", logs[0].RenderMessage());
            }
        }

        [Test]
        public void WriteMethods_RespectOverride()
        {
            using (TestCorrelator.CreateContext())
            {
                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    _sut.Debug("Debug message {Value}", 1);
                    _sut.Information("Info message {Value}", 2);
                    _sut.Warning("Warning message {Value}", 3);
                    _sut.Error("Error message {Value}", 4);
                    _sut.Fatal("Fatal message {Value}", 5);
                    _sut.Verbose("Verbose message {Value}", 6); // Should not log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(5));
                Assert.That(logs.Any(l => l.Level == Serilog.Events.LogEventLevel.Debug));
                Assert.That(logs.Any(l => l.Level == Serilog.Events.LogEventLevel.Information));
                Assert.That(logs.Any(l => l.Level == Serilog.Events.LogEventLevel.Warning));
                Assert.That(logs.Any(l => l.Level == Serilog.Events.LogEventLevel.Error));
                Assert.That(logs.Any(l => l.Level == Serilog.Events.LogEventLevel.Fatal));
                Assert.That(logs.All(l => l.Level >= Serilog.Events.LogEventLevel.Debug));
            }
        }

        [Test]
        public void ExceptionLogging_RespectsOverride()
        {
            using (TestCorrelator.CreateContext())
            {
                var ex = new Exception("Test Exception");
                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Error))
                {
                    _sut.Warning(ex, "Warning with exception"); // Should not log
                    _sut.Error(ex, "Error with exception");     // Should log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1));
                Assert.That(logs[0].Level, Is.EqualTo(Serilog.Events.LogEventLevel.Error));
                Assert.That(logs[0].Exception, Is.EqualTo(ex));
            }
        }

        private static IEnumerable<LogEvent> GetLoggedEventsFromCurrentTestCorrelatorContext()
        {
            return TestCorrelator.GetLogEventsFromCurrentContext();
        }

        [Test]
        public void ForContext_CreatesNewLoggerWithOverrideCapability()
        {
            using (TestCorrelator.CreateContext())
            {
                var contextLogger = _sut.ForContext("Property", "Value");

                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    contextLogger.Debug("Debug message from context logger"); // Should log
                }

                _sut.Debug("Debug message from original logger after context"); // Should not log (original logger still respects its own level)

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1));
                Assert.That(logs[0].Level, Is.EqualTo(Serilog.Events.LogEventLevel.Debug));
                Assert.That(logs[0].Properties.ContainsKey("Property"), Is.True);
                Assert.That(logs[0].Properties["Property"].ToString(), Is.EqualTo("\"Value\""));
            }
        }

        [Test]
        public void ForContext_WithSource_CreatesNewLoggerWithOverrideCapability()
        {
            using (TestCorrelator.CreateContext())
            {
                var contextLogger = _sut.ForContext<LevelOverridableLoggerTests>();

                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    contextLogger.Debug("Debug message from context logger with source"); // Should log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1));
                Assert.That(logs[0].Properties.ContainsKey("SourceContext"), Is.True);
                Assert.That(logs[0].Properties["SourceContext"].ToString(), Is.EqualTo($"\"{typeof(LevelOverridableLoggerTests).FullName}\""));
            }
        }

        [Test]
        public void ForContext_WithEnricher_CreatesNewLoggerWithOverrideCapability()
        {
            using (TestCorrelator.CreateContext())
            {
                var mockEnricher = new Mock<ILogEventEnricher>();
                LogEvent capturedLogEvent = null;
                mockEnricher.Setup(e => e.Enrich(It.IsAny<LogEvent>(), It.IsAny<ILogEventPropertyFactory>()))
                            .Callback<LogEvent, ILogEventPropertyFactory>((le, factory) => capturedLogEvent = le);

                var contextLogger = _sut.ForContext(mockEnricher.Object);

                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    contextLogger.Debug("Debug message from enriched context logger"); // Should log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1));
                mockEnricher.Verify(e => e.Enrich(It.IsAny<LogEvent>(), It.IsAny<ILogEventPropertyFactory>()), Times.Once);
                Assert.That(capturedLogEvent, Is.Not.Null);
                Assert.That(capturedLogEvent.MessageTemplate.Text, Is.EqualTo("Debug message from enriched context logger"));
            }
        }

        [Test]
        public void ForContext_WithMultipleEnrichers_CreatesNewLoggerWithOverrideCapability()
        {
            using (TestCorrelator.CreateContext())
            {
                var mockEnricher1 = new Mock<ILogEventEnricher>();
                var mockEnricher2 = new Mock<ILogEventEnricher>();

                var enrichers = new List<ILogEventEnricher> { mockEnricher1.Object, mockEnricher2.Object };
                var contextLogger = _sut.ForContext(enrichers);

                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    contextLogger.Debug("Debug message from multiple enriched context logger"); // Should log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1));
                mockEnricher1.Verify(e => e.Enrich(It.IsAny<LogEvent>(), It.IsAny<ILogEventPropertyFactory>()), Times.Once);
                mockEnricher2.Verify(e => e.Enrich(It.IsAny<LogEvent>(), It.IsAny<ILogEventPropertyFactory>()), Times.Once);
            }
        }

        [Test]
        public void ForContext_WithNullEnricherInList_HandlesGracefully()
        {
            using (TestCorrelator.CreateContext())
            {
                var mockEnricher = new Mock<ILogEventEnricher>();
                var enrichers = new List<ILogEventEnricher> { mockEnricher.Object, null };
                var contextLogger = _sut.ForContext(enrichers);

                using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Debug))
                {
                    contextLogger.Debug("Debug message with null enricher in list"); // Should log
                }

                var logs = GetLoggedEvents().ToList();
                Assert.That(logs.Count, Is.EqualTo(1));
                mockEnricher.Verify(e => e.Enrich(It.IsAny<LogEvent>(), It.IsAny<ILogEventPropertyFactory>()), Times.Once);
            }
        }

        [Test]
        public void ForContext_WithNullEnrichersList_ThrowsArgumentNullException()
        {
            using (TestCorrelator.CreateContext())
            {
                Assert.Throws<ArgumentNullException>(() => _sut.ForContext((IEnumerable<ILogEventEnricher>)null));
            }
        }

        [Test]
        public void ForContext_WithNullEnricher_ThrowsArgumentNullException()
        {
            using (TestCorrelator.CreateContext())
            {
                Assert.Throws<ArgumentNullException>(() => _sut.ForContext((ILogEventEnricher)null));
            }
        }

        [Test]
        public void Constructor_ThrowsArgumentException_IfTargetDoesNotImplementILogEventSink()
        {
            var mockLogger = new Mock<Serilog.ILogger>();
            // Ensure the mock does NOT implement ILogEventSink
            Assert.Throws<ArgumentException>(() => new LevelOverridableLogger(mockLogger.Object));
        }

        [Test]
        public void Constructor_ThrowsArgumentException_IfTargetIsNotSerilogCoreLogger()
        {
            // Create a mock that implements ILogger and ILogEventSink, but is NOT Serilog.Core.Logger
            var mockLogger = new Mock<Serilog.ILogger>();
            var mockSink = mockLogger.As<ILogEventSink>();

            // Setup Emit to avoid NRE if it's called
            mockSink.Setup(s => s.Emit(It.IsAny<LogEvent>()));

            // This should throw because the underlying logger is not a Serilog.Core.Logger
            Assert.Throws<ArgumentException>(() => new LevelOverridableLogger(mockLogger.Object));
        }

        // A minimal logger stub that implements ILogger and ILogEventSink but is not Serilog.Core.Logger
        private class NonSinkLoggerStub : Serilog.ILogger, ILogEventSink
        {
            public void Emit(LogEvent logEvent) { /* do nothing */ }
            public bool IsEnabled(LogEventLevel level) => true;
            public void Write(LogEvent logEvent) { Emit(logEvent); }

            // Implement all ILogger.Write overloads
            public void Write(LogEventLevel level, string messageTemplate) => Write(new LogEvent(DateTimeOffset.UtcNow, level, null, new MessageTemplateParser().Parse(messageTemplate), Enumerable.Empty<LogEventProperty>()));
            public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue) => Write(level, messageTemplate, new object[] { propertyValue });
            public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 p0, T1 p1) => Write(level, messageTemplate, new object[] { p0, p1 });
            public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(level, messageTemplate, new object[] { p0, p1, p2 });
            public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues) => Write(new LogEvent(DateTimeOffset.UtcNow, level, null, new MessageTemplateParser().Parse(messageTemplate), new MessageTemplateParser().Parse(messageTemplate).Tokens.OfType<PropertyToken>().Select((t, i) => new LogEventProperty(t.PropertyName, new ScalarValue(propertyValues[i])))));

            public void Write(LogEventLevel level, Exception exception, string messageTemplate) => Write(new LogEvent(DateTimeOffset.UtcNow, level, exception, new MessageTemplateParser().Parse(messageTemplate), Enumerable.Empty<LogEventProperty>()));
            public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue) => Write(level, exception, messageTemplate, new object[] { propertyValue });
            public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 p0, T1 p1) => Write(level, exception, messageTemplate, new object[] { p0, p1 });
            public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(level, exception, messageTemplate, new object[] { p0, p1, p2 });
            public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues) => Write(new LogEvent(DateTimeOffset.UtcNow, level, exception, new MessageTemplateParser().Parse(messageTemplate), new MessageTemplateParser().Parse(messageTemplate).Tokens.OfType<PropertyToken>().Select((t, i) => new LogEventProperty(t.PropertyName, new ScalarValue(propertyValues[i])))));

            // Implement all ILogger level-specific convenience methods
            public void Verbose(string messageTemplate) => Write(LogEventLevel.Verbose, messageTemplate);
            public void Verbose<T>(string messageTemplate, T propertyValue) => Write(LogEventLevel.Verbose, messageTemplate, propertyValue);
            public void Verbose<T0, T1>(string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Verbose, messageTemplate, p0, p1);
            public void Verbose<T0, T1, T2>(string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Verbose, messageTemplate, p0, p1, p2);
            public void Verbose(string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Verbose, messageTemplate, propertyValues);
            public void Verbose(Exception exception, string messageTemplate) => Write(LogEventLevel.Verbose, exception, messageTemplate);
            public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue) => Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue);
            public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Verbose, exception, messageTemplate, p0, p1);
            public void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Verbose, exception, messageTemplate, p0, p1, p2);
            public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValues);

            public void Debug(string messageTemplate) => Write(LogEventLevel.Debug, messageTemplate);
            public void Debug<T>(string messageTemplate, T propertyValue) => Write(LogEventLevel.Debug, messageTemplate, propertyValue);
            public void Debug<T0, T1>(string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Debug, messageTemplate, p0, p1);
            public void Debug<T0, T1, T2>(string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Debug, messageTemplate, p0, p1, p2);
            public void Debug(string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Debug, messageTemplate, propertyValues);
            public void Debug(Exception exception, string messageTemplate) => Write(LogEventLevel.Debug, exception, messageTemplate);
            public void Debug<T>(Exception exception, string messageTemplate, T propertyValue) => Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue);
            public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Debug, exception, messageTemplate, p0, p1);
            public void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Debug, exception, messageTemplate, p0, p1, p2);
            public void Debug(Exception exception, string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Debug, exception, messageTemplate, propertyValues);

            public void Information(string messageTemplate) => Write(LogEventLevel.Information, messageTemplate);
            public void Information<T>(string messageTemplate, T propertyValue) => Write(LogEventLevel.Information, messageTemplate, propertyValue);
            public void Information<T0, T1>(string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Information, messageTemplate, p0, p1);
            public void Information<T0, T1, T2>(string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Information, messageTemplate, p0, p1, p2);
            public void Information(string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Information, messageTemplate, propertyValues);
            public void Information(Exception exception, string messageTemplate) => Write(LogEventLevel.Information, exception, messageTemplate);
            public void Information<T>(Exception exception, string messageTemplate, T propertyValue) => Write(LogEventLevel.Information, exception, messageTemplate, propertyValue);
            public void Information<T0, T1>(Exception exception, string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Information, exception, messageTemplate, p0, p1);
            public void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Information, exception, messageTemplate, p0, p1, p2);
            public void Information(Exception exception, string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Information, exception, messageTemplate, propertyValues);

            public void Warning(string messageTemplate) => Write(LogEventLevel.Warning, messageTemplate);
            public void Warning<T>(string messageTemplate, T propertyValue) => Write(LogEventLevel.Warning, messageTemplate, propertyValue);
            public void Warning<T0, T1>(string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Warning, messageTemplate, p0, p1);
            public void Warning<T0, T1, T2>(string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Warning, messageTemplate, p0, p1, p2);
            public void Warning(string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Warning, messageTemplate, propertyValues);
            public void Warning(Exception exception, string messageTemplate) => Write(LogEventLevel.Warning, exception, messageTemplate);
            public void Warning<T>(Exception exception, string messageTemplate, T propertyValue) => Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue);
            public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Warning, exception, messageTemplate, p0, p1);
            public void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Warning, exception, messageTemplate, p0, p1, p2);
            public void Warning(Exception exception, string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Warning, exception, messageTemplate, propertyValues);

            public void Error(string messageTemplate) => Write(LogEventLevel.Error, messageTemplate);
            public void Error<T>(string messageTemplate, T propertyValue) => Write(LogEventLevel.Error, messageTemplate, propertyValue);
            public void Error<T0, T1>(string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Error, messageTemplate, p0, p1);
            public void Error<T0, T1, T2>(string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Error, messageTemplate, p0, p1, p2);
            public void Error(string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Error, messageTemplate, propertyValues);
            public void Error(Exception exception, string messageTemplate) => Write(LogEventLevel.Error, exception, messageTemplate);
            public void Error<T>(Exception exception, string messageTemplate, T propertyValue) => Write(LogEventLevel.Error, exception, messageTemplate, propertyValue);
            public void Error<T0, T1>(Exception exception, string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Error, exception, messageTemplate, p0, p1);
            public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Error, exception, messageTemplate, p0, p1, p2);
            public void Error(Exception exception, string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Error, exception, messageTemplate, propertyValues);

            public void Fatal(string messageTemplate) => Write(LogEventLevel.Fatal, messageTemplate);
            public void Fatal<T>(string messageTemplate, T propertyValue) => Write(LogEventLevel.Fatal, messageTemplate, propertyValue);
            public void Fatal<T0, T1>(string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Fatal, messageTemplate, p0, p1);
            public void Fatal<T0, T1, T2>(string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Fatal, messageTemplate, p0, p1, p2);
            public void Fatal(string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Fatal, messageTemplate, propertyValues);
            public void Fatal(Exception exception, string messageTemplate) => Write(LogEventLevel.Fatal, exception, messageTemplate);
            public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue) => Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue);
            public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 p0, T1 p1) => Write(LogEventLevel.Fatal, exception, messageTemplate, p0, p1);
            public void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2) => Write(LogEventLevel.Fatal, exception, messageTemplate, p0, p1, p2);
            public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues) => Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValues);

            public Serilog.ILogger ForContext(string propertyName, object value, bool destructureObjects = false) => this;
            public Serilog.ILogger ForContext<TSource>() => this;
            public Serilog.ILogger ForContext(Type source) => this;
            public Serilog.ILogger ForContext(ILogEventEnricher enricher) => this;
            public Serilog.ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers) => this;
            public bool BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate, out IEnumerable<LogEventProperty> properties)
            {
                parsedTemplate = new MessageTemplateParser().Parse(messageTemplate);
                properties = Enumerable.Empty<LogEventProperty>();
                return true;
            }
            public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
            {
                property = new LogEventProperty(propertyName, new ScalarValue(value));
                return true;
            }
        }

        [Test]
        public void Constructor_AcceptsSerilogCoreLogger()
        {
            var logger = new LoggerConfiguration().CreateLogger();
            Assert.DoesNotThrow(() => new LevelOverridableLogger(logger));
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_IfTargetIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LevelOverridableLogger(null));
        }
    }
}