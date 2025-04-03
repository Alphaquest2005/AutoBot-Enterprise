# Plan: File-Based Shipment Workflow

**Overall Goal:** Implement a workflow where a folder containing specific files (PDF, XLSX, and `Info.txt`) can trigger the same shipment processing actions currently associated with incoming emails, ultimately generating the Asycuda XML needed for SikuliX scripts like `SaveIM7`. Modify the `CreateShipmentEmail` process to optionally generate `Info.txt`.

**Context Identifier:** Use the **BL Number** from `Info.txt` as the primary key/context identifier for linking data, serving as a proxy for `EmailId`.

---

## Phase 1: Analysis (Completed)

*   [x] **Task 1:** Identified `EmailMapping` 39 and its linked `FileTypes` (1141, 1143, 1144, 1145, 1148, 1158) as the relevant workflow for standard shipment processing leading to `CreateShipmentEmail`.
*   [x] **Task 2:** Identified the sequence of actions associated with the relevant `FileTypes`: `Xlsx2csv` (13), `SaveCsv` (6), `ImportPDF` (52), `CreateShipmentEmail` (111), `ImportUnAttachedSummary` (112).
*   [x] **Task 3:** Identified `ExportPOEntries` (Action 8) as the likely generator of the Asycuda XML needed for `SaveIM7`.
*   [x] **Task 4:** Analyzed `Shipment`, `ShipmentExtensions`, `ShipmentUtils`, `FolderProcessor`, `ImportUtils`, `FileUtils` to understand data flow, action orchestration, and context handling (`EmailId`).
*   [x] **Task 5:** Analyzed `EmailTextProcessor` (`SaveInfo` action) and determined a dedicated parser is preferable for `Info.txt`.

---

## Phase 2: Design

*   [ ] **Task 6:** Define `Info.txt` Parser Action:
    *   Action Name: `ImportShipmentInfoFromTxt`
    *   Method: `ShipmentUtils.ImportShipmentInfoFromTxt(FileTypes ft, FileInfo[] files)`
    *   Logic:
        *   Find `Info.txt` in `files`.
        *   Parse `Key: Value` pairs.
        *   Extract `BLNumber` (from `BL:` line) as the key identifier.
        *   Find/Create `AsycudaDocumentSet` record using `BLNumber`.
        *   Populate `AsycudaDocumentSet` fields (Consignee, Manifest, Weight, Freight, etc.) from parsed `Info.txt` values.
        *   Save DB changes.
        *   Store the `AsycudaDocumentSet.AsycudaDocumentSetId` in `ft.AsycudaDocumentSetId` for subsequent actions.
        *   Store the `BLNumber` in `ft.Data` with a specific key (e.g., "ShipmentKey") for potential use by other actions.
*   [ ] **Task 7:** Define New `FileType` ("ShipmentFolder"):
    *   `Description`: "Shipment Folder (Info.txt + Files)"
    *   `FilePattern`: `Info.txt` (The presence of this file defines the folder type).
    *   `FolderContains`: `*.xlsx;*.pdf` (Or more specific patterns if known). This might be informational or used by the processor.
    *   `IsImportable`: True
    *   `CreateDocumentSet`: True (Likely needed to group related data).
    *   `DocumentSetReference`: Could be set to use the `BLNumber` extracted from `Info.txt`.
    *   `FileImporterInfoId`: TBD (Needs investigation - likely a generic or new one).
*   [x] **Task 8:** Define `FileTypeActions` Sequence for "ShipmentFolder":
    1.  `ImportShipmentInfoFromTxt` (New Action, ID: 122) - Order 1
    2.  `Xlsx2csv` (Action ID: 13) - Order 2
    3.  `SaveCsv` (Action ID: 6) - Order 3 **(Corrected: Imports data from CSV)**
    4.  `AttachToDocSetByRef` (Action ID: 51) - Order 4
    5.  `ExportPOEntries` (Action ID: 8) - Order 5
*   [ ] **Task 9:** Plan `SaveShipmentInfoToFile` Method:
    *   Create `ShipmentUtils.SaveShipmentInfoToFile(Shipment shipment, string outputDirectory)`.
    *   Extract `BLNumber` from `shipment`.
    *   Generate `Info.txt` content (Key: Value format) including `ShipmentKey: <BLNumber>`. (Using a distinct key like `ShipmentKey` avoids potential parsing conflicts if "BL:" appears elsewhere in the data).
    *   Save to `Path.Combine(outputDirectory, "Info.txt")`.
