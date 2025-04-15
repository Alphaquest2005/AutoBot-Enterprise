# AutoBot Knowledge Base

## Overview

*   **Objective:** This application appears to automate processing based on application settings. It primarily focuses on checking emails, downloading attachments, processing files based on predefined types and rules, interacting with a database (likely for customs/shipping documents - Asycuda), and executing scheduled or triggered database actions. It *can* also process files dropped into specific folders (`Downloads`, `ShipmentInput`), although the `Downloads` folder processing is currently commented out in `Program.Main` and the `ShipmentInput` processing trigger is external/unidentified in the main loop. Processing involves parsing files (PDFs, text, Excel), extracting data using regex or external tools/APIs (like `InvoiceReader`, `DeepSeekInvoiceApi`), executing configurable actions stored in the database (mapped to C# methods via `FileUtils.FileActions` for file-based triggers and `SessionsUtils.SessionActions` for session/schedule-based triggers), and updating database records. It also includes functionality for linking documents, downloading files (potentially via UI automation like SikuliX), reporting on data issues (unclassified items, incomplete suppliers, inadequate packages), and processing shipment information from various sources.
*   **Core Workflow:**
    1.  Load active `ApplicationSettings`.
    2.  For each setting:
        a.  Optionally run the last DB session action (if in Test Mode) using `SessionsUtils.SessionActions`.
        b.  Process emails associated with the setting (`ProcessEmails` in `Program.cs`). This involves:
            i.  Downloading emails/attachments (`EmailDownloader`).
            ii. Executing initial actions based on `EmailMapping` (`ImportUtils.ExecuteEmailMappingActions`, using `FileUtils.FileActions` for lookup).
            iii. Processing associated `FileTypes` for each email:
                *   Getting `FileType` definition (`FileTypeManager`).
                *   Finding relevant files.
                *   Potentially creating `AsycudaDocumentSet`.
                *   Saving attachments (`Utils.SaveAttachments`).
                *   Executing data-specific actions (`ImportUtils.ExecuteDataSpecificFileActions`, using `FileUtils.FileActions`).
                *   Executing non-specific actions (immediately or deferred) (`ImportUtils.ExecuteNonSpecificFileActions`, using `FileUtils.FileActions`).
        c.  Execute general DB session actions (`ExecuteDBSessionActions` in `Program.cs`, using `SessionsUtils.SessionActions` dictionary based on `SessionSchedule` or `appSetting` flags).
        d.  *(Currently Commented Out in Program.Main)* Process individual PDF files in a `Downloads` folder (`FolderProcessor.ProcessDownloadFolder`). This involves:
            i.  Copying the PDF to a structured `Documents` folder.
            ii. Identifying "Unknown" `FileTypes` (`FileTypeManager`).
            iii. Attempting PDF import (`PDFUtils.ImportPDF` -> `InvoiceReader.Import`).
            iv. If import fails, attempting deep seek import (`PDFUtils.ImportPDFDeepSeek` -> `DeepSeekInvoiceApi` -> `DataFileProcessor.Process`).
            v. If successful, potentially creating a shipment email/report (`ShipmentUtils.CreateShipmentEmail`).
            vi. Deleting the original PDF if processing succeeded.
        e.  *(External Trigger)* Process shipment folders in `ShipmentInput` (`FolderProcessor.ProcessShipmentFolders` - *Note: Not called from `Program.Main` loop*). This involves:
            i.  Reading `Info.txt` for BL number and other details.
            ii. Getting a specific "ShipmentFolder" `FileType` (`FileTypeManager`).
            iii. Creating placeholder `Emails` and `Attachments` records.
            iv. Executing data-specific and non-specific actions (`ImportUtils`, using `FileUtils.FileActions`). Key action here is likely `ShipmentUtils.ImportShipmentInfoFromTxt` which updates/creates `AsycudaDocumentSet` and related records based on `Info.txt`.
            v. Archiving or moving the folder based on success/failure.
*   **Key Components:**
    *   `Program.cs`: Entry point, main loop orchestrator for email/DB actions.
    *   `CoreEntitiesContext`: Entity Framework DB context for core entities (Settings, FileTypes, Emails, Attachments, Contacts, TODO views).
    *   `DocumentDSContext`: DB context for document-related entities (`AsycudaDocumentSet`, `xcuda_ASYCUDA`, `xcuda_Declarant`, `xcuda_Transport`, etc.).
    *   `AllocationDSContext`: DB context for allocation-related entities (Inventory Aliases).
    *   `EntryDataDSContext`: DB context for entry data entities (`Suppliers`, `ShipmentBL`, `ShipmentInvoice`, `entrydata`, `ShipmentManifest`).
    *   `ApplicationSettings`: Configuration entity driving the process.
    *   `EmailDownloader`: Handles fetching emails and attachments, sending notifications.
    *   `FileTypeManager` (`WaterNut.Business.Services/Utils/FileTypeManager.cs`): Manages `FileTypes` definitions, caching, and identification based on format, entry type, filename patterns, or content headings.
    *   `ImportUtils` (`AutoBot/ImportUtils.cs`): Orchestrates execution of database-defined actions based on context (`FileType`, `EmailMapping`). Relies heavily on `FileUtils.FileActions`.
    *   `EmailTextProcessor` (`AutoBot/ImportUtils.cs`): Extracts data from text files using regex defined in `EmailInfoMappings` and updates the database.
    *   `FileUtils` (`AutoBot/FileUtils.cs`): **Crucial component.** Contains the static `FileActions` dictionary mapping action names (strings from DB) to C# `Action` delegates (lambda expressions or method groups).
    *   `SessionsUtils` (`AutoBot/SessionsUtils.cs`): **Crucial component.** Contains the static `SessionActions` dictionary mapping action names (strings from DB, likely same pool as `FileActions`) to parameterless C# `Action` delegates. Used for scheduled/triggered actions.
    *   `FolderProcessor` (`AutoBot1/FolderProcessor.cs`): Handles processing files found in `Downloads` and `ShipmentInput` folders.
    *   `BaseDataModel`: Singleton/static class holding current application state/settings and potentially helper methods (`AttachEmailPDF`, `LinkPDFs`, `EmailExceptionHandler`).
    *   `PDFUtils` (`AutoBot/PDFUtils.cs`): Handles PDF import orchestration (`ImportPDF`, `ImportPDFDeepSeek`), linking (`LinkPDFs`, `ReLinkPDFs`), attachment (`AttachEmailPDF`), and potentially downloading via UI automation (`DownloadPDFs`).
    *   `InvoiceReader` (`WaterNut.Business.Services.Importers?`): Performs core PDF data extraction (called by `PDFUtils.ImportPDF` and `ImportPDFDeepSeek`).
    *   `DeepSeekInvoiceApi` (`WaterNut.Business.Services.Utils.DeepSeek?`): External API client for alternative PDF data extraction (called by `PDFUtils.ImportPDFDeepSeek`).
    *   `DataFileProcessor` (`WaterNut.Business.Services.Importers?`): Processes data extracted by `DeepSeekInvoiceApi` (called by `PDFUtils.ImportPDFDeepSeek`).
    *   `ShipmentUtils` (`AutoBot/ShipmentUtils.cs`): Handles processing shipment data, reporting issues (unclassified items, suppliers, packages), importing from `Info.txt`, and generating shipment summary emails/reports.
    *   `Shipment` class (`AutoBot/ShipmentExtensions.cs`?): Likely holds aggregated shipment data and has extension methods for loading data from various sources (`LoadEmailPOs`, `LoadDBBL`, etc.) and processing (`AutoCorrect`, `ProcessShipment`).
    *   `WaterNut.DataSpace.Utils` (`WaterNut.Business.Services/Utils/Utils.cs`): Contains various helper methods (DB lookups, packing list generation, SikuliX runner).
    *   Various other Utility Classes (`DocumentUtils`, `AllocateSalesUtils`, `ADJUtils`, `DISUtils`, `EX9Utils`, `CSVUtils`, `POUtils`, `XLSXProcessor`, `EntryDocSetUtils`, `SubmitSalesXmlToCustomsUtils`, `C71Utils`, `LICUtils`, `UpdateInvoice`, `ImportWarehouseErrorsUtils`, `SQLBlackBox`, etc. located in `AutoBot/` or `WaterNut.Business.Services/Utils/`): Contain the actual implementation logic invoked by the action dictionaries.
    *   SikuliX Scripts (e.g., "IM7-PDF"): External UI automation scripts called by `WaterNut.DataSpace.Utils.RunSiKuLi`.

## Understanding Data Relationships

*   **EDMX Files:** The primary source for understanding entity relationships and database structures are the Entity Data Model XML (`.edmx`) files located in various project folders (e.g., `AAll-EDMX`, `AllDataSpaceEDMX`, `DocumentDS`, `EntryDataDS`, `InventoryDS`, etc.). These visually represent tables, columns, primary keys, foreign keys, and navigation properties.
*   **SQL Scripts:** The `.sql` files located in the root directory and subfolders (especially `AutoBot1/WebSource-AutoBot Scripts/`) provide crucial context:
    *   **Schema:** How tables, views, and stored procedures are created or altered.
    *   **Stored Procedures:** Encapsulated database logic (e.g., `[dbo].[PreProcessShipmentSP]`, `[dbo].[Stp_TODO_ImportCompleteEntries]`). Understanding their parameters and output is key.
    *   **Views:** Predefined queries often used for reporting or specific data retrieval tasks (e.g., `TODO_SubmitUnclassifiedItems`, `TODO_PODocSetToExport`, `ToDo_POToXML`). These often simplify complex joins.
    *   **Data Manipulation:** Scripts for cleaning up data, migrating data, or performing specific updates reveal how different entities interact.
*   **Strategy:** Cross-reference the EDMX diagrams with how entities are used in SQL scripts (especially views and SPs) and C# code (LINQ queries in utility methods) to build a comprehensive picture of data flow and dependencies. For example, understanding how `AsycudaDocumentSet` (from `DocumentDS`) relates to `ShipmentBL` or `ShipmentInvoice` (from `EntryDataDS`) requires looking at both the EDMX files and the code/scripts that join or manipulate these entities (like `ShipmentUtils.ImportShipmentInfoFromTxt`).

## OCR / PDF Processing (`InvoiceReader`, `Part`, `Invoice`)

*   **Core Classes:** `InvoiceReader.cs`, `Invoice.cs`, `Part.cs`, `Line.cs`, `Field.cs` (within `WaterNut.Business.Services/Custom Services/DataModels/Custom DataModels/PDF2TXT/`).
*   **Workflow:**
    1.  `PDFUtils.ImportPDF` calls `InvoiceReader.Import`.
    2.  `InvoiceReader.Import` gets PDF text (`InvoiceReader.GetPdftxt`), creates an `Invoice` object (which initializes `Part` objects based on DB config), and calls `Invoice.Read(textLines)`.
    3.  `Invoice.Read` iterates through text lines, calling `Part.Read` for each top-level part.
    4.  `Part.Read` manages state (`_instance`, `WasStarted`), finds start/end lines based on regex (`FindStart`), extracts field values using regex (`Line.Read`), and recursively calls `Part.Read` for child parts, passing the relevant text lines and the *parent's effective instance*.
    5.  After all lines are processed by `Invoice.Read`, it calls `Invoice.SetPartLineValues` for each top-level part.
    6.  `Invoice.SetPartLineValues` recursively calls itself for child parts first to gather all child data. Then, it iterates through the distinct instances found across the parent and its children, populates parent fields for that instance, filters the pre-gathered child data for items matching the current parent instance, and attaches the relevant child items (correctly structured) to the parent item.
    7.  The final list of assembled parent items (with nested child data) is returned up the chain by `SetPartLineValues`.
    8.  `Invoice.Read` receives the list(s) from `SetPartLineValues` (one list per top-level part), combines them (`SelectMany`), and wraps the final list of dictionaries in `List<dynamic>` for the caller (`InvoiceReader`).
*   **Instance Handling & Recurring Parts (Key Learnings):**
    *   **Challenge:** Parsing documents where sections repeat (like multiple invoices in one PDF, e.g., Shein) requires careful instance management. A parent part (e.g., invoice header) might recur, while its child part (e.g., invoice details) might not, or vice-versa.
    *   **`Part.Read` Instance Logic:**
        *   Uses `_instance` to track the internal instance count for the *current* part, incremented when its own `Start` condition is met again.
        *   Accepts `parentInstance` to know the context from which it was called.
        *   Calculates `effectiveInstance`: This is the instance number used for processing lines and passing down to children. It's typically the `parentInstance`, *unless* the current part is itself recurring *and* just found a new start, in which case it uses its own newly incremented `_instance`.
    *   **`Part.Read` Reset Logic:** When a recurring, non-composite parent part detects the end of its section or a new start, it resets its own state (`_startlines`, `_lines`, etc.). Crucially, it should **only reset child parts if the child part itself is configured as recurring** (`child.OCR_Part.RecuringPart != null`). Resetting non-recurring children prematurely discards their buffered data needed for multi-line field extraction.
    *   **`SetPartLineValues` Aggregation:** This method is responsible for assembling the final structure. It must:
        1.  Recursively call itself for children first to get *all* child instances.
        2.  Determine *all* distinct instance numbers present across the parent and its children.
        3.  Iterate through each distinct `currentInstance`.
        4.  Populate the parent's fields for that `currentInstance`.
        5.  Filter the results from the recursive child calls to find child items where `childItem["Instance"] == currentInstance`.
        6.  Attach these *filtered* child items to the parent item for the `currentInstance`.
    *   **Data Structure Consistency:** Ensure child data is attached in the expected format (e.g., wrapping single non-recurring child items in a `List` if the target field expects a list).
*   **Debugging OCR:**
    *   Use extensive `Console.WriteLine` logging within `Invoice.Read`, `Part.Read`, `Line.Read`, and `SetPartLineValues`.
    *   Clearly log Part IDs, Line IDs, internal instance (`_instance`), parent instance (`parentInstance`), and effective instance (`effectiveInstance`) at method entry/exit and key decision points (like calling child `Read` or `Reset`).
    *   Log the lines being processed and the regex matches found (or lack thereof).
    *   Test with specific problematic text sections to isolate issues.

## General Strategy for Complex Tasks

1.  **Trace the Workflow:** Start from the entry point (`Program.Main`) or the relevant trigger (e.g., a specific `FileTypeAction` name) and follow the execution path.
2.  **Identify Key Orchestrators:** Understand the roles of classes like `ImportUtils`, `PDFUtils`, `FolderProcessor`, `ShipmentUtils`, and `BaseDataModel`.
3.  **Understand Action Mapping:** Recognize that `FileUtils.FileActions` and `SessionsUtils.SessionActions` are central to how database configurations trigger C# code. Look up action names in these dictionaries to find the implementing method.
4.  **Analyze Utility Methods:** Drill down into the specific static methods in utility classes that perform the actual work for a given action.
5.  **Examine Data Structures:** For data-related tasks, consult the EDMX files and relevant SQL scripts (views, SPs, table definitions) to understand entity relationships, constraints, and how data is queried or manipulated.
6.  **Focus on State and Recursion (Parsing):** For parsing issues (like OCR), pay close attention to how state is managed across method calls (e.g., instance counters, line buffers in `Part.cs`, `Line.cs`) and how recursion is handled (e.g., `Part.Read` calling itself for children, `SetPartLineValues` calling itself).
7.  **Isolate and Log:** Use specific inputs that trigger the issue and add detailed logging (`Console.WriteLine`) to trace variable values, instance numbers, and execution flow through the relevant classes.
8.  **Cross-Reference:** Combine insights from code structure, database schema (EDMX/SQL), action configurations (DB tables like `Actions`, `FileTypeActions`), and runtime logs to build a complete understanding.

## Analysis: `AutoBot1/Program.cs`

*   **Role:** Acts as the main application entry point, orchestrating processes like email checking (`ProcessEmails`), scheduled database actions (`ExecuteDBSessionActions`), and download folder processing (`FolderProcessor`) based on `ApplicationSettings` and database configurations.

*   **Namespace:** `AutoBot`
*   **Class:** `Program` (partial)
*   **Static Property:** `ReadOnlyMode` (bool, default false) - Global flag to prevent database/file system modifications during processing. Used in `ProcessEmails`.
*   **Method: `Main(string[] args)` (async Task)**
    *   **Lines 38-113:** Main execution block with global try-catch.
    *   **Line 40:** Initializes Z.EntityFramework.Extensions license.
    *   **Line 45:** Creates `CoreEntitiesContext` instance (`ctx`). Database interaction scope.
    *   **Line 49:** Sets DB command timeout to 10 seconds.
    *   **Lines 51-70:** Loads all active `ApplicationSettings` with numerous related entities eagerly loaded (`FileTypes`, `Declarants`, `EmailMapping`, etc.) using `AsNoTracking()`. This suggests settings are read but not modified here.
    *   **Lines 74-96:** Loop through each `appSetting`.
    *   **Line 79:** Updates `appSetting.DataFolder` path for the current user.
    *   **Line 82:** Sets `BaseDataModel.Instance.CurrentApplicationSettings = appSetting`. Critical for context in other parts of the application.
    *   **Line 83:** Instantiates `FolderProcessor`. *Dependency: `FolderProcessor` class.*
    *   **Lines 85-88:** Conditional execution of `ExecuteLastDBSessionAction` if `appSetting.TestMode == true`. *Dependency: `ExecuteLastDBSessionAction` method.*
    *   **Line 90:** Calls `ProcessEmails(appSetting, timeBeforeImport, ctx)`. *Dependency: `ProcessEmails` method.*
    *   **Line 92:** Calls `ExecuteDBSessionActions(ctx, appSetting)`. *Dependency: `ExecuteDBSessionActions` method.*
    *   **Line 95:** Calls `folderProcessor.ProcessDownloadFolder(appSetting)`. *Dependency: `FolderProcessor.ProcessDownloadFolder` method.*
    *   **Lines 106-108:** Exception handler emails error details using `EmailDownloader`. *Dependency: `EmailDownloader`, `Utils.Client`.*
*   **Method: `ExecuteLastDBSessionAction(CoreEntitiesContext ctx, ApplicationSettings appSetting)` (private static bool)**
    *   **Lines 119-121:** Finds the latest `SessionSchedule` for the `appSetting`.
    *   **Lines 125-128:** If found, invokes actions defined in `SessionsUtils.SessionActions` based on the schedule. *Dependency: `SessionSchedule` entity, `SessionsUtils.SessionActions` dictionary.*
*   **Method: `ProcessEmails(ApplicationSettings appSetting, DateTime beforeImport, CoreEntitiesContext ctx)` (private static async Task)**
    *   **Line 138:** Guard clause: Proceeds only if `appSetting.Email` is set.
    *   **Lines 141-151:** Configures static `Utils.Client` (EmailDownloader) based on `appSetting`.
    *   **Lines 153-154:** Fetches emails via `EmailDownloader.EmailDownloader.CheckEmails`. *Dependency: `EmailDownloader`.*
    *   **Lines 160-164:** Executes initial email mapping actions via `ImportUtils.ExecuteEmailMappingActions`. *Dependency: `ImportUtils`, `EmailMapping` entity.*
    *   **Lines 166-273:** Main loop processing individual emails and their associated file types.
    *   **Lines 171-175:** Checks if required files exist and are recent. Uses `Regex.IsMatch`.
    *   **Lines 179-272:** Inner loop processing each `FileType` for an email.
    *   **Line 182:** Gets `FileType` object via `FileTypeManager.GetFileType`. *Dependency: `FileTypeManager`.*
    *   **Line 188-192:** Finds relevant files in the email's specific folder using `Regex.IsMatch`.
    *   **Lines 204-209:** Queries `AsycudaDocumentSetExs` (database view/table).
    *   **Lines 218-233:** Conditionally creates a new `AsycudaDocumentSet` record via raw SQL if `fileType.CreateDocumentSet` is true and it doesn't exist. *Dependency: `AsycudaDocumentSet` table, `Customs_Procedures` table.*
    *   **Lines 235-247:** Retrieves the `AsycudaDocumentSetId` and stores it in `fileType`.
    *   **Line 249:** Saves attachments via `Utils.SaveAttachments`. *Dependency: `Utils.SaveAttachments`.*
    *   **Lines 250-266:** Conditional execution based on `ReadOnlyMode`.
    *   **Line 252:** Executes data-specific file actions via `ImportUtils.ExecuteDataSpecificFileActions`. *Dependency: `ImportUtils`.*
    *   **Lines 254-265:** Executes non-specific file actions either immediately (`IsSingleEmail == true`) or defers by adding to `processedFileTypes` list. *Dependency: `ImportUtils.ExecuteNonSpecificFileActions`.*
    *   **Lines 275-292:** Processes deferred non-specific actions grouped by `AsycudaDocumentSetId`.
*   **Method: `ExecuteDBSessionActions(CoreEntitiesContext ctx, ApplicationSettings appSetting)` (private static void)**
    *   **Lines 303-308:** Executes actions for the "End" session. *Dependency: `SessionActions` entity, `SessionsUtils.SessionActions`.*
    *   **Lines 311-321:** Finds scheduled sessions (`SessionSchedule`) due to run now. Uses `SqlFunctions.DateAdd`.
    *   **Line 323:** Stores current schedules in `BaseDataModel.Instance.CurrentSessionSchedule`.
    *   **Lines 325-343:** If schedules exist, iterates and invokes corresponding actions from `SessionsUtils.SessionActions`, filtering by `ActionId` and `TestMode`. Stores current action in `BaseDataModel.Instance.CurrentSessionAction`.
    *   **Lines 344-363:** If no schedules are due, conditionally executes "AssessIM7" and "AssessEX" session actions based on `appSetting` flags (`AssessIM7`, `AssessEX`).

## Analysis: `AutoBot1/FolderProcessor.cs`

*   **Namespace:** `AutoBot`
*   **Class:** `FolderProcessor`
*   **Purpose:** This class is responsible for two main file processing tasks triggered outside the email flow:
    1.  Processing individual PDF files found in a `Downloads` folder associated with an `ApplicationSetting`.
    2.  Processing entire subfolders within a `ShipmentInput` folder, treating each subfolder as a single shipment unit defined by an `Info.txt` file.
*   **Dependencies (Inferred/Used):** `ApplicationSettings`, `FileTypeManager`, `PDFUtils`, `ShipmentUtils`, `ImportUtils`, `Utils.Client`, `EmailDownloader`, `CoreEntitiesContext`, `Emails` entity, `Attachments` entity, `FileTypes` entity.
*   **Method: `ProcessDownloadFolder(ApplicationSettings appSetting)` (public async Task)**
    *   **Called by:** `Program.Main` loop.
    *   **Lines 31-33:** Defines and ensures the existence of a `Downloads` subfolder within the `appSetting.DataFolder`.
    *   **Lines 35-48:** Iterates through `.pdf` files in the `Downloads` folder. Calls `ProcessFile` for each, wrapped in a try-catch. Errors are logged to console, and processing continues to the next file.
*   **Method: `ProcessFile(ApplicationSettings appSetting, FileInfo file)` (private async Task)**
    *   **Line 53:** Creates a specific `Documents` subfolder based on the PDF filename (sanitized). *Dependency: `CreateDocumentsFolder`.*
    *   **Line 54:** Copies the PDF to the new `Documents` subfolder. *Dependency: `CopyFileToDocumentsFolder`.*
    *   **Line 57:** Gets `FileTypes` specifically marked as "Unknown" for PDF format using `FileTypeManager`. *Dependency: `GetUnknownFileTypes`.*
    *   **Line 59:** Sets the `EmailId` property of the retrieved `FileTypes` to the original PDF filename. This likely provides file-specific context for subsequent "Unknown" PDF processing steps (`ProcessFileTypes`), rather than linking to an actual email record.
    *   **Line 61:** Processes these "Unknown" file types against the copied PDF using `PDFUtils` and potentially `ShipmentUtils`. *Dependency: `ProcessFileTypes`.*
    *   **Lines 63-74:** If `ProcessFileTypes` returns `true` (success), attempts to delete the original PDF from the `Downloads` folder. Handles potential `IOException`.
*   **Method: `CreateDocumentsFolder(...)` (private DirectoryInfo)**
    *   **Lines 80-82:** Creates a sanitized folder name from the PDF filename and constructs the full path under `DataFolder/Documents/`.
    *   **Lines 84-96:** Creates the directory if it doesn't exist, with error handling.
*   **Method: `CopyFileToDocumentsFolder(...)` (private string)**
    *   **Lines 102-117:** Copies the source file to the destination folder if it doesn't already exist. Includes error handling.
*   **Method: `GetUnknownFileTypes(FileInfo file)` (private List<FileTypes>)**
    *   **Lines 124-126:** Uses `FileTypeManager.GetImportableFileType` to find `FileTypes` configured for `EntryTypes.Unknown` and `FileFormats.PDF`, filtering further for those with `Description == "Unknown"`.
*   **Method: `ProcessFileTypes(List<FileTypes> fileTypes, string destFileName, FileInfo originalFile)` (private async Task<bool>)**
    *   **Lines 132-165:** Iterates through the provided "Unknown" `FileTypes`.
    *   **Line 137:** Attempts PDF import using `PDFUtils.ImportPDF`. *Dependency: `PDFUtils.ImportPDF`.*
    *   **Lines 138-149:** If the initial import fails (specifically for `ShipmentInvoice` type), attempts a `PDFUtils.ImportPDFDeepSeek`. If that also fails, notifies developers via email and marks processing as failed (`allgood = false`). *Dependency: `PDFUtils.ImportPDFDeepSeek`, `NotifyUnknownPDF`.*
    *   **Lines 152-164:** *Contingent upon* the success of either `ImportPDF` or the fallback `ImportPDFDeepSeek` for the `fileType`, attempts to create a shipment email using `ShipmentUtils.CreateShipmentEmail`. Errors during this step also mark processing as failed (`allgood = false`). *Dependency: `ShipmentUtils.CreateShipmentEmail`.*
    *   **Line 167:** Returns `true` only if *all* file types were processed without critical errors (including successful `CreateShipmentEmail` if triggered).
*   **Method: `NotifyUnknownPDF(...)` (private void)**
    *   **Lines 174-189:** Sends an email notification using `EmailDownloader.EmailDownloader.SendEmail` to "Developer" contacts, indicating an unknown/unparseable PDF was found. Includes error details and attaches relevant files. *Dependency: `Utils.Client`, `EmailDownloader`.*
*   **Method: `ProcessShipmentFolders(ApplicationSettings appSetting)` (public async Task)**
    *   **Called by:** *Not called by `Program.Main` loop. Trigger mechanism is external/unidentified in this context.*
    *   **Purpose:** Processes subfolders within a `ShipmentInput` directory. Each subfolder represents a shipment and must contain an `Info.txt` file.
    *   **Lines 195-210:** Defines and ensures the existence of the `ShipmentInput` folder.
    *   **Lines 212-356:** Iterates through subdirectories in `ShipmentInput`.
    *   **Lines 215-221:** Checks for the existence of `Info.txt` in the subfolder; skips if missing.
    *   **Lines 229-240:** Retrieves a specific `FileType` (hardcoded ID 1186, assumed to be "ShipmentFolder") from the database using `CoreEntitiesContext`. Skips folder if not found.
    *   **Lines 242-267:** Reads `Info.txt` to extract a "BL" (Bill of Lading?) number. Skips folder on error.
    *   **Lines 270-307:** Creates a placeholder `Emails` record in the database using a new `Guid` as `EmailId`. Creates `Attachments` records for *all* files in the subfolder, linking them to the placeholder `Emails` record. Uses `CoreEntitiesContext`. *Dependency: `Emails` entity, `Attachments` entity.*
    *   **Lines 309-316:** Sets the `EmailId` on the "ShipmentFolder" `FileType` object to the placeholder `Guid`. Executes data-specific and non-specific actions using `ImportUtils`, passing all files in the folder. *Dependency: `ImportUtils`.*
    *   **Lines 318-335:** Moves the processed subfolder to an `Archive` directory upon successful processing. Handles potential name collisions and move errors.
    *   **Lines 337-353:** Catches critical errors during folder processing, moves the subfolder to an `Error` directory, and logs the error.

## Analysis: `AutoBot/ImportUtils.cs`

*   **Namespace:** `AutoBotUtilities` (Note: This differs from the file path `AutoBot/`)
*   **Class: `EmailTextProcessor`**
    *   **Purpose:** Processes text files (likely derived from emails or text attachments) line by line, extracting key-value data based on regex patterns defined in `EmailInfoMappings` associated with a `FileType`, and generates/executes SQL UPDATE statements to modify database records.
    *   **Method: `Execute(FileInfo[] csvFiles, FileTypes fileType)` (static)**
        *   Iterates through input files (`csvFiles`).
        *   Reads each file line by line.
        *   For each line, attempts to match regex patterns defined in `fileType.EmailInfoMappings.InfoMapping.InfoMappingRegEx`. *Dependency: `EmailInfoMappings`, `InfoMapping`, `InfoMappingRegEx` entities.*
        *   Extracts key and value using regex groups and potential replacement patterns (`KeyRegX`, `FieldRx`, `KeyReplaceRx`, `FieldReplaceRx`).
        *   Adds extracted key-value pairs to the passed `fileType.Data` list (`List<KeyValuePair<string, string>>`).
        *   Constructs SQL `UPDATE` statements based on `InfoMapping` properties (`EntityType`, `Field`, `EntityKeyField`) and the extracted data. Uses `fileType.Data` to find the key value for the `WHERE` clause.
        *   Executes the aggregated SQL statements using a **new instance** of `CoreEntitiesContext` within the `Execute` method. *Dependency: `CoreEntitiesContext`.*
    *   **Helper Methods:** `GetEmailMappings`, `GetMappingData`, `GetDbStatement`, `ReplaceSpecialChar`.
*   **Class: `ImportUtils`**
    *   **Purpose:** Orchestrates the execution of actions defined in the database (`FileTypeActions`, `EmailMappingActions`) based on the current context (`FileType`, `EmailMapping`, `ApplicationSettings`). It acts as the bridge between database configuration and C# code execution.
    *   **Dependencies:** `FileTypes`, `FileInfo`, `ApplicationSettings`, `EmailMapping`, `FileTypeActions`, `EmailMappingActions`, `Actions` entity, `CoreEntitiesContext`, `FileUtils.FileActions` (dictionary), `BaseDataModel`, `EmailDownloader`.
    *   **Method: `ExecuteDataSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)` (static)**
        *   Queries the `FileTypeActions` table directly (using a **new `CoreEntitiesContext`**) filtering by `fileType.Id`, `Actions.IsDataSpecific == true`, `AssessIM7`/`AssessEX` flags (from `appSetting`), and `Actions.TestMode` (from `BaseDataModel.Instance.CurrentApplicationSettings.TestMode`).
        *   Orders the results by `FileTypeAction.Id`.
        *   Checks if each resulting action name exists as a key in `FileUtils.FileActions`. Throws an exception if any required actions are missing.
        *   Looks up the C# implementation (delegate) in `FileUtils.FileActions` for each valid action name.
        *   Calls `ExecuteActions` for each found action delegate.
    *   **Method: `ExecuteEmailMappingActions(EmailMapping emailMapping, FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)` (static)**
        *   Retrieves `EmailMappingActions` associated with the `emailMapping`, filtered by `TestMode`. Ordered by `EmailMappingAction.Id`.
        *   Looks up the C# implementation in `FileUtils.FileActions`.
        *   Calls `ExecuteActions` for each found action delegate.
    *   **Method: `ExecuteNonSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)` (static)**
        *   Queries the `FileTypeActions` table directly (using a **new `CoreEntitiesContext`**) filtering by `fileType.Id`, `Actions.IsDataSpecific == null` or `Actions.IsDataSpecific != true`, `AssessIM7`/`AssessEX` flags (from `appSetting`), and `Actions.TestMode` (from `BaseDataModel.Instance.CurrentApplicationSettings.TestMode`).
        *   Orders the results by `FileTypeAction.Id`.
        *   Checks if each resulting action name exists as a key in `FileUtils.FileActions` (logs a warning if missing).
        *   Looks up the C# implementation (delegate) in `FileUtils.FileActions` for each valid action name.
        *   Calls `ExecuteActions` for each found action delegate.
    *   **Method: `ExecuteActions(FileTypes fileType, FileInfo[] files, (string Name, Action<FileTypes, FileInfo[]> Action) x)` (static)**
        *   **Core Action Executor:** Wraps the invocation of a specific action delegate (`x.Action`).
        *   **Logging:** Logs start, success, failure, and duration of actions.
        *   **ProcessNextStep Logic:** Before executing the main action (`x.Action`), it checks if `fileType.ProcessNextStep` (a `List<string>`) contains action names.
            *   If it does, it iterates through this list sequentially.
            *   For each name, it looks up the action in `FileUtils.FileActions`.
            *   If the action name is not found in the dictionary, it is logged as skipped, removed from the list, and processing continues with the next item in `ProcessNextStep`.
            *   If the action name is "Continue", it stops processing the `ProcessNextStep` list and proceeds to execute the main action (`x.Action`).
            *   Otherwise, the found action delegate is invoked.
            *   If the list is exhausted without hitting "Continue", the main action (`x.Action`) is *not* executed.
            *   Errors during `ProcessNextStep` execution are logged and rethrown.
        *   Invokes the main action delegate (`x.Action.Invoke(fileType, files)`).
        *   Handles exceptions during main action execution, logs them, and rethrows.

## Analysis: `AutoBot/FileUtils.cs`

*   **Namespace:** `AutoBot`
*   **Class:** `FileUtils`
*   **Purpose:** Defines the crucial static `FileActions` dictionary. This dictionary serves as the central registry mapping action names (strings defined in the `Actions` database table and referenced by `FileTypeActions` and `EmailMappingActions`) to their corresponding C# implementations (delegates of type `Action<FileTypes, FileInfo[]>`).
*   **Key Member: `FileActions` (static `Dictionary<string, Action<FileTypes, FileInfo[]>>`)**
    *   **Initialization:** Uses a case-insensitive comparer (`WaterNut.DataSpace.Utils.ignoreCase`).
    *   **Content:** Contains numerous key-value pairs representing the available actions.
    *   **Keys:** String names like "ImportSalesEntries", "AllocateSales", "CreateEx9", "SaveCsv", "ImportPDF", "CreateShipmentEmail", "SaveInfo", "Kill", "Continue", "ImportShipmentInfoFromTxt", etc. These names must match the `Actions.Name` column in the database for the configuration to work.
    *   **Values:** Lambda expressions or method group references pointing to static methods in various utility classes. These methods take `FileTypes` (for context) and `FileInfo[]` (for the files being processed) as parameters. *Note: Several actions referencing `async` methods in utility classes are invoked synchronously using `.GetAwaiter().GetResult()`.*
    *   **Examples:**
        *   `{"ImportSalesEntries", (ft, fs) => DocumentUtils.ImportSalesEntries(false)}`
        *   `{"AllocateSales", (ft, fs) => AllocateSalesUtils.AllocateSales()}`
        *   `{"CreateEx9", (ft, fs) => CreateEX9Utils.CreateEx9(false, -1)}`
        *   `{"SaveCsv", (ft, fs) => CSVUtils.SaveCsv(fs, ft)}`
        *   `{"Xlsx2csv", async (ft, fs) => await XLSXProcessor.Xlsx2csv(fs, new List<FileTypes>(){ft})}`
        *   `{"SaveInfo", (ft, fs) => EmailTextProcessor.Execute(fs, ft)}`
        *   `{"ImportPDF", (ft, fs) => PDFUtils.ImportPDF(fs,ft)}`
        *   `{"CreateShipmentEmail", ShipmentUtils.CreateShipmentEmail}`
        *   `{"ImportShipmentInfoFromTxt", ShipmentUtils.ImportShipmentInfoFromTxt}`
        *   `{"Kill", Utils.Kill}` (Likely `WaterNut.DataSpace.Utils.Kill`)
        *   `{"Continue", (ft, fs) => { }}` (Special empty action for `ProcessNextStep` logic in `ImportUtils`)
    *   **Dependencies:** This class implicitly depends on all the utility classes whose methods are referenced in the dictionary values (e.g., `DocumentUtils`, `AllocateSalesUtils`, `CSVUtils`, `POUtils`, `PDFUtils`, `ShipmentUtils`, `EmailTextProcessor`, etc.).

## Analysis: `AutoBot/PDFUtils.cs`

*   **Namespace:** `AutoBot`
*   **Class:** `PDFUtils`
*   **Purpose:** Provides static methods for handling PDF documents, including importing data, linking PDFs to database entries, attaching them based on context, and potentially downloading/managing PDF files related to customs processes (possibly via UI automation).
*   **Dependencies:** `CoreEntitiesContext`, `FileTypes`, `FileInfo`, `BaseDataModel`, `InvoiceReader`, `DeepSeekInvoiceApi`, `FileTypeManager`, `DataFileProcessor`, `WaterNut.DataSpace.Utils`, `AsycudaDocumentSet_Attachments`, `Attachments`, `AsycudaDocuments`, `Stp_TODO_ImportCompleteEntries` SP, SikuliX.
*   **Key Methods:**
    *   `ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)` (**async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>>**):
        *   Orchestrates the primary PDF import process.
        *   Looks up context (`EmailId`, `FileTypeId`) from `AsycudaDocumentSet_Attachments` based on `pdfFiles[x].FullName`, falling back to passed `fileType` context if no attachment record exists.
        *   Delegates the core parsing and data extraction by **awaiting** `InvoiceReader.Import`. **Dependency: `InvoiceReader` class.**
        *   Returns a list of KeyValuePairs indicating the success/failure status per document type identified within the PDF(s).
    *   `ImportPDFDeepSeek(FileInfo[] fileInfos, FileTypes fileType)` (**async Task<List<KeyValuePair<string, (string FileName, string DocumentType, ImportStatus status)>>>**):
        *   Alternative import method, used as a fallback.
        *   Extracts text using `InvoiceReader.GetPdftxt`.
        *   **Awaits** an external `DeepSeekInvoiceApi` call to extract structured data. **Dependency: `DeepSeekInvoiceApi` class.**
        *   Maps results to internal `FileTypes` using `FileTypeManager`.
        *   Sets default values based on `FileTypeMappings` using the `SetFileTypeMappingDefaultValues` helper.
        *   Processes the extracted data by calling `DataFileProcessor.Process` via the `ImportSuccessState` helper method. **Dependency: `DataFileProcessor` class.**
        *   Returns a list of KeyValuePairs indicating the success/failure status per document type identified.
    *   `AttachEmailPDF(FileTypes ft, FileInfo[] fs)`:
        *   Calls `BaseDataModel.AttachEmailPDF` to handle the attachment logic, passing `AsycudaDocumentSetId` and `EmailId`. **Dependency: `BaseDataModel` class.**
    *   `LinkPDFs()`:
        *   Retrieves completed entries using `Stp_TODO_ImportCompleteEntries` stored procedure.
        *   Calls `BaseDataModel.LinkPDFs` to perform the linking logic. **Dependency: `BaseDataModel` class, `Stp_TODO_ImportCompleteEntries` SP.**
    *   `ReLinkPDFs()`:
        *   Scans the "Imports" directory (path derived from `BaseDataModel.GetDocSetDirectoryName("Imports")`) for recently modified PDFs (`LastWriteTime == Today`).
        *   Uses regex `.*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf` to extract a CNumber from the filename.
        *   Finds the corresponding `AsycudaDocuments` record based on the extracted CNumber.
        *   **Checks if an `Attachment` record with the PDF's `FilePath` already exists and if it's already linked to the target `AsycudaDocument` via `AsycudaDocument_Attachments`.**
        *   If not already linked, creates new `Attachments` and `AsycudaDocument_Attachments` records in the database.
    *   `DownloadPDFs()`:
        *   Retrieves completed entries using `Stp_TODO_ImportCompleteEntries`.
        *   Iterates through document sets.
        *   Repeatedly calls `WaterNut.DataSpace.Utils.RunSiKuLi` (SikuliX UI automation runner) with parameters like directory name and script name ("IM7-PDF") until a local check (`ImportPDFComplete`) indicates the required files exist (based on an "OverView-PDF.txt" manifest). **Dependency: `WaterNut.DataSpace.Utils.RunSiKuLi`, SikuliX scripts.**
    *   `ProcessUnknownPDFFileType(FileTypes ft, FileInfo[] fs)`: Empty method stub, mapped in `FileActions` but does nothing.
    *   `ConvertPNG2PDF()`: Appears incomplete/unused.

## Analysis: `AutoBot/ShipmentUtils.cs`

*   **Namespace:** `AutoBot`
*   **Class:** `ShipmentUtils`
*   **Purpose:** Provides static methods for processing shipment-related data, including importing details from text files, updating supplier/item information, generating reports/emails for issues, and potentially aggregating shipment data from various sources.
*   **Dependencies:** `CoreEntitiesContext`, `EntryDataDSContext`, `InventoryDSContext`, `DocumentDSContext`, `FileTypes`, `FileInfo`, `CSVUtils`, `XlsxWriter`, `EmailDownloader`, `BaseDataModel`, `EntryDocSetUtils`, `POUtils`, `Utils` (likely `WaterNut.DataSpace.Utils`), `Shipment` class (and extensions), `Suppliers`, `InventoryItems`, `TODO_SubmitUnclassifiedItems`, `TODO_SubmitIncompleteSupplierInfo`, `TODO_SubmitInadequatePackages`, `AsycudaDocumentSet`, `xcuda_ASYCUDA`, `xcuda_ASYCUDA_ExtendedProperties`, `xcuda_Transport`, `xcuda_Declarant`, `Contacts`, `ActionDocSetLogs`.
*   **Key Methods:**
    *   `ImportShipmentInfoFromTxt(FileTypes ft, FileInfo[] files)`:
        *   **Core logic for "ShipmentFolder" processing.** Reads key-value pairs from `Info.txt`.
        *   Requires "BL" number from `Info.txt` to identify/create `AsycudaDocumentSet` (using `DocumentDSContext`).
        *   Updates `AsycudaDocumentSet` fields (Manifest, Freight, Weight, Currency, Origin, etc.) based on `Info.txt`.
        *   Creates/updates related `xcuda_ASYCUDA`, `xcuda_ASYCUDA_ExtendedProperties`, `xcuda_Transport` (for Location of Goods), and `xcuda_Declarant` (for Consignee Code/Name) records.
        *   Updates the input `FileTypes` object (`ft`) with the `AsycudaDocumentSetId` and adds "ShipmentKey" (BL number) to `ft.Data`.
    *   `CreateShipmentEmail(FileTypes fileType, FileInfo[] files)`:
        *   Aggregates data for a shipment identified by `fileType.EmailId` using a `Shipment` object and numerous `Load...` extension methods (e.g., `LoadEmailPOs`, `LoadDBBL`).
        *   Calls `AutoCorrect()` and `ProcessShipment()` on the `Shipment` object(s). **(Logic within `Shipment` class/extensions is unknown).**
        *   Sends an email containing the shipment summary (`shipment.ToString()`) and attaches related files.
        *   Saves `Attachment` records associated with the shipment to the database (`EntryDataDSContext`).
    *   `MapUnClassifiedItems(FileTypes ft, FileInfo[] fs)`:
        *   Reads a CSV file (likely containing ItemNumber, TariffCode).
        *   Updates the `TariffCode` for matching `InventoryItems` in the database (`InventoryDSContext`).
        *   Updates the `FileType`'s `DocSetRefernece` using `EntryDocSetUtils.SetFileTypeDocSetToLatest`.
    *   `SubmitUnclassifiedItems(FileTypes ft)` / `SubmitDocSetUnclassifiedItems(FileTypes fileType)`:
        *   Queries `TODO_SubmitUnclassifiedItems` view for items missing tariff codes within a specific `AsycudaDocumentSetId`.
        *   Generates a CSV report (`UnclassifiedItems-{docSetId}.csv`).
        *   Emails the report to "Broker" contacts.
    *   `SubmitIncompleteSuppliers(FileTypes ft)`:
        *   Queries `TODO_SubmitIncompleteSupplierInfo` view for suppliers missing details within a specific `AsycudaDocumentSetId`.
        *   Generates a CSV report (`IncompleteSuppliers.csv`).
        *   Emails the report to "Broker" or "PO Clerk" contacts.
    *   `UpdateSupplierInfo(FileTypes ft, FileInfo[] fs)`:
        *   Reads a CSV file (SupplierCode, SupplierName, SupplierAddress, CountryCode).
        *   Updates matching `Suppliers` records in the database (`EntryDataDSContext`).
    *   `ImportUnAttachedSummary(FileTypes ft, FileInfo[] fs)`:
        *   Processes an Excel file using `XlsxWriter.SaveUnAttachedSummary`.
        *   Sets `ft.EmailId` based on the result.
        *   Calls `CreateShipmentEmail`.
    *   `ClearShipmentData(FileTypes fileType, FileInfo[] fileInfos)`:
        *   Deletes data related to a specific `EmailId` from `ShipmentBL`, `ShipmentInvoice`, `entrydata`, and `ShipmentManifest` tables.
    *   `SaveShipmentInfoToFile(Shipment shipment, string outputDirectory)`:
        *   Writes properties of a `Shipment` object to an `Info.txt` file. (Inverse of `ImportShipmentInfoFromTxt`).
    *   `CreateInstructions()`: Utility method, likely not part of main flow.

### Action Method Implementations (Detailed Analysis)

*   **`ImportUnAttachedSummary` (ActionId 112):**
    *   Iterates through input Excel files (`fs`).
    *   Calls `XlsxWriter.SaveUnAttachedSummary(file)` to process the Excel file and get a `reference` string. (Dependency: `XlsxWriter` library).
    *   Sets `ft.EmailId = reference`.
    *   Calls `CreateShipmentEmail(ft, fs)` to trigger email generation based on the processed Excel data. (Dependency: `CreateShipmentEmail` method).
*   **`UpdateSupplierInfo` (ActionId 48):**
    *   Uses `EntryDataDSContext`.
    *   Reads input CSV files (`fs`) into a `DataTable` using `CSVUtils.CSV2DataTable`. (Dependency: `CSVUtils`).
    *   Iterates through CSV rows.
    *   For each row with a `SupplierCode`, fetches the corresponding `Suppliers` record from DB based on `SupplierCode` and `BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId`. (Dependency: `EntryDataDSContext`, `Suppliers`, `BaseDataModel`).
    *   Updates the `SupplierName`, `Street`, and `CountryCode` of the DB record from the CSV row.
    *   Saves changes to the database.
*   **`CreateShipmentEmail` (ActionId 111):**
    *   Retrieves `EmailId` from the input `FileTypes` object (`fileType`).
    *   Executes the `[dbo].[PreProcessShipmentSP]` stored procedure using `EntryDataDSContext`. (Dependency: `EntryDataDSContext`, `PreProcessShipmentSP` SP).
    *   Creates a `Shipment` object and uses chained extension methods (`LoadEmailPOs`, `LoadEmailInvoices`, `LoadDBBL`, etc.) to populate it with data related to the `EmailId`. (Dependency: `Shipment` class, `ShipmentExtensions.cs`).
    *   Calls `shipment.AutoCorrect()` and `shipment.ProcessShipment()` methods. (Dependency: `Shipment` class/extensions).
    *   Fetches email addresses for "PDF Entries", "Developer", "PO Clerk" contacts from DB using `CoreEntitiesContext`. (Dependency: `CoreEntitiesContext`, `Contacts`).
    *   Sends an email using `EmailDownloader.EmailDownloader.SendEmail` with subject "Shipment: {ShipmentName}", body from `shipment.ToString()`, and attachments from `shipment.ShipmentAttachments`. (Dependency: `EmailDownloader`, `Utils.Client`, `Shipment`).
    *   Saves `Attachments` records associated with the shipment to the database using `EntryDataDSContext`. (Dependency: `EntryDataDSContext`, `Attachments`).
    *   Includes error handling that calls `BaseDataModel.EmailExceptionHandler`. (Dependency: `BaseDataModel`).
*   **`MapUnClassifiedItems` (ActionId 43):**
    *   Uses `InventoryDSContext`.
    *   Reads input CSV files (`fs`) into a `DataTable` using `CSVUtils.CSV2DataTable`. (Dependency: `CSVUtils`).
    *   Iterates through CSV rows.
    *   For each row with a `TariffCode`, fetches *all* `InventoryItems` records matching the `ItemNumber` and current `ApplicationSettingsId`. (Dependency: `InventoryDSContext`, `InventoryItems`, `BaseDataModel`).
    *   Updates the `TariffCode` for all matching inventory items.
    *   Saves changes to the database.
    *   Calls `EntryDocSetUtils.SetFileTypeDocSetToLatest(ft)` to update the `DocSetRefernece` on the `FileTypes` object. (Dependency: `EntryDocSetUtils`).
*   **`SubmitUnclassifiedItems` (ActionId 42):**
    *   Gets a directory path using `BaseDataModel.CurrentSalesInfo(-1)`. (Dependency: `BaseDataModel`).
    *   Queries the `TODO_SubmitUnclassifiedItems` view using `CoreEntitiesContext`, filtering by `ft.AsycudaDocumentSetId`. (Dependency: `CoreEntitiesContext`, `TODO_SubmitUnclassifiedItems` view).
    *   For each resulting group (`EmailId`, `AsycudaDocumentSetId`):
        *   Generates a CSV report (`UnclassifiedItems-{docSetId}.csv`) of the unclassified items using `ExportToCSV`. (Dependency: `ExportToCSV`, `Utils.UnClassifiedItem`).
        *   Fetches "Broker" contacts. (Dependency: `CoreEntitiesContext`, `Contacts`).
        *   Forwards the original email (identified by `EmailId`) using `EmailDownloader.EmailDownloader.ForwardMsg`, attaching the CSV report. (Dependency: `EmailDownloader`, `Utils.Client`).
*   **`SubmitDocSetUnclassifiedItems` (ActionId 73 - *Note: Method exists but ActionId 73 not in FileTypeActions data*):**
    *   Similar to `SubmitUnclassifiedItems` but filters the `TODO_` view by `fileType.AsycudaDocumentSetId` directly.
    *   Gets directory context using `POUtils.CurrentPOInfo`. (Dependency: `POUtils`).
    *   Otherwise, the logic for generating the CSV and forwarding the email is the same.
*   **`SubmitIncompleteSuppliers` (ActionId 47):**
    *   Gets a directory path using `BaseDataModel.CurrentSalesInfo(-1)`. (Dependency: `BaseDataModel`).
    *   Queries the `TODO_SubmitIncompleteSupplierInfo` view using `CoreEntitiesContext`, filtering by `ft.AsycudaDocumentSetId`. (Dependency: `CoreEntitiesContext`, `TODO_SubmitIncompleteSupplierInfo` view).
    *   Checks `Utils.GetDocSetActions` to prevent duplicate submissions for the same `AsycudaDocumentSetId`. (Dependency: `Utils.GetDocSetActions`, `ActionDocSetLogs`).
    *   Generates a CSV report (`IncompleteSuppliers.csv`) of the incomplete suppliers using `ExportToCSV`. (Dependency: `ExportToCSV`, `IncompleteSupplier` class).
    *   Fetches "Broker" or "PO Clerk" contacts. (Dependency: `CoreEntitiesContext`, `Contacts`).
    *   Sends a *new* email using `EmailDownloader.EmailDownloader.SendEmail` with the CSV report attached. (Dependency: `EmailDownloader`, `Utils.Client`).
*   **`SubmitInadequatePackages` (ActionId 44):**
    *   Queries the `TODO_SubmitInadequatePackages` view using `CoreEntitiesContext`, filtering by `ft.AsycudaDocumentSetId`. (Dependency: `CoreEntitiesContext`, `TODO_SubmitInadequatePackages` view).
    *   Fetches "PO Clerk" or "Broker" contacts. (Dependency: `CoreEntitiesContext`, `Contacts`).
    *   For each resulting document set with inadequate packages:
        *   Checks `Utils.GetDocSetActions` to prevent duplicate notifications. (Dependency: `Utils.GetDocSetActions`, `ActionDocSetLogs`).
        *   Constructs an email body explaining the package vs. lines issue based on view data (`TotalPackages`, `TotalLines`, `MaxEntryLines`, `RequiredPackages`).
        *   Sends the notification email using `EmailDownloader.EmailDownloader.SendEmail`. (Dependency: `EmailDownloader`, `Utils.Client`).

### Action Method Implementations (CSVUtils.cs) (*Note: `ProcessUnknownCSVFileType` action (ActionId 80) throws `NotImplementedException`*)

*   **`SaveCsv` (ActionId 6):**
    *   Iterates through input CSV files (`csvFiles`).
    *   Calls the private helper `TryImportFile` for each file.
*   **`TryImportFile` (Helper for `SaveCsv`):**
    *   Contains the core logic initiated by `SaveCsv`.
    *   Calls `SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, fileType.OverwriteFiles ?? true).Wait()`. This delegates the main CSV import logic (parsing, mapping via `fileType.FileTypeMappings`, DB interaction) to the `SaveCSVModel` singleton.
    *   The `overwrite` behavior respects the `FileTypes.OverwriteFiles` setting (defaulting to true).
    *   Includes error handling that calls `EmailCSVImportError`. (Dependency: `SaveCSVModel`, `EmailCSVImportError`).
*   **`ReplaceCSV` (ActionId 69):**
    *   Iterates through input CSV files (`csvFiles`).
    *   Calls `SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, true).Wait()`. It uses the same core import logic as `SaveCsv`.
    *   Key difference: The `overwrite` parameter is **hardcoded to `true`**, forcing replacement of existing data.
    *   Includes more elaborate error handling: checks `AttachmentLog` for prior errors, finds original email context, sends an error email back to the sender via `EmailDownloader.SendBackMsg`, and logs the error notification in `AttachmentLog`. (Dependency: `SaveCSVModel`, `CoreEntitiesContext`, `AttachmentLog`, `AsycudaDocumentSet_Attachments`, `EmailDownloader`, `Utils.Client`).

*Note: The actual implementation of CSV parsing, data mapping based on `FileTypeMappings`, and database saving resides within the `SaveCSVModel` class, which is not defined in `CSVUtils.cs`.*

### Action Method Implementations (POUtils.cs)

*   **`RecreatePOEntries` (ActionId 7):**
    *   Calls internal method `RecreatePOEntries(int asycudaDocumentSetId)`.
    *   If `AssessIM7` setting is true, it first checks `TODO_PODocSetToExport` view; returns if the `asycudaDocumentSetId` is not found.
    *   Queries `ToDo_POToXML` view for the `docSetId` to get a list of `EntryDataDetailsId`s.
    *   Calls helper `CreatePOEntries(docSetId, entrylst)` which internally calls `BaseDataModel.Instance.ClearAsycudaDocumentSet`, `BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber`, and `BaseDataModel.Instance.AddToEntry`.
    *   (Dependencies: `CoreEntitiesContext`, `BaseDataModel`, `ToDo_POToXML` view, `TODO_PODocSetToExport` view).
    *   Logs completion in `ActionDocSetLogs`. (Dependency: `Utils.LogDocSetAction`).
*   **`ExportPOEntries` (ActionId 8):**
    *   Calls internal method `ExportPOEntries(int asycudaDocumentSetId)`.
    *   If `AssessIM7` setting is true, it first checks `TODO_PODocSetToExport` view; returns if the `asycudaDocumentSetId` is not found.
    *   Queries `xcuda_ASYCUDA` table (including related entities) for the `asycudaDocumentSetId`.
    *   Gets directory path using `BaseDataModel.GetDocSetDirectoryName`.
    *   Iterates through results, calls `BaseDataModel.Instance.ExportDocument` for each, saving XML files (e.g., `{ReferenceNumber}.xml`) to the directory. Also creates/updates an `Instructions.txt` file.
    *   (Dependencies: `CoreEntitiesContext`, `DocumentDSContext`, `BaseDataModel`, `TODO_PODocSetToExport` view).
    *   Logs completion in `ActionDocSetLogs`. (Dependency: `Utils.LogDocSetAction`).
*   **`AssessPOEntries` (ActionId 10):**
    *   Calls internal method `AssessPOEntries(FileTypes ft)`.
    *   Queries `TODO_PODocSetToAssess` view based on `ft.AsycudaDocumentSetId`.
    *   For each result, calls helper `AssessPOEntry(docReference, docSetId)` which uses SikuliX (`Utils.RunSiKuLi` with script "SaveIM7") to perform UI automation based on `Instructions.txt` and `InstructionResults.txt` files.
    *   Finally, calls `SubmitAssessPOErrors(ft)` which queries `TODO_PODocSetToAssessErrors` view and emails a summary CSV (`POAssesErrors.csv`) to "PO Clerk" / "Broker" contacts if errors are found.
    *   (Dependencies: `CoreEntitiesContext`, `Utils.RunSiKuLi`, SikuliX, `ExportToCSV`, `EmailDownloader`, `TODO_PODocSetToAssess` view, `TODO_PODocSetToAssessErrors` view).
    *   *Note: Logging to ActionDocSetLogs was mentioned in previous KB version but not found in current code.*
*   **`SubmitPOs` (ActionId 53):**
    *   Can be triggered parameterlessly (likely via session schedule, finding the latest docset) or via `SubmitPOs(FileTypes ft, FileInfo[] fs)` (likely via file type action, using `ft.AsycudaDocumentSetId` or CNumbers from `ft.Data` for context).
    *   Both routes query `TODO_SubmitPOInfo` or `AsycudaDocuments` based on the context.
    *   Both entry points ultimately call an internal helper `SubmitPOs(AsycudaDocumentSetEx docSet, List<TODO_SubmitPOInfo> pOs, ...)`:
        *   Gathers associated PDF file paths from `AsycudaDocument_Attachments` for both assessed and new entries, attempting to link PDFs via `BaseDataModel.LinkPDFs` if needed. (Dependency: `CoreEntitiesContext`, `AsycudaDocument_Attachments`, `Attachments`, `BaseDataModel`).
        *   Generates a `Summary.csv` using `ExportToCSV`. (Dependency: `ExportToCSV`, `AssessedEntryInfo`).
        *   Sends two emails (forwarding original if `EmailId` available, otherwise new):
            *   "Document Package" to "PDF Entries"/"Developer" contacts with all PDFs + summary.
            *   "Assessed Entries" to "PO Clerk"/"Developer" contacts with assessed PDFs + summary. (Dependency: `EmailDownloader`, `Utils.Client`).
        *   Logs the "Submit PO Entries" action in `AttachmentLog` for each submitted PO. (Dependency: `CoreEntitiesContext`, `AttachmentLog`).
*   **`DeletePONumber` (ActionId 104):** (*KB Description Accurate*)
    *   Extracts PO Numbers from `ft.Data` where `Key == "PONumber"`.
    *   Uses `EntryDataDSContext`.
    *   For each PO Number, finds the matching `EntryData` record based on `EntryDataId` and `ApplicationSettingsId`.
    *   Removes the found `EntryData` record(s) from the context.
    *   Saves changes, deleting the `EntryData` records (and potentially related `EntryDataDetails`). (Dependency: `EntryDataDSContext`, `EntryData`, `BaseDataModel`).

### Action Method Implementations (DISUtils.cs)

*   **`ReSubmitDiscrepanciesToCustoms` (ActionId 95):**
    *   Calls helper `GetSubmitEntryData(ft)` to identify discrepancy entries based on `ft.Data` (CNumbers) or `ft.AsycudaDocumentSetId`, querying `TODO_SubmitAllXMLToCustoms` view. (Dependency: `GetSubmitEntryData`, `CoreEntitiesContext`, `TODO_SubmitAllXMLToCustoms` view, `BaseDataModel`).
    *   Filters the results based on `ft.EmailId` or `ft.Data`.
    *   Calls helper `SubmitDiscrepanciesToCustoms(toBeProcessed)`:
        *   Iterates through groups of discrepancies (grouped by `EmailId`).
        *   Calls helper `CreateDiscrepancyEmail` for each group:
            *   Gets directory using `POUtils.CurrentPOInfo`. (Dependency: `POUtils`).
            *   Creates `ExecutionReport.csv` by querying `TODO_DiscrepanciesExecutionReport` view. (Dependency: `CoreEntitiesContext`, `TODO_DiscrepanciesExecutionReport` view, `ExportToCSV`).
            *   Gathers associated PDF paths from `AsycudaDocument_Attachments`, linking via `BaseDataModel.LinkPDFs` if needed. (Dependency: `CoreEntitiesContext`, `AsycudaDocument_Attachments`, `Attachments`, `BaseDataModel`).
            *   Creates `Summary.csv` using `ExportToCSV`. (Dependency: `ExportToCSV`).
            *   Sends email (forwarding if `EmailId` exists) to "Customs" contacts with reports and PDFs. (Dependency: `EmailDownloader`, `Utils.Client`).
            *   Logs "Submit XML To Customs" action in `AttachmentLog`. (Dependency: `CoreEntitiesContext`, `AttachmentLog`).
*   **`SubmitDiscrepanciesToCustoms` (ActionId 98):**
    *   Queries `TODO_SubmitDiscrepanciesToCustoms` view based on `ft.EmailId`.
    *   Calls the internal `SubmitDiscrepanciesToCustoms(lst)` helper (which then calls `CreateDiscrepancyEmail` for report generation, PDF linking, emailing, and logging).
    *   (Dependencies: `CoreEntitiesContext`, `TODO_SubmitDiscrepanciesToCustoms` view, `EmailDownloader`, `ExportToCSV`, `BaseDataModel`, `POUtils`, `AttachmentLog`).
*   **`AssessDiscpancyEntries` (ActionId 75):**
    *   Calls internal helper `AssessDISEntries("DIS")`.
    *   This helper queries `TODO_EntriesToAssess` view and calls `EntryDocSetUtils.AssessEntries` for each result, which likely involves SikuliX UI automation for assessment.
    *   (Dependencies: `CoreEntitiesContext`, `EntryDocSetUtils.AssessEntries`, SikuliX, `TODO_EntriesToAssess` view).
*   **`AssessDiscpancyEntries` (ActionId 75):**
    *   Calls internal helper `AssessDISEntries("DIS")`.
    *   This helper queries `TODO_EntriesToAssess` view and calls `EntryDocSetUtils.AssessEntries` for each result, which likely involves SikuliX UI automation for assessment.
    *   (Dependencies: `CoreEntitiesContext`, `EntryDocSetUtils.AssessEntries`, SikuliX, `TODO_EntriesToAssess` view).
    *   Calls internal helper `AssessDISEntries("DIS")`.
    *   This helper queries `TODO_EntriesToAssess` view and calls `EntryDocSetUtils.AssessEntries` for each result, which likely involves SikuliX UI automation for assessment.
    *   (Dependencies: `CoreEntitiesContext`, `EntryDocSetUtils.AssessEntries`, SikuliX, `TODO_EntriesToAssess` view).
    *   Calls internal helper `AssessDISEntries("DIS")`.
    *   This helper queries `TODO_EntriesToAssess` view and calls `EntryDocSetUtils.AssessEntries` for each result, which likely involves SikuliX UI automation for assessment.
    *   (Dependencies: `CoreEntitiesContext`, `EntryDocSetUtils.AssessEntries`, SikuliX, `TODO_EntriesToAssess` view).

### Action Method Implementations (SubmitSalesToCustomsUtils.cs)

### Action Method Implementations (EX9Utils.cs)

*   **`ExportEx9Entries` (ActionId 20):**
    *   Gets sales context using `BaseDataModel.CurrentSalesInfo`.
    *   Calls `ExportDocSetSalesReportUtils.ExportDocSetSalesReport` to generate a sales report.
    *   Calls `BaseDataModel.Instance.ExportDocSet` to export the document set XMLs.
    *   (Dependencies: `BaseDataModel`, `ExportDocSetSalesReportUtils`).
*   **`AssessEx9Entries` (ActionId 21):**
    *   Gets sales context using `BaseDataModel.CurrentSalesInfo`.
    *   Calls helper `AssessSalesEntry` which uses SikuliX (`Utils.RunSiKuLi` with script "AssessIM7") to perform UI automation based on `Instructions.txt` and `InstructionResults.txt` files.
    *   (Dependencies: `BaseDataModel`, `Utils.RunSiKuLi`, SikuliX).
*   **`DownloadSalesFiles` (ActionId 28):**
    *   Calls `Utils.RetryImport` which likely uses SikuliX (`Utils.RunSiKuLi` with the script "IM7History") to download files related to sales history into the "Imports" directory, retrying up to 10 times.
    *   (Dependencies: `BaseDataModel`, `Utils.RetryImport`, `Utils.RunSiKuLi`, SikuliX).

## Building and Testing (Visual Studio 2022 Enterprise)

This section outlines the standard procedures for building the AutoBot-Enterprise solution and running its unit tests using the Visual Studio 2022 Enterprise IDE.

### Prerequisites

*   Visual Studio 2022 Enterprise installed with the ".NET desktop development" workload.
*   Solution file (`AutoBot-Enterprise.sln`) opened in Visual Studio.
*   Database connection strings in configuration files (`App.config` in relevant projects) correctly set up for your environment, especially for running integration tests that interact with the database.

### Building the Solution

1.  **Select Configuration:** Choose the desired build configuration (e.g., `Debug`) and platform (e.g., `x64`) from the dropdown menus in the Visual Studio toolbar. `Debug | x64` is commonly used for development and testing.
2.  **Clean Solution (Optional but Recommended):**
    *   Go to the `Build` menu.
    *   Select `Clean Solution`. This removes previous build outputs.
3.  **Build Solution:**
    *   Go to the `Build` menu.
    *   Select `Build Solution` (or press `Ctrl+Shift+B`).
4.  **Check Output:** Monitor the `Output` window (usually at the bottom, select "Build" from the "Show output from:" dropdown) for any build errors or warnings. Address any errors before proceeding.

### Running Tests

The primary test project is `AutoBotUtilities.Tests`.

1.  **Open Test Explorer:**
    *   Go to the `Test` menu.
    *   Select `Test Explorer`. This window lists all discovered tests. If it's empty after a build, try rebuilding the solution.
2.  **Run All Tests:**
    *   In the Test Explorer toolbar, click the `Run All Tests In View` button (looks like a double green play icon).
3.  **Run Specific Tests:**
    *   In the Test Explorer, you can filter or search for specific tests or test classes (e.g., `PDFImportTests`).
    *   Right-click on a specific test, test class, or group of tests.
    *   Select `Run`.
4.  **Analyze Results:**
    *   The Test Explorer will show the status of each test (Passed, Failed, Skipped).
    *   Click on a failed test to see the error message, stack trace, and any standard output (`Console.WriteLine`) generated during the test run in the details pane below the list.
    *   **Note:** As observed during development, some tests might fail or be skipped due to environment setup issues (database state, file paths, `ApplicationSettings` configuration) when run outside of specific environments like NCrunch or a fully configured local setup. Focus on the tests relevant to the changes being made.