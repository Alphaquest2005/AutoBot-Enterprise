### 3.C2 Workflow Steps: Build/Test Loop, Task History, Reflection, and Completion

*   **Workflow Step 3.4: BUILD/TEST/FIX LOOP:**
    *   **(All `META_LOG_DIRECTIVE`s in this step must be appended to `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md` immediately after being outputted to the primary interaction stream.)**
    *   **1. Execute Build:**
        *   Use `execute_command` with the standard build command (defined in `RMP_Modules/RMP_Section3_CorePrompt_D_RulesInstructions.md`).
    *   **2. Process Build Result & Conduct Mini-Review (if build fails):**
        *   **a. Log Build Outcome:**
            *   If build succeeds: **Formulate `META_LOG_DIRECTIVE`** Type: `BuildExecution_Outcome`, Context: "BUILD/TEST/FIX LOOP", Directive: "Build Succeeded."
            *   If build fails: **Formulate `META_LOG_DIRECTIVE`** Type: `BuildExecution_Outcome`, Context: "BUILD/TEST/FIX LOOP", Directive: "Build Failed. Details: [Copy of build error summary from tool output]."
            *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
        *   **b. If Build Failed, Execute Post-Build Mini-Review Protocol (as defined in `system-prompt-Logger`):**
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "Post-Build Mini-Review - Start", Directive: "Initiating Post-Build Mini-Review after FAILED build."
            *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
            *   Perform PBMR1: Review Recent Task Execution Log (focus on actions leading to build failure).
            *   Perform PBMR2: Strategic Plan & Goal Alignment Review (assess how failure impacts plan, complexity, efficiency, alternatives, timelines).
            *   Perform PBMR3: Checklist Sanity Check (Header/Footer).
            *   Perform PBMR4: Context Window Awareness (Qualitative Check).
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `PostBuildMiniReview_Summary`, Content: "[Concise summary of findings, e.g., 'Build failed due to [X], plan needs adjustment to fix [Y]. Insights from recent logs: Z.']"
            *   *Output META_LOG_DIRECTIVE `PostBuildMiniReview_Summary`.* **Append to Task Execution Log.**
        *   **c. Decide Next Step based on Build Status:**
            *   **If build succeeded:**
                *   **i. Post-Successful-Build Git Commit:**
                    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Context: "BUILD/TEST/FIX LOOP", Directive: "Build successful. Preparing post-build commit."
                    *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
                    *   Determine current Iteration Number. Identify preceding plan step/action for commit message.
                    *   Use `execute_command` with `command: git add .`
                    *   Use `execute_command` with `command: git commit -m "RMP Workflow: Iteration {CurrentIterNum} - Successful build after action: {DescriptionOfPrecedingAction}"`.
                    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Context: "BUILD/TEST/FIX LOOP", Directive: "Post-build commit completed." (Log errors if commit fails).
                    *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
                *   **ii. Execute Tests:**
                    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Context: "BUILD/TEST/FIX LOOP", Directive: "Proceeding to test execution."
                    *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
                    *   Execute test command(s) relevant to the current objective (as defined in Critical Instructions or User Task Plan).
                    *   Analyze test results.
                    *   **Formulate `META_LOG_DIRECTIVE`** Type: `TestExecution_Outcome`, Context: "BUILD/TEST/FIX LOOP", Directive: "Test execution complete. Result: [Pass/Fail]. Details: [Summary of test output/failures]."
                    *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
                    *   If tests pass AND the functional objective is met (or a suitable reflection point is reached): Proceed to **Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY**.
                    *   If tests fail: Proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE** (defined in `RMP_Modules/RMP_Section3_CorePrompt_C1_Workflow_Init_Plan_Exec.md`).
            *   **If build failed:**
                *   Proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE** (The insights from the Mini-Review will inform the diagnosis and sub-plan formulated in Step 3.3).

