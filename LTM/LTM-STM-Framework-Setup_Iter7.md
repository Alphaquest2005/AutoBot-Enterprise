# LTM Entry: LTM/STM Framework Setup - Iteration 7

**STM_ID:** STM-Iter7-1
**Primary_Topic_Or_Error:** LTM-STM-Framework
**Key_Concepts:** LTM, STM, Setup, Protocol, Iteration7
**Outcome_Indicator_Short:** Info
**Distinguisher_Source:** Iter7

## Purpose:
This LTM entry documents the initial setup of the Long-Term Memory (LTM) and Short-Term Memory (STM) framework for Iteration 7. The goal of this framework is to enable continuous learning, improve operational efficiency, and provide a structured approach to knowledge management within the Roo AI system.

## Setup Process:
1.  **LTM Directory Creation:** The `LTM/` directory was created (or confirmed to exist) at the workspace root to store immutable Markdown files representing learned concepts.
2.  **STM Index File Creation:** The `RMP_Modules/RMP_Section10_STM_Iter7.md` file was created to serve as the STM index for the current iteration. This file will contain a structured table of STM entries, each linking to a corresponding LTM file.
3.  **STM Entry Format:** The STM entries adhere to a predefined format (STM_ID, Primary_Topic_Or_Error, Key_Concepts, Outcome_Indicator_Short, Distinguisher_Source, LTM_File_Path, All_Tags) to facilitate deterministic LTM filename construction and efficient indexing.

## Key Principles:
*   **Immutability:** Once an LTM entry is created, its content is never modified. New learnings or updates to existing concepts are captured in new, versioned LTM entries.
*   **Deterministic Naming:** LTM filenames are constructed deterministically from STM entry metadata to ensure unique identification and easy retrieval.
*   **Reactive Consultation:** The system is designed to reactively consult LTM entries based on keywords and context during operation, in addition to deliberate consultation during planning and reflection phases.
*   **Continuous Improvement:** The LTM/STM framework is a core component of the self-improvement mechanism, allowing the AI to learn from its experiences and refine its operational prompt.

## Associated Protocols:
*   Hierarchical Task Management & Context Propagation Protocol
*   RefactoringMasterPlan.md Management Protocol (RMP-MP)
*   RMP Structure & File Management Protocol
*   Iteration Task Checklists Protocol
*   Project Structure Management Protocol (PSM-P)
*   Task Execution Meta-Logging Protocol
*   Post-Build Mini-Review Protocol
*   Typed Logging & Dynamic Filtering System

## Future Considerations:
*   Refinement of LTM content guidelines for optimal knowledge representation.
*   Development of automated tools for LTM/STM management and querying.
*   Integration of LTM/STM with other system components for enhanced decision-making.