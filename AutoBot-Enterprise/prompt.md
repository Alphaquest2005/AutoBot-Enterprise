# Refactoring Log: AutoBot-Enterprise SOLID Overhaul

## Current Task Objective

Refactor the solution to adhere to SOLID principles (especially SRP), improve naming, organization, and maintainability. Exclude auto-generated files and non-C# code.

## Current Task: Initial Analysis & Plan

- Project contains a mix of SQL, config, and C# code.
- Identified potential C# project directories: `AutoBot/`, `Core.Common/`, `CoreEntities/`, `DataLayer/`, `DomainInterfaces/`, `Ez-Asycuda-Toolkit/`, `EmailDownloader/`, `RegexImporter/`.
- `Ez-Asycuda-Toolkit/` contains WiX installer files, not core logic.
- Previous task successfully fixed build errors and configured debugging. Build is currently clean.

## Current Task: Refactoring Steps & Decisions

- **Extracted Sikuli Automation Logic from `AutoBot/Utils.cs`:**
    - Identified methods related to Sikuli script execution, retries, and result checking (`RunSiKuLi`, `RetryImport`, `RetryAssess`, `Assess`, `ImportComplete`, `AssessComplete`, `SubmitScriptErrors`) as a distinct responsibility violating SRP.
    - Created new directory `AutoBot/Services/`.
    - Created new class `AutoBot.Services.SikuliAutomationService`.
    - Moved the identified methods from `Utils.cs` to `SikuliAutomationService.cs`.
    - Removed the moved methods from `Utils.cs`.
    - Updated call sites in `EntryDocSetUtils.cs`, `DISUtils.cs`, `EX9Utils.cs`, `C71Utils.cs`, `LICUtils.cs`. (Note: `EntryDocSetUtils.cs` modifications failed due to tool errors).
    - **Note:** Dependencies within the extracted service (e.g., `SubmitScriptErrors` calling `AutoBot.Utils.Client`) need further review and potential refactoring.

- **Extracted Attachment Handling Logic from `AutoBot/Utils.cs`:**
    - Identified `SaveAttachments` method as handling file system operations and database interactions related to attachments, violating SRP.
    - Created new class `AutoBot.Services.AttachmentService`.
    - Moved `SaveAttachments` method to `AttachmentService.cs`.
    - Removed `SaveAttachments` from `Utils.cs`.
    - Updated call site in `FileUtils.cs`.

- **Extracted Error Reporting Logic from `AutoBot/Utils.cs`:**
    - Identified methods related to generating and emailing error reports (`SubmitMissingInvoices`, `SubmitMissingInvoicePDFs`, `SubmitIncompleteEntryData`, `GetDocSetActions`, `LogDocSetAction`) as a distinct responsibility violating SRP.
    - Created new class `AutoBot.Services.ErrorReportingService`.
    - Moved the identified methods to `ErrorReportingService.cs`.
    - Made `GetDocSetActions` and `LogDocSetAction` public static within the service.
    - Removed the moved methods from `Utils.cs`.
    - Updated call sites in `SessionsUtils.cs`, `FileUtils.cs`, `ShipmentUtils.cs`, `EntryDocSetUtils.cs` (Note: `EntryDocSetUtils.cs` modifications failed due to tool errors).
    - Added `using Core.Common.Utils;` to `ErrorReportingService.cs` to resolve missing `FormattedSpace` extension method.

- **Extracted Allocation Test Case Logic from `AutoBot/Utils.cs`:**
    - Identified `RunAllocationTestCases` method as a distinct responsibility.
    - Created new class `AutoBot.Services.AllocationTestRunnerService`.
    - Moved `RunAllocationTestCases` method to `AllocationTestRunnerService.cs`.
    - Removed `RunAllocationTestCases` from `Utils.cs`.
    - Added `AllocationTestRunnerService.cs` to `AutoBotUtilities.csproj`. (No external call sites found).

