using System;
using Serilog;
using Serilog.Events;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Wrapper around ILogger that automatically monitors all log messages for critical failure patterns
    /// This provides seamless integration with existing code while adding automatic shortcircuit functionality
    /// </summary>
    public class MonitoringEnabledLogger : ILogger
    {
        private readonly ILogger _innerLogger;
        private readonly bool _enableMonitoring;

        public MonitoringEnabledLogger(ILogger innerLogger, bool enableMonitoring = true)
        {
            _innerLogger = innerLogger ?? throw new ArgumentNullException(nameof(innerLogger));
            _enableMonitoring = enableMonitoring;
        }

        /// <summary>
        /// Creates a monitoring-enabled logger from the global Log.Logger
        /// </summary>
        /// <param name="enableMonitoring">Whether to enable failure monitoring</param>
        /// <returns>A monitoring-enabled logger</returns>
        public static MonitoringEnabledLogger Create(bool enableMonitoring = true)
        {
            return new MonitoringEnabledLogger(Log.Logger, enableMonitoring);
        }

        /// <summary>
        /// Creates a monitoring-enabled logger with a specific context
        /// </summary>
        /// <param name="sourceContext">The source context for the logger</param>
        /// <param name="enableMonitoring">Whether to enable failure monitoring</param>
        /// <returns>A monitoring-enabled logger</returns>
        public static MonitoringEnabledLogger CreateForContext(string sourceContext, bool enableMonitoring = true)
        {
            var contextLogger = Log.Logger.ForContext("SourceContext", sourceContext);
            return new MonitoringEnabledLogger(contextLogger, enableMonitoring);
        }

        // ILogger implementation - all methods forward to inner logger with monitoring

        public ILogger ForContext(ILogEventEnricher enricher)
        {
            return new MonitoringEnabledLogger(_innerLogger.ForContext(enricher), _enableMonitoring);
        }

        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
        {
            return new MonitoringEnabledLogger(_innerLogger.ForContext(propertyName, value, destructureObjects), _enableMonitoring);
        }

        public ILogger ForContext<TSource>()
        {
            return new MonitoringEnabledLogger(_innerLogger.ForContext<TSource>(), _enableMonitoring);
        }

        public ILogger ForContext(Type source)
        {
            return new MonitoringEnabledLogger(_innerLogger.ForContext(source), _enableMonitoring);
        }

        public void Write(LogEvent logEvent)
        {
            _innerLogger.Write(logEvent);
            
            // Monitor error-level messages for critical failures
            if (_enableMonitoring && logEvent.Level >= LogEventLevel.Error)
            {
                var message = logEvent.RenderMessage();
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, message, logEvent.Level);
            }
        }

        public void Write(LogEventLevel level, string messageTemplate)
        {
            _innerLogger.Write(level, messageTemplate);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, messageTemplate, level);
            }
        }

        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            _innerLogger.Write(level, messageTemplate, propertyValue);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValue);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _innerLogger.Write(level, messageTemplate, propertyValue0, propertyValue1);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValue0, propertyValue1);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _innerLogger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            _innerLogger.Write(level, messageTemplate, propertyValues);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValues);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
        {
            _innerLogger.Write(level, exception, messageTemplate);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, messageTemplate, level);
            }
        }

        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            _innerLogger.Write(level, exception, messageTemplate, propertyValue);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValue);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            _innerLogger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValue0, propertyValue1);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            _innerLogger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _innerLogger.Write(level, exception, messageTemplate, propertyValues);
            
            if (_enableMonitoring && level >= LogEventLevel.Error)
            {
                var formattedMessage = string.Format(messageTemplate, propertyValues);
                GlobalFailureMonitor.MonitorLogMessage(_innerLogger, formattedMessage, level);
            }
        }

        public bool IsEnabled(LogEventLevel level)
        {
            return _innerLogger.IsEnabled(level);
        }

        // Convenience methods for common logging scenarios
        public void Error(string messageTemplate)
        {
            Write(LogEventLevel.Error, messageTemplate);
        }

        public void Error<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValue);
        }

        public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Error, exception, messageTemplate);
        }

        public void Error<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValue);
        }

        public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValues);
        }

        public void Information(string messageTemplate)
        {
            Write(LogEventLevel.Information, messageTemplate);
        }

        public void Information<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValue);
        }

        public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValues);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Information, exception, messageTemplate);
        }

        public void Information<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValue);
        }

        public void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValues);
        }

        public void Warning(string messageTemplate)
        {
            Write(LogEventLevel.Warning, messageTemplate);
        }

        public void Warning<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValues);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate);
        }

        public void Warning<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValues);
        }

        public void Debug(string messageTemplate)
        {
            Write(LogEventLevel.Debug, messageTemplate);
        }

        public void Debug<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate);
        }

        public void Debug<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValues);
        }

        public void Verbose(string messageTemplate)
        {
            Write(LogEventLevel.Verbose, messageTemplate);
        }

        public void Verbose<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate);
        }

        public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValues);
        }

        public void Fatal(string messageTemplate)
        {
            Write(LogEventLevel.Fatal, messageTemplate);
        }

        public void Fatal<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValue);
        }

        public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate);
        }

        public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue);
        }

        public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValues);
        }
    }
}