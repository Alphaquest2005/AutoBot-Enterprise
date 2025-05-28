3.C1 Workflow Steps: Initialization, Planning, Execution, and Failure Handling
Workflow Steps:
Workflow Step -1: INITIATE ITERATION CYCLE (Adhering to ILMP):
-1.0: Determine Target Iteration Number & User Objective (as per ILMP.2):
    -1.0.1: User Input Analysis: Analyze the user's initial prompt for explicit iteration numbering (e.g., "Start Iteration X", "Initialize Iteration X") or keywords indicating a new objective.
    -1.0.2: Consult `RMP_Modules/RMP_Iteration_Status.md`:
        *   `read_file` `RMP_Modules/RMP_Iteration_Status.md`.
        *   If the file does not exist:
            *   Use `write_to_file` to create `RMP_Modules/RMP_Iteration_Status.md` with initial content:
                ```markdown
                # RMP Iteration Status Log

                | Iteration | Status      | UserObjectiveSummary                             | InitializedTimestampUTC | FinalizedTimestampUTC |
                |-----------|-------------|--------------------------------------------------|-------------------------|-----------------------|
                ```
            *   Log (Type: `ILMP_File_Created`, Directive: "`RMP_Modules/RMP_Iteration_Status.md` created."). Append to Task Execution Log (for current iteration, once its log file is established).
            *   Assume `LastIterNum = 0`, `LastIterStatus = Finalized` (effectively).
        *   Else (file exists): Parse the file to find the highest iteration number (`LastIterNum`) and its `Status` (`LastIterStatus`) and `UserObjectiveSummary` (`LastIterObjective`).
    -1.0.3: Determine `<NewIterNum>` for the current task:
        *   If the user's prompt explicitly specified "Iteration X", then `<NewIterNum> = X`.
        *   Else (no explicit number from user):
            *   If `LastIterStatus` is "Finalized" or `LastIterNum` is 0 (i.e., status file was just created or last iteration is closed), then `<NewIterNum> = LastIterNum + 1`.
            *   Else (`LastIterStatus` is "Initializing", "Active", "Reflecting", or "Finalizing"):
                *   Use `ask_followup_question`: "Iteration `{LastIterNum}` ('{LastIterObjective}') has status '{LastIterStatus}'. Do you want to: (a) Finalize Iteration `{LastIterNum}` now, then start Iteration `{LastIterNum + 1}` with the new objective? (b) Continue working on Iteration `{LastIterNum}` with this new objective (this may override its current objective details if they differ significantly)? (c) Abort starting a new objective?"
                *   **If (a):** Trigger a new sub-task to execute ILMP.4 (IFP) for `LastIterNum`. Upon successful completion of that sub-task (verified via its `SubTask_Completion_Summary`), set `<NewIterNum> = LastIterNum + 1`. If IFP sub-task fails, halt and report to user.
                *   **If (b):** Set `<NewIterNum> = LastIterNum`.
                *   **If (c):** Halt operation and await further user instruction.
    -1.0.4: Set `<CurrentIterNum>`: `<CurrentIterNum> = <NewIterNum>`. Log this (Type: `ILMP_IterationSet`, Directive: "Current iteration number set to `<CurrentIterNum>`.").
    -1.0.5: Define/Update `RMP_Modules/RMP_Section2_UserObjective_Iter<CurrentIterNum>.md`:
        *   Extract the new User Objective text, Priority, Context, Expected Outcome, and Definition of Done from the user's prompt (or use placeholders if initializing).
        *   Use `write_to_file` to create/overwrite `RMP_Modules/RMP_Section2_UserObjective_Iter<CurrentIterNum>.md` with this full information.
    -1.0.6: Update `RMP_Modules/RMP_Iteration_Status.md` for `<CurrentIterNum>`:
        *   `read_file` `RMP_Modules/RMP_Iteration_Status.md`.
        *   Check if an entry for `<CurrentIterNum>` already exists.
        *   If NO existing entry for `<CurrentIterNum>`: Append a new row for `<CurrentIterNum>` with `Status: Initializing`, the `UserObjectiveSummary` (from -1.0.5), and the current `InitializedTimestampUTC`.
        *   If YES an existing entry for `<CurrentIterNum>` (implies resuming/revisiting): Update its `UserObjectiveSummary` if it has changed significantly from the new prompt, ensure `Status` is appropriate (e.g., if it was "Active", it remains "Active" or becomes "Initializing" if objective is vastly different, requiring re-planning). `InitializedTimestampUTC` should generally not change if resuming.
        *   `write_to_file` or `apply_diff` the updated `RMP_Modules/RMP_Iteration_Status.md`.
        *   Log (Type: `ILMP_StatusUpdate`, Directive: "Entry for Iteration `<CurrentIterNum>` added/updated in `RMP_Iteration_Status.md`. Status: [New Status].").

