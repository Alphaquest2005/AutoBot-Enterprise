# AutoBot-Enterprise Build Instructions

## Prerequisites
- Visual Studio 2022 Enterprise with .NET Framework 4.8
- SQL Server 2019 or later
- NuGet package restore enabled
- NUnit 3 Test Adapter extension for Visual Studio (for running tests in VS)

## Build Steps

### 1. Set Platform
Ensure the solution platform is set to **x64** in Visual Studio Configuration Manager or when using MSBuild.

### 2. Clean, Restore, and Rebuild Entire Solution

Use this command to clean, restore NuGet packages, and rebuild all projects in the `AutoBot-Enterprise.sln` solution for the Debug configuration targeting the x64 platform.

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
```

### 3. Clean, Restore, and Rebuild Specific Project (e.g., AutoBotUtilities.Tests)

Use this command if you only need to clean, restore, and rebuild a single project, such as the `AutoBotUtilities.Tests` project.

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "AutoBotUtilities.Tests\AutoBotUtilities.Tests.csproj" /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
```

*(Note: The previous build steps for separate Restore and Rebuild are combined into the single `Clean,Restore,Rebuild` command above for simplicity, as this is a common workflow.)*

## Runtime Requirements
Projects requiring x64 platform must include:
```xml
<PropertyGroup>
    <RuntimeIdentifiers>win;win-x64</RuntimeIdentifiers>
</PropertyGroup>
```

## Testing Instructions

### Test Dependencies
- NUnit (4.3.2 or later)
- NUnit3TestAdapter (5.0.0 or later)
- Moq (4.20.72 or later)
- Entity Framework Core InMemory (for test database, if applicable)

## Test Execution Methods

### Visual Studio Test Explorer
1. Build solution in Debug|x64 configuration (see Build Steps above).
2. Open Test Explorer (Test > Test Explorer).
3. Run all tests or select specific tests/groups to run.

### Command Line (vstest.console)

Make sure the test project (`AutoBotUtilities.Tests.csproj` in this example) has been built successfully first.

```powershell
# Example: Run a specific test in AutoBotUtilities.Tests
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazonMultiSectionInvoice_WithLogging" "/Logger:console;verbosity=detailed"

# Example: Run all tests in AutoBotUtilities.Tests
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" "/Logger:console;verbosity=detailed"

# Example: Run tests matching a filter in a different test project (replace paths/names)
# & "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" bin\x64\Debug\net48\Other.Tests.dll /TestCaseFilter:"FullyQualifiedName~EmailCleanupStepTests"
```

### dotnet test (Not applicable for .NET Framework 4.8 projects)
The `dotnet test` command is typically used for .NET (Core) projects, not .NET Framework 4.8 projects. Use `vstest.console.exe` instead.

### Common Test Filters (`vstest.console.exe`)
- Run tests in namespace: `/TestCaseFilter:"FullyQualifiedName~AutoBotUtilities.Tests"`
- Run specific test class: `/TestCaseFilter:"FullyQualifiedName~AutoBotUtilities.Tests.PDFImportTests"` (Note: `~` means 'contains')
- Run specific test method: `/TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazonMultiSectionInvoice_WithLogging"`
- Run tests with NUnit category: `/TestCaseFilter:"TestCategory=Integration"`

### Test Patterns
Tests follow these conventions:
- Test classes often match implementation classes + "Tests" suffix.
- Tests use NUnit's `[Test]`, `[TestCase]`, `[SetUp]`, `[TearDown]`, etc. attributes.
- Mock dependencies using Moq or NSubstitute frameworks.
- Test database operations might use an InMemory provider or a real test database.
- Output includes pass/fail status and duration.

### Example Test Output
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed   AutoBotUtilities.Tests.PDFImportTests.CanImportAmazonMultiSectionInvoice_WithLogging [47 s]

Test Run Successful.
Total tests: 1
     Passed: 1
 Total time: 48.1234 Seconds
```

## Common Issues & Solutions

### RuntimeIdentifier Errors
**Symptoms:**
- "Your project file doesn't list 'win-x64' as a RuntimeIdentifier"

**Solution:**
1. Add to affected project files:
```xml
<PropertyGroup>
    <RuntimeIdentifiers>win;win-x64</RuntimeIdentifiers>
</PropertyGroup>
```
2. Run NuGet restore (e.g., using the `Clean,Restore,Rebuild` command).
3. Rebuild solution.

### Architecture Mismatch Warnings (MSB3270)
**Symptoms:**
- Warnings about processor architecture mismatches (e.g., building x64 project with AnyCPU dependencies).

**Solution:**
1. Ensure all projects in the solution consistently target the **x64** platform via Configuration Manager.
2. Verify project references use matching architectures. If a dependency *must* be AnyCPU, this warning might be acceptable if runtime behavior is confirmed correct, but targeting x64 everywhere is preferred.

### NuGet Restore Failures / Package Downgrade Errors (NU1605)
**Symptoms:**
- Build fails during the Restore step.
- Errors like "Detected package downgrade: [PackageA] from X.Y.Z to A.B.C".

**Solution:**
1. Run restore/rebuild manually using the MSBuild commands provided in "Build Steps". Examine the output for specific error messages.
2. Check network connectivity to NuGet.org.
3. Verify NuGet package sources in Visual Studio (Tools > NuGet Package Manager > Package Manager Settings > Package Sources).
4. **Resolve Downgrades:** Identify the conflicting dependencies. Often, you need to update the lower version reference in one project (`.csproj` file) to match the higher version required by another project in the dependency chain. Sometimes adding a direct `PackageReference` with the desired higher version to the project causing the conflict can resolve it. Re-run the `Clean,Restore,Rebuild` command after making changes.

## Project-Specific Notes

### WCFConsoleHost
- Requires SQL Server connection strings in App.config
- Depends on AsycudaWorld421 project

### WaterNut Projects
- Require Entity Framework 6.x
- Must be built after Core.Common

### Core.Common
- Must be built before all dependent projects
- Contains shared contracts and base functionality

### Test Projects
- AutoBotUtilities.Tests - Utility function tests
- (Add other test projects as needed)
- Use a dedicated test database or appropriate mocking for database interactions.