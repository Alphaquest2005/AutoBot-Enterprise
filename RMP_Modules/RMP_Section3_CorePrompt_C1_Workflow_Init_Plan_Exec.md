### 3.C1 Workflow Steps: Initialization, Planning, Execution, and Failure Handling

**Workflow Steps:**

*   **Workflow Step -1: INITIATE ITERATION CYCLE:**
    *   **1. System Prompt Assimilation:** Internally process `system-prompt-Logger` to load all foundational directives and protocols.
    *   **2. RMP Context Loading:**
        *   `read_file` the main `RefactoringMasterPlan.md` (hub).
        *   Based on the hub, sequentially `read_file` all linked RMP sub-files for Sections 2 through 10 to establish full iteration context. This includes:
            *   `RMP_Modules/RMP_Section2_UserObjective_Iter<CurrentIterNum>.md`
            *   `RMP_Modules/RMP_Section3_CorePrompt_Hub.md` and all its linked parts (A, B, C1, C2, D).
            *   `RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md` (current or from previous session).
            *   `RMP_Modules/RMP_Task_Header_Iter<CurrentIterNum>.md` (if exists from previous session).
            *   `RMP_Modules/RMP_Task_Footer_Iter<CurrentIterNum>.md` (if exists from previous session).
            *   `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md` (if exists, create if not when first meta-log is appended).
            *   `RMP_Modules/RMP_Section5_LogAnalysis_Iter<CurrentIterNum>.md`.
            *   `RMP_Modules/RMP_Section6_Reflection_Iter<CurrentIterNum>.md` & `RMP_Modules/RMP_Section6_StandingImprovements.md`.
            *   `RMP_Modules/RMP_Section7_Escalation.md`.
            *   `RMP_Modules/RMP_Section8_FailureTracking.md`.
            *   `RMP_Modules/RMP_Section10_STM_Iter<CurrentIterNum>.md`.
    *   **3. Initial State Identification:**
        *   Confirm "Current User-Defined Iteration Objective & Priority".
        *   Note current state of "User Task Plan", "Failure Tracking", "Escalation", "STM".
    *   **4. Proceed based on Escalation Status:**
        *   If Escalation is active (per `RMP_Modules/RMP_Section7_Escalation.md`), await user input.
        *   If Escalation is not active, proceed to **Workflow Step 3.0: INITIAL ITERATION COMMIT**.

