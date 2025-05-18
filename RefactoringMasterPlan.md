# Refactoring Master Plan

## 1. Foundational LLM Directives

These directives are defined in your primary system prompt (`system-prompt-Logger.md`) and establish your immutable ultimate goals and operational mandates.

## 2. User Objective Refinement (Iteration 7)

**Initial User Objective:** Fix the failing NUnit test `CanImportAmazoncomOrder11291264431163432()` in `AutoBotUtilities.Tests\PDFImportTests.cs`.

**Refined Objective:** Implement structured Serilog logging within the `InvoiceReader` project (focusing on `InvoiceProcessingPipeline`, `PDFUtils`, `DataFileProcessor`) to diagnose the root cause of the test failure and enable its correction. Establish the LTM/STM framework featuring:
    1.  LTM as a directory (`LTM/`) of immutable Markdown files, each representing a specific learning concept.
    2.  STM (Section 10) as an index of immutable entries, providing "seeds" for deterministic LTM filename construction.
    3.  New LTM/STM entries are created for new learning, versioned, and inter-linked; existing entries are never modified.
    4.  Reactive LTM consultation triggered by internal logging (`META_LOG_DIRECTIVE`s) to continuously inform the process, in addition to deliberate LTM consultation during planning and failure analysis.
    5.  Integration of comprehensive task history analysis into the reflection phase for deeper learning and LTM population.

**Priority:** High - This is the primary focus of the current iteration.

## 3. Your Core Instrumentation Prompt (Roo - Logger Mode)

This section contains the evolving instructions that guide your operation. You are empowered to propose and implement changes to this section based on your reflections in Section 6.5.

**LTM/STM Core Principles:**
*   **Immutability:** Once created, LTM files (in the `LTM/` directory) and STM entries (in Section 10) are NEVER modified.
*   **Append-Only Learning:** New information, refinements, or alternative approaches related to an existing topic result in a NEW LTM file and a NEW, corresponding STM entry.
*   **Versioning/Linking:** New LTM files that build upon previous knowledge use distinct `Distinguisher_Source` components in their filenames (e.g., `_IterN`, `_vN`, `_AttemptN`, `_FollowUpToSTM-XXX`) and MUST include `Cross_References` (Markdown links) in their content pointing to the older LTM file(s) they relate to. Each LTM file is uniquely identified by its path.
*   **STM as Seed for LTM Path:** STM entries contain the structured "seed" data (`Primary_Topic_Or_Error`, `Key_Concepts`, `Outcome_Indicator_Short`, `Distinguisher_Source`) required to deterministically construct the path to the relevant LTM file using the LTM Filename Convention.
*   **Consultation Modes:**
    *   **Deliberate:** Strategic review of STM (and subsequently LTM) during `PLAN ITERATION` and `HANDLE EXECUTION FAILURE` phases, based on broad contextual keywords.
    *   **Reactive:** Tactical, just-in-time STM/LTM consultation triggered immediately after any `META_LOG_DIRECTIVE` is *formulated* (before it's externally logged if it's an explicit step), using its content to find matching STM entries.
*   **Contextual Influence:** Retrieved LTM content (from either consultation mode) immediately becomes part of the working context, influencing subsequent thoughts, plans, and actions.

**LTM Filename Convention & Construction:**
*   **Purpose:** To allow deterministic construction of LTM file paths from STM entry seeds, eliminating LTM directory searching and improving retrieval performance.
*   **Source Data from STM Entry:** The LTM filename is constructed using specific fields from an STM entry: `Primary_Topic_Or_Error`, `Key_Concepts` (ordered), `Outcome_Indicator_Short`, and `Distinguisher_Source`.
*   **Structure:** `LTM/[Primary_Topic_Or_Error]-[Ordered_Key_Concepts]-[Outcome_Indicator_Short]_[Distinguisher_Source].md`
    *   `Primary_Topic_Or_Error`: The primary error code (e.g., `CS0121`), or a core topic keyword (e.g., `SerilogConfig`). Should be filesystem-safe.
    *   `Ordered_Key_Concepts`: Key related concepts, tools, or libraries (e.g., `AmbiguousCall-Serilog`). If multiple, join with `-`. Order consistently (e.g., alphabetically). Should be filesystem-safe.
    *   `Outcome_Indicator_Short`: A brief tag indicating the result (e.g., `Resolved`, `Failed`, `Workaround`, `Info`, `Analysis`). Should be filesystem-safe.
    *   `Distinguisher_Source`: An iteration number, version, attempt number, or reference to a prior STM ID it builds upon (e.g., `_Iter7`, `_v1`, `_Attempt1`, `_FollowUpSTM-XXX`). Should be filesystem-safe.
*   **Example:**
    *   STM Entry Fields used for filename:
        *   `Primary_Topic_Or_Error: CS0121`
        *   `Key_Concepts: AmbiguousCall, Serilog` (Sorted & Joined: `AmbiguousCall-Serilog`)
        *   `Outcome_Indicator_Short: Resolved`
        *   `Distinguisher_Source: Iter7_Attempt1`
    *   Constructed LTM Path: `LTM/CS0121-AmbiguousCall-Serilog-Resolved_Iter7_Attempt1.md`

