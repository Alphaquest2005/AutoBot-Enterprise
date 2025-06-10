using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // For AsyncLocal
using Serilog;
using Serilog.Core;        // For ILogEventSink, Logger, LoggingLevelSwitch, ILogEventEnricher
using Serilog.Events;      // For LogEventLevel, LogEvent, LogEventProperty, ScalarValue
using Serilog.Parsing;     // For MessageTemplate

namespace Core.Common.Extensions
{
    // Original LogLevelOverride class - uses Serilog.Events.LogEventLevel
    public static class LogLevelOverride
    {
        // Use Serilog's LogEventLevel
        static readonly AsyncLocal<Serilog.Events.LogEventLevel?> _currentLevelOverride = new AsyncLocal<Serilog.Events.LogEventLevel?>();

        sealed class LevelOverrideReset : IDisposable
        {
            readonly Serilog.Events.LogEventLevel? _previousLevelOverride;

            public LevelOverrideReset(Serilog.Events.LogEventLevel? previousLevelOverride)
            {
                _previousLevelOverride = previousLevelOverride;
                
                // **ENTRY LOG**: This will always appear when LogLevelOverride.Begin() is called
                Log.Logger.Information("🔍 **LOGLEVELOVERRIDE_ENTRY**: LogLevelOverride using statement ENTERED - Level set to: {NewLevel}, Previous: {PreviousLevel}", 
                    _currentLevelOverride.Value?.ToString() ?? "NULL", 
                    _previousLevelOverride?.ToString() ?? "NULL");
            }

            public void Dispose()
            {
                // **EXIT LOG**: This will always appear when LogLevelOverride using statement exits
                Log.Logger.Information("🔍 **LOGLEVELOVERRIDE_EXIT**: LogLevelOverride using statement EXITING - Restoring level to: {RestoredLevel}", 
                    _previousLevelOverride?.ToString() ?? "NULL");
                    
                _currentLevelOverride.Value = _previousLevelOverride;
            }
        }

        public static IDisposable Begin(Serilog.Events.LogEventLevel level)
        {
            var resetValue = _currentLevelOverride.Value;
            _currentLevelOverride.Value = level;
            return new LevelOverrideReset(resetValue);
        }

        public static Serilog.Events.LogEventLevel? CurrentLevelOverride => _currentLevelOverride.Value;

        // Optional: For easier cleanup in test TearDown if something goes wrong
        public static void ResetForTesting()
        {
            _currentLevelOverride.Value = null;
        }
    }

    public class LevelOverridableLogger : Serilog.ILogger
    {
        private readonly Serilog.ILogger _target;
        private readonly ILogEventSink _emitter;
        // _concreteTargetForBind is used if the target is a Serilog.Core.Logger
        private readonly Serilog.Core.Logger _concreteTargetForBind;
        // Parser to use if _target is not a Serilog.Core.Logger
        private readonly MessageTemplateParser _fallbackParser = new MessageTemplateParser();

        public LevelOverridableLogger(Serilog.ILogger target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _emitter = target as ILogEventSink;
            if (_emitter == null)
            {
                throw new ArgumentException("The logger must also provide the ILogEventSink interface.", nameof(target));
            }

            _concreteTargetForBind = target as Serilog.Core.Logger;
            if (_concreteTargetForBind == null) 
            {
                if (!(target is Serilog.Core.Logger))
                {
                    throw new ArgumentException(
                        "The target logger must be a Serilog.Core.Logger (or derived) to ensure proper message and property binding. " +
                        "Current target type: " + target.GetType().FullName, nameof(target));
                }
            }
        }

        public bool IsEnabled(LogEventLevel level)
        {
            if (!LogLevelOverride.CurrentLevelOverride.HasValue)
                return _target.IsEnabled(level);
            return level >= LogLevelOverride.CurrentLevelOverride.Value;
        }

        // --- Core Write method ---
        public void Write(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (!IsEnabled(logEvent.Level)) return;
            _emitter.Emit(logEvent);
        }

