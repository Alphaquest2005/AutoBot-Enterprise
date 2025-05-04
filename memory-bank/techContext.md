
## Build & Test Procedures

[2025-05-03 20:03:22] - ## AutoBot-Enterprise Build & Test Instructions (from BUILD_INSTRUCTIONS.md)

**Build Specific Project (Debug, x64):**
```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "<ProjectFile>.csproj" /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
```

**Run Tests (vstest.console):**
```powershell
# Ensure test project is built first
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\<TestProjectFolder>\bin\x64\Debug\net48\<TestProjectName>.dll" /TestCaseFilter:"<Filter>" "/Logger:console;verbosity=detailed"
```

**Common Issues:**
*   **RuntimeIdentifier Errors:** Add `<RuntimeIdentifiers>win;win-x64</RuntimeIdentifiers>` to `.csproj` and rebuild.
*   **Architecture Mismatch (MSB3270):** Ensure all projects target x64.
*   **NuGet Restore/Downgrade (NU1605):** Check sources, resolve version conflicts in `.csproj` files, re-run `Clean,Restore,Rebuild`.

*(Refer to BUILD_INSTRUCTIONS.md for full details and prerequisites.)*
[2025-05-04 08:33:54] - 
**Rule:** Always clean, restore, and rebuild the relevant test project using MSBuild before running tests with vstest.console.exe to ensure the latest changes are included.