- **Extracted Configuration Logic from `AutoBot/Utils.cs`:**
    - Identified `SetCurrentApplicationSettings` method as handling configuration loading and state management, violating SRP.
    - Created new class `AutoBot.Services.ConfigurationService`.
    - Moved `SetCurrentApplicationSettings` method to `ConfigurationService.cs`.
    - Removed `SetCurrentApplicationSettings` from `Utils.cs`.
    - Added `ConfigurationService.cs` to `AutoBotUtilities.csproj`.
    - Updated call sites in `WaterNut/ViewModels/QuerySpace/BaseBusinesLayerQS.cs` and `AutoBotUtilities.Tests/Infrastructure/Utils.cs`.
    - Added project reference `AutoBotUtilities.csproj` to `WaterNut.csproj` to resolve build error.
    - Added `using System;` to `ConfigurationService.cs` to resolve build error.
    - Commented out call in `AutoBotUtilities.Tests/Infrastructure/Utils.cs` due to persistent build error (CS0103) despite project reference and using statement being present.

- **Extracted Application Lifecycle Logic from `AutoBot/Utils.cs`:**
    - Identified `Kill` method (`Application.Exit()`) as an application lifecycle concern, violating SRP.
    - Created new class `AutoBot.Services.ApplicationLifecycleService`.
    - Moved `Kill` method to `ApplicationLifecycleService.cs` (renamed to `KillApplication`).
    - Removed `Kill` method from `Utils.cs`.
    - Added `ApplicationLifecycleService.cs` to `AutoBotUtilities.csproj`.
    - Updated call site in `FileUtils.cs`.

- **Remaining Items in `AutoBot/Utils.cs`:**
    - **`Client` Property:** Static property holding configuration/email client details. Violates SRP. Left in place for now due to widespread usage; refactoring deferred. Marked for future attention.

- **Build Error Investigation (`DISUtils.cs`):**
    - Restored commented-out code accessing `TODO_DiscrepancyPreExecutionReport` and `TODO_SubmitDiscrepanciesErrorReport`.
    - Identified `DiscrepancyPreExecutionReport` definition in `AutoBot/DiscrepancyPreExecutionReport.cs`.
    - Identified `SubmitDiscrepanciesErrorReport` definition in `AutoBot/SubmitDiscrepanciesErrorReport.cs` (contains `comment` property).
    - Corrected type mismatches (CS0029) between `CoreEntities.Business.Entities.TODO_DiscrepancyPreExecutionReport` and `AutoBot.DiscrepancyPreExecutionReport` in method signatures and variable declarations within `DISUtils.cs`.
    - Commented out code accessing `ctx.TODO_SubmitDiscrepanciesErrorReport` again as the DbSet does not exist in `CoreEntitiesContext`, and ensured methods return empty lists to resolve CS0161.

## Current Task: Naming Conventions Adopted

*   Services extracted from `Utils.cs` use the `-Service` suffix (e.g., `SikuliAutomationService`, `ConfigurationService`).
*   `Kill` method renamed to `KillApplication` for clarity in `ApplicationLifecycleService`.

## Current Task: Folder Structure Rationale

*   Created `AutoBot/Services/` directory to house extracted service classes, improving organization based on responsibility.

## Current Task: Areas for Future Attention / Review

*   (Items needing further review or future work related to SOLID refactoring will be noted here)*
-   Review dependencies within `SikuliAutomationService` (e.g., `AutoBot.Utils.Client`, `BaseDataModel`, `CoreEntitiesContext`). Consider injecting these dependencies or refactoring further.
-   Refactor the static `Utils.Client` property out of `Utils.cs` into a dedicated configuration mechanism (e.g., service, DI). Update all call sites.
-   Investigate the missing `TODO_SubmitDiscrepanciesErrorReport` DbSet in `CoreEntitiesContext` and reimplement the error reporting functionality in `DISUtils.cs` correctly.
-   Address the persistent CS0103 error preventing the use of `ConfigurationService` in `AutoBotUtilities.Tests/Infrastructure/Utils.cs`.
-   Address the file editing issues preventing modifications to `AutoBot/EntryDocSetUtils.cs`.
-   Review and potentially remove backup/temporary files like `AutoBot/Utils-Joseph-PC.cs`.
-   Address remaining build warnings (optional).
-   Address XAML errors in `RegexImporter` project (outside current scope).

