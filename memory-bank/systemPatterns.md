# System Patterns *Optional*

This file documents recurring patterns...

*
[2025-04-29 12:42:18] - When executing multiple commands in the Windows `cmd.exe` shell, use `;` as a separator instead of `&&`.

[2025-04-29 12:55:47] - Upon successful task completion, record key learnings and successful methods in the appropriate memory bank file (e.g., `systemPatterns.md` for general patterns, `decisionLog.md` for specific choices) to improve future performance and avoid repeating errors.

## Operational Rules & Patterns

[2025-05-03 17:42:31] - ## Command Execution Rules
*   When chaining multiple commands in `execute_command`, use a semicolon (`;`) as the separator instead of `&&`.

[2025-05-05 07:13:50] - ## Execution Flows/Algorithms (autobot1/autobot Analysis)

1.  **Main Action Loop (Inferred in `WCFConsoleHost`):**
    a.  Periodically (e.g., via Timer) query the database for pending `Actions`.
    b.  For each action, determine if it's session-based or file/email-based.
    c.  Invoke the appropriate handler (`SessionsUtils.SessionActions.TryGetValue` or `ImportUtils.Execute...`).

2.  **Session-Based Action Execution (`SessionsUtils`):**
    a.  Lookup the action name in `SessionsUtils.SessionActions` dictionary.
    b.  If found, invoke the corresponding `Action` delegate.
    c.  The delegate calls a static method in a specialized utility class (e.g., `ADJUtils.CreateAdjustmentEntries`).

3.  **File/Email-Based Action Execution (`ImportUtils`):**
    a.  Distinguish between Data-Specific (`ExecuteDataSpecificFileActions`) and Non-Specific (`ExecuteNonSpecificFileActions`) actions, or Email Mapping actions (`ExecuteEmailMappingActions`).
    b.  Query the database (`CoreEntitiesContext`) for relevant `FileTypeActions` or `EmailMappingActions`, ordered by `Id`.
    c.  Filter actions based on `IsDataSpecific`, `AssessIM7`/`AssessEX`, and `TestMode`.
    d.  For each relevant action from the DB:
        i.  Lookup the action name (`fta.Actions.Name`) in `FileUtils.FileActions` dictionary.
        ii. If found, call `ExecuteActions` with the retrieved delegate.

4.  **Core Action Execution Wrapper (`ImportUtils.ExecuteActions`):**
    a.  **Check `ProcessNextStep`:** If `fileType.ProcessNextStep` list is not empty:
        i.  Iterate through the action names in the list.
        ii. Lookup each name in `FileUtils.FileActions`.
        iii. Invoke the found delegate.
        iv. If an action named "Continue" is encountered, stop processing `ProcessNextStep` and proceed to the main action. If the list finishes without "Continue", exit.
        v.  Remove the action name from the list after processing.
    b.  **Execute Main Action:** Invoke the primary action delegate passed into the method.
    c.  Log start, success/failure, and duration.

5.  **Email Text Processing (`EmailTextProcessor.Execute`):**
    a.  Read lines from input CSV/text files.
    b.  For each line, iterate through `EmailInfoMappings` associated with the `FileType`.
    c.  For each mapping, apply the regex patterns (`KeyRegX`, `FieldRx`) from `InfoMappingRegEx` to extract key-value pairs.
    d.  Apply replacement regex (`KeyReplaceRx`, `FieldReplaceRx`) if defined.
    e.  Store extracted key-value pairs temporarily in `fileType.Data`.
    f.  Construct SQL `UPDATE` statements based on the extracted data and the target entity/field information (`EntityType`, `Field`, `EntityKeyField`) from `InfoMapping`.
    g.  Aggregate all generated SQL statements.
    h.  Execute the combined SQL statement using `CoreEntitiesContext().Database.ExecuteSqlCommand()`.