*   **Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY:**
    *   **(All `META_LOG_DIRECTIVE`s in this step must be appended to `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`.)**
    *   **Trigger:** Called after main execution/fixing phase of an iteration (e.g., from successful BUILD/TEST/FIX LOOP completion) or when explicitly decided for reflection.
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemState`, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "Execution phase concluded for current Iteration. Preparing for reflection. Requesting task history."
    *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
    *   Determine the current iteration number (e.g., from RMP Section 2 sub-file). Let this be `CurrentIterNum`.
    *   Use the `ask_followup_question` tool with:
        *   `question`: "The active execution phase for Iteration `CurrentIterNum` is complete. To proceed with comprehensive self-reflection and learning (Contemplation Phase), please export the full task history for this iteration (containing all `META_LOG_DIRECTIVE`s, code interactions, and tool outputs) and save it as `task_history_iter<CurrentIterNum>.txt` in the root of the workspace (`c:/Insight Software/AutoBot-Enterprise`). I will pause and wait for your confirmation."
        *   `follow_up`:
            1.  "The task history file `task_history_iter<CurrentIterNum>.txt` is now available in the workspace. Proceed with reflection."
            2.  "I am unable to provide the task history file `task_history_iter<CurrentIterNum>.txt` at this time. Proceed with reflection using available data."
    *   **Wait for user response.**
    *   **Process User Response:**
        *   If user confirms file availability (response option 1):
            *   Construct the filename `task_history_iter<CurrentIterNum>.txt`.
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "User confirmed `task_history_iter<CurrentIterNum>.txt` is available. Attempting to read."
            *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
            *   Use `read_file` with `path: task_history_iter<CurrentIterNum>.txt`.
            *   If successful:
                *   Store the retrieved content internally for use in reflection.
                *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "Successfully read `task_history_iter<CurrentIterNum>.txt`. Content captured for reflection."
                *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
            *   If `read_file` fails:
                *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "Failed to read `task_history_iter<CurrentIterNum>.txt`. Error: [Tool Error Message]. Proceeding with reflection using other available data."
                *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
                *   Set a flag internally indicating task history is not available/readable.
        *   If user indicates file is not available (response option 2):
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemState`, Context: "REQUEST & RETRIEVE TASK HISTORY", Directive: "User indicated task history file `task_history_iter<CurrentIterNum>.txt` is not available. Proceeding with reflection using other available data."
            *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
            *   Set a flag internally indicating task history is not available.
    *   Proceed to **Workflow Step 3.5: REFLECT & SELF-IMPROVE**.

*   **Workflow Step 3.1.5: INITIATE ESCALATE PROTOCOL:**
    *   **(All `META_LOG_DIRECTIVE`s in this step must be appended to `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`.)**
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "Escalation", Directive: "Initiating Escalation Protocol due to persistent failure after 3 distinct attempts."
    *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
    *   Update `RMP_Modules/RMP_Section7_Escalation.md` (subject to RMP-MP) with:
        *   `Status: Active`
        *   `Details:` A comprehensive summary of the blocking issue, the User Objective, the three distinct failed solution attempts (referencing failure IDs from `RMP_Section8_FailureTracking.md` and relevant plan steps/LTM consultations from `RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`), and why self-resolution is not proceeding.
    *   Determine `CurrentIterNum`. Extract brief objective snippet.
    *   Use `ask_followup_question` with:
        *   `question`: "Escalation Protocol Initiated for Iteration `CurrentIterNum`. I am blocked on achieving [brief objective snippet] due to [brief reason for blockage]. I have made 3 distinct attempts logged in RMP Section 8 and detailed in the Task Execution Log. Please review RMP files (Sections 2, 4, 5, 7, 8 and Task Execution Log) and provide guidance or a revised approach. I will pause awaiting your input."
        *   `follow_up`: ["Understood, I will review and provide input.", "Proceed with one more specific attempt: [User-defined strategy, if any, else this option implies more user thought needed]"]
    *   Await user input. Do not proceed with plan execution until guidance is received or escalation is resolved by user input.

