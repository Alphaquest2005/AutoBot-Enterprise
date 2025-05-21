### 3.B Logging Directives (Using Typed Extensions)

These directives define the standard logging patterns to be achieved using the `TypedLoggerExtensions` (defined in `Core.Common.Extensions.TypedLoggerExtensions.cs`). Adherence to these patterns ensures consistent, categorized, and filterable logs. The extension methods automatically handle `LogCategory` and detailed caller information (`MemberName`, `SourceFilePath`, `SourceLineNumber`). Always pass `invocationId` where available and appropriate for the extension method signature.

*   **Mandatory Structured Logging & `LogCategory`:**
    *   All application/test logging MUST use the provided `TypedLoggerExtensions` methods.
    *   These methods ensure logs are structured and automatically enriched with a `LogCategory` property (e.g., `LogCategory.MethodBoundary`, `LogCategory.InternalStep`). This is fundamental for the dynamic filtering system.
    *   When using extensions that take a `messageTemplate` and `propertyValues` (e.g., `_log.LogInternalStep`, `_log.LogInfoCategorized`), always use named properties (`{PropertyName}`) in your `messageTemplate` for structured data.

*   **`InvocationId` Handling:**
    *   Most `TypedLoggerExtensions` methods accept an `invocationId` as a parameter. This `invocationId` is pushed into the `Serilog.LogContext` for the scope of the log call by the extension method, making it an ambient property for that event and any subsequent logs within that specific using scope if nested.
    *   It is also typically included as a direct property in the log message for clarity.
    *   Ensure `invocationId` is generated at the start of an operation (e.g., a test method, a pipeline execution) and passed down or made available to logging calls within that operation's scope.

*   **Method Entry/Exit (using `LogCategory.MethodBoundary`):**
    *   **Entry:** `_log.LogMethodEntry(invocationId);`
        *   (Implicitly uses `[CallerMemberName]` for `MethodName`).
        *   Logs at `Information` level.
    *   **Exit Success:** `_log.LogMethodExitSuccess(invocationId, stopwatch.ElapsedMilliseconds);`
        *   Logs at `Information` level.
    *   **Exit Failure:** `_log.LogMethodExitFailure(invocationId, stopwatch.ElapsedMilliseconds, exception);`
        *   Logs at `Error` level.

*   **Action Phase Tracking (using `LogCategory.ActionBoundary`):**
    *   **Start:** `_log.LogActionStart(invocationId, "YourActionName", parentAction: "OptionalParentAction");`
        *   Logs at `Information` level.
    *   **End Success:** `_log.LogActionEndSuccess(invocationId, "YourActionName", stopwatch.ElapsedMilliseconds);`
        *   Logs at `Information` level.
    *   **End Failure:** `_log.LogActionEndFailure(invocationId, "YourActionName", stopwatch.ElapsedMilliseconds, exception);`
        *   Logs at `Error` level.

*   **Internal Step Tracking (typically `LogCategory.InternalStep`):**
    *   `_log.LogInternalStep(invocationId, "Descriptive message about step {Property1} and {Property2}", value1, value2);`
        *   Logs at `Debug` level by default via this specific extension.
    *   `_log.LogInternalStepVerbose(invocationId, "Very fine-grained detail for {Property}", value);`
        *   Logs at `Verbose` level by default via this specific extension.
    *   These methods also capture `[CallerMemberName]` etc.

*   **External Call Tracking (using `LogCategory.ExternalCall`):**
    *   **Start:** `_log.LogExternalCallStart(invocationId, "ExternalSystemName", "OperationName", parameters: new { Param1 = val1, Param2 = val2 });`
        *   Logs at `Information` level. `parameters` object is destructured.
    *   **End:** `_log.LogExternalCallEnd(invocationId, "ExternalSystemName", "OperationName", successBool, stopwatch.ElapsedMilliseconds, result: optionalResultObject, ex: optionalException);`
        *   Logs at `Information` (success) or `Error` (failure).

*   **Log Levels (Semantic vs. Serilog `LogEventLevel`):**
    *   The choice of extension method (e.g., `LogInternalStep` vs `LogInfoCategorized`) often implies a default Serilog `LogEventLevel` (e.g., `Debug` vs `Information`).
    *   The ultimate visibility of a log event is determined by the dynamic filter, which considers both the event's `LogEventLevel` AND its `LogCategory` against the current `LogFilterState` settings (see `system-prompt-Logger` under "Typed Logging & Dynamic Filtering System").

*   **Exception Logging:**
    *   Automatically handled by `LogMethodExitFailure`, `LogActionEndFailure`, `LogExternalCallEnd` (on failure).
    *   For other scenarios, use `_log.LogErrorCategorized(LogCategory.Undefined, ex, "Error during {Operation}", operationName, invocationId);` or `_log.LogWarningCategorized(LogCategory.Undefined, ex, ...);`.

*   **Meta Logging (`META_LOG_DIRECTIVE`) (using `LogCategory.MetaLog`):**
    *   Use `_log.LogMetaDirective(type, context, directive, sourceIteration, expectedChange, details, invocationId);` (defaults to `Warning` level).
    *   Or `_log.LogMetaDirective(LogEventLevel.Information, type, context, ...);` to specify a different level.

*   **Generic Categorized Logging:**
    *   For logs that don't fit a specific predefined extension method type (Method, Action, etc.), use the generic categorized methods:
        *   `_log.LogInfoCategorized(LogCategory.StateChange, "User {UserId} status changed to {Status}", invocationId, userId, newStatus);`
        *   `_log.LogDebugCategorized(LogCategory.DiagnosticDetail, "Cache miss for key {CacheKey}", invocationId, cacheKey);`
        *   (Similar for `Verbose`, `Warning`, `Error`).
    *   Always choose the most semantically appropriate `LogCategory`. Use `LogCategory.Undefined` if no other category fits well for general informational messages.

**Diagnostic Control via `LogFilterState`:**
Remember that the primary way to control log verbosity for troubleshooting is by modifying the static properties of `Core.Common.Extensions.LogFilterState` as detailed in `system-prompt-Logger`. This allows focused debugging without altering these logging directives or the code's logging calls.