*   [ ] **Task 10:** Design Trigger Mechanism (Folder Processor):
    *   Modify `AutoBot1/FolderProcessor.cs` (or create new processor).
    *   Watch input folder (e.g., `appSetting.DataFolder + "\ShipmentInput"`).
    *   Detect folders containing `Info.txt`.
    *   Identify "ShipmentFolder" `FileType`.
    *   Read `Info.txt` -> Get `BLNumber`.
    *   Create placeholder `Emails` record (use `BLNumber` for `EmailUniqueId`), get `EmailId`.
    *   Create `Attachments` records linking all files (Info.txt, XLSX, PDFs) to the placeholder `EmailId`.
    *   Set `fileType.EmailId` = placeholder `EmailId`.
    *   Call `ImportUtils.Execute*FileActions` with the `FileType` and `FileInfo[]`.
*   [ ] **Task 11:** Review Actions (`Xlsx2csv`, `ImportPOEntries`, `AttachToDocSetByRef`, `ExportPOEntries`) for compatibility with the new context (placeholder `EmailId`, `Attachments`).

*   **Notes/Learnings (Phase 2):**
    *   _Refine Info.txt fields if necessary._
    *   _Confirm FileImporterInfoId._
    *   _Verify action sequence order and prerequisites._

---

## Phase 3: Implementation & Testing

*   [ ] **Task 12:** Implement Database Changes:
    *   Generate SQL script to add new `Action` (`ImportShipmentInfoFromTxt`).
    *   Generate SQL script to add new `FileType` (`ShipmentFolder`, ID 1186). (`Add_ShipmentFolder_FileType.sql`)
    *   Generate SQL script to add new `FileTypeActions` linking "ShipmentFolder" (1186) to the defined action sequence (122, 13, 6, 51, 8). (`Add_ShipmentFolder_FileTypeActions.sql`)
    *   Generate SQL script to copy `FileTypeMappings` from FileType 1151 to 1186. (`Add_ShipmentFolder_FileTypeMappings.sql`)
    *   Generate SQL script to add new child `FileType` (ID 1187) with `ParentFileTypeId = 1186`. (`Add_ShipmentFolder_ChildFileType.sql`)
    *   Generate SQL script to copy `FileTypeMappings` from FileType 1152 to 1187. (`Add_ShipmentFolder_ChildMappings.sql`)
    *   Update `WebSource-AutoBot Scripts` folder with these scripts.
*   [ ] **Task 13:** Implement C# Code:
    *   `ShipmentUtils.ImportShipmentInfoFromTxt` method.
    *   `ShipmentUtils.SaveShipmentInfoToFile` method.
    *   Add `ImportShipmentInfoFromTxt` to `FileUtils.FileActions`.
    *   Modify `FolderProcessor` logic.
    *   Adapt existing actions if needed (Task 11 verification).
*   [ ] **Task 14:** Implement Integration Test:
    *   Create test method in `SikuliIntegrationTests.cs` or new file.
    *   Use sample folder `AutoBotUtilities.Tests\Test Data\Test Shipment files`.
    *   Simulate `FolderProcessor` steps (create placeholder Email/Attachments).
    *   Trigger action sequence via `ImportUtils`.
    *   Verify generated Asycuda XML file exists and is correct.
    *   Verify relevant database state.
*   [ ] **Task 15:** Commit all changes (DB scripts, C# code, tests).

*   **Notes/Learnings (Phase 3):**
    *   **Test Execution:**
        *   `dotnet test` struggles with output path differences (`bin\x64\Debug` vs `bin\Debug\net48\win-x64`).
        *   `vstest.console.exe` (using the correct `bin\x64\Debug\net48` path) can run tests but filtering (`/Tests:`, `/TestCaseFilter:`) is unreliable for specific NUnit tests in this setup.
        *   VS Test Explorer is currently the most reliable method for running individual tests.
        *   Test `[Setup]` methods calculating paths relative to `Assembly.GetExecutingAssembly().Location` need careful adjustment. For `vstest.console.exe` runs from the project root, `Path.Combine(assemblyDir, "..", "..", "..", "..", "Test Data", ...)` seems correct to reach the project's `Test Data` folder from `bin\x64\Debug\net48`.
    *   **Entity Framework Validation:**
        *   `DbEntityValidationException` occurs if `SaveChanges()` is called on a *new* entity that is missing values for `NOT NULL` columns without database defaults.
        *   For `AsycudaDocumentSet`, `ApplicationSettingsId`, `Exchange_Rate`, and `Currency_Code` must be set *before* the initial `SaveChanges()` when creating a new record. `FreightCurrencyCode` also needs a value but has a DB default ('USD').
        *   For `Emails`, the string PK `EmailId` must be set explicitly (e.g., using `Guid.NewGuid().ToString()`).
        *   For `Attachments`, the int PK `Id` is an IDENTITY column and handled by the database.

---

## General Notes/Learnings

*   The primary key for linking shipment data in the new workflow is the BL Number extracted from `Info.txt`, used as `AsycudaDocumentSet.Declarant_Reference_Number`.
*   A placeholder `Emails` record is created in the `FolderProcessor` to provide context (`EmailId`) for subsequent actions that expect it.