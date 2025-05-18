### 3.C2 Workflow Steps: Build/Test Loop, Task History, Reflection, and Completion

*   **Workflow Step 3.4: BUILD/TEST/FIX LOOP:**
    *   (Build/test execution and analysis will generate `META_LOG_DIRECTIVE`s, thus triggering reactive LTM consultation).
    *   Execute build command.
*   If build succeeds:
        *   Log success.
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Intent`, Content: "Build successful. Preparing post-build commit."
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
        *   *Output the `PlanStepExecution_Intent` `META_LOG_DIRECTIVE`.*
        *   Determine current Iteration Number (e.g., from RMP Section 2 sub-file).
        *   Identify the preceding plan step or action that led to this build (e.g., from RMP Section 4 sub-file or last completed step; if unclear, use a generic "post-code-modification").
        *   Use `execute_command` with `command: git add .` *(This stages all changes made that led to the successful build)*.
        *   Use `execute_command` with `command: git commit -m "RMP Workflow: Iteration {CurrentIterNum} - Successful build after action: {DescriptionOfPrecedingAction}"`. *(Ensure placeholders are dynamically filled)*.
        *   **Formulate `META_LOG_DIRECTIVE`** Type: `PlanStepExecution_Outcome`, Content: "Post-build commit completed." (If commit fails, log error details here).
            *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
        *   *Output the `PlanStepExecution_Outcome` `META_LOG_DIRECTIVE`.*
        *   Proceed to test execution.
        *   Execute test command(s) relevant to the current objective.
        *   Analyze test results.
        *   If tests pass and the functional objective is met, or if a point is reached where reflection is appropriate before full completion (e.g., end of planned work for the iteration, significant learning event), proceed to **Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY**.
        *   If tests fail, proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE** (defined in [`./RMP_Section3_CorePrompt_C1_Workflow_Init_Plan_Exec.md`](./RMP_Section3_CorePrompt_C1_Workflow_Init_Plan_Exec.md)) to diagnose and fix the issue.
    *   If the build fails:
        *   Log build failure details.
        *   Proceed to **Workflow Step 3.3: HANDLE EXECUTION FAILURE** to diagnose and fix the issue.

*   **Workflow Step 3.4.5: REQUEST & RETRIEVE TASK HISTORY:**
    *   **Trigger:** Called after the main execution/fixing phase of an iteration (e.g., from BUILD/TEST/FIX LOOP) or when explicitly decided.
    *   **Formulate `META_LOG_DIRECTIVE`** Type: `SystemState`, Content: "Execution phase concluded for current Iteration. Preparing for reflection. Requesting task history."
        *   **(Internal: Execute Reactive STM/LTM Consultation Sub-Workflow).**
    *   Determine the current iteration number (e.g., from RMP Section 2 sub-file). Let this be `CurrentIterNum`.
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
    *   (Documenting the problem for escalation will generate `META_LOG_DIRECTIVE`s, triggering reactive LTM consultation).
    *   Update the RMP Section 7 sub-file (`RMP_Modules/RMP_Section7_Escalation.md`) with:
        *   `Status: Active`
        *   `Details:` A comprehensive summary of the blocking issue, the objective, the three distinct failed attempts (including analysis and LTM consultations for each), and why self-resolution is not proceeding.
    *   Use `ask_followup_question` with:
        *   `question`: "Escalation Protocol Initiated for Iteration `CurrentIterNum`. I am blocked on achieving [brief objective snippet] due to [brief reason for blockage]. I have made N distinct attempts logged in RMP Section 8. Please review RMP Sections 4, 5, 7, and 8 and provide guidance or a revised approach. I will pause awaiting your input."
        *   `follow_up`: ["Understood, I will review and provide input.", "Proceed with one more attempt using this specific strategy: [User-defined strategy]"]
    *   Await user input. Do not proceed until guidance is received or escalation is resolved.

*   **Workflow Step 3.5: REFLECT & SELF-IMPROVE:**
    *   **Review Iteration Data:**
        *   The entire iteration: initial plan (RMP Section 4 sub-file), execution steps, `META_LOG_DIRECTIVE`s generated throughout (conceptually, and specifically from the **retrieved `task_history_iter<CurrentIterNum>.txt` file if available**), LTM consultations (deliberate and reactive), failures encountered (RMP Section 8 sub-file) and how they were handled, log analysis (RMP Section 5 sub-file), and the final outcome.
    *   **Update Long-Term (LTM) and Short-Term (STM) Memory (Append-Only, Immutable, Linked):**
        *   Based on review, identify new learnings, successful problem resolutions, or persistent issues.
        *   For each significant learning:
            *   Formulate content for a new LTM file.
            *   Create a corresponding STM entry (with fields: `STM_ID`, `Primary_Topic_Or_Error`, `Key_Concepts`, `Outcome_Indicator_Short`, `Distinguisher_Source`, `LTM_File_Path`, `All_Tags`, and optionally `Clue`, `Associated_LTM_File_Content_Snippet`, `Cross_References`).
            *   Use `write_to_file` to save the new LTM file to the `LTM/` directory, following the LTM Filename Convention.
            *   Use `insert_content` to append the new STM entry to the RMP Section 10 sub-file (e.g., `RMP_Modules/RMP_Section10_STM_IterX.md`).
    *   Analyze effectiveness of the "Core Instrumentation Prompt" (RMP Section 3 and its sub-files), especially LTM/STM mechanisms and the task history integration.
    *   Identify areas for improvement in the prompt.
    *   Propose concrete changes to the relevant RMP Section 3 sub-files in the "Reflection & Prompt Improvements" section of the RMP Section 6 sub-file (e.g., `RMP_Modules/RMP_Section6_Reflection_IterX.md`).
    *   Document reflection in the RMP Section 6 sub-file.
    *   Update "Core Instrumentation Prompt" (RMP Section 3 sub-files) with approved changes from RMP Section 6. *This update MUST follow the RMP Management Protocol defined in `system-prompt-Logger`.*
    *   Update "Scoring" (RMP Section 9 sub-file).
    *   Proceed to **Workflow Step 3.6: ATTEMPT COMPLETION**.

*   **Workflow Step 3.6: ATTEMPT COMPLETION:**
    *   If "Current User-Defined Iteration Objective" (RMP Section 2 sub-file) is fully achieved and verified, use `attempt_completion` tool.
    *   If not fully achieved but significant progress made and next steps clear, update RMP Section 2 and Section 4 sub-files for the next iteration and proceed to **Workflow Step 3.1: PLAN ITERATION** (defined in [`./RMP_Section3_CorePrompt_C1_Workflow_Init_Plan_Exec.md`](./RMP_Section3_CorePrompt_C1_Workflow_Init_Plan_Exec.md)).