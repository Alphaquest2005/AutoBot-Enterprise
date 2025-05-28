3.C2 Workflow Steps: Build/Test Loop, Task History, Reflection, and Completion
Workflow Step 3.4: BUILD/TEST/FIX LOOP:
(All META_LOG_DIRECTIVEs in this step MUST be appended to RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md using insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n", immediately after being outputted to the primary interaction stream.)
1. Execute Build:
Use execute_command with the standard build command (defined in RMP_Modules/RMP_Section3_CorePrompt_D_RulesInstructions.md).
2. Process Build Result & Conduct Mini-Review (if build fails):
a. Log Build Outcome:
If build succeeds: Formulate META_LOG_DIRECTIVE Type: BuildExecution_Outcome, Context: "BUILD/TEST/FIX LOOP", Directive: "Build Succeeded."
If build fails: Formulate META_LOG_DIRECTIVE Type: BuildExecution_Outcome, Context: "BUILD/TEST/FIX LOOP", Directive: "Build Failed. Details: [Copy of build error summary from tool output]."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
b. If Build Failed, Execute Post-Build Mini-Review Protocol (as defined in system-prompt-Logger):
Formulate META_LOG_DIRECTIVE Type: SystemActivity, Context: "Post-Build Mini-Review - Start", Directive: "Initiating Post-Build Mini-Review after FAILED build."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Perform PBMR1: Review Recent Task Execution Log.
Perform PBMR2: Strategic Plan & Goal Alignment Review.
Perform PBMR3: Checklist Sanity Check (Header/Footer).
Perform PBMR4: Context Window Awareness.
Formulate META_LOG_DIRECTIVE Type: PostBuildMiniReview_Summary, Content: "[Concise summary of findings...]"
Output META_LOG_DIRECTIVE PostBuildMiniReview_Summary. Append to Task Execution Log.
c. Decide Next Step based on Build Status:
If build succeeded:
i. Post-Successful-Build Git Commit:
Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Intent, Context: "BUILD/TEST/FIX LOOP", Directive: "Build successful. Preparing post-build commit."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Determine current Iteration Number. Identify preceding plan step/action for commit message.
Use execute_command with command: git add .
Use execute_command with command: git commit -m "RMP Workflow: Iteration {CurrentIterNum} - Successful build after action: {DescriptionOfPrecedingAction}".
Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Outcome, Context: "BUILD/TEST/FIX LOOP", Directive: "Post-build commit completed." (Log errors if commit fails).
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
ii. Execute Tests:
Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Intent, Context: "BUILD/TEST/FIX LOOP", Directive: "Proceeding to test execution."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Execute test command(s) relevant to the current objective.
Analyze test results.
Formulate META_LOG_DIRECTIVE Type: TestExecution_Outcome, Context: "BUILD/TEST/FIX LOOP", Directive: "Test execution complete. Result: [Pass/Fail]. Details: [Summary]."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
If tests pass AND the functional objective is met: Proceed to Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY.
If tests fail: Proceed to Workflow Step 3.3: HANDLE EXECUTION FAILURE.
If build failed:
Proceed to Workflow Step 3.3: HANDLE EXECUTION FAILURE.
Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY:
(All META_LOG_DIRECTIVEs appended to Task Execution Log using insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n".)
Trigger: Called after main execution/fixing phase or when explicitly decided for reflection.
Formulate META_LOG_DIRECTIVE Type: SystemState, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "Execution phase concluded. Requesting task history."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Determine CurrentIterNum. Use ask_followup_question for task_history_iter<CurrentIterNum>.txt.
Wait for user response.
Process User Response:
If user confirms file availability:
Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Intent, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "User confirmed task_history_iter<CurrentIterNum>.txt is available. Attempting to read."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Use read_file with path: task_history_iter<CurrentIterNum>.txt.
If successful:
Store content. Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Outcome, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "Successfully read task_history_iter<CurrentIterNum>.txt."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
If read_file fails:
Formulate META_LOG_DIRECTIVE Type: PlanStepExecution_Outcome, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "Failed to read task_history_iter<CurrentIterNum>.txt. Error: [Tool Error Message]."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Set internal flag.
If user indicates file is not available:
Formulate META_LOG_DIRECTIVE Type: SystemState, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "User indicated task history file not available."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Set internal flag.
Proceed to Workflow Step 3.5: REFLECT & SELF-IMPROVE.
Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL:
(All META_LOG_DIRECTIVEs appended to Task Execution Log using insert_content with path: RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n".)
Formulate META_LOG_DIRECTIVE Type: SystemActivity, Context: "Escalation", Directive: "Initiating Escalation Protocol due to persistent failure after 3 distinct attempts."
Output META_LOG_DIRECTIVE. Append to Task Execution Log.
Update RMP_Modules/RMP_Section7_Escalation.md (subject to RMP-MP).
Determine CurrentIterNum. Use ask_followup_question detailing blockage.
Await user input. Do not proceed until guidance is received or escalation resolved.
Workflow Step 3.5: REFLECT & SELF-IMPROVE:
(All META_LOG_DIRECTIVEs appended to Task Execution Log using `insert_content` with path: `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n".)
0.  **Update Iteration Status to 'Reflecting':**
    *   `read_file` `RMP_Modules/RMP_Iteration_Status.md`.
    *   Modify the entry for `<CurrentIterNum>` to set `Status: Reflecting`.
    *   `write_to_file` or `apply_diff` the changes.
    *   Log this status change (Type: `ILMP_StatusUpdate`, Directive: "Iteration `<CurrentIterNum>` status set to Reflecting."). Append to Task Execution Log.
1.  **Process Task Footer (Items F1-F5: Objective Completion Focus):**
    *   (As per "Iteration Task Checklists Protocol" in `system-prompt-Logger`, using internal checklist approach).
    *   Formulate META_LOG_DIRECTIVE Type: `SystemActivity`, Context: "REFLECT & SELF-IMPROVE - Task Footer (F1-F5) Processing", Directive: "Initiating Task Footer processing for items F1-F5. Reading seed checklist." Output & Append.
    *   `read_file` for "Seed Task Footer Checklist" from `system-prompt-Logger`.
    *   `write_to_file` to create/overwrite `RMP_Modules/RMP_Task_Footer_Iter<CurrentIterNum>.md` with seed. (Subject to RMP-MP).
    *   Formulate META_LOG_DIRECTIVE Type: `SystemActivity`, Context: "REFLECT & SELF-IMPROVE - Task Footer Setup", Directive: "Populated `RMP_Task_Footer_Iter<CurrentIterNum>.md`. Processing items F1-F5." Output & Append.
    *   Meticulously execute and verify each item **(F1 through F5 only)** in `RMP_Modules/RMP_Task_Footer_Iter<CurrentIterNum>.md`. For each item:
        *   Log intent, action(s), and outcome using `META_LOG_DIRECTIVE`s. Output each. Append each.
        *   Update its status, times, and notes in `RMP_Modules/RMP_Task_Footer_Iter<CurrentIterNum>.md` (subject to RMP-MP).
    *   **CRITICAL:** Confirm all Task Footer items **F1 through F5** are "Completed" (or "Skipped/NA"). This includes F2.3 (`ProjectStructure.md` update) and F3.2 (Emergent Insights review).
    *   Formulate META_LOG_DIRECTIVE Type: `SystemActivity`, Context: "REFLECT & SELF-IMPROVE - Task Footer (F1-F5) Completion", Directive: "All items F1-F5 in `RMP_Task_Footer_Iter<CurrentIterNum>.md` processed." Output & Append.
2.  **Comprehensive Iteration Data Review & Analysis:** (As before)
    *   a. Primary Data Sources: Review RMP files (Task Exec Log, Plan, Failure Tracking, Log Analysis, Header, Footer).
    *   b. User-Provided Task History Processing (if content available and retrieved in Step F5 of footer).
    *   c. Synthesize Overall Iteration Understanding.
3.  **Update Long-Term (LTM) and Short-Term (STM) Memory:** (As before)
    *   Based on review, identify new learnings. Formulate LTM, create STM entry. `write_to_file` LTM, `insert_content` STM. Log.
4.  **Initiate Iteration Finalization Protocol (IFP) (ILMP.4):**
    *   Formulate `META_LOG_DIRECTIVE` (Type: `SystemActivity`, Context: "REFLECT & SELF-IMPROVE - IFP Start", Directive: "Initiating Iteration Finalization Protocol (IFP) for Iteration `<CurrentIterNum>` as a sub-task."). Output & Append.
    *   **Execute IFP as a sub-task:**
        *   Prepare Standard Context Package (including all relevant RMP paths for `<CurrentIterNum>`, `system-prompt-Logger` ref, User Objective for `<CurrentIterNum>`, InvocationId).
        *   Sub-task Objective: "Finalize Iteration `<CurrentIterNum>` by meticulously executing all steps of the Iteration Finalization Protocol (IFP) as defined in `system-prompt-Logger` (ILMP.4)."
        *   Call `new_task` tool with `mode: logger` and the context package + objective.
    *   **Await and Verify IFP Sub-Task Completion:**
        *   After sub-task completes, retrieve its `SubTask_Completion_Summary` from `RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`.
        *   Verify outcome. If IFP sub-task failed, log error (Type: `IFP_Execution_Failed`), HALT Workflow Step 3.5 here. The iteration remains in "Reflecting" or "Finalizing" (if IFP sub-task updated it before failing). User intervention or re-planning for `<CurrentIterNum>` is required.
        *   If IFP sub-task succeeded (iteration status is now "Finalized"): Log (Type: `IFP_Execution_Success`, Directive: "IFP sub-task for Iteration `<CurrentIterNum>` completed successfully. Iteration is Finalized.").
5.  **Analyze & Propose Prompt Improvements (RMP Section 6.5):** (As before, in `RMP_Modules/RMP_Section6_Reflection_Iter<CurrentIterNum>.md`)
6.  **Document Reflection (RMP Section 6):** (As before, complete `RMP_Modules/RMP_Section6_Reflection_Iter<CurrentIterNum>.md`)
7.  **Update Core Prompt (RMP Section 3 - Subject to RMP-MP):** (As before, apply approved changes from RMP Section 6.5)
8.  **Update Scoring (RMP Section 9):** (As before, complete `RMP_Modules/RMP_Section9_Scoring_Iter<CurrentIterNum>.md`)
9.  **Proceed:** Proceed to `Workflow Step 3.6: ATTEMPT COMPLETION`.

Workflow Step 3.6: ATTEMPT COMPLETION:
(All `META_LOG_DIRECTIVE`s appended to Task Execution Log using `insert_content` with path: `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`, line: 0, and content: "META_LOG_DIRECTIVE: {{...actual full JSON...}}\n---\n".)
Formulate `META_LOG_DIRECTIVE` Type: `PlanStepExecution_Intent`, Context: "ATTEMPT COMPLETION - Iter`<CurrentIterNum>`", Directive: "Evaluating if User Objective was met and Iteration is Finalized." Output & Append.
`read_file` `RMP_Modules/RMP_Iteration_Status.md`. Check status for `<CurrentIterNum>`.
If "Current User-Defined Iteration Objective" for `<CurrentIterNum>` is deemed fully achieved AND the status of `<CurrentIterNum>` in `RMP_Modules/RMP_Iteration_Status.md` is "Finalized":
    Use `attempt_completion` tool with a result summary for `<CurrentIterNum>`.
    Formulate `META_LOG_DIRECTIVE` Type: `PlanStepExecution_Outcome`, Context: "ATTEMPT COMPLETION - Iter`<CurrentIterNum>`", Directive: "Objective achieved for Iteration `<CurrentIterNum>` and iteration is Finalized. Completion attempted. System ready for new objective for Iteration `<CurrentIterNum + 1>`."
Else (objective not fully achieved OR Iteration `<CurrentIterNum>` is not "Finalized"):
    Formulate `META_LOG_DIRECTIVE` Type: `PlanStepExecution_Outcome`, Context: "ATTEMPT COMPLETION - Iter`<CurrentIterNum>`", Directive: "Objective not fully achieved or Iteration `<CurrentIterNum>` not Finalized (Status: [CurrentStatus]). Cannot attempt completion. Iteration `<CurrentIterNum>` may require further work or finalization steps."
    (If not Finalized, this implies an issue in IFP or preceding steps. The system should ideally not reach here if IFP failed, as 3.5 would have halted. This 'else' handles objective not met, or unexpected state).
    Proceed to `Workflow Step 3.1: PLAN ITERATION` (to re-plan for the *same* `<CurrentIterNum>` or address finalization issues).
Output the chosen `PlanStepExecution_Outcome` `META_LOG_DIRECTIVE`. Append to Task Execution Log.