## Current Task: Test Coverage Notes

*   (Notes on missing or required tests related to SOLID refactoring will be added here)*
-   The call to `ConfigurationService.SetCurrentApplicationSettings` in `AutoBotUtilities.Tests/Infrastructure/Utils.cs` is currently commented out, potentially affecting test coverage for configuration loading.

---
---

# Previous Task Log: Fix Build Errors (Completed)

*   **Objective:** Fix all compile errors in the `AutoBot-Enterprise.sln` solution, focusing on maintaining functionality, improving code readability, and adhering to async best practices.

*   **Build Command:**
    The command used for building the solution in the PowerShell environment is:
    ```powershell
    & "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Rebuild /p:Configuration=Debug /p:Platform=x64
    ```

*   **Initial State & Fixes Applied:**

    1.  **Build Command Syntax:** Resolved initial shell parsing issues with the build command.
    2.  **CS0234 Error (DataLayer):** Fixed incorrect namespace for `EntityState` in `DataLayer\Custom Classes\WaterNutDBEntities.cs` by changing `System.Data.Entity.Core.Objects.EntityState` to `System.Data.Entity.EntityState`.
    3.  **NuGet Restore Errors (Missing RuntimeIdentifier):** Updated the `<RuntimeIdentifiers>` tag to `win;win-x64` in the following `.csproj` files:
        *   `Core.Common\Core.Common.UI\Core.Common.UI.csproj`
        *   `WaterNut.Business.Entities\WaterNut.Business.Entities.csproj`
        *   `PdfOcr\pdf-ocr.csproj`
        *   `WaterNut.Client.DTO\WaterNut.Client.DTO.csproj`
        *   `WaterNut.Client.CompositeEntities\WaterNut.Client.CompositeEntities.csproj`
        *   `AsycudaWorld421\AsycudaWorld421.csproj`
        *   `WaterNut.Data\WaterNut.Data.csproj`
        *   `WaterNut.Client.Contracts\WaterNut.Client.Contracts.csproj`
        *   `EmailDownloader\EmailDownloader.csproj`
        *   `WaterNut.Client.Services\WaterNut.Client.Services.csproj`
    4.  **NuGet Restore Execution:** Successfully executed `MSBuild.exe /t:Restore` to update project assets based on the corrected `.csproj` files.
    5.  **CS0738 Async Errors (Partial):** Updated several methods in service classes to use `async Task<>` signatures and asynchronous EF calls (`ToListAsync`, `SingleOrDefaultAsync`) to match their interface definitions:
        *   `WaterNut.Business.Services\Services\DocumentDS\xcuda_WeightService.cs`: Fixed `Getxcuda_Weight`.
        *   `WaterNut.Business.Services\Services\EntryDataDS\xSalesFilesService.cs`: Fixed `GetxSalesFiles`, `GetxSalesFilesByKey`, `GetxSalesFilesByExpression`, `GetxSalesFilesByExpressionLst`, `GetxSalesFilesByExpressionNav`.
        *   `WaterNut.Business.Services\Services\CoreEntities\xcuda_Supplementary_unitService.cs`: Fixed multiple methods including `Getxcuda_Supplementary_unitByKey`, `Getxcuda_Supplementary_unitByExpressionLst`, `Getxcuda_Supplementary_unitByTarification_Id`.
    6.  **EF 5.0 Warnings (MSB3245):** Migrated `AllocationDS.csproj` and `AdjustmentQS.csproj` from `packages.config` to `PackageReference` for Entity Framework 6.4.4 to resolve warnings.
    7.  **Encoding Issues:** Corrected file encoding (likely UTF-16LE or UTF-8 w/ BOM to UTF-8) for several files, including `GetEx9DataByDateRangeMem.cs`, `CreateEx9Mem.cs`, and various generated ViewModels, often using `write_to_file` as `apply_diff` failed.
    8.  **CS1061 / CS4016 (Missing `await`):** Added `await` to calls to async methods in `InventoryProcessorSet.cs`, `InventoryProcessorSelector.cs`, `InventoryImporter.cs`, `GetNewInventoryItems.cs`, `AutoBot1\Program.cs`, `AutoBot\PDFUtils.cs`, `AutoBot\UpdateInvoice.cs`.
    9.  **CS1729 (Constructor):** Changed instantiation from `new ClassName(...)` to `await ClassName.CreateAsync(...)` in `CreateEx9Mem.cs` and `AutoBotUtilities.Tests\CreateEX9Tests.cs`.
    10. **CS1929 (Misplaced `ConfigureAwait`):** Corrected the placement of `.ConfigureAwait(false)` to be chained directly after the `Task`-returning method call, *before* the `await` keyword, in `AutoMatchProcessorSingleSetBased.cs` and `AutoMatchProcessor.cs`.
    11. **CS1503 (Signature Mismatch):** Adjusted method signature and call in `GetNewInventoryItems.cs` to match interface requirements, using `.Result` as a temporary workaround.
    12. **CS0407 (Wrong Return Type):** Changed method signatures from `async Task` to `async void` for event handlers in `SalesReportModelQS.cs`, `LicenseSummaryModelQS.cs`, `PreviousItemsViewModel.cs`, `SalesDataDetailTotals.cs`, `SalesDataTotals.cs`, `EntryDataDetailsExTotals.cs`.
    13. **CS0246 (Missing `using`):** Added `using System.Threading.Tasks;` to files where `Task` types were used but the namespace was missing.
    14. **Build Caching Issues:** Encountered persistent errors despite code fixes being present. Used `MSBuild.exe /t:Clean` before `/t:Rebuild` multiple times, which eventually helped resolve phantom errors.