*   **Workflow Step 3.0: INITIAL ITERATION COMMIT:**
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Context: "INITIAL ITERATION COMMIT", Directive: "Preparing for initial iteration commit after loading RMP and identifying objective."
        *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
    *   *Output the `PlanStepExecution_Intent` `META_LOG_DIRECTIVE`.* **Then, use `insert_content` to append the full, formatted `META_LOG_DIRECTIVE` string to `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md` (creating file if it doesn't exist).**
    *   Determine current Iteration Number (e.g., from RMP Section 2 sub-file).
    *   Extract a brief summary of the "Refined Objective" from the RMP Section 2 sub-file.
    *   Use `execute_command` with `command: git add .` (Stages all current files, including new/updated RMP files).
    *   Use `execute_command` with `command: git commit -m "RMP Workflow: Iteration {CurrentIterNum} started. Objective: {ObjectiveSummary}"`. *(Ensure placeholders are dynamically filled)*.
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Initial iteration commit completed." (If commit fails, log error details here).
        *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
    *   *Output the `PlanStepExecution_Outcome` `META_LOG_DIRECTIVE`.* **Append to Task Execution Log.**. Use `insert_content` to append it
    *   Proceed to **Workflow Step 3.1: PLAN ITERATION**.

*   **Workflow Step 3.1: PLAN ITERATION (Adhering to "Iteration Task Checklists Protocol"):**
    *   **1. Populate and Process Task Header:**
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "PLAN ITERATION - Task Header Setup", Directive: "Initiating Task Header processing. Reading seed checklist from system-prompt-Logger."
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
        *   Use `read_file` to get the "Seed Task Header Checklist" content from `system-prompt-Logger`.
        *   Use `write_to_file` to create/overwrite `RMP_Modules/RMP_Task_Header_Iter<CurrentIterNum>.md` with this seed content. (This RMP file modification is subject to RMP-MP, including pre/post commits and integrity checks if the checklist definition itself were to change).
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "PLAN ITERATION - Task Header Setup", Directive: "Populated RMP_Task_Header_Iter<CurrentIterNum>.md. Proceeding to process checklist items."
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
        *   Meticulously execute and verify each item (H1-H5) in `RMP_Task_Header_Iter<CurrentIterNum>.md`. For each item:
            *   Log intent, action(s) taken for verification, and outcome using `META_LOG_DIRECTIVE`s (and append them to Task Execution Log).
            *   Update its status, start/end times, and notes/verification details directly in the `RMP_Task_Header_Iter<CurrentIterNum>.md` file (using `apply_diff` or `write_to_file` for the whole file, subject to RMP-MP).
        *   **CRITICAL:** Confirm all Task Header items are "Completed" (or "Skipped/NA" with justification). Any "Failed" items MUST be resolved (potentially by making their resolution the first steps of the User Task Plan) or escalated. Any identified prerequisites (e.g., logging filter setup from H3.2, PSM-P update from H3.1, TDRP applicability from H3.4) MUST inform the User Task Plan.
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "PLAN ITERATION - Task Header Completion", Directive: "All items in RMP_Task_Header_Iter<CurrentIterNum>.md processed and verified. Findings will inform User Task Plan."
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
    *   **2. Review Core Inputs for User Task Plan:**
        *   Review "Current User-Defined Iteration Objective & Priority" (from `RMP_Modules/RMP_Section2_UserObjective_Iter<CurrentIterNum>.md`).
        *   Review previous iteration's "Reflection & Prompt Improvements" (RMP Section 6 sub-file) and "Failure Tracking" (RMP Section 8 sub-file).
        *   Review notes and insights from the completed `RMP_Task_Header_Iter<CurrentIterNum>.md`.
    *   **3. Formulate User Task Plan (`RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md` - as per "Iteration Task Checklists Protocol" Point 2 in `system-prompt-Logger`):**
        *   **a. Initial Brainstorming & Analysis (Internal, log summary as META_LOG_DIRECTIVE):** Brainstorm approaches, select primary strategy, proactively identify roadblocks/dependencies, briefly consider 1-2 high-level alternatives.
        *   **b. Detailed Plan Formulation:** Formulate the step-by-step plan in `RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md`. Ensure plan adheres to Content Mandates (actionable steps, proactive PSM-P for code interaction/design, potential `new_task` segmentation for complex phases, tool usage, `META_LOG_DIRECTIVE`s, LTM/STM consultations, Task Header findings, roadblock mitigations).

        This also includes integrating steps from the Test-Driven Refactoring Protocol (TDRP) if Task Header item H3.4 identified its applicability (e.g., for large files or complex methods).

        Each part of this plan formulation is conceptually a `META_LOG_DIRECTIVE` and triggers Reactive STM/LTM Consultation Sub-Workflow** (defined in [`./RMP_Section3_CorePrompt_A_LTM_STM.md`](./RMP_Section3_CorePrompt_A_LTM_STM.md)).
    *   **4. Pre-Execution Validation of User Task Plan (CRITICAL - as per "Iteration Task Checklists Protocol" Point 2.C in `system-prompt-Logger`):**
        *   Perform a validation review of `RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md` against all criteria: Alignment with User Objective, Consideration of Roadblocks/Alternatives, Consistency with All Protocols, No Contradictions, Optimization Potential, Adherence to Foundational/Operational Goals, and Viability for Footer Tasks. Use self-questioning as a technique.
        *   If validation reveals issues, refine the plan and re-validate until it passes.
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "PLAN ITERATION - User Task Plan Validation", Directive: "User Task Plan (`RMP_UserTask_Plan_Iter<CurrentIterNum>.md`) formulated and validated successfully." (or "User Task Plan refined and validated.")
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
    *   **5. Proceed:**
        *   Proceed to **Workflow Step 3.2: EXECUTE PLAN**.

*   **Workflow Step 3.2: EXECUTE PLAN:**
    *   Execute steps from "Your Plan" (`RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md`) sequentially.
    *   For each step:
        *   Use appropriate tool(s). **Crucially, if the plan step requires a tool, the tool call MUST be the immediate next action.**
        *   Before modifying existing files (code or RMP), ALWAYS `read_file` (unless creating a new file with `write_to_file`).
        *   **Formulate `META_LOG_DIRECTIVE`** with Type `PlanStepExecution_Intent` (before tool call).
        *   **Execute Reactive STM/LTM Consultation Sub-Workflow**.
        *   *Output the `PlanStepExecution_Intent` `META_LOG_DIRECTIVE`.* **Append to Task Execution Log.**
        *   Execute the tool.
        *   Analyze the result.
        *   **Formulate `META_LOG_DIRECTIVE`** with Type `PlanStepExecution_Outcome` (after tool result).
        *   **Execute Reactive STM/LTM Consultation Sub-Workflow**.
        *   *Output the `PlanStepExecution_Outcome` `META_LOG_DIRECTIVE`.* **Append to Task Execution Log.**
        *   If step completes successfully, mark as "Completed" in `RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md` (using `apply_diff` or `write_to_file`, subject to RMP-MP).
        *   If step fails, immediately proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE**.
        *   *(If a plan step involves initiating a sub-task via `new_task` as per Hierarchical Task Management Protocol, follow that protocol for initiating and later verifying the sub-task.)*

*   **Workflow Step 3.3: HANDLE EXECUTION FAILURE:**
    *   **1. Initial Failure Logging & Documentation:**
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "HANDLE EXECUTION FAILURE - Analysis Start", Directive: "Execution of previous plan step failed. Analyzing failure result."
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log (`RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`).**
        *   Carefully analyze the complete failure result (e.g., compiler error messages and codes, test runner output with stack traces, tool error messages, unexpected file content).
        *   Document the failure comprehensively in "Failure Tracking" (`RMP_Modules/RMP_Section8_FailureTracking.md`), including type, description, detailed error output, and initial diagnosis. (This RMP file modification is subject to RMP-MP and involves its own meta-logging to the Task Execution Log).

    *   **2. Specific Failure Type Triage & Initial Response:**
        *   **a. If "File Not Found" for an expected source code file (e.g., during `read_file`):**
            *   **Invoke Project Structure Management Protocol (PSM-P) as the PRIMARY method:** Follow steps in `system-prompt-Logger` (PSM-P section) to locate the file using project files (`.csproj`, `.sln`) and update/create `ProjectStructure.md`. **PSM-P must be attempted before resorting to general directory searches for source code files.**
            *   Log PSM-P actions with `META_LOG_DIRECTIVE`s (appended to Task Execution Log).
            *   If PSM-P successfully locates the file: Formulate a sub-plan to re-attempt the original action (e.g., reading the file) using the verified path. Update User Task Plan and proceed to **Workflow Step 3.2 EXECUTE PLAN**. *(Return from failure handling for this specific case if resolved).*
            *   If PSM-P confirms the file genuinely does not exist as part of the project structure: Note this. The failure remains unresolved by PSM-P. Proceed to general diagnosis (Point 3 below).
        *   **b. If Build Error indicating Missing/Undefined Type, Namespace, or Member (e.g., CS0246, CS0103, CS0117):**
            *   **Invoke Project Structure Management Protocol (PSM-P) for Verification:**
                1.  Identify the missing entity and the file/project where it's expected or referenced.
                2.  Consult `ProjectStructure.md` (PSM-P Step 4.2).
                3.  If not found/verified, analyze relevant `.csproj` and `.sln` files (PSM-P Step 4.3) to check for missing file references, incorrect project dependencies, or if the file containing the definition is not compiled.
                4.  If a file path is found/verified, analyze the source code file (PSM-P Step 4.5) to confirm if the class/member definition actually exists and matches the expected signature.
                5.  Update `ProjectStructure.md` (PSM-P Steps 4.4 & 4.6).
            *   Log PSM-P actions with `META_LOG_DIRECTIVE`s (appended to Task Execution Log).
            *   If PSM-P reveals the cause (e.g., missing `using` directive, incorrect member name/signature, file not included in project, missing project reference): Formulate a sub-plan to correct the issue (e.g., add `using`, correct call, edit `.csproj`). Update User Task Plan and proceed to **Workflow Step 3.2 EXECUTE PLAN**. *(Return from failure handling if directly resolved).*
            *   If PSM-P confirms the entity is genuinely undefined in the expected scope: Proceed to general diagnosis (Point 3 below).
        *   **c. For Other Failures (Test Assertion Failures, Unexpected Tool Behavior, Runtime Exceptions not covered above, etc.):**
            *   Proceed directly to general diagnosis (Point 3 below).

    *   **3. General Failure Diagnosis & Investigation:**
        *   **a. Logging System Check & Adjustment (If detailed logs are needed for diagnosis):**
            *   **Verify Filter Implementation:** As per `system-prompt-Logger`, review Serilog config in the current execution context to confirm the dynamic filter (using `LogFilterState`) is active. Log verification (to Task Execution Log). If not active, this may be a sub-problem to address first, or diagnosis must rely on existing log verbosity.
            *   **Adjust `LogFilterState` (If filter active):** To get more detailed logs from relevant components, consider modifying `Core.Common.Extensions.LogFilterState` properties (`TargetSourceContextForDetails`, `TargetMethodNameForDetails`, `DetailTargetMinimumLevel`). Log any changes made to `LogFilterState` (to Task Execution Log). **Remember to reset `LogFilterState` to defaults after this specific troubleshooting session for the failure.**
            *   If adjustments to `LogFilterState` were made, it may be necessary to re-run the failing operation/test to capture the more detailed logs before proceeding with deep analysis. This re-run would be a specific step in a formulated sub-plan.
        *   **b. Deliberate STM/LTM Review for Broader Insights:**
            *   Identify keywords, error codes, behavioral patterns from the failure and surrounding logs.
            *   Scan STM `All_Tags` in `RMP_Modules/RMP_Section10_STM_Iter<CurrentIterNum>.md`.
            *   If matches found: Log `DeliberateSTM_Consultation_Failure` (to Task Execution Log), construct LTM path(s), `read_file` LTM(s), analyze content (including `Cross_References`), and incorporate insights into diagnostic reasoning.
            *   **Fallback LTM Check:** If no suitable STM entry, attempt direct LTM construction/retrieval based on failure context. Log as `LTM_DirectConstruct_Consultation_Failure` (to Task Execution Log).
        *   **c. Analyze available information:** This includes error messages, stack traces, existing logs (now potentially more detailed), STM/LTM insights, and relevant source code (verified via PSM-P if applicable).

    *   **4. Sub-Plan Formulation to Address Failure:**
        *   Based on all analysis from Points 1-3, formulate a precise, step-by-step sub-plan to correct the identified root cause of the failure.
        *   This sub-plan formulation triggers Reactive STM/LTM Consultation.
        *   The sub-plan may involve code changes, configuration changes, or further focused investigation.

    *   **5. Update User Task Plan & Proceed:**
        *   Integrate the new sub-plan into the main "Your Plan" (`RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md`).
        *   Proceed back to **Workflow Step 3.2 EXECUTE PLAN** to execute this sub-plan.

    *   **6. Persistent Failure Check (Loop Control):**
        *   If this specific failure (or failure to achieve a particular sub-goal related to it) persists after **three distinct, well-reasoned, and documented solution attempts** (each attempt consisting of diagnosis, sub-plan, and execution), then proceed to **Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL** (defined in `RMP_Modules/RMP_Section3_CorePrompt_C2_Workflow_Build_History_Reflect_End.md`).