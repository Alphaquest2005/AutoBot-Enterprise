# Your Plan - Iteration 2

## Current Iteration Objective: Get `AutoBotUtilities.Tests\EmailDownloaderIntegrationTests.cs:301-301 ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest()` to pass.

This plan outlines the steps to achieve the objective. Each step will be executed sequentially.

### Phase 1: Initial Assessment & Test Execution

1.  **Understand the Test:**
    *   Read `AutoBotUtilities.Tests\EmailDownloaderIntegrationTests.cs` to understand the `ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest()` method's logic, setup, and assertions.
    *   Consult `ProjectStructure.md` (PSM-P) to locate the test file and any related source files (e.g., `EmailProcessor.cs`, PDF import logic). If `ProjectStructure.md` is incomplete for these areas, update it.
    *   Log `META_LOG_DIRECTIVE` for intent and outcome.
    *   **Status:** Completed. Test logic, setup, and assertion points understood. Test involves sending an email with a PDF, simulating download, marking unread, then calling `EmailProcessor.ProcessEmailsAsync` and verifying database entries.

2.  **Execute the Test & Capture Initial Output:**
    *   Run the specific test `ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest()` using the provided command.
    *   Capture the full output, including any error messages or stack traces.
    *   Log `META_LOG_DIRECTIVE` for intent and outcome.
    *   **Status:** Completed. Test executed, but failed with extensive error logs. Initial run was too verbose, subsequent run with `MinimumLevel.Error` still produced large output due to repeated errors.

3.  **Analyze Initial Test Output:**
    *   Review the captured test output for immediate clues about the failure.
    *   Check for any existing application logs generated during the test run.
    *   Log `META_LOG_DIRECTIVE` for analysis and initial findings.
    *   **Status:** In Progress.
    *   **Findings:**
        *   **Primary Error:** Repeated `System.ArgumentException: An item with the same key has already been added.` originating from `AutoBotUtilities.ImportUtils.ProcessImportFile`. This suggests `ProcessImportFile` is being called multiple times for the same file/template combination, or the key generation is flawed.
        *   **Secondary Error:** `Error added to context: All 9 required lines appear to have failed for File: ... TemplateId: 3.` This indicates a template processing failure for a specific PDF and template. This might be a symptom of the primary issue (e.g., retries leading to duplicate key attempts).
        *   **Observation:** The test run is still very long due to the sheer volume of these errors being logged. This is not an infinite loop, but a high frequency of discrete error events.
    *   **Next Action:** Investigate `AutoBotUtilities.ImportUtils.ProcessImportFile` to understand how the `Imports` dictionary is managed and how keys are generated. Also, examine the call stack leading to `ProcessImportFile` to see why it might be called repeatedly for the same key.

### Phase 2: Logging Instrumentation & Debugging

4.  **Identify Key Execution Paths for Logging:**
    *   Based on test analysis, identify the critical methods and classes involved in email processing and PDF import that are likely contributing to the failure.
    *   Log `META_LOG_DIRECTIVE` for identified paths.

5.  **Implement Targeted Logging (Typed Logging System):**
    *   For identified methods/classes, add or enhance Serilog logging using `TypedLoggerExtensions`.
    *   Focus on `LogCategory.MethodBoundary` (entry/exit), `LogCategory.InternalStep` (key internal states, conditional branches), and `LogCategory.DiagnosticDetail` (variable values, exceptions).
    *   Ensure `LogFilterState` is configured to capture these detailed logs for the target areas.
    *   Log `META_LOG_DIRECTIVE` for each logging modification.

6.  **Re-execute Test with Enhanced Logging:**
    *   Run the test again.
    *   Capture the new, detailed log output.
    *   Log `META_LOG_DIRECTIVE` for intent and outcome.

7.  **Detailed Log Analysis & Root Cause Identification:**
    *   Analyze the detailed logs to trace the execution flow, identify where the process deviates from expected behavior, and pinpoint the exact cause of the failure.
    *   Update `RMP_Modules/RMP_Section5_LogAnalysis_Iter2.md` with findings.
    *   Log `META_LOG_DIRECTIVE` for root cause.

### Phase 3: Code Correction & Verification

8.  **Implement Code Fix:**
    *   Apply the necessary code changes to resolve the identified root cause.
    *   Log `META_LOG_DIRECTIVE` for code modification.

9.  **Build Solution:**
    *   Build the entire solution to ensure changes compile correctly.
    *   Log `META_LOG_DIRECTIVE` for build outcome.

10. **Verify Test Pass:**
    *   Run the `ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest()` again.
    *   Confirm that the test now passes.
    *   Log `META_LOG_DIRECTIVE` for verification outcome.

### Phase 4: Cleanup & Finalization

11. **Logging Cleanup/Refinement:**
    *   Review the newly added logs. Remove any temporary debugging logs.
    *   Ensure remaining logs are well-categorized and provide lasting value.
    *   Reset `LogFilterState` to default operational settings.
    *   Log `META_LOG_DIRECTIVE` for logging refinement.

12. **Update `ProjectStructure.md`:**
    *   Ensure `ProjectStructure.md` is fully updated with any new files, classes, or members encountered/modified during this iteration (PSM-P Point 5).
    *   Log `META_LOG_DIRECTIVE` for PSM-P update.

13. **Complete Task Footer:**
    *   Execute and verify all items in `RMP_Modules/RMP_Task_Footer_Iter2.md`.
    *   Log `META_LOG_DIRECTIVE` for task footer completion.