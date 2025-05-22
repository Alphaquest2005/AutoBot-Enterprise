### 3.B Logging Directives (Using Typed Extensions)

These directives define the standard logging patterns to be achieved using the `TypedLoggerExtensions` (defined in `Core.Common.Extensions.TypedLoggerExtensions.cs`). Adherence to these patterns ensures consistent, categorized, and filterable logs. The `ILogger` instance (e.g., `_log`) used with these extensions **MUST be an instance of `Core.Common.Extensions.LevelOverridableLogger`**.

**Overarching Logging Strategy with `LevelOverridableLogger` and `LogLevelOverride`:**

To manage legacy logging and enable precise control over new structured logs, a two-gate system is employed:

1.  **Global Suppression & Scope-Specific Emission (Gate 1 - `LevelOverridableLogger` & `LogLevelOverride`):**
    *   The underlying Serilog logger (wrapped by `LevelOverridableLogger`) is configured with a very high minimum level (e.g., `Serilog.Events.LogEventLevel.Fatal`). This suppresses most pre-existing, non-compliant log statements throughout the application by default.
    *   To enable new, structured logs (via `TypedLoggerExtensions`) within a specific code scope (e.g., a method, a block within a method), that scope **MUST** be wrapped in a `using (LogLevelOverride.Begin(targetEventLevel)) { ... }` block.
        *   `targetEventLevel` determines the minimum level of logs that `LevelOverridableLogger` will *attempt* to emit from within that scope. For example, `LogLevelOverride.Begin(LogEventLevel.Debug)` allows `Debug`, `Information`, `Warning`, `Error`, and `Fatal` logs from `TypedLoggerExtensions` to pass this first gate. `LogLevelOverride.Begin(LogEventLevel.Information)` would allow `Information` and higher.
        *   This effectively bypasses the inner logger's `Fatal` minimum level *for that specific scope and for log events at or above `targetEventLevel`*.

2.  **Dynamic Filtering (Gate 2 - `LogFilterState`):**
    *   Logs that pass the first gate (emitted by `LevelOverridableLogger` due to an active `LogLevelOverride`) are then subject to dynamic filtering based on `Core.Common.Extensions.LogFilterState` properties (e.g., `EnabledCategoryLevels`, `TargetSourceContextForDetails`).
    *   This second gate determines the ultimate visibility of the log event.

**In summary:** For a typed log to appear, `LogLevelOverride` must be active at an appropriate level for the scope, *and* the log event must pass the `LogFilterState` criteria.

The extension methods automatically handle `LogCategory` and detailed caller information (`MemberName`, `SourceFilePath`, `SourceLineNumber`). Always pass `invocationId` where available and appropriate for the extension method signature.

*   **Mandatory Structured Logging & `LogCategory`:**
    *   All new application/test logging MUST use the provided `TypedLoggerExtensions` methods with an `ILogger` that is a `LevelOverridableLogger`.
    *   These methods ensure logs are structured and automatically enriched with a `LogCategory` property (e.g., `LogCategory.MethodBoundary`, `LogCategory.InternalStep`). This is fundamental for the dynamic filtering system.
    *   When using extensions that take a `messageTemplate` and `propertyValues` (e.g., `_log.LogInternalStep`, `_log.LogInfoCategorized`), always use named properties (`{PropertyName}`) in your `messageTemplate` for structured data.

*   **`InvocationId` Handling:**
    *   Most `TypedLoggerExtensions` methods accept an `invocationId` as a parameter. This `invocationId` is pushed into the `Serilog.LogContext` for the scope of the log call by the extension method, making it an ambient property for that event and any subsequent logs within that specific using scope if nested.
    *   It is also typically included as a direct property in the log message for clarity.
    *   Ensure `invocationId` is generated at the start of an operation (e.g., a test method, a pipeline execution) and passed down or made available to logging calls within that operation's scope.

*   **Method Entry/Exit (using `LogCategory.MethodBoundary`):**
    *   **Usage Example:**
        ```csharp
        // To enable MethodBoundary (Information) and potentially InternalStep (Debug) logs within this method:
        using (LogLevelOverride.Begin(LogEventLevel.Debug)) // Or LogEventLevel.Information if only boundary logs from this method are intended for the "Default View"
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Ensure Stopwatch is available
            _log.LogMethodEntry(invocationId); // Logs at Information level
            try
            {
                // ... method body ...
                // Example of an internal step log:
                // _log.LogInternalStep(invocationId, "Processing item {ItemId}", itemId); // Logs at Debug level

                _log.LogMethodExitSuccess(invocationId, stopwatch.ElapsedMilliseconds); // Logs at Information level
            }
            catch (Exception ex)
            {
                _log.LogMethodExitFailure(invocationId, stopwatch.ElapsedMilliseconds, ex); // Logs at Error level
                throw;
            }
        }
        ```
    *   **Entry:** `_log.LogMethodEntry(invocationId);`
        *   (Implicitly uses `[CallerMemberName]` for `MethodName`).
        *   Logs at `Information` level.
    *   **Exit Success:** `_log.LogMethodExitSuccess(invocationId, stopwatch.ElapsedMilliseconds);`
        *   Logs at `Information` level.
    *   **Exit Failure:** `_log.LogMethodExitFailure(invocationId, stopwatch.ElapsedMilliseconds, exception);`
        *   Logs at `Error` level.