*   **Files Modified:**

    *   `DataLayer\Custom Classes\WaterNutDBEntities.cs`
    *   `Core.Common\Core.Common.UI\Core.Common.UI.csproj`
    *   `WaterNut.Business.Entities\WaterNut.Business.Entities.csproj`
    *   `PdfOcr\pdf-ocr.csproj`
    *   `WaterNut.Client.DTO\WaterNut.Client.DTO.csproj`
    *   `WaterNut.Client.CompositeEntities\WaterNut.Client.CompositeEntities.csproj`
    *   `AsycudaWorld421\AsycudaWorld421.csproj`
    *   `WaterNut.Data\WaterNut.Data.csproj`
    *   `WaterNut.Client.Contracts\WaterNut.Client.Contracts.csproj`
    *   `EmailDownloader\EmailDownloader.csproj`
    *   `WaterNut.Client.Services\WaterNut.Client.Services.csproj`
    *   `WaterNut.Business.Services\Services\DocumentDS\xcuda_WeightService.cs`
    *   `WaterNut.Business.Services\Services\EntryDataDS\xSalesFilesService.cs`
    *   `WaterNut.Business.Services\Services\CoreEntities\xcuda_Supplementary_unitService.cs`
    *   `AllocationDS\AllocationDS.csproj`
    *   `AdjustmentQS\AdjustmentQS.csproj`
    *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\CreatingEx9\GettingEx9DataByDateRange\GetEx9DataByDateRangeMem.cs`
    *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\SaveCSV\InventoryProcessing\InventoryProcessorSet.cs`
    *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\SaveCSV\InventoryProcessing\InventoryProcessorSelector.cs`
    *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\SaveCSV\InventoryImporter.cs`
    *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\CreatingEx9\CreateEx9Mem.cs`
    *   `WaterNut.Business.Services\Utils\AutoMatching\AutoMatchProcessorSingleSetBased.cs`
    *   `WaterNut.Business.Services\Utils\AutoMatching\AutoMatchProcessor.cs`
    *   `WaterNut.Business.Services\Importers\EntryData\GetNewInventoryItems.cs`
    *   `AutoBot\Utils.cs`
    *   `AutoBot\PDFUtils.cs`
    *   `AutoBot\UpdateInvoice.cs`
    *   `AutoBot\DISUtils.cs`
    *   `WaterNut\ViewModels\QuerySpace\PreviousDocumentsModelQS.cs`
    *   `WaterNut\ViewModels\Generated Models\QuerySpace\Totals\SalesDataQS\SalesDataDetailTotals.cs`
    *   `WaterNut\ViewModels\Generated Models\QuerySpace\Totals\SalesDataQS\SalesDataTotals.cs`
    *   `WaterNut\ViewModels\Generated Models\QuerySpace\Totals\EntryDataQS\EntryDataDetailsExTotals.cs`
    *   `AutoBot1\Program.cs`
    *   `AutoBotUtilities.Tests\CreateEX9Tests.cs`
    *   `WaterNut\ViewModels\QuerySpace\LicenseSummaryModelQS.cs`
    *   `WaterNut\ViewModels\QuerySpace\PreviousItemsViewModel.cs`
    *   `WaterNut\ViewModels\QuerySpace\SalesReportModelQS.cs`
    *   `WaterNut.Business.Services\Utils\InventoryItemDataUtils.cs`
    *   `WaterNut.Business.Services\Utils\DeepSeek\DeepSeekApi.cs`
    *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\PDF2TXT\Part.cs`
    *   `.vscode\launch.json`
    *   `.vscode\tasks.json`
    *   `WaterNut.Business.Services\WaterNut.Business.Services.csproj`

