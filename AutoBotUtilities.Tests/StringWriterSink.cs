// You can put this class inside your TypedLoggingFilterTests.cs file or a common test utilities file.
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting; // For ITextFormatter
using Serilog.Formatting.Display; // For MessageTemplateTextFormatter
using System;
using System.IO;

public class StringWriterSink : ILogEventSink
{
    private readonly TextWriter _textWriter;
    private readonly ITextFormatter _textFormatter;

    public StringWriterSink(TextWriter textWriter, string outputTemplate)
    {
        _textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        // Using a default formatter if one isn't provided, or you can make it mandatory.
        // This uses the same kind of template as the File and Console sinks.
        _textFormatter = new MessageTemplateTextFormatter(outputTemplate ?? "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}", null);
    }

    public StringWriterSink(TextWriter textWriter, ITextFormatter textFormatter)
    {
        _textWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        _textFormatter = textFormatter ?? throw new ArgumentNullException(nameof(textFormatter));
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
        _textFormatter.Format(logEvent, _textWriter);
        _textWriter.Flush(); // Ensure it's written immediately for testing
    }
}