        // --- ILogger.Write overloads ---
        // No Exception
        public void Write(LogEventLevel level, string messageTemplate)
        {
            if (IsEnabled(level))
                EmitInternal(level, null, messageTemplate, Array.Empty<object>());
        }
        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            if (IsEnabled(level))
                EmitInternal(level, null, messageTemplate, new object[] { propertyValue });
        }
        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 p0, T1 p1)
        {
            if (IsEnabled(level))
                EmitInternal(level, null, messageTemplate, new object[] { p0, p1 });
        }
        public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 p0, T1 p1, T2 p2)
        {
            if (IsEnabled(level))
                EmitInternal(level, null, messageTemplate, new object[] { p0, p1, p2 });
        }
        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            if (IsEnabled(level))
                EmitInternal(level, null, messageTemplate, propertyValues);
        }

        // With Exception
        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
        {
            if (IsEnabled(level))
                EmitInternal(level, exception, messageTemplate, Array.Empty<object>());
        }
        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            if (IsEnabled(level))
                EmitInternal(level, exception, messageTemplate, new object[] { propertyValue });
        }
        public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 p0, T1 p1)
        {
            if (IsEnabled(level))
                EmitInternal(level, exception, messageTemplate, new object[] { p0, p1 });
        }
        public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 p0, T1 p1, T2 p2)
        {
            if (IsEnabled(level))
                EmitInternal(level, exception, messageTemplate, new object[] { p0, p1, p2 });
        }
        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (IsEnabled(level))
                EmitInternal(level, exception, messageTemplate, propertyValues);
        }

        // --- Internal Emit Helper ---
        private void EmitInternal(LogEventLevel level, Exception exception, string messageTemplate, object[] propertyValues)
        {
            MessageTemplate parsedTemplate;
            IEnumerable<LogEventProperty> properties;

            if (_concreteTargetForBind != null)
            {
                // Use the target's efficient binding if available
                _concreteTargetForBind.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out properties);
            }
            else
            {
                // Fallback to manual parsing and simplified binding
                parsedTemplate = _fallbackParser.Parse(messageTemplate);
                var boundProperties = new List<LogEventProperty>();
                if (propertyValues != null && propertyValues.Length > 0)
                {
                    var propertyTokens = parsedTemplate.Tokens.OfType<PropertyToken>().ToList();
                    for (int i = 0; i < propertyValues.Length && i < propertyTokens.Count; i++)
                    {
                        var token = propertyTokens[i];
                        LogEventProperty prop;
                        if (_target is Serilog.Core.Logger coreTargetForPropBind && coreTargetForPropBind.BindProperty(token.PropertyName, propertyValues[i], token.Destructuring == Destructuring.Destructure, out prop))
                        {
                            boundProperties.Add(prop);
                        }
                        else 
                        {
                            boundProperties.Add(new LogEventProperty(token.PropertyName, new ScalarValue(propertyValues[i])));
                        }
                    }
                }
                properties = boundProperties;
            }

            var logEvent = new LogEvent(DateTimeOffset.UtcNow, level, exception, parsedTemplate, properties);
            _emitter.Emit(logEvent);
        }

        // --- Level-specific convenience methods ---
        // Verbose
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

        // Debug
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

        // Information
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

        // Warning
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

        // Error
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

        // Fatal
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

        // --- ForContext ---
        public Serilog.ILogger ForContext(string propertyName, object value, bool destructureObjects = false) => new LevelOverridableLogger(_target.ForContext(propertyName, value, destructureObjects));
        public Serilog.ILogger ForContext<TSource>() => new LevelOverridableLogger(_target.ForContext<TSource>());
        public Serilog.ILogger ForContext(Type source) => new LevelOverridableLogger(_target.ForContext(source));
        public Serilog.ILogger ForContext(ILogEventEnricher enricher)
        {
            if (enricher == null) throw new ArgumentNullException(nameof(enricher));
            return new LevelOverridableLogger(_target.ForContext(enricher));
        }
        public Serilog.ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
        {
            if (enrichers == null) throw new ArgumentNullException(nameof(enrichers));
            Serilog.ILogger enrichedLogger = _target;
            foreach (var enricher in enrichers.Where(e => e != null))
            {
                enrichedLogger = enrichedLogger.ForContext(enricher);
            }
            return new LevelOverridableLogger(enrichedLogger);
        }

        // --- Binding Methods (delegating or using fallback) ---
        public bool BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate, out IEnumerable<LogEventProperty> properties)
        {
            if (_concreteTargetForBind != null)
            {
                return _concreteTargetForBind.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out properties);
            }

            // Fallback behavior
            parsedTemplate = _fallbackParser.Parse(messageTemplate);
            var boundProperties = new List<LogEventProperty>();
            if (propertyValues != null && propertyValues.Length > 0)
            {
                var propertyTokens = parsedTemplate.Tokens.OfType<PropertyToken>().ToList();
                for (int i = 0; i < propertyValues.Length && i < propertyTokens.Count; i++)
                {
                    var token = propertyTokens[i];
                    LogEventProperty prop;
                    if (_target is Serilog.Core.Logger coreTargetForPropBind && coreTargetForPropBind.BindProperty(token.PropertyName, propertyValues[i], token.Destructuring == Destructuring.Destructure, out prop))
                    {
                        boundProperties.Add(prop);
                    }
                    else
                    {
                        boundProperties.Add(new LogEventProperty(token.PropertyName, new ScalarValue(propertyValues[i])));
                    }
                }
            }
            properties = boundProperties;
            return true; 
        }

        public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
        {
            if (_concreteTargetForBind != null)
            {
                return _concreteTargetForBind.BindProperty(propertyName, value, destructureObjects, out property);
            }
            if (_target is Serilog.Core.Logger coreTarget) 
            {
                return coreTarget.BindProperty(propertyName, value, destructureObjects, out property);
            }
            property = new LogEventProperty(propertyName, new ScalarValue(value)); 
            return true; 
        }
    }
}