**Reactive STM/LTM Consultation Sub-Workflow (Internal Process):**
*   **Trigger:** Immediately after *formulating* the content for any `META_LOG_DIRECTIVE` (before it's externally output if it's an explicit step).
*   **Action:**
    1.  Extract keywords from the `Content` of the formulated `META_LOG_DIRECTIVE`.
    2.  Scan Section 10 (Short-Term Memory) `All_Tags` field for STM entries with significant keyword overlap.
    3.  If relevant STM entries are found:
        *   For each highly relevant STM entry, log its `STM_ID` using a `META_LOG_DIRECTIVE` with Type `ReactiveSTM_Consultation_Triggered` (Include triggering `META_LOG_DIRECTIVE`'s Type and a snippet of its content).
        *   Construct the `LTM_File_Path` from the STM entry's seed fields (`Primary_Topic_Or_Error`, `Key_Concepts`, `Outcome_Indicator_Short`, `Distinguisher_Source`) using the LTM Filename Convention.
        *   Use `read_file` to retrieve the LTM content. If `read_file` fails (e.g., file unexpectedly missing, though construction should be robust), log a warning and proceed without this LTM's insight.
        *   Log using `META_LOG_DIRECTIVE` with Type `ReactiveLTM_Retrieved`, noting the LTM file path and a brief summary of the insight gained, if any.
        *   The retrieved LTM content is now part of the immediate working context, influencing the next thought or action.
*   **Note:** This sub-workflow is an internal mental step. The `META_LOG_DIRECTIVE`s logging this reactive consultation will appear in sequence. These meta-logs about reactive consultation are generally NOT themselves primary triggers for further reactive consultation to avoid unproductive loops, unless they contain new, distinct problem/solution keywords.

## 3.1 Logging Directives

These directives define the standard Serilog logging patterns to be applied throughout the codebase, particularly in areas targeted for instrumentation. Adherence to these patterns is crucial for consistent observability and effective log analysis.

*   **Mandatory Structured Logging:** All application and test logging MUST use Serilog with structured message templates and named properties (`{PropertyName}`). Avoid simple string interpolation (`Log.Information("Value is " + value);`). Instead, use `Log.Information("Value is {Value}", value);`.
*   **Invocation Tracking:** At the entry point of any significant operation, process, or test method, a unique identifier MUST be added to the `Serilog.Context.LogContext` using `LogContext.PushProperty("InvocationId", Guid.NewGuid().ToString());`. This property MUST be included in all subsequent logs within that context.
*   **Method Entry/Exit:**
    *   Log `METHOD_ENTRY` at the beginning of significant methods. Include `MethodName` and `InvocationId`.
    *   Log `METHOD_EXIT_SUCCESS` upon successful method completion. Include `MethodName`, `InvocationId`, and `DurationMs` (calculated using a `Stopwatch`).
    *   Log `METHOD_EXIT_FAILURE` when a method exits due to an exception. Include `MethodName`, `InvocationId`, `DurationMs`, and `ExceptionType`.
*   **Action Phase Tracking:** Delineate logical phases within operations (e.g., Arrange, Act, Assert, Initialization, Processing, Cleanup).
    *   Log `ACTION_START` at the beginning of a phase. Include `ActionName`, `InvocationId`, and optionally `ParentAction`.
    *   Log `ACTION_END_SUCCESS` upon successful phase completion. Include `ActionName`, `InvocationId`, and `DurationMs`.
    *   Log `ACTION_END_FAILURE` when a phase fails (e.g., due to an exception or failed assertion). Include `ActionName`, `InvocationId`, `DurationMs`, and `ExceptionType`.
*   **Internal Step Tracking:** Log granular steps within actions or methods to trace execution flow.
    *   Log `INTERNAL_STEP`. Include `StepDescription`, `InvocationId`, and any relevant contextual properties (e.g., file paths, IDs, counts, boolean outcomes).
*   **External Call Tracking:** Log interactions with significant external systems or components (databases, APIs, other services, core processing logic).
    *   Log `EXTERNAL_CALL_START`. Include `ExternalSystem`, `Operation`, `InvocationId`, and relevant input parameters.
    *   Log `EXTERNAL_CALL_END`. Include `ExternalSystem`, `Operation`, `InvocationId`, `Success` (boolean), and relevant output/result properties. Log exceptions if the call failed.
*   **Log Levels:** Use appropriate Serilog levels (`Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal`) to categorize messages based on their severity and detail.
*   **Exception Logging:** Always log exceptions using the `Log.Error(ex, message, properties)` overload to ensure exception details (stack trace, inner exceptions) are captured.
*   **Meta Logging (`META_LOG_DIRECTIVE`):** Continue using `META_LOG_DIRECTIVE`s as defined in the workflow to log the AI's internal process, reasoning, and actions. Ensure the required properties (`Type`, `Context`, `Directive`, `SourceIteration`, and optionally `ExpectedChange`) are included. These meta-logs are critical for self-reflection and debugging the AI's operation.
**Workflow Steps:**

*   **Workflow Step -1: INITIATE ITERATION CYCLE:**
    *   Read `RefactoringMasterPlan.md` (all sections).
    *   Identify "Current User-Defined Iteration Objective & Priority" (Section 2).
    *   Identify current "Your Plan" (Section 4).
    *   Identify current "Failure Tracking" (Section 8).
    *   Identify current "Escalation" status (Section 7).
    *   Identify current "Short-Term Memory" (Section 10) for context.
    *   If Escalation is active, wait for user input.
    *   If Escalation is not active, proceed to **Workflow Step 3.1: PLAN ITERATION**.

*   **Workflow Step 3.1: PLAN ITERATION:**
    *   Review "Current User-Defined Iteration Objective & Priority" (Section 2).
    *   Review previous iteration's "Reflection & Prompt Improvements" (Section 6) and "Failure Tracking" (Section 8). (This review itself can be considered as generating internal `META_LOG_DIRECTIVE`s, potentially triggering reactive LTM consultation).
    *   **Deliberate STM/LTM Review for Iteration Planning:**
        *   Identify primary keywords, error codes, concepts from the current iteration's overall objective or known blocking issues (context tags).
        *   Scan Section 10 (Short-Term Memory) `All_Tags` field for STM entries with significant keyword overlap.
        *   If relevant STM entries are found:
            *   For each, log `STM_ID` with `META_LOG_DIRECTIVE` Type `DeliberateSTM_Consultation_Planning`.
            *   Construct `LTM_File_Path` from STM seeds, `read_file` LTM.
            *   Analyze LTM content (including `Cross_References`), incorporate insights into planning. Log retrieved LTM path.
        *   **Fallback LTM Check (If no suitable STM entry):**
            *   From current context tags, derive potential LTM seed components (`Primary_Topic_Or_Error`, etc.).
            *   Attempt to construct a hypothetical `LTM_File_Path`.
            *   Use `read_file`. If successful, log `LTM_DirectConstruct_Consultation_Planning` and use insights. Else, log no direct LTM found.
    *   Formulate or refine "Your Plan" (Section 4). Each part of this formulation (e.g., "Step X: Do Y because Z") is conceptually a `META_LOG_DIRECTIVE` and triggers the **Reactive STM/LTM Consultation Sub-Workflow**.
    *   Document the plan in Section 4.
    *   Proceed to **Workflow Step 3.2: EXECUTE PLAN**.

*   **Workflow Step 3.2: EXECUTE PLAN:**
    *   Execute steps from "Your Plan" (Section 4) sequentially.
    *   For each step:
        *   Use appropriate tool(s). **Crucially, if the plan step requires a tool, the tool call MUST be the immediate next action.**
        *   Before modifying existing files, ALWAYS `read_file`.
        *   **Formulate** `META_LOG_DIRECTIVE` with Type `PlanStepExecution_Intent` (before tool call).
        *   **Execute Reactive STM/LTM Consultation Sub-Workflow** (using content of the intent `META_LOG_DIRECTIVE`).
        *   *Output the `PlanStepExecution_Intent` `META_LOG_DIRECTIVE`.*
        *   Execute the tool.
        *   Analyze the result.
        *   **Formulate** `META_LOG_DIRECTIVE` with Type `PlanStepExecution_Outcome` (after tool result).
        *   **Execute Reactive STM/LTM Consultation Sub-Workflow** (using content of the outcome `META_LOG_DIRECTIVE`).
        *   *Output the `PlanStepExecution_Outcome` `META_LOG_DIRECTIVE`.*
        *   If step completes successfully, mark as "Completed" in Section 4.
        *   If step fails, immediately proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE**.

*   **Workflow Step 3.3: HANDLE EXECUTION FAILURE:**
    *   Analyze failure result. (This analysis generates internal `META_LOG_DIRECTIVE`s, triggering reactive consultation).
    *   Document failure in "Failure Tracking" (Section 8).
    *   **Deliberate STM/LTM Review for Failure Diagnosis:**
        *   Identify keywords, error codes, etc., from the failure.
        *   Scan STM `All_Tags`. If matches: log `DeliberateSTM_Consultation_Failure`, construct LTM path, `read_file` (if not the failing tool), analyze, incorporate.
        *   **Fallback LTM Check:** (As in 3.1, log as `LTM_DirectConstruct_Consultation_Failure`).
    *   Based on analysis (informed by deliberate and reactive LTM), formulate a sub-plan. This formulation triggers reactive LTM consultation.
    *   Update "Your Plan" (Section 4).
    *   Proceed back to **Workflow Step 3.2 EXECUTE PLAN**.
    *   If a specific failure persists after three distinct attempts (distinct solution strategies, potentially informed by different LTM paths), proceed to **Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL**.

*   **Workflow Step 3.4: BUILD/TEST/FIX LOOP:**
    *   (As before, noting that build/test execution and analysis will generate `META_LOG_DIRECTIVE`s, thus triggering reactive LTM consultation).
    *   If tests pass and the functional objective is met, or if a point is reached where reflection is appropriate before full completion (e.g., end of planned work for the iteration, significant learning event), proceed to **Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY**.
    *   If the build fails or tests fail, proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE** to diagnose and fix the issue.

*   **Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY:**
    *   **Trigger:** Called after the main execution/fixing phase of an iteration (e.g., from BUILD/TEST/FIX LOOP) or when explicitly decided.
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemState`, Content: "Execution phase concluded for current Iteration. Preparing for reflection. Requesting task history."
        *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
    *   Determine the current iteration number (e.g., Iteration 7). Let this be `CurrentIterNum`.
    *   Use the `ask_followup_question` tool. Dynamically insert `CurrentIterNum` into the question and response options:
        *   `question`: "The active execution phase for Iteration `CurrentIterNum` is complete. To proceed with comprehensive self-reflection and learning (Contemplation Phase), please export the full task history for this iteration (containing all `META_LOG_DIRECTIVE`s, code interactions, and tool outputs) and save it as `task_history_iter<CurrentIterNum>.txt` in the root of the workspace (`c:/Insight Software/AutoBot-Enterprise`). I will pause and wait for your confirmation."
        *   `follow_up`:
            1.  "The task history file `task_history_iter<CurrentIterNum>.txt` is now available in the workspace. Proceed with reflection."
            2.  "I am unable to provide the task history file `task_history_iter<CurrentIterNum>.txt` at this time. Proceed with reflection using available data."
    *   **Wait for user response.**
    *   **Process User Response:**
        *   If user confirms file availability:
            *   Construct the filename `task_history_iter<CurrentIterNum>.txt` using the `CurrentIterNum`.
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "User confirmed `task_history_iter<CurrentIterNum>.txt` is available. Attempting to read."
                *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Use `read_file` with `path: task_history_iter<CurrentIterNum>.txt`.
            *   If successful:
                *   Store the retrieved content internally for use in Step 3.5.
                *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Successfully read `task_history_iter<CurrentIterNum>.txt`. Content captured for reflection."
                    *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   If `read_file` fails:
                *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Failed to read `task_history_iter<CurrentIterNum>.txt`. Proceeding with reflection using other available data."
                    *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
                *   Set a flag internally indicating task history is not available.
        *   If user indicates file is not available:
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemState`, Content: "User indicated task history file `task_history_iter<CurrentIterNum>.txt` is not available. Proceeding with reflection using other available data."
                *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Set a flag internally indicating task history is not available.
    *   Proceed to **Workflow Step 3.5: REFLECT & SELF-IMPROVE**.

*   **Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL:**
    *   (As before, noting that documenting the problem for escalation will generate `META_LOG_DIRECTIVE`s, triggering reactive LTM consultation).

*   **Workflow Step 3.5: REFLECT & SELF-IMPROVE:**
    *   **Review Iteration Data:**
        *   The entire iteration: initial plan (Section 4), execution steps, `META_LOG_DIRECTIVE`s generated throughout (conceptually, and specifically from the **retrieved `task_history_iter<CurrentIterNum>.txt` file if available**), LTM consultations (deliberate and reactive), failures encountered (Section 8) and how they were handled, log analysis (Section 5), and the final outcome.
    *   **Update Long-Term (LTM) and Short-Term (STM) Memory (Append-Only, Immutable, Linked):**
        *   (Process for creating NEW, immutable, versioned, and linked LTM/STM entries remains as previously defined. The insights leading to LTM creation will be heavily informed by the **retrieved `task_history_iter<CurrentIterNum>.txt` if available**, in addition to other documented sections.)
    *   Analyze effectiveness of the "Core Instrumentation Prompt," especially LTM/STM mechanisms and the task history integration.
    *   Identify areas for improvement in the prompt.
    *   Propose concrete changes to Section 3 in "Reflection & Prompt Improvements" (Section 6.5).
    *   Document reflection in Section 6.
    *   Update "Core Instrumentation Prompt" (Section 3) with approved changes from Section 6.5.
    *   Update "Scoring" (Section 9).
    *   Proceed to **Workflow Step 3.6: ATTEMPT COMPLETION**.

*   **Workflow Step 3.6: ATTEMPT COMPLETION:**
    *   If "Current User-Defined Iteration Objective" (Section 2) is fully achieved and verified, use `attempt_completion` tool.
    *   If not fully achieved but significant progress made and next steps clear, update Section 2 and 4 for next iteration and proceed to **Workflow Step 3.1: PLAN ITERATION**.

**Cardinal Rules:** (As before, e.g., `read_file` before modify, `write_to_file` on `apply_diff` failure, etc.)

**Critical Instructions:** (As before, e.g., build command, Serilog usage, NuGet resolution strategies.)

## 4. Your Plan (Iteration 7)

**Objective:** Make the `CanImportAmazoncomOrder11291264431163432()` test in `AutoBotUtilities.Tests\PDFImportTests.cs` pass. Establish LTM/STM framework with reactive consultation and integrate task history analysis into reflection.

**Current Status:** PRE-ITERATION SETUP (Workflow Step 3.1.0) is complete, including updating ProjectStructure.md and resolving file locations. The LTM/STM framework and task history integration are operational. The focus is now on diagnosing the cause of Failure 7.13 (Test failure). Encountered Failure 7.14 (File not found) when attempting to read test logs, and Failure 7.15 (Tool repetition limit) when attempting to read the plan. Failure 7.14 and 7.15 are resolved. Log analysis (4.13.16.4.4.5) and initial hypothesis (4.13.16.4.5) are complete. Code analysis of `HandleImportSuccessStateStep.cs` (4.13.16.4.6.5) is complete.

**Plan Steps:**
*   4.1 - 4.13 (Completed steps from previous execution leading to Failure 7.13. These will be reviewed at the end of Iteration 7 for potential LTM/STM entry creation and summarization in this plan, using task history if provided.)
*   **Diagnosing Failure 7.13: Test `CanImportAmazoncomOrder11291264431163432()` failed.**
    *   4.13.16.4.2 Analyze test output (Assertion: "ShipmentInvoice '112-9126443-1163432' not created."). (Completed)
    *   4.13.16.4.3 Update Failure Tracking (Section 8) with Failure 7.13. (Completed)
    *   4.13.16.4.4 Analyze generated logs to understand why the ShipmentInvoice is not created.
        *   4.13.16.4.4.1: Identify the log file path from the test execution output or standard logging location. (Assume `AutoBotUtilities.Tests\bin\x64\Debug\net48\logs\AutoBotUtilities.Tests-.log` or similar).
        *   4.13.16.4.4.2: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Read test log file `AutoBotUtilities.Tests\\bin\\x64\\Debug\\net48\\logs\\AutoBotUtilities.Tests-.log` to analyze ShipmentInvoice '112-9126443-1163432' creation failure."
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.4.3: Use `read_file` with `path: AutoBotUtilities.Tests\bin\x64\Debug\net48\Logs\AutoBotTests-20250517.log` (or the specifically identified log file for the failed test run). (Failed - See Failure 7.14)
    *   **Addressing Failure 7.14: Test log file not found.**
        *   4.13.16.4.4.3.1: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Investigate Serilog configuration in AutoBotUtilities.Tests project to determine actual log file path."
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.4.3.2: Read the Serilog configuration code in `AutoBotUtilities.Tests`. (Need to identify the relevant file - likely in a TestBase or Setup class).
        *   4.13.16.4.4.3.3: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Successfully read Serilog configuration. Analyzing content to find log path." (Or error if read fails).
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.4.3.4: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "List files in the test log directory `AutoBotUtilities.Tests\\bin\\x64\\Debug\\net48\\Logs` to find the specific log file name."
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.4.3.5: Use `list_files` with `path: AutoBotUtilities.Tests\bin\x64\Debug\net48\Logs`.
        *   4.13.16.4.4.3.6: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Successfully listed files in log directory. Analyzing output to find the latest log file matching `AutoBotTests-*.log`." (Or error).
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.4.3.7: Analyze the output of `list_files` to identify the most recent file matching the pattern `AutoBotTests-*.log`.
        *   4.13.16.4.4.3.8: Store the identified specific log file path.
        *   4.13.16.4.4.3.9: Update Plan Step 4.13.16.4.4.3 with the correct log file path (identified in 4.13.16.4.4.3.8).
        *   4.13.16.4.4.3.10: Re-attempt Plan Step 4.13.16.4.4.3 using the corrected path. (Completed successfully)
    *   4.13.16.4.4.5: Meticulously analyze the log content (once successfully read). Trace order ID `112-9126443-1163432`. Look for logs related to `InvoiceProcessingPipeline`, `PDFUtils.ImportSuccessState`, `HandleImportSuccessStateStep`, `DataFileProcessor.Process` calls (especially `lookupFormat` and `Invoke`), Serilog `Error`/`Warning` messages for this order, and where `ShipmentInvoice` entity creation/saving is expected. (Completed)
        *   **(Each analytical finding here is conceptually a `META_LOG_DIRECTIVE` and triggers reactive LTM consultation.)**
    *   4.13.16.4.5 Document detailed findings from log analysis in Section 5 of `RefactoringMasterPlan.md`. (Completed)
        *   **(This documentation will trigger reactive LTM consultation based on the content being written to Section 5.)**
    *   **Addressing Failure 7.15: Tool call repetition limit reached for read_file.**
        *   4.13.16.4.5.1: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Document Failure 7.15 (read_file repetition limit) in Section 8 of RefactoringMasterPlan.md."
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.5.2: Update Section 8 with Failure 7.15 details. (Completed)
        *   4.13.16.4.5.3: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Re-attempt reading RefactoringMasterPlan.md to continue planning."
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.5.4: Use `read_file` with `path: RefactoringMasterPlan.md`. (Failed - See Failure 7.15)
        *   4.13.16.4.5.5: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Successfully re-read RefactoringMasterPlan.md." (Or error if it fails again).
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`. (Completed successfully)
    *   **Investigating `HandleImportSuccessStateStep` Failure (Based on Section 5 Analysis):**
        *   4.13.16.4.6.1: Identify the source file for `HandleImportSuccessStateStep`. (Likely in `InvoiceReader` project, potentially `InvoiceReader/InvoiceReader/PipelineInfrastructure/HandleImportSuccessStateStep.cs`). (Completed - `InvoiceReader/InvoiceReader/PipelineInfrastructure/HandleImportSuccessStateStep.cs`)
        *   4.13.16.4.6.2: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Read the source file for `HandleImportSuccessStateStep` to understand its logic."
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`.
        *   4.13.16.4.6.3: Use `read_file` with the identified path for `HandleImportSuccessStateStep.cs`. (Completed)
        *   4.13.16.4.6.4: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Successfully read `HandleImportSuccessStateStep.cs`. Analyzing content." (Or error).
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
            *   Output `META_LOG_DIRECTIVE`. (Completed)
        *   4.13.16.4.6.5: Analyze the code in `HandleImportSuccessStateStep.cs`. Identify the conditions under which it returns `False`. Look for dependencies, database interactions, or state checks. (Completed - Analysis indicates failure if *any* template processing fails).
            *   **(Each analytical finding here is conceptually a `META_LOG_DIRECTIVE` and triggers reactive LTM consultation.)**
        *   4.13.16.4.6.6: Refine hypothesis based on code analysis: `HandleImportSuccessStateStep` returns `False` if *any* template processing fails within the loop, even if others succeed. The failure for order `112-9126443-1163432` likely occurred because the processing of *another template* within the same `InvoiceProcessingContext` failed, causing `overallStepSuccess` to be set to `false`.
            *   **(Hypothesis formulation will trigger reactive LTM consultation.)**
        *   4.13.16.4.6.7: Propose specific log analysis steps to verify the refined hypothesis:
            *   Re-read the test log file (`AutoBotTests-20250517.log`).
            *   Identify all log entries within the `HandleImportSuccessStateStep` context for the relevant file path.
            *   Group these log entries by template (using TemplateId or TemplateName from logs).
            *   For each template group, trace the execution flow and identify the specific point of failure (if any) that caused `overallStepSuccess` to be set to `false`.
        *   4.13.16.4.6.8: Update "Your Plan" (Section 4) with steps to perform the detailed log analysis proposed in 4.13.16.4.6.7. (Completed - This step is the current action).
        *   4.13.16.4.6.9: Perform the detailed log analysis as planned in 4.13.16.4.6.7.
            *   4.13.16.4.6.9.1: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Re-read the test log file `AutoBotTests-20250517.log` for detailed analysis of `HandleImportSuccessStateStep` execution across all templates."
                *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
                *   Output `META_LOG_DIRECTIVE`.
            *   4.13.16.4.6.9.2: Use `read_file` with `path: AutoBotUtilities.Tests\bin\x64\Debug\net48\Logs\AutoBotTests-20250517.log`.
            *   4.13.16.4.6.9.3: **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Successfully re-read test log file. Proceeding with detailed analysis." (Or error).
                *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
                *   Output `META_LOG_DIRECTIVE`.
            *   4.13.16.4.6.9.4: Analyze the log content to identify all templates processed by `HandleImportSuccessStateStep` for the relevant file path and trace their individual outcomes. Document findings in Section 5.
                *   **(Each analytical finding here is conceptually a `META_LOG_DIRECTIVE` and triggers reactive LTM consultation.)**
        *   4.13.16.4.6.10: Based on the detailed log analysis (from 4.13.16.4.6.9.4):
            *   Identify the root cause of the failure for the specific template(s) that caused `overallStepSuccess` to be false.
            *   Formulate a fix (either correct the logic for the failing template type, or adjust pipeline logic).
            *   Update "Your Plan" (Section 4) with steps to implement the fix.
            *   Implement the fix using `apply_diff` or `write_to_file`.
*   **Fix Formulation Refinement (Based on InvoiceProcessingContext Analysis):** Analysis of `InvoiceProcessingContext.cs` confirms there is no dedicated property to store only the templates that were identified as matching the document (`IsMatch: True` in the logs from `GetPossibleInvoicesStep`). The `context.Templates` property contains all possible templates loaded by `GetTemplatesStep`. The `HandleImportSuccessStateStep` currently iterates over this full list.
            *   **Refined Proposed Fix:**
                1.  Add a new property, `MatchedTemplates` (type `IEnumerable<Invoice>`), to `InvoiceProcessingContext.cs`.
                2.  Modify `GetPossibleInvoicesStep.Execute` to populate this new `context.MatchedTemplates` property with only the `Invoice` objects for which `IsInvoiceDocument` returns `true`.
                3.  Modify `HandleImportSuccessStateStep.Execute` to iterate over `context.MatchedTemplates` instead of `context.Templates`.
            *   Update "Your Plan" (Section 4) with steps to implement this refined fix.
            *   Implement the refined fix:
                *   Add `MatchedTemplates` property to `InvoiceProcessingContext.cs`.
                *   Modify `GetPossibleInvoicesStep.Execute` to populate `context.MatchedTemplates`.
                *   Modify `HandleImportSuccessStateStep.Execute` to use `context.MatchedTemplates`.
            *   Proceed to **Workflow Step 3.4: BUILD/TEST/FIX LOOP** (specifically, build and run the test).
*   **Fix Formulation Refinement:** The current analysis of `HandleImportSuccessStateStep.Execute` confirms it iterates through `context.Templates`. The log analysis shows that `GetPossibleInvoicesStep` correctly identifies the matching template (TemplateId 5 for the Amazon file). To implement the proposed fix (only consider success/failure of *matched* templates), the `HandleImportSuccessStateStep` needs access to the list of templates that were actually identified as matching the document. This information should be stored in the `InvoiceProcessingContext`.
            *   **Next Step:** Examine the `InvoiceProcessingContext.cs` file to understand how identified templates are stored and how `HandleImportSuccessStateStep` can access this information.
            *   Update "Your Plan" (Section 4) with steps to examine `InvoiceProcessingContext.cs`.
            *   Read `InvoiceProcessingContext.cs`.
            *   Based on the structure of `InvoiceProcessingContext`, refine the fix implementation plan for `HandleImportSuccessStateStep.Execute`.
            *   Implement the refined fix using `apply_diff` or `write_to_file`.
            *   Proceed to **Workflow Step 3.4: BUILD/TEST/FIX LOOP** (specifically, build and run the test).
*   **Fix Formulation:** The `HandleImportSuccessStateStep` should not fail if attempts to process non-matching templates return `false`. It should only consider the success/failure of the template(s) that were actually identified as matching the document earlier in the pipeline. The current logic iterates through `context.PossibleInvoices` and sets `overallStepSuccess = false` if `DataFileProcessor.Process` returns `false` for *any* of them. This needs to be changed. The step should perhaps check if the template being processed is one of the *identified* templates for the file, or the `DataFileProcessor.Process` method itself needs to handle non-matching templates more gracefully (e.g., return a specific status indicating "not applicable" rather than `false` for success). Given the current structure, modifying `HandleImportSuccessStateStep` to only consider templates marked as 'matched' or 'identified' in the context seems the most direct fix within this step.
            *   **Proposed Fix:** Modify `HandleImportSuccessStateStep.Execute` to check if the `InvoiceTemplate` being processed is present in a list of successfully identified templates stored in the `InvoiceProcessingContext`. If it's not an identified template, its processing outcome should not affect `overallStepSuccess`.
            *   Update "Your Plan" (Section 4) with steps to implement the fix.
            *   Implement the fix using `apply_diff` or `write_to_file`.
            *   Proceed to **Workflow Step 3.4: BUILD/TEST/FIX LOOP** (specifically, build and run the test).
            *   Proceed to **Workflow Step 3.4: BUILD/TEST/FIX LOOP** (specifically, build and run the test).
    *   4.13.16.4.7 If test `CanImportAmazoncomOrder11291264431163432()` now passes, proceed to **Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY**.
    *   4.13.16.4.8 If test still fails, re-evaluate, potentially update hypothesis/sub-plan, or consider if 3 distinct attempts have been made on this specific aspect of the failure. If so, and still blocked, proceed to **Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL**. Otherwise, loop back to a refined version of 4.13.16.4.6 or 4.13.16.4.7.

## 5. Log Analysis & Diagnosis

*   **Previous Log Analysis (Pre-Iteration 7 Test Failure - Failure 7.13):** Test `CanImportAmazoncomOrder11291264431163432()` failed with assertion "ShipmentInvoice '112-9126443-1163432' not created." Log analysis showed `InvoiceProcessingPipeline` completed with "Final Status: Success", but a `WRN` from `Import - PipelineResultCheck` indicated "Pipeline execution failed or reported unsuccessful," pointing to "DataFileProcessor failed for File: ..." and "Success step 4 (HandleImportSuccessStateStep) reported failure (returned false)...". This suggested failure within `DataFileProcessor.Process`.
*   **(New analysis from current execution of 4.13.16.4.4 onwards will be added here.)**
*   **Log Analysis (Iteration 7 - Failure 7.13 Diagnosis):** Analysis of `AutoBotTests-20250517.log` for order ID `112-9126443-1163432` revealed the following:
    *   The initial pipeline successfully identified the Amazon template (ID 5) for the test file.
    *   The processing pipeline was initiated and executed the `ProcessInvoiceStep` (Step 1) using the identified Amazon template (ID 5).
    *   `ProcessInvoiceStep` successfully invoked `DataFileProcessor.Process` with FormatId 5 (Amazon), and `DataFileProcessor.Process` logged a SUCCESS outcome and returned `True`.
    *   The subsequent step in the processing pipeline, `HandleImportSuccessStateStep` (Step 2), logged a FAILURE outcome and returned `False`.
    *   This failure in `HandleImportSuccessStateStep` caused the entire processing pipeline, the overall `RunPipeline`, and the `InvoiceReader.Import` process to report failure.
    *   The test assertion "ShipmentInvoice '112-9126443-1163432' not created." is a direct consequence of the import process failing due to the failure in `HandleImportSuccessStateStep`.
*   **Hypothesis (Initial):** The root cause of the test failure is the `HandleImportSuccessStateStep` returning `False`, which prevents the successful completion of the import process and thus the creation of the ShipmentInvoice.
*   **Hypothesis (Refined based on `HandleImportSuccessStateStep.Execute` code analysis):** The `HandleImportSuccessStateStep.Execute` method returns `False` if *any* template processed within the context fails, even if other templates (like the target Amazon template ID 5) succeed. The failure for order `112-9126443-1163432` likely occurred because the processing of *another template* within the same `InvoiceProcessingContext` failed, causing `overallStepSuccess` to be set to `false`.
*   **Detailed Log Analysis (Iteration 7 - Failure 7.13 Diagnosis - Plan Step 4.13.16.4.6.9.4):** Analysis of `AutoBotTests-20250517.log` focusing on `HandleImportSuccessStateStep` execution for file `C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\Amazon.com - Order 112-9126443-1163432.pdf` reveals the following:
    *   The `HandleImportSuccessStateStep` starts processing multiple templates within a loop.
    *   Logs show processing attempts for various templates (e.g., TemplateId 117, 3, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, etc.).
    *   Crucially, the log for TemplateId 5 (Amazon) shows successful processing: `[INF] [] ACTION_END_SUCCESS: IsInvoiceDocument - Invoice 5. Outcome: Document identified as template 5...` and subsequent successful steps related to extracting data using this template.
    *   However, the log also shows attempts to process *other* templates that *do not match* the document (e.g., TemplateId 117, 3, 6, etc., all logging `Outcome: Document NOT identified as template...`).
    *   The `HandleImportSuccessStateStep` logic, as analyzed in 4.13.16.4.6.5, sets `overallStepSuccess` to `false` if *any* template processing fails.
    *   While Template 5 succeeded, the attempts to process the numerous other non-matching templates likely resulted in failures within the loop, causing `overallStepSuccess` to become `false`.
    *   There are no explicit `ERR` or `WRN` logs directly within the `HandleImportSuccessStateStep` loop for the non-matching templates, but the `IsInvoiceDocument` calls for these templates return `false`, which the step interprets as a failure to process *that specific template*.
    *   The final log for `HandleImportSuccessStateStep` confirms its overall failure: `[INF] [] INTERNAL_STEP (Run - StepResult): Pipeline step completed unsuccessfully.. CurrentState: [PipelineName: Initial Pipeline, StepNumber: 4, StepName: HandleImportSuccessStateStep].` (Note: The plan incorrectly listed this as Step 4, the log shows it as Step 2). This discrepancy needs to be noted.
    *   The log confirms the refined hypothesis: The test fails because `HandleImportSuccessStateStep` returns `False` due to the failure of processing *other* non-matching templates, even though the correct Amazon template (ID 5) was successfully identified and processed.
*   **(Detailed log analysis findings from 4.13.16.4.6.9.4 will be added here.)**

## 6. Reflection & Prompt Improvements

*   **Iteration 7 Reflection Summary:** Analysis of the task history for Iteration 7 identified a critical operational learning: the failure to immediately use a required tool after a workflow step caused a system error and interruption. This highlights the need for stricter adherence to the workflow's tool usage requirements.
*   **6.5 Proposed Core Instrumentation Prompt Improvements (to be reviewed at end of Iteration 7):**
    *   (Existing proposals from prior to Iteration 7 start)
    *   Refine the `Distinguisher_Source` strategy for LTM filenames to clearly indicate evolutionary relationships (e.g., `_FollowUpSTM-XXX_IterN` vs. `_IterN_AttemptM`).
*   Evaluate the process for integrating task history analysis into reflection, ensuring it effectively informs LTM/STM creation and prompt improvements.

## 7. Escalation

*   **Status:** Not Active.
*   **Details:** (If active, details of the blocking issue and attempts made will be documented here.)

## 8. Failure Tracking

*   **Failure 7.1:** (Details from previous iterations)
*   **Failure 7.2:** (Details from previous iterations)
*   **Failure 7.3:** (Details from previous iterations)
*   **Failure 7.4:** (Details from previous iterations)
*   **Failure 7.5:** (Details from previous iterations)
*   **Failure 7.6:** (Details from previous iterations)
*   **Failure 7.7:** (Details from previous iterations)
*   **Failure 7.8:** (Details from previous iterations)
*   **Failure 7.9:** (Details from previous iterations)
*   **Failure 7.10:** (Details from previous iterations)
*   **Failure 7.11:** (Details from previous iterations)
*   **Failure 7.12:** (Details from previous iterations)
*   **Failure 7.13:**
    *   **Type:** Test Failure
    *   **Description:** NUnit test `CanImportAmazoncomOrder11291264431163432()` in `AutoBotUtilities.Tests\PDFImportTests.cs` failed.
    *   **Details:** Assertion failed: "ShipmentInvoice '112-9126443-1163432' not created."
    *   **Diagnosis:** Initial log analysis (Section 5) indicated failure in `HandleImportSuccessStateStep`. Code analysis of `HandleImportSuccessStateStep.Execute` suggests failure occurs if *any* template processing within the context fails.
    *   **Attempts:** 0 distinct attempts on the root cause fix yet. Currently in diagnosis phase.
    *   **Status:** Active - Under Diagnosis.
*   **Failure 7.14:**
    *   **Type:** Tool Execution Failure (`read_file`)
    *   **Description:** Attempted to read test log file `AutoBotUtilities.Tests\bin\x64\Debug\net48\logs\AutoBotUtilities.Tests-.log` (Plan Step 4.13.16.4.4.3) but file was not found.
    *   **Diagnosis:** The log file name was incorrect/not the most recent.
    *   **Resolution:** Identified correct log file path (`AutoBotUtilities.Tests\bin\x64\Debug\net48\Logs\AutoBotTests-20250517.log`) by listing directory contents. Updated plan and re-attempted read.
    *   **Status:** Resolved.
*   **Failure 7.15:**
    *   **Type:** Tool Execution Failure (`read_file`)
    *   **Description:** Attempted to read `RefactoringMasterPlan.md` (Plan Step 4.13.16.4.5.4) but the tool call repetition limit was reached.
    *   **Diagnosis:** System limitation on consecutive tool calls of the same type.
    *   **Resolution:** The system automatically allowed the tool call after the previous turn.
    *   **Status:** Resolved.

## 9. Scoring (Iteration 7)

*   **Objective Achievement:** 0/10 (Test still failing)
*   **Code Quality:** N/A (No code changes yet)
*   **Logging Quality:** N/A (No new logging added yet)
*   **Process Adherence:** 8/10 (Minor initial hiccup with tool usage, but recovered and following plan/workflow)
*   **Self-Improvement (LTM/STM/Prompt):** 7/10 (Framework established, initial reflection on process hiccup noted, task history integration planned)
*   **Total Score:** (Will be calculated at the end of the iteration)

## 10. Short-Term Memory (STM) Index

The purpose and format of STM entries are defined in your primary system prompt (`system-prompt-Logger.md`). STM entries act as seeds for LTM filename construction and are indexed here. The comprehensive LTM/STM management principles are detailed in Section 3 of this document.

*   **Entries:**
    *   `STM_ID: STM-Iter7-1`
        *   `Primary_Topic_Or_Error: TestFailure`
        *   `Key_Concepts: HandleImportSuccessStateStep, LogAnalysis, PipelineFailure`
        *   `Outcome_Indicator_Short: Analysis`
        *   `Distinguisher_Source: Iter7`
        *   `LTM_File_Path: LTM/TestFailure-HandleImportSuccessStateStep-LogAnalysis-PipelineFailure-Analysis_Iter7.md` (LTM file not yet created, will be created during reflection)
        *   `All_Tags: TestFailure, HandleImportSuccessStateStep, LogAnalysis, PipelineFailure, Iter7`
    *   `STM_ID: STM-Iter7-2`
        *   `Primary_Topic_Or_Error: ToolFailure`
        *   `Key_Concepts: read_file, FileNotFoundError`
        *   `Outcome_Indicator_Short: Resolved`
        *   `Distinguisher_Source: Iter7_Failure7.14`
        *   `LTM_File_Path: LTM/ToolFailure-read_file-FileNotFoundError-Resolved_Iter7_Failure7.14.md` (LTM file not yet created)
        *   `All_Tags: ToolFailure, read_file, FileNotFoundError, Resolved, Iter7, Failure7.14`
    *   `STM_ID: STM-Iter7-3`
        *   `Primary_Topic_Or_Error: ToolFailure`
        *   `Key_Concepts: read_file, RepetitionLimit`
        *   `Outcome_Indicator_Short: Resolved`
        *   `Distinguisher_Source: Iter7_Failure7.15`
        *   `LTM_File_Path: LTM/ToolFailure-read_file-RepetitionLimit-Resolved_Iter7_Failure7.15.md` (LTM file not yet created)
        *   `All_Tags: ToolFailure, read_file, RepetitionLimit, Resolved, Iter7, Failure7.15`
    *   `STM_ID: STM-Iter7-4`
        *   `Primary_Topic_Or_Error: Workflow`
        *   `Key_Concepts: ToolUsage, ProcessAdherence`
        *   `Outcome_Indicator_Short: Learning`
        *   `Distinguisher_Source: Iter7_ProcessHiccup`
        *   `LTM_File_Path: LTM/Workflow-ToolUsage-ProcessAdherence-Learning_Iter7_ProcessHiccup.md` (LTM file not yet created)
        *   `All_Tags: Workflow, ToolUsage, ProcessAdherence, Learning, Iter7, ProcessHiccup`
    *   Evaluate the effectiveness and potential overhead of reactive LTM consultation. Consider tuning keyword extraction from `META_LOG_DIRECTIVE`s or the matching threshold for STM `All_Tags`.
    *   Clarify if the `META_LOG_DIRECTIVE`s logging the reactive LTM consultation process itself should be excluded from triggering further reactive consultations to prevent deep, non-productive recursion.
    *   Refine the optional step of updating `Associated_STM_ID(s)` in LTM files: assess its value versus complexity.
    *   Consider a structured format for LTM file content (e.g., specific YAML frontmatter or sections) to make parsing/extraction of specific sections (like `Cross_References` or `Generalizable_Lessons_Learned`) easier if needed for future advanced LTM querying.
    *   Clarify how to robustly parse/utilize the `task_history_iter<CurrentIterNum>.txt` if its format varies or is very large. For now, assume it's readable text and I'll extract key `META_LOG_DIRECTIVE`s and code diffs.
    *   **Add explicit instruction in Section 3.3 and Section 3.2 to ensure immediate tool execution when a workflow step requires it, to prevent system errors.**

## 7. Escalation

*   **Status:** Resolved. (From previous escalation prior to Iteration 7). Autonomous operation resumed.
*   **(Will be updated if new escalation occurs in Iteration 7)**

## 8. Failure Tracking

*   **(Failures 7.1 - 7.12 from prior to Iteration 7 start. These will be candidates for LTM creation/summarization at end of Iteration 7, using task history.)**
*   **Failure 7.13 (Plan Step 4.13.16.4.1 - During Iteration 7):** The target test `CanImportAmazoncomOrder11291264431163432()` failed.
    *   **Errors:** NUnit.Framework.AssertionException: ShipmentInvoice '112-9126443-1163432' not created.
    *   **Outcome:** TestFailed_Assertion.
    *   **Resolution:** In progress. Current focus is detailed log analysis (Plan Step 4.13.16.4.4).
*   **Failure 7.14 (Plan Step 4.13.16.4.4.3 - During Iteration 7):** Attempted to read the test log file but it was not found at the expected location.
    *   **Errors:** File not found: c:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\bin\x64\Debug\net48\Logs\AutoBotTests-20250517.log
    *   **Outcome:** FileRead_Failed_FileNotFound.
    *   **Resolution:** In progress. Investigating log file location (Plan Step 4.13.16.4.4.3.1 onwards).
*   **Failure 7.15 (Plan Step 4.13.16.4.5.4 - During Iteration 7):** Attempted to read RefactoringMasterPlan.md but hit a tool repetition limit.
    *   **Errors:** Tool call repetition limit reached for read_file.
    *   **Outcome:** ToolExecution_Failed_RepetitionLimit.
    *   **Resolution:** In progress. Documenting failure and re-attempting read.
*   **(New failures from Iteration 7 execution will be added here.)**

## 9. Scoring

*   **(Scores from Iterations 1-6)**
*   **Iteration 7 (Current - To be updated at end of iteration):**
    *   Adherence to Cardinal Rules: /5
    *   Achievement of Objective: /5
    *   Quality of Self-Logging (META_LOG_DIRECTIVEs): /5
    *   Quality of Reflection & Prompt Improvements (including LTM/STM management & task history use): /5
    *   **Total Score (Iteration 7):** /20

## 10. Short-Term Memory (STM) / LTM Index

**Purpose:** STM entries act as a quick reference/index to immutable Long-Term Memory (LTM) files stored in the `LTM/` directory. They provide the "seed" data necessary to deterministically construct the LTM filename for direct access. Each STM entry corresponds to a unique, unchangeable LTM file. STM is consulted both deliberately (planning, failure analysis) and reactively (triggered by internal `META_LOG_DIRECTIVE` formulation).

**Format for STM Entries (Immutable after creation):**

*   `STM_ID`: Unique sequential identifier (e.g., `STM-001`).
*   `Clue`: A concise (1-2 sentence) summary of the LTM's core lesson, problem-solution, and outcome.
*   `Primary_Topic_Or_Error`: The main error code (e.g., `CS0121`) or core topic (e.g., `SerilogConfig`) used as the first part of the LTM filename. (Filesystem-safe).
*   `Key_Concepts`: A comma-separated list of key concepts, tools, or libraries associated with the LTM entry (e.g., `Serilog, Configuration, File Sink`). Used for LTM filename construction and STM searching. (Filesystem-safe, sorted alphabetically).
*   `Outcome_Indicator_Short`: A brief tag indicating the result (e.g., `Resolved`, `Failed`, `Workaround`, `Info`, `Analysis`). Used for LTM filename construction. (Filesystem-safe).
*   `Distinguisher_Source`: An iteration number, version, attempt number, or reference to a prior STM ID it builds upon (e.g., `_Iter7`, `_v1`, `_Attempt1`, `_FollowUpSTM-XXX`). Used for LTM filename construction. (Filesystem-safe).
*   `LTM_File_Path`: The deterministically constructed path to the immutable LTM file (e.g., `LTM/SerilogConfig-Configuration-FileSink-Info_Iter7.md`).
*   `All_Tags`: A comprehensive, comma-separated list of all keywords from `Primary_Topic_Or_Error`, `Key_Concepts`, `Outcome_Indicator_Short`, and `Distinguisher_Source`, plus any other relevant terms from the `Clue`. Used for searching/matching during LTM consultation. (Sorted alphabetically).
*   `Associated_LTM_File_Content_Snippet`: A brief snippet of the LTM file content for quick context during STM review. (Optional).
*   `Cross_References`: Markdown links to other relevant STM entries or LTM files. (Optional).

**STM Entries (Append-Only):**
*   `STM_ID`: STM-001
    `Clue`: Learned how to configure Serilog programmatically in NUnit tests using TestContext.CurrentContext.TestDirectory for dynamic log file paths.
    `Primary_Topic_Or_Error`: SerilogConfig
    `Key_Concepts`: Configuration, File Sink, NUnit, TestContext
    `Outcome_Indicator_Short`: Info
    `Distinguisher_Source`: Iter7_Setup
    `LTM_File_Path`: LTM/SerilogConfig-Configuration-FileSink-NUnit-TestContext-Info_Iter7_Setup.md
    `All_Tags`: Configuration, File Sink, Info, Iter7_Setup, NUnit, SerilogConfig, TestContext
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`:

*   `STM_ID`: STM-002
    `Clue`: Learned the protocol for handling File Not Found errors during setup, involving checking csproj files and using search_files.
    `Primary_Topic_Or_Error`: FileNotFound
    `Key_Concepts`: Csproj, File Search, Protocol, RefactoringMasterPlan
    `Outcome_Indicator_Short`: Resolved
    `Distinguisher_Source`: Iter7_Setup
    `LTM_File_Path`: LTM/FileNotFound-Csproj-FileSearch-Protocol-RefactoringMasterPlan-Resolved_Iter7_Setup.md
    `All_Tags`: Csproj, File Not Found, File Search, FileNotFound, Iter7_Setup, Protocol, RefactoringMasterPlan, Resolved
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`:

*   `STM_ID`: STM-003
    `Clue`: Learned the specific log file naming convention and location for AutoBotUtilities.Tests Serilog output, resolving Failure 7.14.
    `Primary_Topic_Or_Error`: LogFileLocation
    `Key_Concepts`: File Naming, File Search, Serilog, Test Logs
    `Outcome_Indicator_Short`: Resolved
    `Distinguisher_Source`: Iter7_Failure7.14
    `LTM_File_Path`: LTM/LogFileLocation-FileNaming-FileSearch-Serilog-TestLogs-Resolved_Iter7_Failure7.14.md
    `All_Tags`: File Naming, File Search, LogFileLocation, Resolved, Serilog, Test Logs
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`: STM-002

*   `STM_ID`: STM-004
    `Clue`: Identified through log analysis that the HandleImportSuccessStateStep returned False, causing the test failure (Failure 7.13). Refined hypothesis based on code analysis suggests failure is due to another template's processing failing.
    `Primary_Topic_Or_Error`: TestFailureRootCause
    `Key_Concepts`: HandleImportSuccessStateStep, Log Analysis, Pipeline, Test Failure, Code Analysis, Hypothesis
    `Outcome_Indicator_Short`: Analysis
    `Distinguisher_Source`: Iter7_Failure7.13_CodeAnalysis
    `LTM_File_Path`: LTM/TestFailureRootCause-CodeAnalysis-HandleImportSuccessStateStep-Hypothesis-LogAnalysis-Pipeline-TestFailure-Analysis_Iter7_Failure7.13_CodeAnalysis.md
    `All_Tags`: Analysis, Code Analysis, HandleImportSuccessStateStep, Hypothesis, Log Analysis, Pipeline, Test Failure, TestFailureRootCause
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`: Failure 7.13, STM-004

*   `STM_ID`: STM-005
    `Clue`: Encountered a tool repetition limit error when attempting to read RefactoringMasterPlan.md. Documented as Failure 7.15.
    `Primary_Topic_Or_Error`: ToolExecutionFailure
    `Key_Concepts`: Read File, Repetition Limit, Tool Error
    `Outcome_Indicator_Short`: Failed
    `Distinguisher_Source`: Iter7_Failure7.15
    `LTM_File_Path`: LTM/ToolExecutionFailure-ReadFile-RepetitionLimit-ToolError-Failed_Iter7_Failure7.15.md
    `All_Tags`: Failed, Read File, Repetition Limit, Tool Error, ToolExecutionFailure
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`: Failure 7.15
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveSTM_Consultation_Triggered",
  "Context": "Workflow Step 3.2: EXECUTE PLAN",
  "Directive": "Executing Reactive STM/LTM Consultation Sub-Workflow triggered by formulation of PlanStepExecution_Intent for re-reading test log.",
  "SourceIteration": 7,
  "TriggeringDirectiveType": "PlanStepExecution_Intent",
  "TriggeringDirectiveContentSnippet": "Re-read the test log file ... detailed analysis of HandleImportSuccessStateStep execution across all templates."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveSTM_Consultation_Triggered",
  "Context": "Reactive STM/LTM Consultation Sub-Workflow",
  "Directive": "Consulting STM entry STM-003 based on keywords from formulated intent.",
  "SourceIteration": 7,
  "STM_ID": "STM-003",
  "TriggeringDirectiveType": "PlanStepExecution_Intent",
  "TriggeringDirectiveContentSnippet": "Re-read the test log file ... detailed analysis of HandleImportSuccessStateStep execution across all templates."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveLTM_Retrieved",
  "Context": "Reactive STM/LTM Consultation Sub-Workflow",
  "Directive": "Failed to retrieve LTM content for STM-003. File not found.",
  "SourceIteration": 7,
  "STM_ID": "STM-003",
  "LTM_File_Path": "LTM/LogFileLocation-FileNaming-FileSearch-Serilog-TestLogs-Resolved_Iter7_Failure7.14.md",
  "Outcome": "Failed",
  "Details": "File not found during read_file attempt."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveSTM_Consultation_Triggered",
  "Context": "Reactive STM/LTM Consultation Sub-Workflow",
  "Directive": "Executing Reactive STM/LTM Consultation Sub-Workflow triggered by formulation of PlanStepExecution_Intent for re-reading test log. Consulting STM entry STM-004.",
  "SourceIteration": 7,
  "STM_ID": "STM-004",
  "TriggeringDirectiveType": "PlanStepExecution_Intent",
  "TriggeringDirectiveContentSnippet": "Re-read the test log file ... detailed analysis of HandleImportSuccessStateStep execution across all templates."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveLTM_Retrieved",
  "Context": "Reactive STM/LTM Consultation Sub-Workflow",
  "Directive": "Failed to retrieve LTM content for STM-004. File not found.",
  "SourceIteration": 7,
  "STM_ID": "STM-004",
  "LTM_File_Path": "LTM/TestFailureRootCause-CodeAnalysis-HandleImportSuccessStateStep-Hypothesis-LogAnalysis-Pipeline-TestFailure-Analysis_Iter7_Failure7.13_CodeAnalysis.md",
  "Outcome": "Failed",
  "Details": "File not found during read_file attempt."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveSTM_Consultation_Triggered",
  "Context": "Reactive STM/LTM Consultation Sub-Workflow",
  "Directive": "Executing Reactive STM/LTM Consultation Sub-Workflow triggered by formulation of PlanStepExecution_Intent for re-reading test log. Consulting STM entry STM-005.",
  "SourceIteration": 7,
  "STM_ID": "STM-005",
  "TriggeringDirectiveType": "PlanStepExecution_Intent",
  "TriggeringDirectiveContentSnippet": "Re-read the test log file ... detailed analysis of HandleImportSuccessStateStep execution across all templates."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveLTM_Retrieved",
  "Context": "Reactive STM/LTM Consultation Sub-Workflow",
  "Directive": "Failed to retrieve LTM content for STM-005. File not found.",
  "SourceIteration": 7,
  "STM_ID": "STM-005",
  "LTM_File_Path": "LTM/ToolExecutionFailure-ReadFile-RepetitionLimit-ToolError-Failed_Iter7_Failure7.15.md",
  "Outcome": "Failed",
  "Details": "File not found during read_file attempt."
}}
META_LOG_DIRECTIVE: {{
  "Type": "PlanStepExecution_Intent",
  "Context": "Workflow Step 3.2: EXECUTE PLAN",
  "Directive": "Execute Plan Step 4.13.16.4.6.9.1: Formulate META_LOG_DIRECTIVE for re-reading test log file.",
  "SourceIteration": 7,
  "Content": "Re-read the test log file `AutoBotUtilities.Tests\\bin\\x64\\Debug\\net48\\Logs\\AutoBotTests-20250517.log` for detailed analysis of `HandleImportSuccessStateStep` execution across all templates."
}}
META_LOG_DIRECTIVE: {{
  "Type": "PlanStepExecution_Outcome",
  "Context": "Workflow Step 3.2: EXECUTE PLAN",
  "Directive": "Executed Plan Step 4.13.16.4.6.9.2: Read test log file.",
  "SourceIteration": 7,
  "Outcome": "Success",
  "Details": "Successfully read test log file `AutoBotUtilities.Tests\\bin\\x64\\Debug\\net48\\Logs\\AutoBotTests-20250517.log`. Content is available for analysis.",
  "AssociatedFilePath": "AutoBotUtilities.Tests\\bin\\x64\\Debug\\net48\\Logs\\AutoBotTests-20250517.log"
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveLTM_Retrieved",
  "Context": "Reactive STM/LTM Consultation Sub-Workflow",
  "Directive": "Failed to retrieve LTM content for STM-005. File not found.",
  "SourceIteration": 7,
  "STM_ID": "STM-005",
  "LTM_File_Path": "LTM/ToolExecutionFailure-ReadFile-RepetitionLimit-ToolError-Failed_Iter7_Failure7.15.md",
  "Outcome": "Failed",
  "Details": "File not found during read_file attempt."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveSTM_Consultation_Triggered",
  "Context": "Workflow Step 3.2: EXECUTE PLAN",
  "Directive": "Executing Reactive STM/LTM Consultation Sub-Workflow triggered by formulation of PlanStepExecution_Intent for analyzing test log content.",
  "SourceIteration": 7,
  "TriggeringDirectiveType": "PlanStepExecution_Intent",
  "TriggeringDirectiveContentSnippet": "Analyze the log content to identify all templates processed by HandleImportSuccessStateStep..."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveSTM_Consultation_Triggered",
  "Context": "Workflow Step 3.2: EXECUTE PLAN",
  "Directive": "Executing Reactive STM/LTM Consultation Sub-Workflow triggered by formulation of PlanStepExecution_Intent for reading the rest of the test log file.",
  "SourceIteration": 7,
  "TriggeringDirectiveType": "PlanStepExecution_Intent",
  "TriggeringDirectiveContentSnippet": "Read the rest of the test log file ... to analyze HandleImportSuccessStateStep execution."
}}
META_LOG_DIRECTIVE: {{
  "Type": "ReactiveSTM_Consultation_Triggered",
  "Context": "Workflow Step 3.2: EXECUTE PLAN",
  "Directive": "Executing Reactive STM/LTM Consultation Sub-Workflow triggered by formulation of PlanStepExecution_Intent for reading the rest of the test log file.",
  "SourceIteration": 7,
  "TriggeringDirectiveType": "PlanStepExecution_Intent",
  "TriggeringDirectiveContentSnippet": "Read the rest of the test log file ... to analyze HandleImportSuccessStateStep execution."
}}