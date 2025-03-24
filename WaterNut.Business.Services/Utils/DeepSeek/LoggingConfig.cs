using Serilog;
using Microsoft.Extensions.Logging;
using WaterNut.Business.Services.Utils;

public static class LoggingConfig
{
    public static ILogger<DeepSeekInvoiceApi> CreateLogger()
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console() // Now works with Serilog.Sinks.Console
            .WriteTo.File("logs/deepseek.log")
            .CreateLogger();

        // Create logger factory
        var factory = LoggerFactory.Create(builder => {
            builder
                .AddSerilog() // Add Serilog provider
                .AddFilter("Microsoft", LogLevel.Warning);
        });

        return factory.CreateLogger<DeepSeekInvoiceApi>();
    }
}