### 3.A LTM/STM Core Principles & Mechanisms

**LTM/STM Core Principles:**
*   **Immutability:** Once created, LTM files (in the `LTM/` directory) and STM entries (in Section 10's sub-file) are NEVER modified.
*   **Append-Only Learning:** New information, refinements, or alternative approaches related to an existing topic result in a NEW LTM file and a NEW, corresponding STM entry.
*   **Versioning/Linking:** New LTM files that build upon previous knowledge use distinct `Distinguisher_Source` components in their filenames (e.g., `_IterN`, `_vN`, `_AttemptN`, `_FollowUpToSTM-XXX`) and MUST include `Cross_References` (Markdown links) in their content pointing to the older LTM file(s) they relate to. Each LTM file is uniquely identified by its path.
*   **STM as Seed for LTM Path:** STM entries contain the structured "seed" data (`Primary_Topic_Or_Error`, `Key_Concepts`, `Outcome_Indicator_Short`, `Distinguisher_Source`) required to deterministically construct the path to the relevant LTM file using the LTM Filename Convention.
*   **Consultation Modes:**
    *   **Deliberate:** Strategic review of STM (and subsequently LTM) during `PLAN ITERATION` and `HANDLE EXECUTION FAILURE` phases, based on broad contextual keywords.
    *   **Reactive:** Tactical, just-in-time STM/LTM consultation triggered immediately after any `META_LOG_DIRECTIVE` is *formulated* (before it's externally logged if it's an explicit step), using its content to find matching STM entries.
*   **Contextual Influence:** Retrieved LTM content (from either consultation mode) immediately becomes part of the working context, influencing subsequent thoughts, plans, and actions.

**LTM Filename Convention & Construction:**
*   **Purpose:** To allow deterministic construction of LTM file paths from STM entry seeds, eliminating LTM directory searching and improving retrieval performance.
*   **Source Data from STM Entry:** The LTM filename is constructed using specific fields from an STM entry: `Primary_Topic_Or_Error`, `Key_Concepts` (ordered), `Outcome_Indicator_Short`, and `Distinguisher_Source`.
*   **Structure:** `LTM/[Primary_Topic_Or_Error]-[Ordered_Key_Concepts]-[Outcome_Indicator_Short]_[Distinguisher_Source].md`
    *   `Primary_Topic_Or_Error`: The primary error code (e.g., `CS0121`), or a core topic keyword (e.g., `SerilogConfig`). Should be filesystem-safe.
    *   `Ordered_Key_Concepts`: Key related concepts, tools, or libraries (e.g., `AmbiguousCall-Serilog`). If multiple, join with `-`. Order consistently (e.g., alphabetically). Should be filesystem-safe.
    *   `Outcome_Indicator_Short`: A brief tag indicating the result (e.g., `Resolved`, `Failed`, `Workaround`, `Info`, `Analysis`). Should be filesystem-safe.
    *   `Distinguisher_Source`: An iteration number, version, attempt number, or reference to a prior STM ID it builds upon (e.g., `_Iter7`, `_v1`, `_Attempt1`, `_FollowUpSTM-XXX`). Should be filesystem-safe.
*   **Example:**
    *   STM Entry Fields used for filename:
        *   `Primary_Topic_Or_Error: CS0121`
        *   `Key_Concepts: AmbiguousCall, Serilog` (Sorted & Joined: `AmbiguousCall-Serilog`)
        *   `Outcome_Indicator_Short: Resolved`
        *   `Distinguisher_Source: Iter7_Attempt1`
    *   Constructed LTM Path: `LTM/CS0121-AmbiguousCall-Serilog-Resolved_Iter7_Attempt1.md`

**Reactive STM/LTM Consultation Sub-Workflow (Internal Process):**
*   **Trigger:** Immediately after *formulating* the content for any `META_LOG_DIRECTIVE` (before it's externally output if it's an explicit step).
*   **Action:**
    1.  Extract keywords from the `Content` of the formulated `META_LOG_DIRECTIVE`.
    2.  Scan the RMP Section 10 sub-file (STM Index) `All_Tags` field for STM entries with significant keyword overlap.
    3.  If relevant STM entries are found:
        *   For each highly relevant STM entry, log its `STM_ID` using a `META_LOG_DIRECTIVE` with Type `ReactiveSTM_Consultation_Triggered` (Include triggering `META_LOG_DIRECTIVE`'s Type and a snippet of its content).
        *   Construct the `LTM_File_Path` from the STM entry's seed fields (`Primary_Topic_Or_Error`, `Key_Concepts`, `Outcome_Indicator_Short`, `Distinguisher_Source`) using the LTM Filename Convention.
        *   Use `read_file` to retrieve the LTM content. If `read_file` fails (e.g., file unexpectedly missing, though construction should be robust), log a warning and proceed without this LTM's insight.
        *   Log using `META_LOG_DIRECTIVE` with Type `ReactiveLTM_Retrieved`, noting the LTM file path and a brief summary of the insight gained, if any.
        *   The retrieved LTM content is now part of the immediate working context, influencing the next thought or action.
*   **Note:** This sub-workflow is an internal mental step. The `META_LOG_DIRECTIVE`s logging this reactive consultation will appear in sequence. These meta-logs about reactive consultation are generally NOT themselves primary triggers for further reactive consultation to avoid unproductive loops, unless they contain new, distinct problem/solution keywords.