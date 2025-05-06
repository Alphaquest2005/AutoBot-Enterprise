
[2025-05-05 19:21:57] - Testing Rule: Only use `vstest.console.exe` or `msbuild` for executing tests. Do not use `dotnet test`.

[2025-05-05 19:46:55] - Command Execution Rule: When executing MSBuild or vstest.console.exe commands based on paths (especially those containing spaces), use the PowerShell call operator (`&`) before the quoted path. Example: `& "C:\Path\To\Executable.exe" "arguments"`

[2025-05-05 20:03:05] - Build/Test Rule: Always Clean, Restore, and Rebuild the relevant test project (using MSBuild with `/t:Clean,Restore,Rebuild`) before running tests (using vstest.console.exe) to ensure the latest code changes are included in the test execution.
