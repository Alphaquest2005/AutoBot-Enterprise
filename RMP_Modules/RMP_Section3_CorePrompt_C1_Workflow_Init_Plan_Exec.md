### 3.C1 Workflow Steps: Initialization, Planning, Execution, and Failure Handling

**Workflow Steps:**

*   **Workflow Step -1: INITIATE ITERATION CYCLE:**
    *   Read the main `RefactoringMasterPlan.md` (hub) and all linked RMP sub-files for Sections 2 through 10 to establish full context.
    *   Identify "Current User-Defined Iteration Objective & Priority" (from RMP Section 2 sub-file).
    *   Identify current "Your Plan" (from RMP Section 4 sub-file for the current iteration).
    *   Identify current "Failure Tracking" (from RMP Section 8 sub-file).
    *   Identify current "Escalation" status (from RMP Section 7 sub-file).
    *   Identify current "Short-Term Memory" (from RMP Section 10 sub-file) for context.
    *   If Escalation is active, wait for user input.
    *   If Escalation is not active, proceed to **Workflow Step 3.0: INITIAL ITERATION COMMIT**.

*   **Workflow Step 3.0: INITIAL ITERATION COMMIT:**
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Preparing for initial iteration commit after loading RMP and identifying objective."
        *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
    *   *Output the `PlanStepExecution_Intent` `META_LOG_DIRECTIVE`.*
    *   Determine current Iteration Number (e.g., from RMP Section 2 sub-file, or if not available yet, assume based on file naming like Iter7).
    *   Extract a brief summary of the "Refined Objective" from the RMP Section 2 sub-file.
    *   Use `execute_command` with `command: git add .` (This stages all current files, including RMP hub and sub-files, and any other setup changes).
    *   Use `execute_command` with `command: git commit -m "RMP Workflow: Iteration {CurrentIterNum} started. Objective: {ObjectiveSummary}"`. *(Ensure placeholders are dynamically filled)*.
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Initial iteration commit completed." (If commit fails, log error details here).
        *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
    *   *Output the `PlanStepExecution_Outcome` `META_LOG_DIRECTIVE`.*
    *   Proceed to **Workflow Step 3.1: PLAN ITERATION**.

*   **Workflow Step 3.1: PLAN ITERATION:**
    *   Review "Current User-Defined Iteration Objective & Priority" (RMP Section 2 sub-file).
    *   Review previous iteration's "Reflection & Prompt Improvements" (RMP Section 6 sub-file) and "Failure Tracking" (RMP Section 8 sub-file). (This review itself can be considered as generating internal `META_LOG_DIRECTIVE`s, potentially triggering reactive LTM consultation).
    *   **Deliberate STM/LTM Review for Iteration Planning:**
        *   Identify primary keywords, error codes, concepts from the current iteration's overall objective or known blocking issues (context tags).
        *   Scan the RMP Section 10 sub-file (STM Index) `All_Tags` field for STM entries with significant keyword overlap.
        *   If relevant STM entries are found:
            *   For each, log `STM_ID` with `META_LOG_DIRECTIVE` Type `DeliberateSTM_Consultation_Planning`.
            *   Construct `LTM_File_Path` from STM seeds, `read_file` LTM.
            *   Analyze LTM content (including `Cross_References`), incorporate insights into planning. Log retrieved LTM path.
        *   **Fallback LTM Check (If no suitable STM entry):**
            *   From current context tags, derive potential LTM seed components (`Primary_Topic_Or_Error`, etc.).
            *   Attempt to construct a hypothetical `LTM_File_Path`.
            *   Use `read_file`. If successful, log `LTM_DirectConstruct_Consultation_Planning` and use insights. Else, log no direct LTM found.
    *   Formulate or refine "Your Plan" (RMP Section 4 sub-file for the current iteration). Each part of this formulation (e.g., "Step X: Do Y because Z") is conceptually a `META_LOG_DIRECTIVE` and triggers the **Reactive STM/LTM Consultation Sub-Workflow** (defined in [`./RMP_Section3_CorePrompt_A_LTM_STM.md`](./RMP_Section3_CorePrompt_A_LTM_STM.md)).
    *   Document the plan in the RMP Section 4 sub-file.
    *   Proceed to **Workflow Step 3.2: EXECUTE PLAN**.

*   **Workflow Step 3.2: EXECUTE PLAN:**
    *   Execute steps from "Your Plan" (RMP Section 4 sub-file) sequentially.
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
        *   If step completes successfully, mark as "Completed" in the RMP Section 4 sub-file.
        *   If step fails, immediately proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE**.

*   **Workflow Step 3.3: HANDLE EXECUTION FAILURE:**
    *   Analyze failure result. (This analysis generates internal `META_LOG_DIRECTIVE`s, triggering reactive consultation).
    *   Document failure in "Failure Tracking" (RMP Section 8 sub-file).
    *   **Deliberate STM/LTM Review for Failure Diagnosis:**
        *   Identify keywords, error codes, etc., from the failure.
        *   Scan STM `All_Tags` in RMP Section 10 sub-file. If matches: log `DeliberateSTM_Consultation_Failure`, construct LTM path, `read_file` (if not the failing tool), analyze, incorporate.
        *   **Fallback LTM Check:** (As in 3.1, log as `LTM_DirectConstruct_Consultation_Failure`).
    *   Based on analysis (informed by deliberate and reactive LTM), formulate a sub-plan. This formulation triggers reactive LTM consultation.
    *   Update "Your Plan" (RMP Section 4 sub-file).
    *   Proceed back to **Workflow Step 3.2 EXECUTE PLAN**.
    *   If a specific failure persists after three distinct attempts (distinct solution strategies, potentially informed by different LTM paths), proceed to **Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL** (defined in [`./RMP_Section3_CorePrompt_C2_Workflow_Build_History_Reflect_End.md`](./RMP_Section3_CorePrompt_C2_Workflow_Build_History_Reflect_End.md)).