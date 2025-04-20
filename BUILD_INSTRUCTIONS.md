# AutoBot-Enterprise Build Instructions

## Prerequisites
- Visual Studio 2022 Enterprise with .NET Framework 4.8
- SQL Server 2019 or later
- NuGet package restore enabled
- xUnit test runner extension for Visual Studio

& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazonMultiSectionInvoice_WithLogging" "/Logger:console;verbosity=detailed"

## Build Steps
1. Set platform to x64 in Configuration Manager
2. Run NuGet restore:
   ```powershell
   & "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Restore /p:Configuration=Debug /p:Platform=x64
   ```
3. Rebuild solution:
   ```powershell
   & "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
   ```

## Runtime Requirements
Projects requiring x64 platform must include:
```xml
<PropertyGroup>
    <RuntimeIdentifiers>win;win-x64</RuntimeIdentifiers>
</PropertyGroup>
```

## Testing Instructions

### Test Dependencies
- xUnit (2.4.1 or later)
- Moq (4.18.4 or later)
- Entity Framework Core InMemory (for test database)

## Test Execution Methods

### Visual Studio Test Explorer
1. Build solution in Debug|x64 configuration
2. Open Test Explorer (Test > Windows > Test Explorer)
3. Run all tests or select specific tests to run

### Command Line (vstest.console)
```powershell
# Build tests
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot.Pipeline.Tests.csproj /p:Configuration=Debug /p:Platform=x64

# Run all tests
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" bin\x64\Debug\net48\AutoBot.Pipeline.Tests.dll

# Run specific test class
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" bin\x64\Debug\net48\AutoBot.Pipeline.Tests.dll /Tests:EmailCleanupStepTests

# Run tests matching filter
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" bin\x64\Debug\net48\AutoBot.Pipeline.Tests.dll /TestCaseFilter:"FullyQualifiedName~EmailCleanupStepTests"
```

### dotnet test (if using .NET Core)
```powershell
dotnet test AutoBot.Pipeline.Tests.csproj --filter "FullyQualifiedName~EmailCleanupStepTests"
```

### Common Test Filters
- Run tests in namespace: `FullyQualifiedName~AutoBot.Pipeline.Tests.Processing.Email`
- Run specific test: `FullyQualifiedName=AutoBot.Pipeline.Tests.Processing.Email.Steps.EmailCleanupStepTests.ExecuteAsync_MarksEmailAsProcessed`
- Run tests with category: `TestCategory=Integration`

### Test Patterns
Tests follow these conventions:
- Test classes match implementation classes + "Tests" suffix
- Tests use xUnit's [Fact] and [Theory] attributes
- Mock dependencies using Moq framework
- Test database operations using InMemory provider
- Output includes pass/fail status and duration

### Example Test Output
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     4, Skipped:     0, Total:     4, Duration: 120 ms
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
2. Run NuGet restore
3. Rebuild solution

### Architecture Mismatch Warnings
**Symptoms:**
- MSB3270 warnings about processor architecture mismatches

**Solution:**
1. Ensure all projects target x64 platform
2. Verify project references use matching architectures

### NuGet Restore Failures
**Solution:**
1. Run restore manually:
```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Restore
```
2. Check network connectivity to NuGet.org
3. Verify NuGet package sources in Visual Studio

## Project-Specific Notes

### WCFConsoleHost
- Requires SQL Server connection strings in App.config
- Depends on AsycudaWorld421 project

### WaterNut Projects
- Require Entity Framework 6.4.4
- Must be built after Core.Common

### Core.Common
- Must be built before all dependent projects
- Contains shared contracts and base functionality

### Test Projects
- AutoBot.Pipeline.Tests - Tests for main pipeline
- AutoBotUtilities.Tests - Utility function tests
- Use InMemory database for faster test execution