# Refactoring Master Plan

## 1. Foundational LLM Directives

*   Achieve a functional and correct codebase.
*   Prioritize efficiency and simplicity in solutions.
*   Maintain code readability and adherence to best practices.
*   Utilize structured logging (Serilog) for enhanced observability and debugging.
*   Operate autonomously within the defined workflow and constraints.
*   Continuously learn and improve the Core Instrumentation Prompt based on iteration outcomes.
*   Escalate complex, persistent issues after documented attempts.

## 2. Current User-Defined Iteration Objective & Priority

**Objective:** Make the `ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest()` in `AutoBotUtilities.Tests\EmailDownloaderIntegrationTests.cs` pass.
**Priority:** High - This test failure indicates a critical issue in the email processing and attachment saving logic that needs to be resolved for the application to function correctly.

## 3. Your Core Instrumentation Prompt (Roo's Operational Instructions)

This section contains Roo's self-defined and evolving instructions for performing tasks, managing workflow, using tools, logging, handling errors, reflecting, and self-improving. Roo will update this section based on learnings from each iteration.

**Workflow Steps:**

1.  **INITIATE ITERATION CYCLE:**
    *   Read `RefactoringMasterPlan.md`.
    *   Identify the "Current User-Defined Iteration Objective" (Section 2).
    *   Review the "Your Plan" (Section 4) for the current iteration's steps. If Section 4 is empty or marked complete, create a new plan based on the objective and current state.
    *   Review "Failure Tracking" (Section 8) to understand previous roadblocks and attempt counts.
    *   Begin "Workflow Step 2 - EXECUTE PLAN".

2.  **EXECUTE PLAN:**
    *   Execute the next step in "Your Plan" (Section 4).
    *   If the step involves code modification, use `read_file` first, then `apply_diff` or `write_to_file`.
    *   If the step involves running a command (build, test), use `execute_command`.
    *   If the step involves adding logging, use `insert_content` or `apply_diff`. Ensure logging adheres to Serilog structured logging best practices and includes `META_LOG_DIRECTIVE` where appropriate.
    *   After executing a step, proceed to "Workflow Step 3 - BUILD/TEST/FIX LOOP".

3.  **BUILD/TEST/FIX LOOP:**
    *   **Build:** Execute the defined build command (`& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64`).
    *   **Analyze Build Output:**
        *   If build fails with errors: Analyze the errors. Formulate a fix. Update "Your Plan" (Section 4) with the fix steps. Increment the failure counter for the current roadblock in Section 8. Return to "Workflow Step 2 - EXECUTE PLAN" to apply the fix.
        *   If build succeeds or has only warnings: Proceed to "Test".
    *   **Test:** If the plan requires running tests, execute the defined test command (`& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" 'AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll' '/Tests:ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest'`).
    *   **Analyze Test Output:**
        *   If test fails: Analyze the failure (exception type, stack trace, log output). Formulate a plan to diagnose further (e.g., add more logging) or fix the issue. Update "Your Plan" (Section 4) with the new steps. Increment the failure counter for the current roadblock in Section 8. Check the failure counter for the current roadblock in Section 8. If the counter reaches 3, proceed to "Workflow Step 5 - ESCALATE". Otherwise, return to "Workflow Step 2 - EXECUTE PLAN".
        *   If test passes: The current objective is achieved. Proceed to "Workflow Step 6 - REFLECT & SELF-IMPROVE".

4.  **HANDLE TOOL FAILURES:**
    *   If a tool execution fails (e.g., `apply_diff` fails):
        *   Log the tool failure and the error message using `META_LOG_DIRECTIVE`.
        *   Attempt an alternative tool if available and appropriate (e.g., use `write_to_file` if `apply_diff` fails). Update "Your Plan" (Section 4) to reflect the alternative tool usage.
        *   If no alternative tool is suitable or the alternative also fails, increment the failure counter for the current roadblock in Section 8. Check the failure counter. If it reaches 3, proceed to "Workflow Step 5 - ESCALATE". Otherwise, return to "Workflow Step 2 - EXECUTE PLAN" to try the next step or a revised approach.

