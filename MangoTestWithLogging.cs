using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Core.Common.Extensions;

namespace TestRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set up minimal console logger for visibility
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("🚀 **MANGO_TEST_RUNNER**: Starting surgical debugging test execution");

            try
            {
                // Use surgical debugging LogLevelOverride to capture complete execution
                // using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // COMMENTED OUT: Preventing singleton conflicts
                // {
                    Log.Information("🔬 **SURGICAL_DEBUGGING_START**: Running MANGO test with comprehensive logging");
                    
                    // This scope will terminate the application when done, providing focused logs
                    var testInstance = new AutoBotUtilities.Tests.PDFImportTests();
                    
                    Log.Information("📋 **TEST_SETUP**: Running CanImportMango03152025TotalAmount_AfterLearning test");
                    
                    await testInstance.CanImportMango03152025TotalAmount_AfterLearning();
                    
                    Log.Information("✅ **TEST_COMPLETED**: MANGO test finished successfully");
                // }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ **TEST_EXECUTION_EXCEPTION**: Exception during MANGO test execution");
                Log.Error("   - **EXCEPTION_TYPE**: {ExceptionType}", ex.GetType().FullName);
                Log.Error("   - **EXCEPTION_MESSAGE**: {ExceptionMessage}", ex.Message);
                Log.Error("   - **STACK_TRACE**: {StackTrace}", ex.StackTrace);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}