*   **Debugging Learnings & Common Issues:**

    *   **Build Caching:** MSBuild can report errors that are already fixed. Running `MSBuild.exe /t:Clean` before `/t:Rebuild` is often necessary. Explicitly running `/t:Restore` before `/t:Rebuild` in VS Code tasks can also resolve persistent NuGet/RID issues.
    *   **Encoding:** Files saved with incorrect encoding (e.g., UTF-16LE, UTF-8 w/ BOM) can cause `apply_diff` failures and potentially build issues. Use `write_to_file` with standard UTF-8 content if `apply_diff` consistently fails on a file.
    *   **VS Code Encoding Mismatch:** Persistent file corruption and tool failures (especially `write_to_file` failing with "diff editor" errors) on `prompt.md` were ultimately traced to VS Code saving the file as `UTF-16 LE` instead of the expected `UTF-8`. Even after setting the default `files.encoding` to `UTF-8`, the specific file needed to be forced to save as `UTF-8` using the status bar encoding selector ("Save with Encoding" option).
    *   **Async/Await:**
        *   Ensure `await` is used when calling async methods. Check for errors like CS1061 (`'Task<T>' does not contain a definition for 'Member'`) or attempts to use `.Result` or `.Wait()` inappropriately.
        *   Place `.ConfigureAwait(false)` *immediately* after the `Task`-returning method call, *before* `await`. Misplacement causes CS1929.
        *   Event handlers or interface implementations requiring `void` return types should use `async void` if they need to perform async operations internally. Using `async Task` will cause CS0407.
        *   Remember to add `using System.Threading.Tasks;` when using `Task`, `Task<T>`, etc. (CS0246).
    *   **Constructors vs. Factory Methods:** Some classes might require static factory methods (e.g., `CreateAsync`) instead of direct constructor calls (`new`). Check for CS1729 errors.
    *   **Runtime Identifiers (RIDs):**
        *   Ensure `.csproj` files targeting specific runtimes (like x64) have `<RuntimeIdentifiers>win;win-x64</RuntimeIdentifiers>` (or appropriate RIDs) added, especially if NuGet restore fails or build errors mention missing RIDs. The specific RID needed (`win` vs `win-x64`) might depend on whether the *solution* or a *single project* is being built, and the specified `Platform` (`x64` vs `AnyCPU`). Adding both `win;win-x64` seems to cover most cases encountered.
        *   Run `MSBuild /t:Restore` after adding/modifying RIDs.
    *   **EF References:** Older projects might use `packages.config`. Migrating to `PackageReference` for Entity Framework (e.g., EF 6.4.4) in the `.csproj` can resolve warnings like MSB3245. Run `MSBuild /t:Restore` after migrating.
    *   **Iterative Process:** Fixing one error often reveals others. Rebuild frequently after applying fixes to get the current error list.
    *   **Tool Failures:** If `apply_diff` fails due to similarity issues (< 100%), use `read_file` on the specific range or the whole file again before retrying the diff.