*   **Action Phase Tracking (using `LogCategory.ActionBoundary`):**
    *   **Scope Requirement:** The code block performing the action and these logging calls must be within an appropriate `using (LogLevelOverride.Begin(LogEventLevel.Information)) { ... }` (or lower, like `Debug`, if internal steps within the action also need logging).
    *   **Start:** `_log.LogActionStart(invocationId, "YourActionName", parentAction: "OptionalParentAction");`
        *   Logs at `Information` level.
    *   **End Success:** `_log.LogActionEndSuccess(invocationId, "YourActionName", stopwatch.ElapsedMilliseconds);`
        *   Logs at `Information` level.
    *   **End Failure:** `_log.LogActionEndFailure(invocationId, "YourActionName", stopwatch.ElapsedMilliseconds, exception);`
        *   Logs at `Error` level.

*   **Internal Step Tracking (typically `LogCategory.InternalStep`):**
    *   **Scope Requirement:** The code block containing these logging calls must be within an appropriate `using (LogLevelOverride.Begin(LogEventLevel.Verbose)) { ... }` (or `Debug` if `LogInternalStepVerbose` is not used) to allow these fine-grained logs to pass the first gate.
    *   `_log.LogInternalStep(invocationId, "Descriptive message about step {Property1} and {Property2}", value1, value2);`
        *   Logs at `Debug` level by default via this specific extension.
    *   `_log.LogInternalStepVerbose(invocationId, "Very fine-grained detail for {Property}", value);`
        *   Logs at `Verbose` level by default via this specific extension.
    *   These methods also capture `[CallerMemberName]` etc.

*   **External Call Tracking (using `LogCategory.ExternalCall`):**
    *   **Scope Requirement:** The code block performing the external call and these logging calls must be within an appropriate `using (LogLevelOverride.Begin(LogEventLevel.Information)) { ... }` (or lower, like `Debug`, if finer details around the call need logging).
    *   **Start:** `_log.LogExternalCallStart(invocationId, "ExternalSystemName", "OperationName", parameters: new { Param1 = val1, Param2 = val2 });`
        *   Logs at `Information` level. `parameters` object is destructured.
    *   **End:** `_log.LogExternalCallEnd(invocationId, "ExternalSystemName", "OperationName", successBool, stopwatch.ElapsedMilliseconds, result: optionalResultObject, ex: optionalException);`
        *   Logs at `Information` (success) or `Error` (failure).

*   **Log Levels (Semantic vs. Serilog `LogEventLevel`):**
    *   The choice of extension method (e.g., `LogInternalStep` vs `LogInfoCategorized`) often implies a default Serilog `LogEventLevel` (e.g., `Debug` vs `Information`).
    *   **First Gate:** For any log to be emitted, the scope must be wrapped in `using (LogLevelOverride.Begin(targetLevel)) { ... }` where `targetLevel` is less than or equal to the log event's level.
    *   **Second Gate:** If emitted, the ultimate visibility of a log event is determined by the dynamic filter (`LogFilterState`), which considers both the event's `LogEventLevel` AND its `LogCategory` against the current `LogFilterState` settings (see `system-prompt-Logger` under "Typed Logging & Dynamic Filtering System").

*   **Exception Logging:**
    *   Automatically handled by `LogMethodExitFailure`, `LogActionEndFailure`, `LogExternalCallEnd` (on failure), provided their enclosing scope has an active `LogLevelOverride` allowing `Error` level logs (e.g., `LogLevelOverride.Begin(LogEventLevel.Error)` or lower like `Debug`, `Information`).
    *   For other scenarios, use `_log.LogErrorCategorized(LogCategory.Undefined, ex, "Error during {Operation}", operationName, invocationId);` or `_log.LogWarningCategorized(LogCategory.Undefined, ex, ...);`. These also require an appropriate `LogLevelOverride` scope.

*   **Meta Logging (`META_LOG_DIRECTIVE`) (using `LogCategory.MetaLog`):**
    *   **Scope Requirement:** `using (LogLevelOverride.Begin(LogEventLevel.Warning)) { ... }` (or the level specified in the `LogMetaDirective` call if different, e.g., `Information`).
    *   Use `_log.LogMetaDirective(type, context, directive, sourceIteration, expectedChange, details, invocationId);` (defaults to `Warning` level).
    *   Or `_log.LogMetaDirective(LogEventLevel.Information, type, context, ...);` to specify a different level.

*   **Generic Categorized Logging:**
    *   **Scope Requirement:** The code block containing these calls must be within a `using (LogLevelOverride.Begin(levelOfLog)) { ... }` where `levelOfLog` is appropriate for the specific call (e.g., `LogEventLevel.Information` for `LogInfoCategorized`, `LogEventLevel.Debug` for `LogDebugCategorized`).
    *   For logs that don't fit a specific predefined extension method type (Method, Action, etc.), use the generic categorized methods:
        *   `_log.LogInfoCategorized(LogCategory.StateChange, "User {UserId} status changed to {Status}", invocationId, userId, newStatus);`
        *   `_log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Cache miss for key {CacheKey}", invocationId, cacheKey);`
        *   (Similar for `Verbose`, `Warning`, `Error`).
    *   Always choose the most semantically appropriate `LogCategory`. Use `LogCategory.Undefined` if no other category fits well for general informational messages.

**Diagnostic Control via `LogFilterState`:**
Remember that the primary way to control log verbosity for troubleshooting is by modifying the static properties of `Core.Common.Extensions.LogFilterState` as detailed in `system-prompt-Logger` by "Typed Logging & Dynamic Filtering System". This allows focused debugging without altering these logging directives or the code's logging calls, *once logs have been permitted to emit by `LogLevelOverride`*.