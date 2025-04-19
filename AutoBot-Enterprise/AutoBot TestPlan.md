# AutoBot Test Plan

**Version:** 1.0
**Date:** 2025-04-03

**Goal:** Achieve comprehensive test coverage for the AutoBot application, focusing Integration Tests on core workflows (Email, Download Folder, Shipment Folder) while ensuring Unit Test coverage for individual components and actions.

**Test Data Source:** Utilize SQL scripts from `AutoBot1/WebSource-AutoBot Scripts` (especially configuration scripts like `Add_ShipmentFolder_*.sql`) for DB setup. Create and manage representative mock data files (PDFs, CSVs, TXTs, Info.txt) covering valid, invalid, and edge-case scenarios, stored in a dedicated test data directory.

**Testing Environment:**
*   Requires a dedicated test database schema populated via setup scripts and potentially sample data. Mechanisms for resetting DB state between integration tests are needed.
*   Mocking frameworks (e.g., Moq for .NET) required for unit tests.
*   Controlled file system area for testing folder processing, with cleanup procedures.
*   Mocking external dependencies (Email Server, DeepSeek API, SikuliX) recommended for reliable unit/integration testing.

**Mocking Strategy (Unit Tests):**
*   Utilize mocking frameworks (e.g., Moq) to isolate components.
*   Mock interfaces where available.
*   Create test doubles or wrappers for static classes or sealed dependencies if necessary.
*   Mock database contexts (e.g., using Effort.EF6 or InMemory provider for EF Core if applicable, or repository pattern).
*   Mock file system interactions (e.g., using `System.IO.Abstractions`).
*   Mock external services (`EmailDownloader`, `DeepSeekInvoiceApi`, `Utils.RunSiKuLi`).

**SikuliX (UI Automation) Handling:**
*   Tests relying on SikuliX (`Utils.RunSiKuLi`) are considered End-to-End UI tests.
*   These require a specific, stable environment with the target application UI available.
*   Due to potential brittleness, these might be run separately, manually, or semi-automated.
*   Unit/Integration tests should **mock** the `Utils.RunSiKuLi` call to verify it's invoked correctly with the right parameters, without performing actual UI automation.

---

## I. Configuration & Setup Tests

*   **Unit Tests:**
    *   **`FileTypeManager`:**
        *   Test `GetFileType(id)`: Mock DB context, verify correct `FileType` retrieval by ID, including eager-loaded related entities if applicable. Test caching behavior.
        *   Test `GetImportableFileType(entryType, format, path)`: Mock DB context, test retrieval based on entry type, format, and path matching (regex). Test filtering logic (e.g., `Description == "Unknown"`).
    *   **`FileUtils`/`SessionsUtils`:**
        *   Test `FileActions` dictionary: Verify all expected action names (from DB `Actions` table) exist as keys. Verify delegates point to the correct static methods.
        *   Test `SessionActions` dictionary: Verify all expected session action names exist. Verify delegates point to correct methods.
    *   **`ApplicationSettings` Loading (in `Program.cs`):**
        *   Unit test the logic that constructs the query to load `ApplicationSettings` (if refactored into a testable unit), verifying correct `.Include()` calls for eager loading.
*   **Integration Tests:**
    *   **DB Setup Scripts:** Execute configuration scripts (e.g., `Add_ShipmentFolder_FileType.sql`, `Add_ImportShipmentInfoFromTxt_Action.sql`, etc.) against the test DB. Verify the expected records are created in `FileTypes`, `Actions`, `FileTypeActions`, `EmailMapping`, etc.
    *   **Application Initialization:** Run `Program.Main` against the test DB with various `ApplicationSettings` states (active/inactive, different flags like `TestMode`, `AssessIM7`, `AssessEX`). Verify:
        *   Only active settings are loaded.
        *   `BaseDataModel.Instance.CurrentApplicationSettings` is set correctly within the loop.
        *   Correct initial actions are triggered based on flags (e.g., `ExecuteLastDBSessionAction` in `TestMode`).

---

## II. Core Component Unit Tests

