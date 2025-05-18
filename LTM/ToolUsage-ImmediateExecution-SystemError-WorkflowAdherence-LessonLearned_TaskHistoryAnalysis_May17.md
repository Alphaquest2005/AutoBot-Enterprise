# LTM: Tool Usage - Immediate Execution - Workflow Adherence - System Error - Lesson Learned (Task History Analysis May 17)

**STM ID:** STM-003

**Date Created:** 2025-05-17

**Source:** Analysis of task history file `c:\Insight Software\Task history\roo_task_may-17-2025_9-58-39-pm.md`

**Problem:**
During Iteration 7, a workflow step (Initiate Iteration Cycle) required reading the `RefactoringMasterPlan.md` using the `read_file` tool. Instead of immediately executing the tool, the Assistant generated conversational text ("Okay, I will initiate iteration 7 by reading the RefactoringMasterPlan.md..."). This resulted in a system error indicating that no tool was used when one was expected, interrupting the workflow.

**Analysis:**
The workflow steps in Section 3 of the `RefactoringMasterPlan.md` implicitly or explicitly define when a tool should be used. Failing to execute the required tool as the immediate next action after determining the step violates the intended operational flow and can lead to system-level errors or misinterpretations of the Assistant's state. The system expects a tool call when the workflow dictates one.

**Lesson Learned:**
When a workflow step in the `RefactoringMasterPlan.md` requires the use of a specific tool, the Assistant MUST prioritize the immediate execution of that tool as the very next action. Any explanatory or conversational text should either precede the tool call only if explicitly allowed by the workflow (e.g., `ask_followup_question`) or follow the tool's result.

**Generalizable Lessons:**
*   Strict adherence to the defined workflow, including the timing and sequence of tool usage, is critical for smooth autonomous operation.
*   System feedback (like the "You did not use a tool" error) provides valuable information about discrepancies between internal process and expected external behavior.
*   Workflow instructions should be explicit about when tools are mandatory and immediate.

**Cross_References:**
(None identified as this is a new operational learning about workflow execution itself, not directly related to a prior code/build/test issue LTM).