5.  **ESCALATE:**
    *   This step is reached after 3 failed attempts on the same specific roadblock preventing the achievement of the user's functional objective.
    *   Populate Section 7 (Escalation to Human Input) with:
        *   Date and Time.
        *   Current Iteration number.
        *   Clear description of the roadblock.
        *   Detailed list of steps taken to resolve it, including analysis and why they failed.
        *   Current state of the code/tests.
        *   Specific request for human guidance.
    *   Stop autonomous operation and wait for human input.

6.  **REFLECT & SELF-IMPROVE:**
    *   This step is reached when the "Current User-Defined Iteration Objective" (Section 2) is achieved (test passes).
    *   Review the entire iteration:
        *   What was the objective?
        *   What steps were taken?
        *   What roadblocks were encountered?
        *   How were roadblocks overcome (or why weren't they)?
        *   How effective was the logging?
        *   How effective was the "Core Instrumentation Prompt"?
    *   Populate Section 6 (Post-Iteration Reflection & Learning).
    *   Based on the reflection, identify potential improvements to the "Core Instrumentation Prompt" (Section 3).
    *   **Directly edit Section 3** to implement the identified improvements.
    *   Populate Section 9 (Scoring) with a summary of the iteration's outcome.
    *   Clear Section 2 (Current User-Defined Iteration Objective & Priority) and Section 4 (Your Plan) to prepare for the next iteration.
    *   Use the `attempt_completion` tool to signal the end of the iteration and present the outcome (test passing).

**Logging Directives:**

*   Use Serilog structured logging (`log.Information`, `log.Debug`, `log.Warning`, `log.Error`, `log.Fatal`).
*   Include relevant context in log events (e.g., method name, parameters, state).
*   Use `LogContext.PushProperty` for contextual information that applies to a block of code (e.g., `InvocationId`, `OperationName`).
*   Add `META_LOG_DIRECTIVE` warnings to the log output to explain:
    *   The *Type* of directive (e.g., "Instrumentation", "Analysis", "ObjectiveConfirmation", "PlanStep").
    *   The *Context* it relates to (e.g., "Method:ProcessEmailsAsync", "PlanStep_4.1", "BuildFailure:MSB3277").
    *   The *Directive* itself (a brief explanation of the action taken or insight gained).
    *   The *ExpectedChange* or *ExpectedBehavioralChange* resulting from the action.
    *   The *SourceIteration* (e.g., "LLM_Iter_1.1", "LLM_Iter_1.1_PlanStep_4.2").
*   Ensure `META_LOG_DIRECTIVE`s are distinct and easily searchable in the log output.

**Tool Usage Directives:**

*   Always use `read_file` before attempting to modify an existing file.
*   Prefer `apply_diff` for targeted code changes.
*   Use `write_to_file` for creating new files or when `apply_diff` is not suitable or fails.
*   Use `insert_content` for adding blocks of code (like new log statements) at specific locations.
*   Use `execute_command` for all build and test execution.
*   Use `ask_followup_question` only when explicitly allowed by the workflow (initial clarification or after escalation).
*   Use `attempt_completion` only at the successful conclusion of an iteration (test passes) after reflection.

**Error Handling Directives:**

*   Analyze build and test output carefully to understand the root cause of errors.
*   Use the information from logs (including `META_LOG_DIRECTIVE`s) to diagnose issues.
*   Increment failure counters in Section 8 for persistent roadblocks.
*   Follow the escalation protocol in "Workflow Step 5 - ESCALATE" after 3 failed attempts on the same specific roadblock.

**Self-Improvement Directives:**

*   Regularly review the effectiveness of the "Core Instrumentation Prompt" during the "REFLECT & SELF-IMPROVE" step.
*   Identify patterns in failures or inefficiencies.
*   Propose concrete, written improvements to the instructions in Section 3.
*   Implement the approved improvements by directly editing Section 3.

**Cardinal Rules (Strictly Adhere):**

*   DO NOT change application logic unless it is a direct fix required to achieve the functional objective and is part of the documented plan. Logging instrumentation is the primary focus.
*   DO NOT modify autogenerated files or files in folders with "generated" in the path.
*   ALWAYS use the specified build command: `& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64`.
*   ALWAYS use the specified test command (modified with the correct test name): `& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\TestPlatform\vstest.console.exe" 'AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll' '/Tests:ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest'`.
*   ALWAYS use `read_file` before modifying an existing file.
*   ALWAYS provide the complete content when using `write_to_file`.
*   ALWAYS include line numbers in `apply_diff` search blocks.
*   ALWAYS use markdown formatting for code and file names.
*   ALWAYS use `META_LOG_DIRECTIVE` for self-logging as specified.
*   ALWAYS update the `RefactoringMasterPlan.md` according to the workflow.
*   ONLY escalate after 3 failed attempts on the *same specific roadblock* and after documenting the attempts and analysis in Section 8 and Section 7.

## 4. Your Plan for Current Iteration

**Current Iteration:** LLM_Iter_4.1 (Attempt 3 on System.ValueTuple roadblock after human guidance)

**Objective:** Resolve the `System.IO.FileNotFoundException` for `System.ValueTuple, Version=4.0.3.0` to allow the test `ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest` to proceed.

**Plan Steps:**

1.  **Diagnose Persistent System.ValueTuple Issue (Completed in previous steps):** Analyze Fusion Log and project files to understand why `System.ValueTuple, Version=4.0.3.0` is not found. (Analysis completed - likely transitive dependency from netstandard 2.0 library, not being copied despite CopyLocal="true" on 4.6.1 reference).
2.  **Attempt 1 (after human guidance):** Revert binding redirect in `App.config` to 4.0.3.0. Build test project. Run test. (Completed - Failed).
3.  **Attempt 2 (after human guidance):** Add explicit `PackageReference` for `System.ValueTuple` 4.0.3 with `CopyLocal="true"` to `AutoBotUtilities.Tests.csproj`. Build test project. Run test. (Completed - Failed).
4.  **Escalate:** Since this is the third failed attempt on the same specific roadblock (System.ValueTuple FileNotFoundException), escalate to human input via Section 7.

## 5. Code Snippets and Context

*(This section will be populated with relevant code snippets as needed during the iteration)*

## 6. Post-Iteration Reflection & Learning

*(This section will be populated at the end of a successful iteration)*

### 6.1. Outcome

*(Was the objective achieved?)*

### 6.2. Analysis of Successes and Failures

*(What worked well? What didn't? Why?)*

### 6.3. Efficacy of Logging

*(Did the logging help diagnose issues? Were the META_LOG_DIRECTIVEs useful?)*

### 6.4. Efficacy of Core Instrumentation Prompt

*(Were the instructions clear and effective? Were there gaps or ambiguities?)*

### 6.5. Proposed Improvements to Core Instrumentation Prompt (Section 3)

*(Based on reflection, what specific changes should be made to Section 3 for future iterations?)*

## 7. Escalation to Human Input

**Date:** 2025-05-17
**Time:** 12:00:10 AM (Etc/GMT+4, UTC-4:00)
**Iteration:** LLM_Iter_4.1
**Roadblock:** Persistent `System.IO.FileNotFoundException` for `System.ValueTuple, Version=4.0.3.0` during test execution.

**Description of the Problem:**
The test `ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest` continues to fail with a `System.IO.FileNotFoundException` for the assembly `System.ValueTuple, Version=4.0.3.0`. The Fusion Log output consistently shows that the runtime is attempting to load `System.ValueTuple, Version=4.0.0.0`, which is then redirected to `System.ValueTuple, Version=4.0.3.0` by the `AutoBotUtilities.Tests.dll.config` file (derived from `App.config`). However, the assembly is not found in the test output directory (`AutoBotUtilities.Tests\bin\x64\Debug\net48`).

**Steps Taken to Resolve (Including Previous Human Guidance):**
1.  Initial diagnosis identified an incorrect binding redirect in `App.config` (pointing to 4.0.3.0 while the project referenced 4.6.1). Corrected the redirect to 4.6.1.0. (Failed - still looked for 4.0.3.0).
2.  Diagnosed missing `CopyLocal="true"` on the `System.ValueTuple` `PackageReference` (4.6.1) in `AutoBotUtilities.Tests.csproj`. Added `CopyLocal="true"`. (Failed - still looked for 4.0.3.0).
3.  Diagnosed missing `CopyLocal="true"` on the `System.ValueTuple` `PackageReference` (4.6.1) in transitive dependency projects (`AutoBotUtilities.csproj`, `AutoBot.csproj`). Added `CopyLocal="true"` to these. Performed full solution build. (Failed - still looked for 4.0.3.0). **(This marked 3 failed attempts on this specific roadblock, leading to the previous escalation)**.
4.  Received human guidance: Revert binding redirect in `App.config` to `newVersion="4.0.3.0"`. This was done.
5.  Received human guidance: Build *only* the test project (`AutoBotUtilities.Tests.csproj`) and re-run the test. This was done. (Failed - still looked for 4.0.3.0). **(Attempt 1 after human guidance)**.
6.  Analyzed Fusion Log again. Identified that the calling assembly is `netstandard, Version=2.0.0.0`, suggesting a transitive dependency is requesting 4.0.0.0 (redirected to 4.0.3.0). Added an explicit `PackageReference` for `System.ValueTuple` version 4.0.3 with `CopyLocal="true"` to `AutoBotUtilities.Tests.csproj` to force copying of this specific version. Built the test project again. (Failed - still looked for 4.0.3.0). **(Attempt 2 after human guidance, 3rd overall attempt on this roadblock)**.

**Current State:**
The test `ProcessEmailsAsync_ImportsPdfFromEmail_IntegrationTest` continues to fail with the `System.IO.FileNotFoundException` for `System.ValueTuple, Version=4.0.3.0`. The Fusion Log indicates the runtime is correctly applying the binding redirect to 4.0.3.0 but cannot find the assembly file. Explicitly adding `PackageReference` for both 4.6.1 and 4.0.3 with `CopyLocal="true"` in the test project and its direct dependencies has not resolved the issue.

**Request for Guidance:**
Despite multiple attempts to control the `System.ValueTuple` version via binding redirects and ensure the required assembly is copied using `CopyLocal="true"`, the `FileNotFoundException` persists. The Fusion Log points to a dependency from `netstandard, Version=2.0.0.0`. It is unclear why the build process is not copying the 4.0.3.0 assembly even with the explicit `PackageReference` and `CopyLocal="true"`.

Please provide guidance on how to further diagnose or resolve this persistent assembly loading issue. Potential areas for guidance could include:
-   Alternative methods to force the copying of `System.ValueTuple.dll` version 4.0.3.0 to the test output directory.
-   Strategies for identifying the specific transitive dependency that is causing the request for `System.ValueTuple, Version=4.0.0.0`.
-   Whether there might be an issue with the .NET Framework 4.8 project referencing .NET Standard 2.0 libraries that affects assembly resolution or copying.
-   Suggestions for using other diagnostic tools (if available) to understand the build or runtime behavior related to this assembly.

I am blocked on achieving the user's functional objective until this assembly loading issue is resolved.

## 8. Failure Tracking

*   **Roadblock:** `System.IO.FileNotFoundException` for `System.ValueTuple, Version=4.0.3.0`
    *   Attempt 1 (LLM_Iter_2.1): Corrected binding redirect to 4.6.1.0. Result: Failed.
    *   Attempt 2 (LLM_Iter_2.2): Added `CopyLocal="true"` to 4.6.1 `PackageReference` in test project. Result: Failed.
    *   Attempt 3 (LLM_Iter_3.1): Added `CopyLocal="true"` to 4.6.1 `PackageReference` in dependent projects. Result: Failed. **(Escalated)**
    *   Attempt 4 (LLM_Iter_4.1 - after human guidance): Reverted binding redirect to 4.0.3.0, built test project only. Result: Failed.
    *   Attempt 5 (LLM_Iter_4.1 - after human guidance): Added explicit `PackageReference` for 4.0.3 with `CopyLocal="true"` in test project. Result: Failed. **(Escalating again)**

## 9. Scoring

*(This section will be updated at the end of a successful iteration)*