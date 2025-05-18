### 3.B Logging Directives

These directives define the standard Serilog logging patterns to be applied throughout the codebase, particularly in areas targeted for instrumentation. Adherence to these patterns is crucial for consistent observability and effective log analysis.

*   **Mandatory Structured Logging:** All application and test logging MUST use Serilog with structured message templates and named properties (`{PropertyName}`). Avoid simple string interpolation (`Log.Information("Value is " + value);`). Instead, use `Log.Information("Value is {Value}", value);`.
*   **Invocation Tracking:** At the entry point of any significant operation, process, or test method, a unique identifier MUST be added to the `Serilog.Context.LogContext` using `LogContext.PushProperty("InvocationId", Guid.NewGuid().ToString());`. This property MUST be included in all subsequent logs within that context.
*   **Method Entry/Exit:**
    *   Log `METHOD_ENTRY` at the beginning of significant methods. Include `MethodName` and `InvocationId`.
    *   Log `METHOD_EXIT_SUCCESS` upon successful method completion. Include `MethodName`, `InvocationId`, and `DurationMs` (calculated using a `Stopwatch`).
    *   Log `METHOD_EXIT_FAILURE` when a method exits due to an exception. Include `MethodName`, `InvocationId`, `DurationMs`, and `ExceptionType`.
*   **Action Phase Tracking:** Delineate logical phases within operations (e.g., Arrange, Act, Assert, Initialization, Processing, Cleanup).
    *   Log `ACTION_START` at the beginning of a phase. Include `ActionName`, `InvocationId`, and optionally `ParentAction`.
    *   Log `ACTION_END_SUCCESS` upon successful phase completion. Include `ActionName`, `InvocationId`, and `DurationMs`.
    *   Log `ACTION_END_FAILURE` when a phase fails (e.g., due to an exception or failed assertion). Include `ActionName`, `InvocationId`, `DurationMs`, and `ExceptionType`.
*   **Internal Step Tracking:** Log granular steps within actions or methods to trace execution flow.
    *   Log `INTERNAL_STEP`. Include `StepDescription`, `InvocationId`, and any relevant contextual properties (e.g., file paths, IDs, counts, boolean outcomes).
*   **External Call Tracking:** Log interactions with significant external systems or components (databases, APIs, other services, core processing logic).
    *   Log `EXTERNAL_CALL_START`. Include `ExternalSystem`, `Operation`, `InvocationId`, and relevant input parameters.
    *   Log `EXTERNAL_CALL_END`. Include `ExternalSystem`, `Operation`, `InvocationId`, `Success` (boolean), and relevant output/result properties. Log exceptions if the call failed.
*   **Log Levels:** Use appropriate Serilog levels (`Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal`) to categorize messages based on their severity and detail.
*   **Exception Logging:** Always log exceptions using the `Log.Error(ex, message, properties)` overload to ensure exception details (stack trace, inner exceptions) are captured.
*   **Meta Logging (`META_LOG_DIRECTIVE`):** Continue using `META_LOG_DIRECTIVE`s as defined in the workflow to log the AI's internal process, reasoning, and actions. Ensure the required properties (`Type`, `Context`, `Directive`, `SourceIteration`, and optionally `ExpectedChange`) are included. These meta-logs are critical for self-reflection and debugging the AI's operation.