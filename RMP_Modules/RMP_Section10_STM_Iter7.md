## 10. Short-Term Memory (STM) Index (Iteration 7)

The purpose and overall format of STM entries are defined in your primary system prompt (`system-prompt-Logger`). STM entries act as seeds for LTM filename construction and are indexed here. The comprehensive LTM/STM management principles are detailed in the RMP Section 3 sub-files.
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

**STM Entries (Append-Only for Iteration 7):**
*   `STM_ID: STM-001`
    `Clue`: Learned how to configure Serilog programmatically in NUnit tests using TestContext.CurrentContext.TestDirectory for dynamic log file paths.
    `Primary_Topic_Or_Error`: SerilogConfig
    `Key_Concepts`: Configuration, File Sink, NUnit, TestContext
    `Outcome_Indicator_Short`: Info
    `Distinguisher_Source`: Iter7_Setup
    `LTM_File_Path`: LTM/SerilogConfig-Configuration-FileSink-NUnit-TestContext-Info_Iter7_Setup.md
    `All_Tags`: Configuration, File Sink, Info, Iter7_Setup, NUnit, SerilogConfig, TestContext
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`:

*   `STM_ID: STM-002`
    `Clue`: Learned the protocol for handling File Not Found errors during setup, involving checking csproj files and using search_files.
    `Primary_Topic_Or_Error`: FileNotFound
    `Key_Concepts`: Csproj, File Search, Protocol, RefactoringMasterPlan
    `Outcome_Indicator_Short`: Resolved
    `Distinguisher_Source`: Iter7_Setup
    `LTM_File_Path`: LTM/FileNotFound-Csproj-FileSearch-Protocol-RefactoringMasterPlan-Resolved_Iter7_Setup.md
    `All_Tags`: Csproj, File Not Found, File Search, FileNotFound, Iter7_Setup, Protocol, RefactoringMasterPlan, Resolved
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`:

*   `STM_ID: STM-003`
    `Clue`: Learned the specific log file naming convention and location for AutoBotUtilities.Tests Serilog output, resolving Failure 7.14.
    `Primary_Topic_Or_Error`: LogFileLocation
    `Key_Concepts`: File Naming, File Search, Serilog, Test Logs
    `Outcome_Indicator_Short`: Resolved
    `Distinguisher_Source`: Iter7_Failure7.14
    `LTM_File_Path`: LTM/LogFileLocation-FileNaming-FileSearch-Serilog-TestLogs-Resolved_Iter7_Failure7.14.md
    `All_Tags`: File Naming, File Search, LogFileLocation, Resolved, Serilog, Test Logs
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`: STM-002

*   `STM_ID: STM-004`
    `Clue`: Identified through log analysis that the HandleImportSuccessStateStep returned False, causing the test failure (Failure 7.13). Refined hypothesis based on code analysis suggests failure is due to another template's processing failing.
    `Primary_Topic_Or_Error`: TestFailureRootCause
    `Key_Concepts`: HandleImportSuccessStateStep, Log Analysis, Pipeline, Test Failure, Code Analysis, Hypothesis
    `Outcome_Indicator_Short`: Analysis
    `Distinguisher_Source`: Iter7_Failure7.13_CodeAnalysis
    `LTM_File_Path`: LTM/TestFailureRootCause-CodeAnalysis-HandleImportSuccessStateStep-Hypothesis-LogAnalysis-Pipeline-TestFailure-Analysis_Iter7_Failure7.13_CodeAnalysis.md
    `All_Tags`: Analysis, Code Analysis, HandleImportSuccessStateStep, Hypothesis, Log Analysis, Pipeline, Test Failure, TestFailureRootCause
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`: Failure 7.13, STM-004

*   `STM_ID: STM-005`
    `Clue`: Encountered a tool repetition limit error when attempting to read RefactoringMasterPlan.md. Documented as Failure 7.15.
    `Primary_Topic_Or_Error`: ToolExecutionFailure
    `Key_Concepts`: Read File, Repetition Limit, Tool Error
    `Outcome_Indicator_Short`: Failed
    `Distinguisher_Source`: Iter7_Failure7.15
    `LTM_File_Path`: LTM/ToolExecutionFailure-ReadFile-RepetitionLimit-ToolError-Failed_Iter7_Failure7.15.md
    `All_Tags`: Failed, Read File, Repetition Limit, Tool Error, ToolExecutionFailure
    `Associated_LTM_File_Content_Snippet`: (Content will be added when LTM file is created during reflection)
    `Cross_References`: Failure 7.15