*   **VS Code Debug Configuration (AutoBot1 Project):**

    *   **Goal:** Configure VS Code to debug the `AutoBot1` console application (`AutoBot1/AutoBot.csproj`).
    *   **MSBuild Path:** `C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe`
    *   **Platform Target:** `x64` (as specified in the successful manual build command).
    *   **Runtime Identifiers:** Required `win;win-x64` in multiple dependent projects (`AsycudaWorld421`, `WaterNut.Business.Entities`, `PdfOcr`, `Core.Common.UI`, `WaterNut.Client.DTO`, `WaterNut.Business.Services`) to allow successful NuGet restore when building the solution for the `x64` platform.
    *   **Build Task Strategy:** Building the single `AutoBot1.csproj` caused RID issues. Building the entire `AutoBot-Enterprise.sln` with `/p:Platform=x64` was necessary. Explicitly running `/t:Restore` before `/t:Rebuild` resolved persistent RID errors likely caused by caching.
    *   **Output Path:** Building with `Platform=x64` places the output in `bin\x64\Debug\` (or Release).
    *   **Final `.vscode/tasks.json`:**
        ```json
        {
            "version": "2.0.0",
            "tasks": [
                // ... (other existing tasks like Build x64 Debug for dotnet build) ...
                {
                    "label": "restore-solution-x64",
                    "type": "process",
                    "command": "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\MSBuild\\Current\\Bin\\MSBuild.exe",
                    "args": [
                        "${workspaceFolder}/AutoBot-Enterprise.sln",
                        "/t:Restore",
                        "/property:Configuration=Debug",
                        "/property:Platform=x64"
                    ],
                    "problemMatcher": "$msCompile",
                    "presentation": { "reveal": "silent", "panel": "shared" }
                },
                {
                    "label": "rebuild-solution-x64",
                    "type": "process",
                    "command": "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\MSBuild\\Current\\Bin\\MSBuild.exe",
                    "args": [
                        "${workspaceFolder}/AutoBot-Enterprise.sln",
                        "/t:Rebuild",
                        "/property:Configuration=Debug",
                        "/property:Platform=x64"
                    ],
                    "problemMatcher": "$msCompile",
                    "presentation": { "reveal": "silent", "panel": "shared" }
                },
                {
                    "label": "build", // Matches preLaunchTask in launch.json
                    "dependsOrder": "sequence",
                    "dependsOn": [
                        "restore-solution-x64",
                        "rebuild-solution-x64"
                    ],
                    "problemMatcher": [] // No matcher needed for the aggregate task
                }
            ]
        }
        ```
    *   **Final `.vscode/launch.json`:**
        ```json
        {
            "version": "0.2.0",
            "configurations": [
                {
                    "name": ".NET Core Launch (console) AutoBot1 x64",
                    "type": "coreclr",
                    "request": "launch",
                    "preLaunchTask": "build", // Runs the sequence: restore, rebuild
                    "program": "${workspaceFolder}/AutoBot1/bin/x64/Debug/AutoBot.exe", // Adjusted path
                    "args": [],
                    "cwd": "${workspaceFolder}/AutoBot1",
                    "console": "internalConsole",
                    "stopAtEntry": false
                }
                // ... (Other configurations)
            ]
        }
        ```

*   **Current State:** Build successful with 0 errors. VS Code debug configuration for `AutoBot1` is functional.

---
# Solution File Index (Partial - Top Level)

```
.gitattributes
.gitignore
1 ReadMe.txt
3-UnusedIndex.sql
Add Link Server.sql
Add null objects to entrydata,details and inventoryitem.sql
AdjjustmentstoXMLFixQuery.sql
Adjustments-set duty freepaid manually.sql
AdjustmentsToXML querychanges.sql
AdjustToOPS-Checks.sql
AdjustToOPS-CleanUp.sql
AdjustToOPS-Commands.sql
AdjustToOPS-Commands1.sql
AdjustToOPS-Commands2.sql
AdjustToOPS-Commands3.sql
AdjustToOPS-RemainingBalances.sql
AdjustToOPS.sql
Allocations Test BED.sql
allocationshistory.sql
AllQuerySpaceViewModels.tt
AsycudaWarehouseToOPS.sql
AsycudaWarehouseVSAsycudaData.sql
AutoBot-Enterprise.sln
AutoBot-Enterprise.sln - Shortcut.lnk
AutoBot-Enterprise.sln.startup.json
AutoBot-Enterprise.v3.ncrunchsolution
AutoBot-EnterpriseDB.sql
AutoBot-Tao Setup.sql
AutoBot-Tao Test query.sql
AutoMatic Index Maintenance.sql
Block Allocations for Adjustments.sql
Bond Balance Report.sql
Bond Balance Work Book.sql
Bond Balance.sql
BondBalance.sql
C71.xsd
Change over to EntryData_Id.sql
Check Allocations.sql
Classifier WorkSheet.sql
Clean Database Total.sql
clean up Entry Previous items.sql
clean up FileMappings.sql
Clean up Inventory Item Alias - for referential integreity.sql
Clean up Inventory Item Alias.sql
Clean up Rogue EntryData.sql
Cleanup FileTypeActions.sql
Clear Allocations.sql
Clear Attachments from DocSet.sql
Clear Brokerage info.sql
Clear Database After Date.sql
Clear duplicate entries.sql
clear duplicate entrypreviousitems.sql
Clear Sales Allocations.sql
clear Sales Data.sql
Clear Short Allocations.sql
Clear Specific Asycuda Sales Allocations.sql
Clear TODO-DiscrepanciesToDoList.sql
clear unattached documents.sql
ClearAdjustments.sql
ClearDatabase.sql
Clone Company ApplicationSettings.sql
Clone FileType.sql
Collate issue.sql
Convert PreviousItems To Sales.sql
Convert Sales to IM7.sql
Create Error OPS.sql
Create Missing Indexes.sql
Customs Quote.xlsx
Database-AdjustToOPS.sql
DataProcessChecks.sql
datetime to datetime2 upgrade needs work.sql
dbo error fix.sql
debug-autobot.bat
debugging-setup-plan.md
Delete Alisa with No item ID.sql
Delete All Asycuda Documents.sql
Delete application settings.sql
Delete asycuda document Attachments.sql
Delete Asycuda Documents after Date.sql
Delete Asycuda Documents before OpeningStockDate.sql
Delete Asycuda Documents before startDate.sql
Delete Certain Asycuda Documents.sql
Delete companies.sql
delete Company from database.sql
delete duplicate inventroryalias.sql
Delete Duplicate Rows.sql
Delete Entries of type in Documentset.sql
Delete non im7 Asycuda Documents.sql
Delete Purchase Orders with no Documentset.sql
Delete UnlinkedPI Asycuda Documents.sql
Delete Unregistered Asycuda Documents.sql
Delete Unregistered Asycuda Items that expired and wrong country.sql
Delete Unregistered Asycuda Items.sql
Delete Unregistered Lines.sql
Detect unreported OPS Differences.sql
DIS 2 ADJ.sql
Discrepancies KIM vs AdjustmentDetails.sql
discrepancies work book.sql
Discrepancy Testing - clean database.sql
DownloadCPSalesDateRange.sql
drop DTA Views.sql
Duplicate EntryPreviousItems Worksheet.sql
duty-free-green_~k16015023.jpg
electronic document.jpg
Email Upgrade.sql
Ex9 vs Sales Differences.sql
Expected Sales Worksheet.sql
Expired Entries WorkBook.sql
Exwarehouse WorkSheet.sql
Ez-Asycuda Toolkit For Grenada Customs.pptx
FileType-DocSetReference-WorkSheet.sql
FileTypeInfo-Delete Type WorkSheet.sql
FileTypeState WorkSheet.sql
Find Long Item Code.sql
Fix Duplicate InventoryItems.sql
Fix invalid office_segment_code.sql
Fix_ConsigneeName_Extraction_And_Action_Order.sql
Freight verification.sql
IM9 Check.sql
Index-Drop all except primary.sql
Index-Drop DuplicateIndex.sql
Insert Destruction DocSet.sql
Insert FileTypeActions.sql
insert inventorymapping.sql
InventoryAsycudaMapping.sql
invoice data.sql
kim item values work sheet.sql
License.xsd
link server.sql
MaintenanceSolution.sql
Mapping Suggestions.sql
Missing Sales worksheet.sql
Most popular suppliers.sql
Outlook temp folder location.txt
POSvsAsycuda Differences.sql
PRE-Ex-warehouse-POSvsAsycuda Differences.sql
prompt.md
Reconcile Discrepancies.sql
Recovery pending issue.sql
Recovery-Restore State.sql
rename Documents.sql
Reorder_FileType109_Actions.sql
Rouge exwarehouse test script.sql
Sales Database Report Double Check for PreAssesed.sql
Sales Database Report Double Check.sql
search query text.sql
ServiceLayer.bat
Shipment MisMatch worksheet.sql
Split-PartialClasses.ps1
SqlSchemaCompare.scmp
summaryviewmodel.txt
.git/
.nuget/
.vs/
.vscode/
AAll-EDMX/
AdjustmentQS/
AllDataSpaceEDMX/
AllocationDS/
AllocationQS/
AllQuerySpaceEDMX/
AsycudaWorld421/
AutoBot/
AutoBot1/
AutoBotUtilities.Tests/
AutoUtilitiesTest/
Common/
Common1 Dlls/
Core.Common/
Core.Common.PDF2TXT/
CoreEntities/
CounterPointQS/
DataLayer/
DocumentDS/
DocumentItemDS/
DocumentQS/
Documents/
DomainInterfaces/
EFAutoRefresh/
EmailDownloader/
EntryDataDS/
EntryDataQS/
Ez-Asycuda-Toolkit/
Ez-Asycuda-Toolkit-Setup/
FunctionModeling/
InventoryDS/
InventoryQS/
LicenseDS/
MigrationBackup/
nuget/
NullEdmx/
packages/
PdfOcr/
PreviousDocumentDS/
PreviousDocumentQS/
QuickBooks/
QuickBooksDS/
RegexImporter/
SalesDataQS/
StoredProcedures/
```

---
# Plan for Robust File Editing (Especially .csproj)

This plan incorporates lessons learned from previous attempts to edit files, particularly `.csproj` files, which were prone to corruption (BOM, spacing issues) and tool failures (`apply_diff`).

1.  **Mode Check:** **ALWAYS** verify you are in the correct mode (`code`) before attempting any file modification (`write_to_file` or `apply_diff`). If not, use `<switch_mode>` first.
2.  **Get Current State:** Use `<read_file>` to get the *complete* and *current* content of the target file (e.g., `WaterNut.Business.Services.csproj`).
3.  **Clean Content:**
    *   Remove any leading/trailing whitespace from the retrieved content.
    *   Explicitly remove known BOM characters (e.g., `﻿`, `��`) from the beginning of the content.
    *   *If necessary (based on prior `read_file` results showing odd spacing):* Normalize internal whitespace (e.g., replace multiple spaces with single spaces where appropriate, fix `c h a r` spacing). *Caution: This requires careful handling to avoid breaking intentional formatting.*
4.  **Apply Logical Changes:** Modify the *cleaned* content in memory to incorporate the desired changes (e.g., add `<ProjectReference>`, add `<Compile Include=...>`, fix `using` statements).
5.  **Calculate Line Count:** Determine the exact number of lines in the *final, modified* content string.
6.  **Execute Write:** Use `<write_to_file>`:
    *   `<path>`: The target file path.
    *   `<content>`: The complete, cleaned, and modified content from step 4.
    *   `<line_count>`: The exact line count calculated in step 5.
7.  **Verify (After ALL Writes):** Once all planned file modifications for a logical step (like fixing all `.csproj` issues, or fixing a specific set of C# errors) are complete and confirmed successful by the user, run a build command (`msbuild /t:Clean /t:Rebuild`) using `<execute_command>` to verify the outcome.