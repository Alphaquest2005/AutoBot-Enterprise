Okay, here's a checklist for both system-prompt-Logger.md and RefactoringMasterPlan.md, outlining each section and its requirements based on the recent refactoring and the provided content.
Checklist for system-prompt-Logger.md
Overall Purpose: To provide foundational, relatively static instructions and context to Roo, the AI. It defines Roo's high-level goals, available tools, and critical user overrides, while clearly deferring to RefactoringMasterPlan.md (Section 3) for evolving operational directives.
1. Preamble / AI Identity & Evaluation
* Purpose: Define Roo's role, skills, and how performance is evaluated.
* Requirements:
* [ ] States Roo's identity as a skilled software engineer.
* [ ] Lists performance evaluation criteria (adherence to Cardinal Rules, objective achievement, self-logging quality, reflection quality, scoring).
* [ ] Emphasizes guidance by RefactoringMasterPlan.md.
2. Section: "Foundational LLM Directives"
* Purpose: Establish immutable ultimate goals and operational mandates for Roo.
* Requirements:
* [ ] Contains the list of 9 foundational directives (achieve functional codebase, prioritize efficiency, etc.).
* [ ] Clearly states these are immutable.
* [ ] Content matches the original Section 1 of RefactoringMasterPlan.md.
3. Section: "Your Core Directive"
* Purpose: Instruct Roo on how to locate, interpret, and prioritize RefactoringMasterPlan.md.
* Requirements:
* [ ] Mandates locating, reading, and strictly adhering to RefactoringMasterPlan.md.
* [ ] Briefly lists the key sections of RefactoringMasterPlan.md (1-10) and their general purpose.
* [ ] Critically: States that RefactoringMasterPlan.md Section 3 ("Your Core Instrumentation Prompt") is the primary, definitive, and evolving operational guide, superseding any conflicting system prompt information.
4. Section: "Short-Term Memory (STM) Framework Overview"
* Purpose: Provide a static, high-level overview of the STM system's purpose and entry format relevant for LTM filename construction.
* Requirements:
* [ ] Explains STM purpose: index of immutable entries, seeds for deterministic LTM filename construction.
* [ ] Lists the core STM entry fields used for LTM filename construction: STM_ID, Primary_Topic_Or_Error, Key_Concepts, Outcome_Indicator_Short, Distinguisher_Source, LTM_File_Path, All_Tags.
* [ ] States that actual STM entries are listed in RefactoringMasterPlan.md Section 10.
* [ ] States that comprehensive LTM/STM management rules (including LTM content, creation, consultation) are in RefactoringMasterPlan.md Section 3.
5. Section: "Tool Usage & Interaction"
* Purpose: General guidelines on tool usage, workflow execution, problem-solving, and escalation.
* Requirements:
* [ ] Mentions access to a suite of tools.
* [ ] Specifies tool use is one at a time, as per RefactoringMasterPlan.md Section 3.
* [ ] References RefactoringMasterPlan.md Section 3 for the primary action loop.
* [ ] Describes expectations for autonomous problem-solving.
* [ ] Outlines human escalation protocol, referencing RefactoringMasterPlan.md Section 3.
* [ ] Defines primary output (modified C# files, updated RefactoringMasterPlan.md, LTM/ files).
* [ ] Specifies attempt_completion tool usage.
6. Section: "Markdown Output Rule"
* Purpose: Define standard formatting for filenames and language constructs in Roo's output.
* Requirements:
* [ ] Specifies clickable formatting for language constructs and filenames.
* [ ] States this applies to ALL markdown responses and <attempt_completion> content.
7. Section: "Available Tools"
* Purpose: List all tools Roo can use, with descriptions and parameters.
* Requirements:
* [ ] Lists each tool by name (e.g., read_file, apply_diff, write_to_file, ask_followup_question, etc.).
* [ ] Provides an accurate description for each tool.
* [ ] Lists all required and optional parameters for each tool correctly.
8. Section: "Tool Use Formatting"
* Purpose: Specify the XML-style tag format for tool calls.
* Requirements:
* [ ] Shows the correct example format: <actual_tool_name><parameter1_name>value1</parameter1_name></actual_tool_name>.
9. Section: "MCP Servers (If applicable)"
* Purpose: List any connected MCP servers and their available tools.
* Requirements:
* [ ] Correctly lists server names and their startup commands/details.
* [ ] Correctly lists available tools for each MCP server.
10. Section: "System Information"
* Purpose: Provide static information about the operating environment.
* Requirements:
* [ ] Lists Operating System.
* [ ] Lists Default Shell.
* [ ] Lists Home Directory.
* [ ] Lists Current Workspace Directory.
11. Section: "USER'S CUSTOM INSTRUCTIONS (Global & Rules - These are critical overrides)"
* Purpose: Consolidate user-provided global instructions and rules that take precedence.
* Requirements:
* [ ] Clearly marked as "critical overrides."
* [ ] Includes "Language Preference."
* [ ] Includes "Global Instructions" (e.g., build command, write_to_file fallback, no autogen mods).
* [ ] Includes "Rules" (e.g., from rules.md, specific test command).
12. Concluding Instructions
* Purpose: Direct Roo's initial action and provide final meta-instructions.
* Requirements:
* [ ] States first action is to locate and process RefactoringMasterPlan.md according to its Section 3.
* [ ] Includes "Use code with caution."
* [ ] Includes the user's meta-request about refactoring the prompts (which has now been acted upon).
Checklist for RefactoringMasterPlan.md
Overall Purpose: To be the primary, dynamic, and evolving document guiding Roo's iteration-specific work. It contains the current objective, Roo's evolving core instructions (Section 3), detailed plans, analysis, reflections, and memory systems.
1. Header: # Refactoring Master Plan
* Purpose: Main title of the document.
* Requirements:
* [ ] Present at the beginning of the file.
2. Section 1: "Foundational LLM Directives"
* Purpose: Placeholder; refers to the definitions now in system-prompt-Logger.md.
* Requirements:
* [ ] States that these directives are defined in system-prompt-Logger.md.
* [ ] Does NOT contain the actual list of directives anymore.
3. Section 2: "User Objective Refinement (Iteration X)"
* Purpose: Define the specific goal for the current iteration.
* Requirements:
* [ ] Includes the iteration number in the heading.
* [ ] Contains "Initial User Objective."
* [ ] Contains "Refined Objective" (potentially AI-refined for clarity/actionability).
* [ ] Contains "Priority" for the objective.
* [ ] Content is specific to the current iteration.
4. Section 3: "Your Core Instrumentation Prompt (Roo - Logger Mode)"
* Purpose: Roo's primary, detailed, and evolving set of operational instructions. This is the most critical section for Roo's behavior.
* Requirements:
* [ ] Clearly states it's an evolving section and Roo can propose changes via Section 6.5.
* [ ] LTM/STM Core Principles: Includes immutability, append-only learning, versioning/linking, STM as seed, consultation modes, contextual influence.
* [ ] LTM Filename Convention & Construction: Defines structure, source data from STM, and an example.
* [ ] Reactive STM/LTM Consultation Sub-Workflow: Defines trigger and action steps for this internal process.
* [ ] Section 3.1 Logging Directives: Defines detailed Serilog logging patterns (mandatory structured logging, invocation tracking, method entry/exit, action phase tracking, internal step, external call, log levels, exception logging, meta logging).
* [ ] Workflow Steps: Details each step from -1: INITIATE ITERATION CYCLE through 3.6: ATTEMPT COMPLETION, including LTM/STM interactions, task history retrieval, and reflection triggers.
* [ ] Cardinal Rules: Lists non-negotiable operational rules.
* [ ] Critical Instructions: Lists key commands or procedures.
* [ ] Content is comprehensive and directly drives Roo's actions and self-improvement.
5. Section 4: "Your Plan (Iteration X)"
* Purpose: Roo's detailed, step-by-step plan to achieve the iteration's objective.
* Requirements:
* [ ] Includes the iteration number in the heading.
* [ ] States the overall "Objective" being addressed by the plan.
* [ ] Includes "Current Status" of the plan execution.
* [ ] Lists "Plan Steps" which are:
* Numbered or clearly delineated.
* Specific and actionable.
* Marked with completion status (e.g., "Completed," "In Progress," "Pending").
* Includes META_LOG_DIRECTIVE formulation and reactive LTM/STM consultation triggers where appropriate.
* [ ] Dynamically updated by Roo throughout the iteration.
6. Section 5: "Log Analysis & Diagnosis"
* Purpose: Document Roo's analysis of logs, diagnostic findings, and hypotheses related to problems encountered.
* Requirements:
* [ ] Contains structured findings from log analysis.
* [ ] Includes any hypotheses formulated by Roo.
* [ ] Clearly links findings to specific failures or observations.
* [ ] Dynamically updated by Roo as analysis occurs.
7. Section 6: "Reflection & Prompt Improvements"
* Purpose: Roo's end-of-iteration reflection on performance, learning, and proposed improvements to Section 3.
* Requirements:
* [ ] Contains an "Iteration X Reflection Summary."
* [ ] Section 6.5: "Proposed Core Instrumentation Prompt Improvements":
* Lists concrete, specific, and actionable proposed changes for Section 3.
* Proposals should aim to improve efficiency, LTM/STM usage, problem-solving, etc.
* [ ] Primarily generated by Roo during the "REFLECT & SELF-IMPROVE" workflow step.
8. Section 7: "Escalation"
* Purpose: Track if and why Roo needs to escalate to human input.
* Requirements:
* [ ] "Status" (e.g., Not Active, Active, Resolved).
* [ ] If "Active," includes "Details" of the blocking issue and attempts made.
* [ ] Updated by Roo according to the escalation protocol in Section 3.
9. Section 8: "Failure Tracking"
* Purpose: Maintain a log of significant failures encountered during iterations.
* Requirements:
* [ ] Each failure has a unique ID (e.g., Failure X.Y).
* [ ] For each failure: "Type," "Description," "Details," "Diagnosis," "Attempts," "Status."
* [ ] Dynamically updated by Roo as failures occur and are addressed.
10. Section 9: "Scoring (Iteration X)"
* Purpose: Quantify Roo's performance for the current iteration.
* Requirements:
* [ ] Includes the iteration number in the heading.
* [ ] Lists defined scoring criteria (e.g., Objective Achievement, Code Quality, Logging Quality, Process Adherence, Self-Improvement).
* [ ] Provides a score for each criterion.
* [ ] Includes a "Total Score."
* [ ] Updated by Roo at the end of an iteration.
11. Section 10: "Short-Term Memory (STM) Index"
* Purpose: Act as an index for LTM files by listing STM entries created during reflections.
* Requirements:
* [ ] States that the purpose and format overview of STM entries are in system-prompt-Logger.md.
* [ ] Reinforces that comprehensive LTM/STM management principles are in Section 3.
* [ ] Sub-heading: "* Entries:"
* [ ] Lists STM entries. Each entry should minimally contain the fields defined in Section 3 for LTM filename construction (STM_ID, Primary_Topic_Or_Error, Key_Concepts, Outcome_Indicator_Short, Distinguisher_Source, LTM_File_Path, All_Tags).
* (Note: The actual example entries in the provided RMP are richer and also include Clue, Associated_LTM_File_Content_Snippet, Cross_References. This richer format is good for human readability and broader keyword matching, but the core LTM path construction relies on the fields defined in Section 3.)
* [ ] STM entries are append-only and immutable once created.
* [ ] New entries are added by Roo during the "REFLECT & SELF-IMPROVE" step.
* [ ] Self-correction based on provided file: The provided RefactoringMasterPlan.md (first version) also appends a list of "Proposed Core Instrumentation Prompt Improvements" at the very end of Section 10. Ideally, all such proposals should be consolidated under Section 6.5. The checklist notes Section 10's primary purpose is STM entries. The RMP itself should be updated to move these proposals to 6.5 for consistency if this wasn't intended. For now, the checklist reflects what is in the provided file. Update: Looking at the first RMP again, the proposed improvements are under "6.5 Proposed Core Instrumentation Prompt Improvements". The second RMP example had some items under its Section 10. The checklist should stick to the structure of the first RMP example that was refactored.
* [ ] Self-correction based on the first RMP example provided in the initial prompt: The list of items like "Evaluate the effectiveness and potential overhead of reactive LTM consultation..." etc., appears after the STM entries in Section 10. These seem to be general reflection points or to-do items for prompt improvement, distinct from the structured proposals in 6.5. This section of the checklist will reflect their presence in Section 10 as per the input document.
This checklist should help verify the structure and content requirements for both files, ensuring they correctly fulfill their roles within your system.