*   **Workflow Step 3.5: REFLECT & SELF-IMPROVE:**
    *   **(All `META_LOG_DIRECTIVE`s in this step must be appended to `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`.)**
    *   **1. Process Task Footer (as per "Iteration Task Checklists Protocol" in `system-prompt-Logger`):**
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "REFLECT & SELF-IMPROVE - Task Footer Setup", Directive: "Initiating Task Footer processing. Reading seed checklist from system-prompt-Logger."
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
        *   Use `read_file` to get the "Seed Task Footer Checklist" content from `system-prompt-Logger`.
        *   Use `write_to_file` to create/overwrite `RMP_Modules/RMP_Task_Footer_Iter<CurrentIterNum>.md` with this seed content. (Subject to RMP-MP).
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "REFLECT & SELF-IMPROVE - Task Footer Setup", Directive: "Populated RMP_Task_Footer_Iter<CurrentIterNum>.md. Proceeding to process checklist items."
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
        *   Meticulously execute and verify each item (F1-F5) in `RMP_Task_Footer_Iter<CurrentIterNum>.md`. For each item:
            *   Log intent, action(s), and outcome using `META_LOG_DIRECTIVE`s (appended to Task Execution Log).
            *   Update its status, times, and notes in `RMP_Task_Footer_Iter<CurrentIterNum>.md` (subject to RMP-MP).
        *   **CRITICAL:** Confirm all Task Footer items are "Completed" (or "Skipped/NA" with justification). This includes F3.2 (Review Emergent Insights from Task Execution Log).
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "REFLECT & SELF-IMPROVE - Task Footer Completion", Directive: "All items in RMP_Task_Footer_Iter<CurrentIterNum>.md processed and verified."
        *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
    *   **2. Comprehensive Iteration Data Review & Analysis:**
        *   **a. Primary Data Sources:** Review `RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`, `RMP_UserTask_Plan_Iter<CurrentIterNum>.md`, `RMP_Section8_FailureTracking.md`, `RMP_Section5_LogAnalysis_Iter<CurrentIterNum>.md`, `RMP_Task_Header_Iter<CurrentIterNum>.md`, and `RMP_Task_Footer_Iter<CurrentIterNum>.md`.
        *   **b. User-Provided Task History Processing (if `task_history_iter<CurrentIterNum>.txt` was retrieved and content is available):**
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "Reflection - User Task History Processing", Directive: "Processing user-provided task_history_iter<CurrentIterNum>.txt."
            *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
            *   Scan the `task_history_iter<CurrentIterNum>.txt` specifically for: Tool Calls & Outputs (compare with Task Execution Log), User Feedback/Instructions, detailed Error Messages/Stack Traces, File Content Previews.
            *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemActivity`, Context: "Reflection - User Task History Processing", Directive: "User-provided task history processed. Key insights/discrepancies noted: [brief summary, or 'None']."
            *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
        *   **c. Synthesize Overall Iteration Understanding:** Combine insights from all reviewed data.
    *   **3. Update Long-Term (LTM) and Short-Term (STM) Memory (Append-Only, Immutable, Linked):**
        *   Based on review, identify new learnings, successful problem resolutions, or persistent issues.
        *   For each significant learning:
            *   Formulate content for a new LTM file.
            *   Create a corresponding STM entry (with fields: `STM_ID`, `Primary_Topic_Or_Error`, `Key_Concepts`, `Outcome_Indicator_Short`, `Distinguisher_Source`, `LTM_File_Path`, `All_Tags`, and optionally `Clue`, `Associated_LTM_File_Content_Snippet`, `Cross_References`).
            *   Use `write_to_file` to save the new LTM file to the `LTM/` directory, following the LTM Filename Convention.
            *   Use `insert_content` to append the new STM entry to the RMP Section 10 sub-file (e.g., `RMP_Modules/RMP_Section10_STM_IterX.md`).
            * (All meta-logged to Task Execution Log).
    *   **4. Analyze & Propose Prompt Improvements:**
        *   Analyze effectiveness of "Core Instrumentation Prompt" (RMP Section 3 sub-files) and all System Prompt protocols.
        *   Identify areas for improvement. Propose concrete changes in `RMP_Modules/RMP_Section6_Reflection_Iter<CurrentIterNum>.md` (Section 6.5).
    *   **5. Document Reflection:** Complete `RMP_Modules/RMP_Section6_Reflection_Iter<CurrentIterNum>.md`.
    *   **6. Update Core Prompt (Subject to RMP-MP):** Apply approved changes from RMP Section 6.5 to RMP Section 3 sub-files.
    *   **7. Update Scoring:** Complete `RMP_Modules/RMP_Section9_Scoring_Iter<CurrentIterNum>.md`.
    *   **8. Proceed:** Proceed to **Workflow Step 3.6: ATTEMPT COMPLETION**.

*   **Workflow Step 3.6: ATTEMPT COMPLETION:**
    *   **(All `META_LOG_DIRECTIVE`s in this step must be appended to `RMP_Modules/RMP_TaskExecutionLog_Iter<CurrentIterNum>.md`.)**
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Context: "ATTEMPT COMPLETION", Directive: "Evaluating if User Objective is met for potential completion. Task Footer checklist must be completed."
    *   *Output META_LOG_DIRECTIVE.* **Append to Task Execution Log.**
    *   If "Current User-Defined Iteration Objective" (RMP Section 2 sub-file) is fully achieved and verified, AND all items in `RMP_Task_Footer_Iter<CurrentIterNum>.md` are "Completed":
        *   Use `attempt_completion` tool.
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Context: "ATTEMPT COMPLETION", Directive: "Objective achieved. Completion attempted."
    *   Else (not fully achieved or Task Footer incomplete):
        *   Update RMP Section 2 and `RMP_Modules/RMP_UserTask_Plan_Iter<NextIterNum>.md` (creating it for the next iteration).
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Context: "ATTEMPT COMPLETION", Directive: "Objective not fully achieved or prerequisites for completion not met. Preparing for next iteration/cycle."
        *   Proceed to **Workflow Step 3.1: PLAN ITERATION** (defined in `RMP_Modules/RMP_Section3_CorePrompt_C1_Workflow_Init_Plan_Exec.md`).
    *   *Output the chosen `PlanStepExecution_Outcome` `META_LOG_DIRECTIVE`.* **Append to Task Execution Log.**