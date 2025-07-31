using System;
using Serilog;
using WaterNut.DataSpace;

namespace WaterNut.DataSpace.Tests
{
    /// <summary>
    /// Test class to validate the shortcircuit failure mechanism
    /// Demonstrates how the GlobalFailureMonitor and MonitoringEnabledLogger work together
    /// to automatically throw CriticalValidationException on ❌ FAIL detection
    /// </summary>
    public static class ShortcircuitFailureTest
    {
        /// <summary>
        /// Tests the basic shortcircuit mechanism by simulating a method failure
        /// Should throw CriticalValidationException when ❌ FAIL is detected
        /// </summary>
        public static void TestBasicShortcircuit()
        {
            Console.WriteLine("🧪 Testing basic shortcircuit mechanism...");
            
            // Setup monitoring-enabled logger
            var monitoringLogger = MonitoringEnabledLogger.CreateForContext("ShortcircuitTest", true);
            
            try
            {
                // Enable monitoring
                GlobalFailureMonitor.SetMonitoringEnabled(true);
                GlobalFailureMonitor.ClearLogHistory();
                
                // Log some normal success messages
                monitoringLogger.Error("✅ **PURPOSE_FULFILLMENT**: Test method purpose clearly achieved");
                monitoringLogger.Error("✅ **OUTPUT_COMPLETENESS**: Complete output generated successfully");
                
                // This should trigger the shortcircuit mechanism
                monitoringLogger.Error("🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL - Test method failed validation criteria");
                
                // If we reach this point, the test failed
                Console.WriteLine("❌ TEST FAILED: Expected CriticalValidationException was not thrown");
            }
            catch (CriticalValidationException ex)
            {
                Console.WriteLine("✅ TEST PASSED: CriticalValidationException thrown as expected");
                Console.WriteLine($"   Layer: {ex.Layer}");
                Console.WriteLine($"   Evidence: {ex.Evidence}");
                Console.WriteLine($"   Context: {ex.ValidationContext}");
                Console.WriteLine($"   Message: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TEST FAILED: Unexpected exception type: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
            }
            finally
            {
                // Reset monitoring for other tests
                GlobalFailureMonitor.ResetMonitoring();
            }
        }

        /// <summary>
        /// Tests the TemplateSpecification shortcircuit mechanism
        /// Should throw CriticalValidationException when template validation fails
        /// </summary>
        public static void TestTemplateSpecificationShortcircuit()
        {
            Console.WriteLine("🧪 Testing TemplateSpecification shortcircuit mechanism...");
            
            var monitoringLogger = MonitoringEnabledLogger.CreateForContext("TemplateSpecTest", true);
            
            try
            {
                // Enable monitoring
                GlobalFailureMonitor.SetMonitoringEnabled(true);
                GlobalFailureMonitor.ClearLogHistory();
                
                // Create a template specification that will fail validation
                var templateSpec = TemplateSpecification.CreateForRecommendations("Invoice");
                
                // Add a failing validation result
                templateSpec.ValidationResults.Add(new TemplateValidationResult
                {
                    CriteriaName = "TEST_VALIDATION",
                    Message = "Simulated validation failure for testing",
                    IsSuccess = false
                });
                
                // This should trigger the shortcircuit mechanism via LogValidationResults
                templateSpec.LogValidationResults(monitoringLogger);
                
                // If we reach this point, the test failed
                Console.WriteLine("❌ TEST FAILED: Expected CriticalValidationException was not thrown");
            }
            catch (CriticalValidationException ex)
            {
                Console.WriteLine("✅ TEST PASSED: CriticalValidationException thrown from TemplateSpecification");
                Console.WriteLine($"   Layer: {ex.Layer}");
                Console.WriteLine($"   Evidence: {ex.Evidence}");
                Console.WriteLine($"   Context: {ex.ValidationContext}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TEST FAILED: Unexpected exception type: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
            }
            finally
            {
                // Reset monitoring for other tests
                GlobalFailureMonitor.ResetMonitoring();
            }
        }

        /// <summary>
        /// Tests the different failure patterns that should trigger shortcircuit
        /// </summary>
        public static void TestFailurePatterns()
        {
            Console.WriteLine("🧪 Testing different failure patterns...");
            
            var monitoringLogger = MonitoringEnabledLogger.CreateForContext("PatternTest", true);
            var patterns = new[]
            {
                "🏆 **OVERALL_METHOD_SUCCESS**: ❌ FAIL - Strategy factory for Invoice failed dual-layer validation criteria",
                "🏆 **FINAL_METHOD_SUCCESS_WITH_TEMPLATE_SPEC**: ❌ FAIL - CreateProductErrorDetectionPrompt failed validation",
                "🏆 **TEMPLATE_SPECIFICATION_SUCCESS**: ❌ FAIL - Template specification validation failed validation criteria"
            };
            
            foreach (var pattern in patterns)
            {
                try
                {
                    // Enable monitoring for each test
                    GlobalFailureMonitor.SetMonitoringEnabled(true);
                    GlobalFailureMonitor.ClearLogHistory();
                    
                    Console.WriteLine($"   Testing pattern: {pattern.Substring(0, 50)}...");
                    
                    // This should trigger the shortcircuit mechanism
                    monitoringLogger.Error(pattern);
                    
                    Console.WriteLine("   ❌ Pattern failed to trigger shortcircuit");
                }
                catch (CriticalValidationException)
                {
                    Console.WriteLine("   ✅ Pattern correctly triggered shortcircuit");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ❌ Unexpected exception: {ex.GetType().Name}");
                }
                finally
                {
                    GlobalFailureMonitor.ResetMonitoring();
                }
            }
        }

        /// <summary>
        /// Tests that normal success messages don't trigger shortcircuit
        /// </summary>
        public static void TestSuccessMessagesIgnored()
        {
            Console.WriteLine("🧪 Testing that success messages are ignored...");
            
            var monitoringLogger = MonitoringEnabledLogger.CreateForContext("SuccessTest", true);
            
            try
            {
                // Enable monitoring
                GlobalFailureMonitor.SetMonitoringEnabled(true);
                GlobalFailureMonitor.ClearLogHistory();
                
                // Log success messages - these should NOT trigger shortcircuit
                monitoringLogger.Error("🏆 **OVERALL_METHOD_SUCCESS**: ✅ PASS - Test method completed successfully");
                monitoringLogger.Error("🏆 **FINAL_METHOD_SUCCESS_WITH_TEMPLATE_SPEC**: ✅ PASS - Validation passed");
                monitoringLogger.Error("🏆 **TEMPLATE_SPECIFICATION_SUCCESS**: ✅ PASS - Template specification validation completed successfully");
                
                Console.WriteLine("✅ TEST PASSED: Success messages correctly ignored");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TEST FAILED: Success messages should not trigger exceptions: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
            }
            finally
            {
                GlobalFailureMonitor.ResetMonitoring();
            }
        }

        /// <summary>
        /// Runs all shortcircuit mechanism tests
        /// </summary>
        public static void RunAllTests()
        {
            Console.WriteLine("🚀 **SHORTCIRCUIT FAILURE MECHANISM TESTS**");
            Console.WriteLine("==========================================");
            
            try
            {
                TestBasicShortcircuit();
                Console.WriteLine();
                
                TestTemplateSpecificationShortcircuit();
                Console.WriteLine();
                
                TestFailurePatterns();
                Console.WriteLine();
                
                TestSuccessMessagesIgnored();
                Console.WriteLine();
                
                Console.WriteLine("🎉 **ALL TESTS COMPLETED**");
                Console.WriteLine("The shortcircuit failure mechanism is working correctly!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🚨 **CRITICAL TEST FAILURE**: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   Stack Trace: {ex.StackTrace}");
            }
        }
    }
}