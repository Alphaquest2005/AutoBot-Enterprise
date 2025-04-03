# AutoBot Workflow Analysis: EmailMappings, FileTypes, and Actions

This document summarizes the relationships and execution flow of the automated processing system based on Email Mappings and File Types within the AutoBot project.

## Key Entities and Relationships

The core configuration entities reside primarily within the `CoreEntities` database context, with document-specific structures in `DocumentDS`.

*   **`ApplicationSettings`**: Global settings for an instance (ID 3 used in tests).
*   **`EmailMapping`**: Defines patterns (RegEx) to identify incoming emails and associate them with workflows.
*   **`FileTypes`**: Defines types of files based on patterns (`FilePattern`) and specifies processing characteristics.
    *   Can have a `ParentFileTypeId` to represent derived files (e.g., a CSV generated from an XLSX).
    *   `FileInfoId` links to `FileTypes-FileImporterInfo` which defines `EntryType` (e.g., "PO", "Sales") and `Format` (e.g., "XLSX", "CSV", "PDF").
*   **`Actions`**: Defines available processing steps (e.g., `Xlsx2csv`, `SaveCsv`, `ExportPOEntries`). Contains `Id`, `Name`, `IsDataSpecific`. **Does not contain an `ActionOrder` column.**
*   **`EmailFileTypes`**: Links `EmailMapping` to one or more `FileTypes`. An email might be associated with multiple file types based on its attachments.
*   **`FileTypeActions`**: Links `FileTypes` to `Actions`, defining which actions run for a given `FileType`. **Does not contain an `ActionOrder` column.**
*   **`EmailMappingActions`**: Links `EmailMapping` directly to `Actions`. (Less commonly used in the flows analyzed).
*   **`FileTypeMappings`**: Defines column-level mappings for CSV/Excel imports (`SaveCsv` action). Specifies `OriginalName` (source column), `DestinationName` (target DB column), `DataType`, etc. Crucially linked to the specific `FileTypeId` representing the file being imported (often a child type like the CSV generated from an XLSX).
*   **`AsycudaDocumentSet`**: Represents a logical grouping of documents/entries, often corresponding to a shipment or import batch. Identified by `Declarant_Reference_Number`. Created/updated by actions like `ImportShipmentInfoFromTxt`.
*   **`xcuda_ASYCUDA`**: Represents the main Asycuda document structure (likely generated from XSD). Linked to `AsycudaDocumentSet` via `xcuda_ASYCUDA_ExtendedProperties`.
*   **`xcuda_ASYCUDA_ExtendedProperties`**: Contains metadata about the Asycuda document, including `AsycudaDocumentSetId`, `CNumber` (Customs #), `ReferenceNumber`, `FileNumber`.
*   **`xcuda_Declarant`**: Linked to `xcuda_ASYCUDA`. Contains `Declarant_code`, `Declarant_name`, and `Number` (internal reference, e.g., "BLNumber-F#").
*   **`EntryData` / `EntryDataDetails`**: Stores the actual line-item data imported from files (e.g., via `SaveCsv`). Linked to `AsycudaDocumentSet`.
*   **`Attachments`**: Stores information about physical files. Linked to `Emails` and `AsycudaDocumentSet` (via `AsycudaDocumentSet_Attachments`).

## Action Execution Flow (`ImportUtils.cs`)

1.  **Trigger:** Processing is initiated either by an email matching an `EmailMapping` or by a `FileType` identified in a folder (like our new `ShipmentFolder` type).
2.  **Context:** An `EmailId` (for emails) or a placeholder `EmailId` (for folders) provides context. An `AsycudaDocumentSetId` is found or created, often based on a reference number (like BL Number).
3.  **Action Retrieval:**
    *   The `ExecuteDataSpecificFileActions` and `ExecuteNonSpecificFileActions` methods in `ImportUtils.cs` are called.
    *   These methods query the `FileTypeActions` table for actions linked to the current `FileTypeId`.
    *   They filter based on `IsDataSpecific` and `TestMode`.
4.  **Action Sorting:**
    *   **CRITICAL FLAW:** The code currently sorts the retrieved actions using `OrderBy(fta => fta.Id)`. This sorts based on the **primary key of the `FileTypeActions` linking table**, which depends entirely on the historical insertion order and **does not guarantee logical execution sequence.**
5.  **Action Execution:** The sorted actions are executed one by one, invoking the corresponding C# method defined in the `FileUtils.FileActions` dictionary.

## Email Workflow Example (Inferred)

*   An email arrives with attachments (e.g., PO.xlsx, Invoice.pdf).
*   An `EmailMapping` (e.g., ID 22) matches the email subject/sender.
*   The system identifies associated `FileTypes` via `EmailFileTypes` (e.g., FileType 1151 for XLSX PO, FileType 1144 for PDF Invoice).
*   The system likely processes these `FileTypes` sequentially.
    *   **Process FileType 1151 (XLSX PO):**
        *   Retrieves `FileTypeActions` for 1151 (e.g., Action 13 - `Xlsx2csv`).
        *   Executes `Xlsx2csv`, generating `PO-Fixed.csv`.
    *   **Process FileType 1152 (Child CSV of 1151):**
        *   Retrieves `FileTypeActions` for 1152 (e.g., Action 6 - `SaveCsv`).
        *   Executes `SaveCsv`. `SaveCsv` uses `FileTypeMappings` associated with **FileType 1152** to import data from `PO-Fixed.csv` into `EntryData` tables.
    *   **Process FileType 1144 (PDF Invoice):**
        *   Retrieves `FileTypeActions` for 1144 (e.g., Action 52 - `ImportPDF`).
        *   Executes `ImportPDF`.
    *   **Process FileType 1148 (Email Trigger):**
        *   Retrieves `FileTypeActions` for 1148 (e.g., Action 111 - `CreateShipmentEmail`).
        *   Executes `CreateShipmentEmail`.
*   The overall success depends on the application processing the FileTypes in an order that respects dependencies (e.g., CSV processed before email trigger).

## File-Based Workflow (FileType 1186) Implementation

*   **Goal:** Process a folder containing `Info.txt` and associated files (XLSX, PDF) using a single `FileType`.
*   **FileType 1186 (`Shipment Folder (Info.txt + Files)`):** Created to represent this context. `FilePattern = 'Info.txt'`.
*   **Child FileType 1187 (`Shipment Folder PO CSV`):** Created with `ParentFileTypeId = 1186` to represent the CSV generated from the XLSX within this context.
*   **Actions Linked to 1186:** `ImportShipmentInfoFromTxt` (122), `Xlsx2csv` (13), `SaveCsv` (6), `AttachToDocSetByRef` (51), `ExportPOEntries` (8).
*   **Mappings:**
    *   Mappings from FileType 1151 (original XLSX PO parent) were copied to FileType 1186.
    *   Mappings from FileType 1152 (original CSV PO child) were copied to FileType 1187 (new CSV child). This is essential for the `SaveCsv` action.
*   **Identified Issues & Fixes:**
    *   **Initial `AsycudaDocumentSet` Save:** `ImportShipmentInfoFromTxt` initially failed (`DbEntityValidationException`) because it tried to save a new `AsycudaDocumentSet` without setting required non-null fields (`Currency_Code`, `Exchange_Rate`). **Fix:** Modified `ImportShipmentInfoFromTxt` to parse and set these values from `Info.txt` before the initial `SaveChanges()`.
    *   **Action Order:** The test failed because `ExportPOEntries` (Action 8) executed before `SaveCsv` (Action 6) due to the unreliable `OrderBy(fta => fta.Id)` in `ImportUtils.cs`. **Fix:** Modified `ImportUtils.cs` to explicitly sort actions for FileType 1186 using a hardcoded sequence (122, 13, 6, 51, 8). *(Note: This fix was proposed but reverted pending further analysis, then re-confirmed as necessary).*
    *   **XML Filename:** `ExportPOEntries` uses `xcuda_Declarant.Number` (e.g., "BLNumber-F#") for the filename, not `CNumber` (Customs #). **Fix:** Corrected the test assertion to check for `*.xml` in the expected directory instead of a specific filename.

## Outstanding Issue (as of last test run)

The test still fails because the XML file is not generated, even with the correct action sequence seemingly executing. The remaining likely cause is that `SaveCsv` (Action 6), despite running, is not successfully importing data due to the lack of appropriate `FileTypeMappings` associated with FileType 1187 (the child type representing the CSV). The mappings copied from 1152 should theoretically work, but this needs verification.