-1.1: System Prompt Assimilation: Internally process `system-prompt-Logger` to load all foundational directives and protocols (including ILMP, Hierarchical Task Management, RMP-MP, etc.).

-1.2: RMP Context Loading & Critical Workfile Verification (for `<CurrentIterNum>`):
    -1.2.A: `read_file` the main `RefactoringMasterPlan.md` (hub). Verify its links will correctly resolve to `_Iter<CurrentIterNum>.md` files where appropriate (links themselves might be generic, e.g. `RMP_Section2_UserObjective_Current.md` which is a symlink or copy, or they might need to be updated if they contain iteration numbers explicitly and a *new* iteration number is starting. For now, assume links in hub are general or will be handled by RMP-MP if direct editing is needed).
    -1.2.B: Critical Workfile Verification & Initialization for `<CurrentIterNum>`:
        *   `ProjectStructure.md`:
            *   `read_file` `ProjectStructure.md`. If it does NOT exist, log (Type: `SystemActivity`, Directive: "`ProjectStructure.md` missing. Will be created/updated via PSM-P during Task Header H3.1 or as User Task Plan step."). If it exists, basic structural check; detailed validation in H3.1.
        *   `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`:
            *   `read_file` (to check existence).
            *   If it does NOT exist (typical for a truly new iteration number):
                *   Use `write_to_file` to create `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md` with initial content: `# Task Execution Log - Iteration <CurrentIterNum>\n---\n`.
                *   Log (Type: `SystemActivity`, Directive: "Created `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`."). Append this meta-log to the newly created file.
            *   Else (file exists, resuming iteration): Log (Type: `SystemActivity`, Directive: "Appending to existing `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`.").
        *   (All `META_LOG_DIRECTIVE`s from -1.0, -1.1, -1.2 MUST be appended to `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md` once it's available).
    -1.2.C: Sequentially `read_file` all *other* relevant RMP sub-files for `<CurrentIterNum>` if they exist (e.g., Plan, Header, Footer, LogAnalysis, Reflection, Scoring, STM from a previous session of the *same* `<CurrentIterNum>` if resuming). This also includes reading all parts of RMP Section 3 (Core Prompt Hub and its linked A, B, C1, C2, D files).

-1.3: Initial State Identification:
    *   Confirm "Current User-Defined Iteration Objective & Priority" for `<CurrentIterNum>` (from file created/updated in -1.0.5).
    *   Note current state of "User Task Plan", "Failure Tracking", "Escalation", "STM" for `<CurrentIterNum>` if resuming, or acknowledge they are new/empty if a fresh iteration number.

-1.4: Proceed based on Escalation Status:
    *   `read_file` `RMP_Modules/RMP_Section7_Escalation.md`.
    *   If Escalation is active, await user input.
    *   If Escalation is not active, proceed to `Workflow Step 3.0: INITIAL ITERATION COMMIT`.
Workflow Step 3.0: INITIAL ITERATION COMMIT:
Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Intent, Context: "INITIAL ITERATION COMMIT", Directive: "Preparing for initial iteration commit after loading RMP and identifying objective."
(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).
Output the PlanStepExecution_Intent META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append the full, formatted META_LOG_DIRECTIVE string.
Determine current Iteration Number (e.g., from RMP Section 2 sub-file).
Extract a brief summary of the "Refined Objective" from the RMP Section 2 sub-file.
Use execute_command with command: git add . (Stages all current files, including new/updated RMP files).
Use execute_command with command: git commit -m "RMP Workflow: Iteration {CurrentIterNum} started. Objective: {ObjectiveSummary}". (Ensure placeholders are dynamically filled).
Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Outcome, Content: "Initial iteration commit completed." (If commit fails, log error details here).
(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).
Output the PlanStepExecution_Outcome META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append the full, formatted META_LOG_DIRECTIVE string.
Proceed to Workflow Step 3.1: PLAN ITERATION.
Workflow Step 3.1: PLAN ITERATION (Adhering to "Iteration Task Checklists Protocol"):
1. Populate and Process Task Header:
Formulate META_LOG_DIRECTIVE Type: SystemActivity, Context: "PLAN ITERATION - Task Header Setup", Directive: "Initiating Task Header processing. Reading seed checklist from system-prompt-Logger."
Output META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append the full, formatted META_LOG_DIRECTIVE string.
Use read_file to get the "Seed Task Header Checklist" content from system-prompt-Logger.
Use write_to_file to create/overwrite RMP_Modules/RMP_Task_Header_Iter<CurrentIterNum>.md with this seed content. (This RMP file modification is subject to RMP-MP).
Formulate META_LOG_DIRECTIVE Type: SystemActivity, Context: "PLAN ITERATION - Task Header Setup", Directive: "Populated RMP_Task_Header_Iter<CurrentIterNum>.md. Proceeding to process checklist items."
Output META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append the full, formatted META_LOG_DIRECTIVE string.
Meticulously execute and verify each item (H1-H5) in RMP_Task_Header_Iter<CurrentIterNum>.md using an internal checklist approach. For each item:
Formulate META_LOG_DIRECTIVEs detailing intent, action(s) for verification, and outcome.
Output each META_LOG_DIRECTIVE. Then, for each, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append the full, formatted META_LOG_DIRECTIVE string.
Update its status, start/end times, and notes/verification details directly in the RMP_Task_Header_Iter<CurrentIterNum>.md file (using apply_diff or write_to_file for the whole file, subject to RMP-MP).
For H3.1 (ProjectStructure.md Status): Verification MUST confirm ProjectStructure.md exists and meets all content requirements from system-prompt-Logger (PSM-P Point 2). If not, its creation/update becomes a prerequisite.
For H3.2 (Logging System Filter Active): To locate Core.Common.Extensions.TypedLoggerExtensions.cs and Core.Common.Extensions.LogFilterState.cs (expected in Core.Common project), FIRST consult ProjectStructure.md (PSM-P Step 4.2). If not found/verified, THEN analyze Core.Common/Core.Common.csproj (PSM-P Step 4.3). Only if both fail, use search_files as a last resort within Core.Common/. If files are missing, their implementation is a MANDATORY PREREQUISITE.
(Processing the Task Header MAY be executed as a dedicated sub-task following the Hierarchical Task Management Protocol if its complexity warrants it.)
CRITICAL: Confirm all Task Header items are "Completed" (or "Skipped/NA" with justification). Any "Failed" items MUST be resolved or escalated. Prerequisites identified MUST inform the User Task Plan.
Formulate META_LOG_DIRECTIVE Type: SystemActivity, Context: "PLAN ITERATION - Task Header Completion", Directive: "All items in RMP_Task_Header_Iter<CurrentIterNum>.md processed and verified. Findings will inform User Task Plan."
Output META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append the full, formatted META_LOG_DIRECTIVE string.
2. Review Core Inputs for User Task Plan: (Review User Objective, previous Reflection/Failures, Task Header insights).
3. Formulate User Task Plan (RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md - as per "Iteration Task Checklists Protocol" Point 2 in system-prompt-Logger):
a. Initial Brainstorming & Analysis: Formulate a META_LOG_DIRECTIVE summarizing brainstorming, strategy, roadblocks, alternatives. Output META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append.
b. Detailed Plan Formulation: Create step-by-step plan in RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md. Adhere to Content Mandates (PSM-P, new_task for complex phases as per Hierarchical Task Management Protocol, etc.). Each conceptual part of this plan formulation triggers Reactive STM/LTM Consultation, with associated META_LOG_DIRECTIVEs appended to Task Execution Log via insert_content (path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n").
4. Pre-Execution Validation of User Task Plan (CRITICAL - as per "Iteration Task Checklists Protocol" Point 2.C in system-prompt-Logger):
Perform a validation review of RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md against all criteria.
Formulate META_LOG_DIRECTIVE Type: SystemActivity, Context: "PLAN ITERATION - User Task Plan Validation", Directive: "User Task Plan (RMP_UserTask_Plan_Iter<CurrentIterNum>.md) formulated and validated successfully." (or "User Task Plan refined and validated.")
Output META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append.
5. Proceed: Proceed to Workflow Step 3.2: EXECUTE PLAN.
Workflow Step 3.2: EXECUTE PLAN:
Execute steps from "Your Plan" (RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md) sequentially.
For each step:
Use appropriate tool(s). Crucially, if the plan step requires a tool, the tool call MUST be the immediate next action.
Before modifying existing files (code or RMP), ALWAYS read_file (unless creating a new file with write_to_file).
Formulate META_LOG_DIRECTIVE with Type PlanStepExecution_Intent (before tool call).
Execute Reactive STM/LTM Consultation Sub-Workflow.
Output the PlanStepExecution_Intent META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append.
Execute the tool.
Analyze the result.
Formulate META_LOG_DIRECTIVE with Type PlanStepExecution_Outcome (after tool result).
Execute Reactive STM/LTM Consultation Sub-Workflow.
Output the PlanStepExecution_Outcome META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append.
If step completes successfully, mark as "Completed" in RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md (using apply_diff or write_to_file, subject to RMP-MP).
If step fails, immediately proceed to Workflow Step 3.3: HANDLE EXECUTION FAILURE.
(If a plan step involves initiating a sub-task via new_task as per Hierarchical Task Management Protocol, follow that protocol for initiating and later verifying the sub-task.)
Workflow Step 3.3: HANDLE EXECUTION FAILURE:
1. Initial Failure Logging & Documentation:
Formulate META_LOG_DIRECTIVE Type: SystemActivity, Context: "HANDLE EXECUTION FAILURE - Analysis Start", Directive: "Execution of previous plan step failed. Analyzing failure result."
Output META_LOG_DIRECTIVE. Then, MUST use insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n" to append.
Carefully analyze the complete failure result (e.g., compiler error messages and codes, test runner output with stack traces, tool error messages, unexpected file content).
Document the failure comprehensively in "Failure Tracking" (RMP_Modules/RMP_Section8_FailureTracking.md), including type, description, detailed error output, and initial diagnosis. (This RMP file modification is subject to RMP-MP and involves its own meta-logging. Any such META_LOG_DIRECTIVE must also be appended to RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md using insert_content, path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n").
2. Specific Failure Type Triage & Initial Response:
a. If "File Not Found" for an expected source code file (e.g., during read_file):
Invoke Project Structure Management Protocol (PSM-P) as the PRIMARY method: Follow steps in system-prompt-Logger (PSM-P section) to locate the file using project files (.csproj, .sln) and update/create ProjectStructure.md. PSM-P must be attempted before resorting to general directory searches for source code files.
Log PSM-P actions with META_LOG_DIRECTIVEs (each appended to RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md using insert_content, path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n").
If PSM-P successfully locates the file: Formulate a sub-plan to re-attempt the original action. Update User Task Plan and proceed to Workflow Step 3.2 EXECUTE PLAN.
If PSM-P confirms the file genuinely does not exist as part of the project structure: Note this. Proceed to general diagnosis (Point 3 below).
b. If Build Error indicating Missing/Undefined Type, Namespace, or Member (e.g., CS0246, CS0103, CS0117):
Invoke Project Structure Management Protocol (PSM-P) for Verification:
Identify the missing entity and the file/project.
Consult ProjectStructure.md (PSM-P Step 4.2).
If not found/verified, analyze relevant .csproj and .sln files (PSM-P Step 4.3).
If a file path is found/verified, analyze the source code file (PSM-P Step 4.5).
Update ProjectStructure.md (PSM-P Steps 4.4 & 4.6).
Log PSM-P actions with META_LOG_DIRECTIVEs (each appended to RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md using insert_content, path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n").
If PSM-P reveals the cause: Formulate a sub-plan to correct the issue. Update User Task Plan and proceed to Workflow Step 3.2 EXECUTE PLAN.
If PSM-P confirms the entity is genuinely undefined: Proceed to general diagnosis (Point 3 below).
c. For Other Failures (Test Assertion Failures, Unexpected Tool Behavior, Runtime Exceptions not covered above, etc.):
Proceed directly to general diagnosis (Point 3 below).
3. General Failure Diagnosis & Investigation:
a. Logging System Check & Adjustment (If detailed logs are needed for diagnosis):
Verify filter implementation. Log verification (appended via insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n").
Adjust LogFilterState if active. Log changes made (appended via insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n"). Remember to reset LogFilterState later.
If re-run needed for detailed logs, add to sub-plan.
b. Deliberate STM/LTM Review for Broader Insights:
Identify keywords from failure. Scan STM All_Tags.
If matches: Log DeliberateSTM_Consultation_Failure (appended via insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n"), construct LTM path(s), read_file LTM(s), analyze, incorporate insights.
Fallback LTM Check: If no STM match, attempt direct LTM construction. Log as LTM_DirectConstruct_Consultation_Failure (appended via insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n").
c. Analyze available information: Error messages, stack traces, logs, STM/LTM insights, source code.
4. Sub-Plan Formulation to Address Failure:
Based on analysis, formulate a precise sub-plan.
This sub-plan formulation triggers Reactive STM/LTM Consultation (associated META_LOG_DIRECTIVEs appended via insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n").
The sub-plan may involve code changes, config changes, or further investigation.
5. Update User Task Plan & Proceed:
Integrate the new sub-plan into RMP_Modules/RMP_UserTask_Plan_Iter<CurrentIterNum>.md.
Proceed back to Workflow Step 3.2 EXECUTE PLAN.
6. Persistent Failure Check (Loop Control):
If this specific failure (or failure to achieve a particular sub-goal related to it) persists after three distinct, well-reasoned, and documented solution attempts (each attempt consisting of diagnosis, sub-plan, and execution), then proceed to Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL (defined in RMP_Modules/RMP_Section3_CorePrompt_C2_Workflow_Build_History_Reflect_End.md).