*   **Unit Tests (Mocking Dependencies):**
    *   **`EmailTextProcessor` (`ImportUtils.cs`):**
        *   Test `Execute`: Provide sample text lines and mock `FileType` with `EmailInfoMappings`. Verify correct key-value extraction based on regex (`KeyRegX`, `FieldRx`, `KeyReplaceRx`, `FieldReplaceRx`). Verify extracted data is added to the mock `fileType.Data`. Verify correct SQL `UPDATE` statement generation. Mock `CoreEntitiesContext` to verify `ExecuteSqlCommand` is called with the expected SQL.
    *   **`ImportUtils`:**
        *   Test `ExecuteDataSpecificFileActions`/`ExecuteNonSpecificFileActions`: Mock `CoreEntitiesContext` to return specific `FileTypeActions` based on input `FileTypeId` and `appSetting` flags. Mock `FileUtils.FileActions`. Verify the correct actions are selected, ordered by ID, and `ExecuteActions` is called for each. Test filtering logic (`IsDataSpecific`, `AssessIM7`/`EX`, `TestMode`).
        *   Test `ExecuteEmailMappingActions`: Mock `EmailMapping` with actions. Mock `FileUtils.FileActions`. Verify correct actions are selected based on `TestMode`, ordered by ID, and `ExecuteActions` is called.
        *   Test `ExecuteActions`: Mock `FileUtils.FileActions`. Test the `ProcessNextStep` logic: provide `fileType.ProcessNextStep` list, verify sequential execution, handling of "Continue", handling of missing actions in the dictionary, and correct invocation of the main action delegate. Test logging/stopwatch usage.
    *   **`FolderProcessor`:**
        *   Test `ProcessFile`: Mock file system operations (`DirectoryInfo`, `FileInfo`, `File`). Mock `FileTypeManager.GetUnknownFileTypes`. Mock `PDFUtils.ImportPDF`/`ImportPDFDeepSeek`. Mock `ShipmentUtils.CreateShipmentEmail`. Mock `EmailDownloader` for `NotifyUnknownPDF`. Verify correct sequence of calls (copy, get types, import, create email/notify, delete). Test error handling for file operations and import failures.
        *   Test `ProcessShipmentFolders`: Mock file system. Mock `CoreEntitiesContext` for `FileType` lookup and saving `Emails`/`Attachments`. Mock `ImportUtils` methods. Verify `Info.txt` reading, BL# extraction, placeholder record creation, action execution, and folder archiving/error moving logic.
    *   **`PDFUtils`:**
        *   Test `ImportPDF`: Mock `CoreEntitiesContext` for attachment lookup. Mock `InvoiceReader.Import`. Verify correct parameters passed to `InvoiceReader` and result handling.
        *   Test `ImportPDFDeepSeek`: Mock `InvoiceReader.GetPdftxt`. Mock `DeepSeekInvoiceApi`. Mock `FileTypeManager.GetFileType`. Mock `DataFileProcessor.Process` (via `ImportSuccessState`). Verify text extraction, API call, default value setting (`SetFileTypeMappingDefaultValues`), and processing call.
        *   Test `AttachEmailPDF`: Mock `BaseDataModel.AttachEmailPDF`. Verify it's called with correct parameters.
        *   Test `LinkPDFs`: Mock `CoreEntitiesContext` `Database.SqlQuery` for `Stp_TODO_ImportCompleteEntries`. Mock `BaseDataModel.LinkPDFs`. Verify SP execution and call to `BaseDataModel`.
        *   Test `ReLinkPDFs`: Mock `CoreEntitiesContext` for `Attachments`, `AsycudaDocuments`, `AsycudaDocument_Attachments`. Provide mock PDF files. Verify regex matching, existing link checks, and creation of new `Attachments`/`AsycudaDocument_Attachments` records.
        *   Test `DownloadPDFs`: Mock `CoreEntitiesContext` for SP/`AsycudaDocumentSetExs`. **Mock `Utils.RunSiKuLi`**. Mock `ImportPDFComplete` helper logic. Verify loop condition and parameters passed to Sikuli runner call.
    *   **`ShipmentUtils`:**
        *   Test `ImportShipmentInfoFromTxt`: Provide mock `Info.txt` content. Mock `DocumentDSContext`. Verify finding/creating `AsycudaDocumentSet`, `xcuda_ASYCUDA_ExtendedProperties`, `xcuda_ASYCUDA`, `xcuda_Transport`, `xcuda_Declarant`. Verify correct fields are updated based on `Info.txt`. Verify `SaveChanges` is called. Verify the input `FileTypes` object is updated correctly. Test error handling for missing "BL" or "Currency".
        *   Test `CreateShipmentEmail`: Mock `EntryDataDSContext` for SP execution and saving attachments. Mock `Shipment` class and its extension methods (`Load*`, `AutoCorrect`, `ProcessShipment`). Mock `CoreEntitiesContext` for contacts. Mock `EmailDownloader`. Verify SP execution, `Shipment` method calls, email sending, attachment saving, and error handling (`BaseDataModel.EmailExceptionHandler`).
        *   Test `MapUnClassifiedItems`: Mock `InventoryDSContext`. Mock `CSVUtils.CSV2DataTable`. Mock `EntryDocSetUtils.SetFileTypeDocSetToLatest`. Verify `InventoryItems.TariffCode` update logic and call to `SetFileTypeDocSetToLatest`.
        *   Test `Submit*` methods (`SubmitUnclassifiedItems`, `SubmitIncompleteSuppliers`, `SubmitInadequatePackages`): Mock `CoreEntitiesContext` for view queries and contacts. Mock `ExportToCSV`. Mock `EmailDownloader`. Mock `Utils.GetDocSetActions`. Verify view querying, report generation, duplicate submission checks, and email sending logic.
        *   Test `UpdateSupplierInfo`: Mock `EntryDataDSContext`. Mock `CSVUtils.CSV2DataTable`. Verify `Suppliers` record update logic.
        *   Test `ClearShipmentData`: Mock `EntryDataDSContext`. Verify `ExecuteSqlCommand` is called with the correct `DELETE` statements.
        *   Test `SaveShipmentInfoToFile`: Mock `File.WriteAllText`. Provide mock `Shipment` object. Verify correct `Info.txt` content generation.
    *   **Other Utility Classes (`CSVUtils`, `POUtils`, `DISUtils`, `C71Utils`, `LICUtils`, etc.):**
        *   Unit test the specific logic within each action method defined in `FileUtils.FileActions`, mocking database contexts, file system, emailers, **SikuliX (`Utils.RunSiKuLi`)**, and other dependencies as needed.

---

## III. Core Workflow Integration Tests (High Priority)

*   **Integration (Test DB, Controlled File System, Mocked External Services - Email, DeepSeek API):**
    *   **Email Workflow:**
        *   **Scenario 1 (Simple CSV):** Setup: `ApplicationSettings`, `EmailMapping` for CSV, `FileType` for CSV, `FileTypeActions` mapping "SaveCsv" action. Test: Simulate email arrival (create `Emails`, `Attachments` records pointing to a test CSV file). Run `Program.ProcessEmails`. Verify: `FileType` identified, `SaveCsv` action executed, data saved correctly to target tables via `SaveCSVModel` (check test DB).
        *   **Scenario 2 (Text Extraction):** Setup: `EmailMapping` with `EmailInfoMappings`, `FileType` linked to mapping, `FileTypeActions` mapping "SaveInfo" action. Test: Simulate email arrival (create records pointing to test text file). Run `Program.ProcessEmails`. Verify: `FileType` identified, `EmailTextProcessor.Execute` runs, target DB record updated based on extracted text.
        *   **Scenario 3 (Multiple Actions/Deferred):** Setup: `EmailMapping` (IsSingleEmail=false), multiple `FileTypes`, mix of data-specific and non-specific actions, including one deferred non-specific action. Test: Simulate email arrival. Run `Program.ProcessEmails`. Verify: Data-specific actions run per file type. Non-specific actions are grouped by `AsycudaDocumentSetId` and run *after* all file types for that email are processed. Verify correct execution order based on `FileTypeAction.Id`.
        *   **Scenario 4 (ProcessNextStep):** Setup: `FileTypeActions` with `ProcessNextStep` list including "Continue". Test: Simulate email arrival. Run `Program.ProcessEmails`. Verify: Actions in `ProcessNextStep` run sequentially before the main action, stopping at "Continue".
    *   **Download Folder Workflow:**
        *   **Scenario 1 (Successful PDF Import):** Setup: `ApplicationSettings`, "Unknown" PDF `FileType`. Place valid test PDF in test `Downloads` folder. Test: Run `FolderProcessor.ProcessDownloadFolder`. Verify: PDF copied to `Documents` subfolder, `PDFUtils.ImportPDF` called (mock `InvoiceReader` success), `ShipmentUtils.CreateShipmentEmail` called (mock `EmailDownloader`), original PDF deleted.
        *   **Scenario 2 (DeepSeek Fallback):** Setup: As above. Test: Run `FolderProcessor.ProcessDownloadFolder`. Mock `InvoiceReader.Import` failure. Mock `DeepSeekInvoiceApi` success. Mock `DataFileProcessor.Process` success. Verify: `ImportPDF` fails, `ImportPDFDeepSeek` called, `CreateShipmentEmail` called, original PDF deleted.
        *   **Scenario 3 (Import Failure):** Setup: As above. Test: Run `FolderProcessor.ProcessDownloadFolder`. Mock `ImportPDF` failure. Mock `ImportPDFDeepSeek` failure. Verify: `NotifyUnknownPDF` called (mock `EmailDownloader`), original PDF *not* deleted.
    *   **Shipment Folder Workflow:**
        *   **Scenario 1 (Successful Processing):** Setup: Test DB with "ShipmentFolder" `FileType` (ID 1186) and associated `FileTypeActions` including "ImportShipmentInfoFromTxt". Create test subfolder in `ShipmentInput` with valid `Info.txt` (including BL, Currency) and other files. Test: Run `FolderProcessor.ProcessShipmentFolders`. Verify: `Info.txt` parsed, placeholder `Emails`/`Attachments` created, `ShipmentUtils.ImportShipmentInfoFromTxt` executed, `AsycudaDocumentSet` and related `xcuda_*` records created/updated in test DB, folder moved to `Archive`.
        *   **Scenario 2 (Missing Info.txt):** Setup: Create test subfolder without `Info.txt`. Test: Run `FolderProcessor.ProcessShipmentFolders`. Verify: Folder is skipped, no DB changes, folder remains in `ShipmentInput`.
        *   **Scenario 3 (Missing BL/Currency in Info.txt):** Setup: Create test subfolder with `Info.txt` missing required "BL" or "Currency". Test: Run `FolderProcessor.ProcessShipmentFolders`. Verify: Error logged, folder moved to `Error` directory.
        *   **Scenario 4 (Action Failure):** Setup: Valid folder/`Info.txt`. Mock a failure within the `ImportShipmentInfoFromTxt` action logic. Test: Run `FolderProcessor.ProcessShipmentFolders`. Verify: Error logged, folder moved to `Error` directory.

---

## IV. Other Integration Tests (Lower Priority)

*   **Integration:**
    *   **Session Actions:** Perform basic tests confirming actions linked to `SessionSchedule` (past due) or "End" session are triggered by `ExecuteDBSessionActions`. Verify actions triggered by `AssessIM7`/`AssessEX` flags run when no schedule is due. Detailed outcome verification relies on Unit Tests.
    *   **Specific Action Integration:** Select a few non-core-workflow actions (e.g., `RecreatePOEntries`, `SubmitUnclassifiedItems`) and run targeted integration tests verifying their interaction with the DB and expected side effects, assuming their trigger mechanism (e.g., via email workflow) works based on core workflow tests.

---

## V. Error Handling & Reporting Tests

*   **Unit:** Test specific `try-catch` blocks, logging calls, and exception handling logic within methods (e.g., `BaseDataModel.EmailExceptionHandler` calls).
*   **Integration:**
    *   Inject errors into core workflows (invalid files, DB errors, mocked API failures). Verify expected error logging, email notifications (mocked), and state changes (e.g., files not deleted, folders moved to error).
    *   Trigger reporting actions (e.g., `SubmitUnclassifiedItems`) with data that should generate a report. Verify the report generation logic runs and email sending is attempted (mocked).

---