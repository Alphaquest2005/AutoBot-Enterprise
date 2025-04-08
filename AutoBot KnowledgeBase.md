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

## OCR / PDF Processing (`InvoiceReader`, `Part`, `Invoice`)

*   **Core Classes:** `InvoiceReader.cs`, `Invoice.cs`, `Part.cs`, `Line.cs`, `Field.cs` (within `WaterNut.Business.Services/Custom Services/DataModels/Custom DataModels/PDF2TXT/`).
*   **Workflow:**
    1.  `PDFUtils.ImportPDF` calls `InvoiceReader.Import`.
    2.  `InvoiceReader.Import` gets PDF text (`InvoiceReader.GetPdftxt`), creates an `Invoice` object (which initializes `Part` objects based on DB config), and calls `Invoice.Read(textLines)`.
    3.  `Invoice.Read` iterates through text lines, calling `Part.Read` for each top-level part.
    4.  `Part.Read` manages state (`_currentInstance`, `_wasStarted`), finds start/end lines based on regex (`FindStart`, `FindEnd`), extracts field values using regex (`ReadLine`), and recursively calls `Part.Read` for child parts, passing the relevant text lines and the *parent's effective instance*.
    5.  After all lines are processed by `Invoice.Read`, it calls `Invoice.SetPartLineValues` for each top-level part.
    6.  `Invoice.SetPartLineValues` recursively calls itself for child parts first to gather all child data. Then, it iterates through the *parent's* distinct instances, populates parent fields for that instance, filters the pre-gathered child data for items matching the current parent instance, and attaches the relevant child items (correctly structured) to the parent item.
    7.  The final list of assembled parent items (with nested child data) is returned up the chain.
*   **Key Learnings (Shein Debugging - Recurring Header / Non-Recurring Detail):**
    *   **Instance Propagation:** It is crucial that `Part.Read` correctly determines and passes the `effectiveInstance` (the instance associated with the *parent* part that triggered the read) when recursively calling `Read` on child parts. This ensures that data extracted from the child part is tagged with the correct parent instance number, even if the child part itself doesn't have lines that define instance boundaries.
    *   **Data Aggregation (`SetPartLineValues`):** The aggregation logic must handle parent-child relationships correctly:
        1.  Process child parts *recursively first* to gather all child items across all instances they might belong to. Store these results (e.g., in a dictionary keyed by child part ID).
        2.  Iterate through the distinct instances captured for the *parent* part.
        3.  For each parent instance, create the parent item object.
        4.  Populate the parent item's fields based on data captured for that specific instance.
        5.  For each child part associated with the parent:
            *   Filter the pre-gathered child results (from step 1) to find items whose `Instance` property matches the *current parent instance*.
            *   Attach these filtered child items to the parent item under the appropriate field name.
    *   **Data Structure Consistency:** When attaching child data, ensure the data structure matches downstream expectations. If a subsequent process (e.g., `ShipmentInvoiceImporter`) expects a field containing child items to be a `List<IDictionary<string, object>>`, then even single, non-recurring child items must be wrapped in a `List` (e.g., `parentDitm[fieldname] = new List<IDictionary<string, object>> { childItem };`) before assignment in `SetPartLineValues`. Failure to do so can cause `InvalidCastException` errors later.
    *   **Debugging:** Extensive `Console.WriteLine` logging, clearly indicating Part IDs, Line IDs, internal vs. effective instances, and method entry/exit points, is invaluable for tracing complex recursive logic and instance handling.


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
        *   Saves `Attachment` records to the database (`EntryDataDSContext`).
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
*   **`RecreateEx9` (ActionId 90):**
    *   Calls `CreateEX9Utils.CreateEx9(true, -1)`.
    *   Based on whether documents were generated, it dynamically populates `filetype.ProcessNextStep` with a sequence of follow-up actions, either for re-warehousing (`ExportEx9Entries`, `AssessEx9Entries`, etc.) or for submission (`LinkPDFs`, `SubmitToCustoms`, etc.). This leverages the `ProcessNextStep` mechanism in `ImportUtils`.
    *   (Dependencies: `CreateEX9Utils`, `BaseDataModel`, `ImportUtils`).
    *   (Dependencies: `BaseDataModel`, `Utils.RetryImport`, `Utils.RunSiKuLi`, SikuliX).
*   **`RecreateEx9` (ActionId 90):**
    *   Calls `CreateEX9Utils.CreateEx9(true, -1)`.
    *   Based on whether documents were generated, it dynamically populates `filetype.ProcessNextStep` with a sequence of follow-up actions, either for re-warehousing (`ExportEx9Entries`, `AssessEx9Entries`, etc.) or for submission (`LinkPDFs`, `SubmitToCustoms`, etc.). This leverages the `ProcessNextStep` mechanism in `ImportUtils`.
    *   (Dependencies: `CreateEX9Utils`, `BaseDataModel`, `ImportUtils`).

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
*   **`RecreateEx9` (ActionId 90):**
    *   Calls `CreateEX9Utils.CreateEx9(true, -1)`.
    *   Based on whether documents were generated, it dynamically populates `filetype.ProcessNextStep` with a sequence of follow-up actions, either for re-warehousing (`ExportEx9Entries`, `AssessEx9Entries`, etc.) or for submission (`LinkPDFs`, `SubmitToCustoms`, etc.). This leverages the `ProcessNextStep` mechanism in `ImportUtils`.
    *   (Dependencies: `CreateEX9Utils`, `BaseDataModel`, `ImportUtils`).

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
*   **`RecreateEx9` (ActionId 90):**
    *   Calls `CreateEX9Utils.CreateEx9(true, -1)`.
    *   Based on whether documents were generated, it dynamically populates `filetype.ProcessNextStep` with a sequence of follow-up actions, either for re-warehousing (`ExportEx9Entries`, `AssessEx9Entries`, etc.) or for submission (`LinkPDFs`, `SubmitToCustoms`, etc.). This leverages the `ProcessNextStep` mechanism in `ImportUtils`.
    *   (Dependencies: `CreateEX9Utils`, `BaseDataModel`, `ImportUtils`).

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
*   **`RecreateEx9` (ActionId 90):**
    *   Calls `CreateEX9Utils.CreateEx9(true, -1)`.
    *   Based on whether documents were generated, it dynamically populates `filetype.ProcessNextStep` with a sequence of follow-up actions, either for re-warehousing (`ExportEx9Entries`, `AssessEx9Entries`, etc.) or for submission (`LinkPDFs`, `SubmitToCustoms`, etc.). This leverages the `ProcessNextStep` mechanism in `ImportUtils`.
    *   (Dependencies: `CreateEX9Utils`, `BaseDataModel`, `ImportUtils`).

### Action Method Implementations (C71Utils.cs)

*   **`ReSubmitSalesToCustoms` (ActionId 97):**
    *   Calls `DISUtils.GetSubmitEntryData(ft)` to retrieve the sales entries that need to be submitted, reusing discrepancy logic (querying `TODO_SubmitAllXMLToCustoms` view based on `ft` context). (Dependency: `DISUtils.GetSubmitEntryData`).
    *   Calls internal helper `SubmitSalesToCustoms(lst)` with the retrieved entries.
*   **`SubmitSalesToCustoms` (Helper):**
    *   Fetches "Customs" / "Clerk" contacts. (Dependency: `CoreEntitiesContext`, `Contacts`).
    *   Gathers associated PDF attachments for the entries, linking via `BaseDataModel.LinkPDFs` if needed. (Dependency: `BaseDataModel`, `CoreEntitiesContext`, `AsycudaDocument_Attachments`, `Attachments`).
    *   Constructs email body summarizing assessed sales entries.
    *   Creates `SalesSummary.csv` using `ExportToCSV`. (Dependency: `POUtils`, `ExportToCSV`, `StaTaskScheduler`).
    *   Sends a *new* email to contacts with subject "Assessed Ex-Warehoused Entries", body, summary CSV, and PDFs. (Dependency: `EmailDownloader`, `Utils.Client`).
    *   Logs "Submit Sales To Customs" action in `AttachmentLog` for each entry. (Dependency: `CoreEntitiesContext`, `AttachmentLog`).

### Action Method Implementations (C71Utils.cs)

*   **`CreateC71` (ActionId 36):**
    *   Queries `TODO_C71ToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   Defines target directory (`Imports/C71`).
    *   Defines target directory (`Imports/C71`).
    *   Defines target directory (`Imports/C71`).
    *   For each result:
        *   Skips if C71 XML already exists and matches CIF total.
        *   Queries `TODO_C71ToXML` view for data.
        *   Fetches supplier info and calls `GetConsigneeData` helper to retrieve consignee code, name, and address.
        *   Calls `C71ToDataBase.Instance.CreateC71` to generate C71 data structure. (Dependency: `C71ToDataBase`).
        *   Saves the generated `xC71_Value_declaration_form` to `ValuationDSContext`. (Dependency: `ValuationDSContext`).
        *   Calls `C71ToDataBase.Instance.ExportC71` to write the `C71.xml` file. (Dependency: `C71ToDataBase`).
*   **`AssessC71` (ActionId 49):**
    *   Queries `TODO_C71ToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   For each result:
        *   Checks for `C71-Instructions.txt`.
        *   Loops, calling `Utils.RunSiKuLi` with script "AssessC71" until helper `AssessC71Complete` indicates success by comparing instruction and result files. (Dependency: `Utils.RunSiKuLi`, SikuliX).
*   **`DownLoadC71` (ActionId 38):**
    *   Queries `TODO_C71ToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   Defines target directory (`Imports/C71`).
    *   Loops, calling `Utils.RunSiKuLi` with script "C71" until helper `ImportC71Complete` indicates success by checking `C71OverView-PDF.txt` against downloaded XML files. (Dependency: `Utils.RunSiKuLi`, SikuliX).
*   **`ImportC71` (ActionId 34):**
    *   Queries `TODO_C71ToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   For each result, calls helper `ImportC71(declarantRef, docSetId)`:
        *   Finds last imported C71 attachment date for the `docSetId`.
        *   Gets C71 `FileTypes` record.
        *   Finds new C71 XML files in `Imports/C71` folder.
        *   If new files exist, calls `BaseDataModel.Instance.ImportC71` to parse XMLs and save data to DB for the `docSetId`. (Dependency: `BaseDataModel`).

### Action Method Implementations (LICUtils.cs)

*   **`SubmitBlankLicenses` (ActionId 79):**
    *   Queries `TODO_LicenseToXML` view based on `ft.AsycudaDocumentSetId`.
    *   Generates a CSV report (`BlankLicenseDescription-{docSetId}.csv`) listing items with blank license descriptions using `ExportToCSV`.
    *   Forwards the original email (identified by `EmailId` from the view results) to "Broker" contacts, attaching the generated CSV report.
    *   (Dependencies: `CoreEntitiesContext`, `TODO_LicenseToXML` view, `ExportToCSV`, `EmailDownloader`, `Contacts`).

*   **`SubmitBlankLicenses` (ActionId 79):**
    *   Queries `TODO_LicenseToXML` view based on `ft.AsycudaDocumentSetId`.
    *   Generates a CSV report (`BlankLicenseDescription-{docSetId}.csv`) listing items with blank license descriptions using `ExportToCSV`.
    *   Forwards the original email (identified by `EmailId` from the view results) to "Broker" contacts, attaching the generated CSV report.
    *   (Dependencies: `CoreEntitiesContext`, `TODO_LicenseToXML` view, `ExportToCSV`, `EmailDownloader`, `Contacts`).

*   **`SubmitBlankLicenses` (ActionId 79):**
    *   Queries `TODO_LicenseToXML` view based on `ft.AsycudaDocumentSetId`.
    *   Generates a CSV report (`BlankLicenseDescription-{docSetId}.csv`) listing items with blank license descriptions using `ExportToCSV`.
    *   Forwards the original email (identified by `EmailId` from the view results) to "Broker" contacts, attaching the generated CSV report.
    *   (Dependencies: `CoreEntitiesContext`, `TODO_LicenseToXML` view, `ExportToCSV`, `EmailDownloader`, `Contacts`).

*   **`DownLoadLicence` (ActionId 39):**
    *   Queries `TODO_LICToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   Defines target directory (`Imports/LIC`).
    *   If `redownload` parameter is false (as it is when called via action):
        *   Loops, calling `Utils.RunSiKuLi` with script "LIC" until helper `ImportLICComplete` indicates success by checking `LICOverView-PDF.txt` against downloaded XML files. (Dependency: `Utils.RunSiKuLi`, SikuliX).
*   **`ImportLicense` (ActionId 35):**
    *   Queries `TODO_LICToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   For each result, calls helper `ImportLicense(declarantRef, docSetId)`:
        *   Finds last imported License attachment date for the `docSetId`.
        *   Gets License `FileTypes` record.
        *   Finds new License XML files (`*-LIC.xml`) in `Imports/LIC` folder.
        *   Filters XML files to ensure the Exporter Address in the XML matches an `EntryDataId` or `SupplierInvoiceNo` associated with the `docSetId`.
        *   Calls `BaseDataModel.Instance.ImportLicense` to parse filtered XMLs and save data to DB for the `docSetId`. (Dependency: `BaseDataModel`).
        *   Calls `BaseDataModel.Instance.SaveAttachedDocuments` to create attachment records. (Dependency: `BaseDataModel`).
*   **`CreateLicence` (ActionId 37):**
    *   Queries `TODO_LICToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   For each result:
        *   Fetches consignee details.
        *   Queries `TODO_LicenseToXML` view for data, grouping by EntryDataId, TariffCategoryCode, SourceFile.
        *   For each group:
            *   Skips if XML exists and assessment is complete (using `AssessLICComplete` helper).
            *   Fetches Broker contact and Supplier details.
            *   Calls `LicenseToDataBase.Instance.CreateLicense` to generate License data structure. (Dependency: `LicenseToDataBase`).
            *   Saves the generated `xLIC_License` to `LicenseDSContext`. (Dependency: `LicenseDSContext`).
            *   Calls `LicenseToDataBase.Instance.ExportLicense` to write the XML file. (Dependency: `LicenseToDataBase`).
*   **`AssessLicense` (ActionId 50):**
    *   Queries `TODO_LICToCreate` view based on `ft.AsycudaDocumentSetId`.
    *   For each result:
        *   Checks for `LIC-Instructions.txt`.
        *   Loops, calling `Utils.RunSiKuLi` with script "AssessLIC" until helper `AssessLICComplete` indicates success by comparing instruction and result files. (Dependency: `Utils.RunSiKuLi`, SikuliX).

### Action Method Implementations (Utils.cs - from AutoBot namespace)

*   **`SubmitMissingInvoices` (ActionId 40):**
*   Queries `TODO_SubmitMissingInvoices` view based on `ft.AsycudaDocumentSetId`.
*   Checks `ActionDocSetLogs` to prevent duplicates.
*   Fetches "PO Clerk" / "Broker" contacts.
*   Gets directory path via `POUtils.CurrentPOInfo`.
*   Generates `MissingInvoices-{docSetId}.csv` report using `ExportToCSV`.
*   Sends email with subject "Missing Invoices: {docSetRef}" attaching the CSV.
*   Logs action in `ActionDocSetLogs`. (Dependencies: `CoreEntitiesContext`, `TODO_SubmitMissingInvoices` view, `ActionDocSetLogs`, `Contacts`, `POUtils`, `ExportToCSV`, `EmailDownloader`).
*   **`SubmitIncompleteEntryData` (ActionId 41):**
*   Queries `TODO_SubmitIncompleteEntryData` view based on `ft.AsycudaDocumentSetId`.
*   Checks `ActionDocSetLogs` to prevent duplicates.
*   Fetches "PO Clerk" / "Broker" contacts.
*   Gets directory path via `POUtils.CurrentPOInfo`.
*   Generates `IncompleteEntryData-{docSetId}.csv` report using `ExportToCSV`.
*   Sends email with subject "Incomplete EntryData: {docSetRef}" attaching the CSV.
*   Logs action in `ActionDocSetLogs`. (Dependencies: `CoreEntitiesContext`, `TODO_SubmitIncompleteEntryData` view, `ActionDocSetLogs`, `Contacts`, `POUtils`, `ExportToCSV`, `EmailDownloader`).

### Action Method Implementations (XLSXProcessor.cs)

*   **`Xlsx2csv` (ActionId 13):**
*   Iterates through input Excel files (`files`).
*   Extracts data tables using `XLSXUtils.ExtractTables`. (Dependency: `XLSXUtils`).
*   Cleans/restructures data rows using `XLSXUtils.FixupDataSet`. (Dependency: `XLSXUtils`).
*   For the given `fileType`:
    *   Converts data rows to a CSV string using `XLSXUtils.GetText`. (Dependency: `XLSXUtils`).
    *   Checks if the file type is `Unknown` and attempts detection/reprocessing via `ProcessUnknownFileType` helper. (Dependency: `FileTypeManager`, `XLSXUtils`, `ImportUtils`).
    *   Throws error if XLSX `fileType` has no child `FileTypes`.
    *   Writes the CSV string to a new file (`*-Fixed.csv`) using `XLSXUtils.CreateCSVFile`. (Dependency: `XLSXUtils`).
    *   Calls `async XLSXUtils.FixCSVFile(fileType, overwrite, output)` to further process the newly created CSV file, likely invoking `CSVUtils` methods based on child `FileType` definitions. (Dependency: `XLSXUtils`).

### Action Method Implementations (InvoiceReader.cs - called by PDFUtils)

*   **`Import` (Called by `PDFUtils.ImportPDF`):**
*   Orchestrates PDF data extraction using OCR templates.
*   Calls `GetPdftxt` to extract text from the PDF file using multiple methods (PdfPig text ripping, Tesseract OCR).
*   Loads all active OCR templates (`Invoices`) from `OCRContext`.
*   Filters templates to find those matching the extracted text using identification regexes (`IsInvoiceDocument` helper).
*   For each matching template (`tmp`):
    *   Calls `TryReadFile(tmp)`:
        *   Applies template-specific formatting (`tmp.Format`).
        *   Parses the formatted text using the template's rules (`tmp.Read`) to get structured data (`csvLines`).
        *   Adds default Name/SupplierCode and required field values if missing.
        *   Writes formatted text to a `.txt` file.
        *   If parsing succeeds (`csvLines` has data and `tmp.Success` is true):
            *   Calls `ImportSuccessState`: Gets the target `FileTypes` based on the template and calls `DataFileProcessor.Process(dataFile)` to save the extracted `csvLines` to the database. (Dependency: `DataFileProcessor`). Returns `ImportStatus.Success` or `ImportStatus.HasErrors`.
        *   If parsing fails:
            *   Calls `ErrorState`: Determines if failure is partial or critical based on failed required fields. If partial, calls `ReportUnImportedFile`. Returns `ImportStatus.HasErrors` or `ImportStatus.Failed`.
    *   Records the import status for the template.
    *   If overall status is `Failed`, calls `ReportUnImportedFile`.
*   Returns a dictionary summarizing import results per template.
*   Includes top-level error handling that emails the original sender.
*   **`GetPdftxt` (Called by `Import` and `PDFUtils.ImportPDFDeepSeek`):**
*   Asynchronously extracts text from a PDF file using multiple methods:
    *   Text ripping via `PdfPig` library (`PdfPigText` helper).
    *   OCR via `PdfOcr` (Tesseract) using `PageSegMode.SingleColumn`.
    *   OCR via `PdfOcr` (Tesseract) using `PageSegMode.SparseText`.
*   Waits for all tasks to complete.
*   Combines the results from all methods into a single string, separated by headers.
*   **Helper `ReportUnImportedFile`:**
*   Writes extracted PDF text to `.txt` file.
*   Constructs an error email body.
*   Creates a test case file.
*   Saves detailed error information (PDF text, error message, failed lines/fields) to the `ImportErrors` table in `OCRContext`. (Dependency: `OCRContext`, `ImportErrors`).

### Action Method Implementations (DeepSeekInvoiceApi.cs - called by PDFUtils)

*   **Purpose:** Acts as a client for the external **DeepSeek API** (a third-party LLM service at `api.deepseek.com`). It's used as a fallback mechanism (`PDFUtils.ImportPDFDeepSeek`) to extract structured data from PDF text when primary OCR template matching fails.
*   **Constructor:**
*   Reads `DEEPSEEK_API_KEY` from environment variables.
*   Configures an `HttpClient` with authorization, headers, and timeout.
*   Sets a default `PromptTemplate` containing detailed instructions for the LLM on data extraction rules, field definitions, validation requirements, and the target JSON output schema.
*   Configures a Polly retry policy for handling transient API errors.
*   **`ExtractShipmentInvoice` (Called by `PDFUtils.ImportPDFDeepSeek`):**
*   Takes a list of text extractions (`pdfTextVariants`) from a single PDF.
*   For each variant:
    *   Cleans the text (`CleanText` helper).
    *   Calls `ProcessTextVariant`:
        *   Formats the `PromptTemplate` with the cleaned text.
        *   Calls `GetCompletionAsync` to send the request to the DeepSeek API (`/chat/completions` endpoint) using the configured model (`deepseek-chat`), temperature, and max tokens. (Dependency: DeepSeek API).
        *   Calls `ParseApiResponse` to process the JSON response from the LLM.
*   Calls `MergeDocuments` helper to combine results from different text variants.
*   Returns the merged, structured data as `List<dynamic>`.
*   **`ParseApiResponse` & Helpers:**
*   Cleans the raw JSON response from the LLM (removing markdown, etc.).
*   Parses the cleaned JSON using `System.Text.Json`.
*   Extracts data for "Invoices" and "CustomsDeclarations" sections based on the schema defined in the prompt, converting values to appropriate types.
*   Performs post-processing validation and enhancement (`ValidateAndEnhanceData`): checks totals, ensures SupplierCode != ConsigneeName, cleans supplier codes, derives country codes, validates Tariff Codes (defaulting to "000000").

### Action Method Implementations (DataFileProcessor.cs - called by InvoiceReader/SaveCSVModel)

*   **Purpose:** Acts as a dispatcher to route already extracted and structured data (contained in a `DataFile` object) to the appropriate data-saving logic based on file format and entry type.
*   **`Process(DataFile dataFile)`:**
*   Validates that the `AsycudaDocumentSet` in the `dataFile` belongs to the current `ApplicationSettings`.
*   Uses a nested dictionary (`_dataFileActions`) keyed by Format (e.g., "Csv", "Pdf") and then EntryType (e.g., "Invoice", "PO").
*   Looks up the specific action delegate based on `dataFile.FileType.FileImporterInfos.Format` and `dataFile.FileType.FileImporterInfos.EntryType`.
*   The delegates are sourced from static `Actions` dictionaries in other classes (e.g., `CSVDataFileActions.Actions`, `PDFDataFileActions.Actions`). These external dictionaries presumably map entry types to the actual methods that save the specific data (e.g., save invoice data, save PO data) to the database.
*   Invokes the found delegate asynchronously, passing the `dataFile` object.
*   Returns the boolean success status from the invoked delegate. (Dependency: `DataFile` class, `BaseDataModel`, `CSVDataFileActions`, `PDFDataFileActions`).

### Action Method Implementations (UpdateInvoice.cs)

*   **`UpdateRegEx` (ActionId 117):**
*   Acts as a command processor based on input `.txt` files.
*   Reads lines matching `Command: Param1: Value1...` from each input text file.
*   Parses the command name and parameters.
*   Looks up the command name in an internal dictionary (`RegExCommands`) which maps names to action delegates and required parameters.
*   Validates that all required parameters are present.
*   Invokes the corresponding action delegate.
*   The defined actions (`UpdateRegex`, `AddFieldRegEx`, `AddInvoice`, `AddPart`, `AddLine`, `UpdateLine`, `AddFieldFormatRegex`, `RequestInvoice`) primarily modify OCR configuration data within the `OCRContext` database (e.g., `RegularExpressions`, `Invoices`, `Parts`, `Lines`, `Fields`).
*   The `FileTypes` parameter is only used as context for the `RequestInvoice` command handler. (Dependency: `OCRContext`, `RegExCommands` helper and its delegates).

### Action Method Implementations (EntryDocSetUtils.cs)

*   **`SyncConsigneeInDB` (ActionId 121):**
    *   Retrieves `AsycudaDocumentSet` from `DocumentDSContext` based on `ft.AsycudaDocumentSetId`.
    *   Extracts `ConsigneeCode` from `ft.Data`.
    *   Finds the matching `Consignees` record in `CoreEntitiesContext` based on `ConsigneeCode` and `ApplicationSettingsId`.
    *   Updates `ConsigneeName` in the `DocumentDS.AsycudaDocumentSet`.
    *   Updates `Declarant_code` and `Declarant_name` in the associated `xcuda_Declarant` record.
    *   Saves changes to `DocumentDSContext`. (Dependency: `DocumentDSContext`, `CoreEntitiesContext`, `AsycudaDocumentSet`, `Consignees`, `xcuda_Declarant`, `BaseDataModel`).
*   **`AttachToDocSetByRef` (ActionId 51):**
    *   Gets document set directory path.
    *   Extracts attachment filenames from `ft.Data` (where `Key == "AttFileName"`).
    *   Uses `CoreEntitiesContext`.
    *   Fetches existing attachments for the `ft.AsycudaDocumentSetId`.
    *   Iterates through provided filenames:
        *   Skips if file doesn't exist or is already attached.
        *   Creates new `Attachments` and `AsycudaDocumentSet_Attachments` records to link the file to the document set.
    *   Saves changes. (Dependency: `BaseDataModel`, `CoreEntitiesContext`, `Attachments`, `AsycudaDocumentSet_Attachments`).
*   **`SubmitEntryCIF` (ActionId 106):**
    *   Gets document set info and directory path using `POUtils.CurrentPOInfo`. (Dependency: `POUtils`).
    *   Queries `AsycudaDocumentSetEntryCIF` view using `CoreEntitiesContext`, filtering by `ft.AsycudaDocumentSetId`. (Dependency: `CoreEntitiesContext`, `AsycudaDocumentSetEntryCIF` view).
    *   Generates `EntryCIF-{docSetId}.csv` report using `ExportToCSV`. (Dependency: `ExportToCSV`, `StaTaskScheduler`).
    *   Fetches "Broker" contacts. (Dependency: `CoreEntitiesContext`, `Contacts`).
    *   Sends a new email with subject "Entry CIF: {docSetRef}" attaching the CSV report. (Dependency: `EmailDownloader`, `Utils.Client`).

### Action Method Implementations (SubmitSalesToCustomsUtils.cs)

*   **`ReSubmitSalesToCustoms` (ActionId 97):**
    *   Calls `DISUtils.GetSubmitEntryData(ft)` to get sales entries to submit, reusing discrepancy logic (querying `TODO_SubmitAllXMLToCustoms` view based on `ft` context). (Dependency: `DISUtils.GetSubmitEntryData`).
    *   Calls internal helper `SubmitSalesToCustoms(lst)` with the retrieved entries.
*   **`SubmitSalesToCustoms` (Helper):**
    *   Fetches "Customs" / "Clerk" contacts. (Dependency: `CoreEntitiesContext`, `Contacts`).
    *   Gathers associated PDF attachments for the entries, linking via `BaseDataModel.LinkPDFs` if needed. (Dependency: `BaseDataModel`, `CoreEntitiesContext`, `AsycudaDocument_Attachments`, `Attachments`).
    *   Constructs email body summarizing assessed sales entries.
    *   Creates `SalesSummary.csv` using `ExportToCSV`. (Dependency: `POUtils`, `ExportToCSV`, `StaTaskScheduler`).
    *   Sends a *new* email to contacts with subject "Assessed Ex-Warehoused Entries", body, summary CSV, and PDFs. (Dependency: `EmailDownloader`, `Utils.Client`).
    *   Logs "Submit Sales To Customs" action in `AttachmentLog` for each entry. (Dependency: `CoreEntitiesContext`, `AttachmentLog`).

## Analysis: `AutoBot/SessionsUtils.cs`

*   **Namespace:** `AutoBot`
*   **Class:** `SessionsUtils`
*   **Purpose:** Defines the static `SessionActions` dictionary, analogous to `FileUtils.FileActions`, but for actions triggered by database sessions/schedules rather than file processing events.
*   **Key Member: `SessionActions` (static `Dictionary<string, Action>`)**
    *   **Initialization:** Uses a case-insensitive comparer (`WaterNut.DataSpace.Utils.ignoreCase`).
    *   **Content:** Contains numerous key-value pairs representing available session-based actions.
    *   **Keys:** String names like "CreateDiscpancyEntries", "AllocateSales", "CreateEx9", "AssessEx9Entries", "SubmitToCustoms", "DownloadPDFs", "LinkPDFs", "RunSQLBlackBox", etc. These names must match `Actions.Name` in the database and be referenced by `SessionActions` records.
    *   **Values:** Lambda expressions or method group references pointing to static methods in various utility classes. **Crucially, these delegates are parameterless (`Action`)**, unlike `FileActions` which take `FileTypes` and `FileInfo[]`. This implies session actions operate on broader system state or use context from `BaseDataModel` rather than specific file context.
    *   **Examples:**
        *   `{"CreateDiscpancyEntries", () => ADJUtils.CreateAdjustmentEntries(false, "DIS")}`
        *   `{"AllocateSales", AllocateSalesUtils.AllocateSales}`
        *   `{"CreateEx9", () => CreateEX9Utils.CreateEx9(false, -1)}`
        *   `{"AssessEx9Entries", () => EX9Utils.AssessEx9Entries(-1)}`
        *   `{"SubmitToCustoms", () => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1)}`
        *   `{"DownloadPDFs", PDFUtils.DownloadPDFs}`
        *   `{"LinkPDFs", PDFUtils.LinkPDFs}`
        *   `{"RunSQLBlackBox", SQLBlackBox.RunSqlBlackBox}`
        *   Many actions call methods with default parameters (e.g., `new FileTypes()`) suggesting they operate globally or retrieve context internally.
    *   **Dependencies:** Implicitly depends on all utility classes referenced in the dictionary values (e.g., `ADJUtils`, `AllocateSalesUtils`, `CreateEX9Utils`, `EX9Utils`, `SubmitSalesXmlToCustomsUtils`, `DISUtils`, `PDFUtils`, `EntryDocSetUtils`, `POUtils`, `C71Utils`, `LICUtils`, `ShipmentUtils`, `SQLBlackBox`, etc.).

## Analysis: SQL Scripts (`AutoBot1/WebSource-AutoBot Scripts/`)

*   **Initial Observation:** The list contains a large number of SQL files, primarily defining Views (`.View.sql`), Tables (`.Table.sql`), and Stored Procedures (`.StoredProcedure.sql`).
*   **Naming Conventions:** Names suggest a focus on:
    *   Asycuda Documents (`AsycudaDocument...`, `AsycudaDocumentSet...`, `AsycudaItem...`)
    *   Allocations (`Allocations...`, `AsycudaSalesAllocations...`, `AdjustmentShortAllocations...`)
    *   Entry Data (`EntryData...`, `AsycudaDocumentItemEntryDataDetails...`)
    *   Adjustments (`Adjustment...`)
    *   File/Email Processing (`FileType...`, `Email...`, `Actions...`, `Sessions...`, `EmailInfoMappings...`, `InfoMapping...`)
    *   Data Validation/Checks (`DataCheck...`, `TODO_...`)
    *   Inventory (`Inventory...`)
    *   Shipment (`ShipmentBL...`, `ShipmentInvoice...`, `ShipmentManifest...`)
*   **Key Scripts (Based on C# analysis):**
    *   Scripts related to `ApplicationSettings`, `FileTypes`, `FileTypeActions`, `Actions`, `EmailMapping`, `EmailFileTypes`, `EmailMappingActions`, `EmailInfoMappings`, `InfoMapping`, `InfoMappingRegEx`. **Crucially, `dbo.Actions.Table.sql` defines the available action names used as keys in `FileUtils.FileActions` and `SessionsUtils.SessionActions`.**
    *   Scripts defining `AsycudaDocumentSet`, `AsycudaDocumentSetExs`, `Customs_Procedures`, `AsycudaDocuments`, `xcuda_ASYCUDA`, `xcuda_Declarant`, `xcuda_Transport`.
    *   Scripts defining `SessionSchedule`, `Sessions`, `SessionActions`, `ParameterSet`, `Parameters`. **`dbo.SessionActions.Table.sql` links `Sessions` to `Actions`.**
    *   Scripts defining `Emails`, `Attachments`, `AsycudaDocumentSet_Attachments`.
    *   Scripts defining `Suppliers`, `InventoryItems`.
    *   Scripts defining `ShipmentBL`, `ShipmentInvoice`, `entrydata`, `ShipmentManifest`.
    *   Scripts defining Views: `TODO_SubmitUnclassifiedItems`, `TODO_SubmitIncompleteSupplierInfo`, `TODO_SubmitInadequatePackages`.
    *   Scripts related to `FileType` ID 1186 ("ShipmentFolder").
    *   Stored Procedure: `Stp_TODO_ImportCompleteEntries`, `PreProcessShipmentSP`.
*   **Analysis: `dbo.Actions.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the action.
        *   `Name` (nvarchar(100)): The string name of the action. **This must match the keys in `FileUtils.FileActions` and `SessionsUtils.SessionActions`.**
        *   `TestMode` (bit): Flag indicating if the action should run in test mode (likely compared against `ApplicationSettings.TestMode`).
        *   `IsDataSpecific` (bit, nullable): Flag indicating if the action requires specific file context (`FileTypes`, `FileInfo[]`). `true` means it's used by `ImportUtils.ExecuteDataSpecificFileActions`. `false` or `NULL` means it's used by `ImportUtils.ExecuteNonSpecificFileActions` or potentially `SessionsUtils.SessionActions`.
    *   **Data:** The script pre-populates the table with numerous actions, linking names like "ImportSalesEntries", "AllocateSales", "SaveCsv", "ImportPDF", "RunSQLBlackBox" to their respective `TestMode` and `IsDataSpecific` flags.
*   **Analysis: `dbo.SessionActions.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the link.
        *   `SessionId` (int, FK to `Sessions.Id`): Identifies the session this action belongs to.
        *   `ActionId` (int, FK to `Actions.Id`): Identifies the action to be executed for this session.
    *   **Purpose:** This table defines which actions (from `dbo.Actions`) are executed when a specific session (from `dbo.Sessions`) is triggered (either by schedule via `SessionSchedule` or by name like "End", "AssessIM7", "AssessEX").
    *   **Data:** The script pre-populates the table, linking various `SessionId`s to `ActionId`s. For example, Session 1 might execute actions 59, 25, 42, 2, 57, 56, 21, 22, 58. Session 2 executes actions 2, 3, 4, 5. This defines the sequence of operations for different sessions.
*   **Analysis: `dbo.Sessions.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the session.
        *   `Name` (nvarchar(50)): The human-readable name of the session (e.g., "Discrepancies", "Exwarehouse", "End", "AssessIM7", "AssessEX"). Used in `Program.cs` to trigger specific sessions.
        *   `WindowInMinutes` (int): Used by `Program.cs` when querying `SessionSchedule` to determine if a scheduled session is due to run (checks if `RunDateTime` is within `NOW +/- WindowInMinutes`).
    *   **Purpose:** Defines named sessions that group related actions (via `SessionActions`) and provides a time window for scheduled execution checks.
    *   **Data:** Pre-populates with sessions like "Discrepancies", "Exwarehouse", "CleanUp", "AssessIM7", "AssessEX", etc., each with a defined time window.
*   **Analysis: `dbo.FileTypeActions.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the link.
        *   `FileTypeId` (int, FK to `FileTypes.Id`): Identifies the file type this action applies to.
        *   `ActionId` (int, FK to `Actions.Id`): Identifies the action to execute for this file type.
        *   `AssessIM7` (bit, nullable): Filter flag, likely used in `ImportUtils` to conditionally execute actions based on `ApplicationSettings.AssessIM7`.
        *   `AssessEX` (bit, nullable): Filter flag, likely used in `ImportUtils` to conditionally execute actions based on `ApplicationSettings.AssessEX`.
    *   **Purpose:** This table defines the sequence of actions (from `dbo.Actions`) to be executed for a specific file type (from `dbo.FileTypes`). It allows customizing the processing pipeline for different types of input files. The `AssessIM7` and `AssessEX` flags provide further conditional logic based on application settings.
    *   **Data:** The script pre-populates the table, linking various `FileTypeId`s to `ActionId`s. For example, FileType 93 executes Action 43. FileType 109 executes Actions 15, 121, 40, 42, 47, 41, 44, 36, 49, 38, 34, 39, 35, 37, 50, 7, 51, 106, 8, 10. The order is determined by the `Id` column (implicitly used by `OrderBy(fta => fta.Id)` in `ImportUtils`).
*   **Analysis: `dbo.SessionSchedule.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the schedule entry.
        *   `SesseionId` (int, FK to `Sessions.Id`): Identifies the session to run. (Typo in original schema: `SesseionId`)
        *   `RunDateTime` (datetime): The specific date and time when the session should run.
        *   `ApplicationSettingId` (int, nullable): Optional filter to run the session only for a specific application setting.
        *   `ActionId` (int, nullable, FK to `Actions.Id` - *Implied, not explicitly defined in script*): Optional filter to run only a *specific action* within the scheduled session, rather than all actions linked via `SessionActions`.
        *   `ParameterSetId` (int, nullable, FK to `ParameterSet.Id`): Optional link to a set of parameters (defined in `ParameterSet` and `ParameterSetParameters`) to be used by the action(s).
    *   **Purpose:** Defines specific instances when a session (and potentially a specific action within it, with specific parameters) should be executed. This table drives the scheduled execution logic checked in `Program.cs` (`ExecuteDBSessionActions`).
    *   **Data:** Contains only one sample entry scheduling Session 40 to run at a specific past date/time (likely test data).
*   **Analysis: `dbo.EmailMappingActions.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the link.
        *   `EmailMappingId` (int, FK to `EmailMapping.Id`): Identifies the email mapping rule this action applies to.
        *   `ActionId` (int, FK to `Actions.Id`): Identifies the action to execute for this email mapping.
    *   **Purpose:** This table defines the sequence of actions (from `dbo.Actions`) to be executed when a specific email mapping rule (from `dbo.EmailMapping`) is matched during email processing. This allows triggering actions based on email sender, subject, etc., *before* processing specific file types attached to the email.
    *   **Data:** Contains only two sample entries linking EmailMapping 39 and 43 to Action 120.
*   **Analysis: `dbo.ParameterSet.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the parameter set.
        *   `Name` (nvarchar(50)): Human-readable name for the parameter set.
    *   **Purpose:** Defines named sets that can group parameters. These sets are referenced by `SessionSchedule` to potentially provide context or configuration to scheduled actions. The actual parameters are likely stored in `dbo.ParameterSetParameters`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.ParameterSetParameters.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the link.
        *   `ParameterSetId` (int, FK to `ParameterSet.Id`): Links to the named parameter set.
        *   `ParameterId` (int, FK to `Parameters.Id`): Links to the actual parameter definition (key/value pair, likely stored in `dbo.Parameters`).
    *   **Purpose:** This table acts as a many-to-many link between a named `ParameterSet` and the individual `Parameters` that belong to that set. It allows reusing parameters across different sets or defining multiple parameters for a single set.
    *   **Data:** No data inserted in this script. This means either parameters aren't currently used with schedules, or they are defined elsewhere/dynamically, or the `dbo.Parameters` table itself needs examination.
*   **Analysis: `dbo.Parameters.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the parameter.
        *   `Name` (nvarchar(50)): The name (key) of the parameter.
        *   `Value` (nvarchar(50)): The value of the parameter.
    *   **Purpose:** Stores the actual key-value pairs for parameters that can be grouped into sets (`ParameterSet`) via the `ParameterSetParameters` table and potentially passed to scheduled actions.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.AsycudaDocumentSetEx.View.sql`**
    *   **Purpose:** Provides an extended, aggregated view of `AsycudaDocumentSet` data. It joins the base table with other tables/views (`AsycudaDocumentSetEntryDataExTotals`, `xcuda_ASYCUDA_ExtendedProperties`, `AsycudaDocumentItemValueWeights`, `AsycudaDocumentSetEntryDataPackages`, `CurrencyRates`, `Customs_Procedure`) to calculate summary information (like `DocumentsCount`, `TotalCIF`, `TotalWeight`) and retrieve related data (like `ClassifiedLines`, `TotalLines`, `InvoiceTotal`, `EntryPackages`, `CurrencyRate`, `FreightCurrencyRate`, `Document_TypeId`) in a single query.
    *   **Usage:** Queried in `Program.ProcessEmails` to check for the existence of a document set based on `Declarant_Reference_Number` and `Customs_ProcedureId` before creating a new one, and to retrieve the `AsycudaDocumentSetId` if found.
*   **Analysis: `dbo.EmailMapping.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the email mapping rule.
        *   `ApplicationSettingsId` (int, FK to `ApplicationSettings`): Links to the application settings this rule belongs to.
        *   `Pattern` (nvarchar(max)): The regex pattern applied (likely to email subject) to identify the email type and extract data (e.g., using named capture group `?<Subject>`).
        *   `IsSingleEmail` (bit, nullable): Controls whether associated file actions run immediately or are deferred for batch processing per `AsycudaDocumentSet`.
        *   `ReplacementValue` (nvarchar(50), nullable): Potential value used in conjunction with the `Pattern` (usage not fully clear from C# code).
        *   `InfoFirst` (bit, nullable): Controls the processing order of `FileTypes` associated with the email (Info types first or last).
    *   **Purpose:** Defines rules based on regex patterns to classify incoming emails, extract key information (like a reference number), and control subsequent processing behavior.
    *   **Data:** Contains several regex patterns for matching different email subjects (e.g., "Shipment: ...", "Error: ...", "Submit Entries for: ...", "Invoice Template"). The last entry uses a complex negative lookahead to match subjects *not* containing specific keywords, likely as a default "Shipments" mapping.
*   **Analysis: `dbo.EmailInfoMappings.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the link.
        *   `EmailMappingId` (int, FK to `EmailMapping.Id`): Links to the email classification rule.
        *   `InfoMappingId` (int, FK to `InfoMapping.Id`): Links to the specific data extraction rule.
        *   `UpdateDatabase` (bit, nullable): Flag likely indicating whether the extracted info should trigger a database update via `EmailTextProcessor`.
    *   **Purpose:** This table links a matched email rule (`EmailMapping`) to one or more data extraction rules (`InfoMapping`). This allows applying specific data extraction logic (defined in `InfoMapping` and `InfoMappingRegEx`) based on the type of email identified. The `UpdateDatabase` flag controls whether the extracted data is used to update the DB.
    *   **Data:** Links EmailMapping 22 to several InfoMappings (1024, 1026-1030, 1036-1040, 1042, 1044, 1046), all with `UpdateDatabase = 1`.
*   **Analysis: `dbo.InfoMapping.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the mapping rule.
        *   `Key` (nvarchar(50)): The key name used to identify the extracted data (e.g., "Expected Entries", "Weight(Kg)"). This likely corresponds to the key extracted by `EmailTextProcessor`.
        *   `Field` (nvarchar(50)): The name of the database field (column) to update.
        *   `EntityType` (nvarchar(255)): The name of the database table (entity) to update.
        *   `ApplicationSettingsId` (int, FK to `ApplicationSettings`): Links to the application settings.
        *   `EntityKeyField` (nvarchar(50), nullable): The name of the key field in the `EntityType` table used in the `WHERE` clause of the `UPDATE` statement generated by `EmailTextProcessor`.
    *   **Purpose:** This table defines the mapping between a named piece of information (`Key`) extracted from text (likely via regex defined in `InfoMappingRegEx`) and a specific field in a specific database table (`EntityType`.`Field`). It also specifies the key field (`EntityKeyField`) needed to identify the correct row to update.
    *   **Data:** Contains mappings like: "Expected Entries" -> `AsycudaDocumentSet`.`ExpectedEntries`, "Weight(Kg)" -> `AsycudaDocumentSet`.`TotalWeight`, etc.
*   **Analysis: `dbo.InfoMappingRegEx.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the regex rule.
        *   `InfoMappingId` (int, FK to `InfoMapping.Id`): Links to the parent mapping rule.
        *   `KeyRegX` (nvarchar(1000)): Regex to identify the line containing the key.
        *   `FieldRx` (nvarchar(1000)): Regex to extract the value (field) from the line identified by `KeyRegX`. Uses named capture group `?<Value>`.
        *   `KeyReplaceRx` (nvarchar(1000), nullable): Optional regex for replacing parts of the extracted key.
        *   `FieldReplaceRx` (nvarchar(1000), nullable): Optional regex for replacing parts of the extracted value.
        *   `LineRegx` (nvarchar(1000)): Regex to identify the overall structure of the line containing the key-value pair (used by `EmailTextProcessor` to initially parse lines).
        *   `KeyValue` (nvarchar(50), nullable): Alternative way to specify the key if `KeyRegX` isn't sufficient? (Usage unclear).
        *   `FieldValue` (nvarchar(1000), nullable): Alternative way to specify the value if `FieldRx` isn't sufficient? (Usage unclear).
    *   **Purpose:** Stores the specific regular expressions used by `EmailTextProcessor` to locate and extract key-value pairs from text lines, based on a parent `InfoMapping` rule. Allows for complex extraction logic, including optional replacements.
    *   **Data:** Contains various regex patterns for extracting specific fields like Manifest number, BL number, Freight cost, Weight, Country of Origin, etc., often looking for a key followed by a colon/hash and capturing the subsequent value. Some entries have specific `KeyValue` overrides (e.g., for `CNumber`).
*   **Analysis: `dbo.AsycudaDocumentSet.Table.sql`**
    *   **Schema:**
        *   `AsycudaDocumentSetId` (int, PK, Identity): Primary key for the document set.
        *   `ApplicationSettingsId` (int, FK to `ApplicationSettings`): Links to the application settings.
        *   `Declarant_Reference_Number` (nvarchar(50), nullable): Reference number for the declarant.
        *   `Exchange_Rate` (float): Exchange rate used.
        *   `Customs_ProcedureId` (int, nullable, FK to `Customs_Procedure`): Links to the customs procedure.
        *   `Country_of_origin_code` (nvarchar(3), nullable): Code for the country of origin.
        *   `Currency_Code` (nvarchar(3), Default: 'USD'): Currency code for the document set.
        *   `Description` (nvarchar(255), nullable): Description of the document set.
        *   `Manifest_Number` (nvarchar(50), nullable): Manifest number.
        *   `BLNumber` (nvarchar(50), nullable): Bill of Lading number.
        *   `EntryTimeStamp` (datetime2(7), nullable, Default: sysutcdatetime()): Timestamp of record creation.
        *   `StartingFileCount` (int, nullable): Initial count of files?
        *   `ApportionMethod` (nvarchar(50), nullable): Method used for apportionment (e.g., "Equal").
        *   `TotalWeight` (float, nullable): Total weight.
        *   `TotalFreight` (float, nullable): Total freight cost.
        *   `TotalPackages` (int, nullable): Total number of packages.
        *   `LastFileNumber` (int, nullable): Last file number processed?
        *   `TotalInvoices` (int, nullable): Total number of invoices.
        *   `MaxLines` (int, nullable): Maximum lines allowed per document?
        *   `LocationOfGoods` (nvarchar(50), nullable): Location of goods.
        *   `FreightCurrencyCode` (nvarchar(3), Default: 'USD'): Currency code for freight.
        *   `Office` (nvarchar(50), nullable): Customs office?
        *   `UpgradeKey` (int, nullable): Purpose unclear, possibly related to data migration or versioning.
        *   `ExpectedEntries` (int, nullable): Expected number of entries?
        *   `PackageType` (nvarchar(50), nullable): Type of packages.
        *   `ConsigneeName` (nvarchar(100), nullable, FK to `Consignees`): Name of the consignee.
    *   **Purpose:** Represents a collection of documents related to a single customs declaration or shipment. It holds metadata like reference numbers, currency, totals (weight, freight, packages, invoices), and links to other entities like `ApplicationSettings`, `Customs_Procedure`, and `Consignees`. This table is central to organizing related customs documents and is populated/updated by various processes, including email processing (`Program.cs`) and shipment folder processing (`ShipmentUtils.ImportShipmentInfoFromTxt`).
    *   **Data:** Contains two sample entries, one for "Imports" and one for "Shipments", with some pre-filled data like currency codes and customs procedure ID.
*   **Analysis: `dbo.xcuda_ASYCUDA.Table.sql`**
    *   **Schema:**
        *   `id` (nvarchar(10), nullable): Seems like a document identifier, possibly from the ASYCUDA system itself. Indexed.
        *   `ASYCUDA_Id` (int, PK, Identity): Primary key for this table, likely used as the foreign key in related `xcuda_` tables.
        *   `EntryTimeStamp` (datetime2(7), nullable, Default: sysutcdatetime()): Timestamp of record creation.
        *   `UpgradeKey` (int, nullable): Purpose unclear, possibly related to data migration or versioning.
    *   **Purpose:** Appears to be the root table representing a single ASYCUDA document. It holds a minimal set of identifiers (`id`, `ASYCUDA_Id`) and metadata. The actual details of the document (declarant, transport, items, etc.) are likely stored in other `xcuda_` tables linked via `ASYCUDA_Id`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.xcuda_Declarant.Table.sql`**
    *   **Schema:**
        *   `Declarant_code` (nvarchar(20), nullable): Code identifying the declarant.
        *   `Declarant_name` (nvarchar(255), nullable): Name of the declarant.
        *   `Declarant_representative` (nvarchar(255), nullable): Name of the declarant's representative.
        *   `ASYCUDA_Id` (int, PK, FK to `xcuda_ASYCUDA`): Links to the main ASYCUDA document. This is the primary key, implying a one-to-one relationship with `xcuda_ASYCUDA`.
        *   `Number` (nvarchar(30), nullable): Declarant number? (Usage unclear).
    *   **Purpose:** Stores information about the declarant associated with an ASYCUDA document. Linked directly to `xcuda_ASYCUDA` via `ASYCUDA_Id`.
    *   **Data:** No data inserted in this script.

## FileTypeActions to C# Method Mapping

This section provides an exhaustive mapping from the `(FileTypeId, ActionId)` pairs defined in the `dbo.FileTypeActions.Table.sql` data to the specific C# method delegates invoked via the `FileUtils.FileActions` dictionary. The actions for each `FileTypeId` are listed in the order they are executed (based on the `Id` column in `FileTypeActions`).

*   **FileType 93:**
    *   ActionId 43 (`MapUnClassifiedItems`) -> `(ft, fs) => ShipmentUtils.MapUnClassifiedItems(ft,fs)`
*   **FileType 94:**
    *   ActionId 48 (`UpdateSupplierInfo`) -> `(ft, fs) => ShipmentUtils.UpdateSupplierInfo(ft,fs)`
*   **FileType 102:**
    *   ActionId 95 (`ReSubmitDiscrepanciesToCustoms`) -> `DISUtils.ReSubmitDiscrepanciesToCustoms`
*   **FileType 103:**
    *   ActionId 97 (`ReSubmitSalesToCustoms`) -> `SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms`
*   **FileType 107:**
    *   ActionId 1 (`ImportSalesEntries`) -> `(ft, fs) => DocumentUtils.ImportSalesEntries(false)`
*   **FileType 109:** (Execution Order: 1373-1392)
    *   ActionId 15 (`SaveInfo`) -> `(ft, fs) => EmailTextProcessor.Execute(fs, ft)`
    *   ActionId 121 (`SyncConsigneeInDB`) -> `EntryDocSetUtils.SyncConsigneeInDB`
    *   ActionId 40 (`SubmitMissingInvoices`) -> `(ft, fs) => Utils.SubmitMissingInvoices(ft)`
    *   ActionId 42 (`SubmitUnclassifiedItems`) -> `(ft, fs) => ShipmentUtils.SubmitUnclassifiedItems(ft)`
    *   ActionId 47 (`SubmitIncompleteSuppliers`) -> `(ft, fs) => ShipmentUtils.SubmitIncompleteSuppliers(ft)`
    *   ActionId 41 (`SubmitIncompleteEntryData`) -> `(ft, fs) => Utils.SubmitIncompleteEntryData(ft)`
    *   ActionId 44 (`SubmitInadequatePackages`) -> `(ft, fs) => ShipmentUtils.SubmitInadequatePackages(ft)`
    *   ActionId 36 (`CreateC71`) -> `(ft, fs) => C71Utils.CreateC71(ft)`
    *   ActionId 49 (`AssessC71`) -> `(ft, fs) => C71Utils.AssessC71(ft)`
    *   ActionId 38 (`DownLoadC71`) -> `(ft, fs) => C71Utils.DownLoadC71(ft)`
    *   ActionId 34 (`ImportC71`) -> `(ft, fs) => C71Utils.ImportC71(ft)`
    *   ActionId 39 (`DownLoadLicense`) -> `(ft, fs) => LICUtils.DownLoadLicence(false, ft)`
    *   ActionId 35 (`ImportLicense`) -> `(ft, fs) => LICUtils.ImportLicense(ft)`
    *   ActionId 37 (`CreateLicense`) -> `(ft, fs) => LICUtils.CreateLicence(ft)`
    *   ActionId 50 (`AssessLicense`) -> `(ft, fs) => LICUtils.AssessLicense(ft)`
    *   ActionId 7 (`RecreatePOEntries`) -> `(ft, fs) => POUtils.RecreatePOEntries(ft.AsycudaDocumentSetId)`
    *   ActionId 51 (`AttachToDocSetByRef`) -> `(ft, fs) => EntryDocSetUtils.AttachToDocSetByRef(ft)`
    *   ActionId 106 (`SubmitEntryCIF`) -> `EntryDocSetUtils.SubmitEntryCIF`
    *   ActionId 8 (`ExportPOEntries`) -> `(ft, fs) => POUtils.ExportPOEntries(ft.AsycudaDocumentSetId)`
    *   ActionId 10 (`AssessPOEntries`) -> `(ft, fs) => POUtils.AssessPOEntries(ft)`
*   **FileType 114:**
    *   ActionId 69 (`ReplaceCSV`) -> `(ft, fs) => CSVUtils.ReplaceCSV(fs, ft)`
*   **FileType 1126:** (Execution Order: 1324-1325)
    *   ActionId 15 (`SaveInfo`) -> `(ft, fs) => EmailTextProcessor.Execute(fs, ft)`
    *   ActionId 53 (`SubmitPOs`) -> `POUtils.SubmitPOs`
*   **FileType 1127:** (Execution Order: 1326-1327)
    *   ActionId 15 (`SaveInfo`) -> `(ft, fs) => EmailTextProcessor.Execute(fs, ft)`
    *   ActionId 104 (`DeletePONumber`) -> `POUtils.DeletePONumber`
*   **FileType 1141:**
    *   ActionId 13 (`Xlsx2csv`) -> `(ft, fs) => XLSXProcessor.Xlsx2csv(fs, new List<FileTypes>(){ft}).GetAwaiter().GetResult()`
*   **FileType 1143:**
    *   ActionId 6 (`SaveCsv`) -> `(ft, fs) => CSVUtils.SaveCsv(fs, ft).GetAwaiter().GetResult()`
*   **FileType 1144:**
    *   ActionId 52 (`ImportPDF`) -> `(ft, fs) => PDFUtils.ImportPDF(fs, ft).GetAwaiter().GetResult()`
*   **FileType 1145:**
    *   ActionId 6 (`SaveCsv`) -> `(ft, fs) => CSVUtils.SaveCsv(fs, ft).GetAwaiter().GetResult()`
*   **FileType 1146:**
    *   ActionId 13 (`Xlsx2csv`) -> `(ft, fs) => XLSXProcessor.Xlsx2csv(fs, new List<FileTypes>(){ft}).GetAwaiter().GetResult()`
*   **FileType 1148:**
    *   ActionId 111 (`CreateShipmentEmail`) -> `ShipmentUtils.CreateShipmentEmail`
*   **FileType 1151:**
    *   ActionId 13 (`Xlsx2csv`) -> `(ft, fs) => XLSXProcessor.Xlsx2csv(fs, new List<FileTypes>(){ft}).GetAwaiter().GetResult()`
*   **FileType 1152:**
    *   ActionId 69 (`ReplaceCSV`) -> `(ft, fs) => CSVUtils.ReplaceCSV(fs, ft)`
*   **FileType 1158:**
    *   ActionId 112 (`ImportUnAttachedSummary`) -> `(ft,fs) => ShipmentUtils.ImportUnAttachedSummary(ft, fs)`
*   **FileType 1173:**
    *   ActionId 117 (`UpdateRegEx`) -> `UpdateInvoice.UpdateRegEx`
*   **Analysis: `dbo.xcuda_Transport.Table.sql`**
    *   **Schema:**
        *   `Container_flag` (bit): Flag indicating if goods are containerized.
        *   `Single_waybill_flag` (bit): Flag indicating a single waybill.
        *   `Transport_Id` (int, PK, Identity): Primary key for this transport record.
        *   `ASYCUDA_Id` (int, nullable, FK to `xcuda_ASYCUDA`): Links to the main ASYCUDA document. Nullable suggests it might not always be directly linked or could be linked via another table (though the FK implies a direct link).
        *   `Location_of_goods` (nvarchar(50), nullable): Code or description of the goods' location.
    *   **Purpose:** Stores transport-related information for an ASYCUDA document, specifically flags for containerization and waybill type, and the location of goods. Linked to `xcuda_ASYCUDA` via `ASYCUDA_Id`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.xcuda_Item.Table.sql`**
    *   **Schema:**
        *   `Amount_deducted_from_licence` (nvarchar(10), nullable): Amount deducted from a license.
        *   `Quantity_deducted_from_licence` (nvarchar(4), nullable): Quantity deducted from a license.
        *   `Item_Id` (int, PK, Identity): Primary key for the item line.
        *   `ASYCUDA_Id` (int, FK to `xcuda_ASYCUDA`): Links to the main ASYCUDA document.
        *   `Licence_number` (nvarchar(50), nullable): License number associated with the item.
        *   `Free_text_1`, `Free_text_2` (nvarchar(35/30), nullable): Free text fields for additional item information.
        *   `EntryDataDetailsId` (int, nullable, FK to `EntryDataDetails`): Foreign key linking to the source data line item.
        *   `LineNumber` (int): Line number of the item within the document.
        *   `IsAssessed` (bit, nullable): Flag indicating if the item has been assessed.
        *   `DPQtyAllocated`, `DFQtyAllocated` (float): Quantities allocated (Duty Paid? Duty Free?).
        *   `EntryTimeStamp` (datetime2(7), nullable, Default: sysutcdatetime()): Timestamp of record creation.
        *   `AttributeOnlyAllocation` (bit, nullable): Flag for allocation type?
        *   `DoNotAllocate` (bit, nullable): Flag to prevent allocation.
        *   `DoNotEX` (bit, nullable): Flag to prevent EX processing?
        *   `ImportComplete` (bit): Flag indicating if import processing is complete for this item.
        *   `WarehouseError` (nvarchar(50), nullable): Stores warehouse error messages.
        *   `SalesFactor` (float, Default: 1): Factor used in sales calculations?
        *   `PreviousInvoiceNumber`, `PreviousInvoiceLineNumber`, `PreviousInvoiceItemNumber` (nvarchar(50), nullable): Fields for linking to previous invoice data.
        *   `EntryDataType` (nvarchar(50), nullable, FK to `EntryDataType`): Type of entry data associated with the item.
        *   `UpgradeKey` (int, nullable): Purpose unclear, possibly related to data migration or versioning.
        *   `PreviousInvoiceKey` (Computed, Persisted): Concatenated key from previous invoice number and line number. Indexed.
        *   `xWarehouseError` (nvarchar(255), nullable): Extended warehouse error message?
    *   **Purpose:** Represents individual line items within an ASYCUDA document. Contains details about the item itself (potentially via `EntryDataDetailsId`), links to licenses, processing status flags (`IsAssessed`, `ImportComplete`), allocation details (`DPQtyAllocated`, `DFQtyAllocated`, `DoNotAllocate`), links to previous invoice data, and error tracking (`WarehouseError`). This table is likely heavily involved in allocation and processing logic.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.ApplicationSettings.Table.sql`**
    *   **Schema:** Contains a wide range of settings:
        *   `ApplicationSettingsId` (PK), `Description`, `CompanyName`, `IsActive`.
        *   UI/Feature Flags: `AllowCounterPoint`, `AllowTariffCodes`, `AllowWareHouse`, `AllowXBond`, `AllowAsycudaManager`, `AllowQuickBooks`, `AllowExportToExcel`, `AllowAutoWeightCalculation`, `AllowEntryPerIM7`, `AllowSalesToPI`, `AllowEffectiveAssessmentDate`, `AllowAutoFreightCalculation`, `AllowSubItems`, `AllowEntryDoNotAllocate`, `AllowPreviousItems`, `AllowOversShort`, `AllowContainers`, `AllowNonXEntries`, `AllowValidateTariffCodes`, `AllowCleanBond`, `AllowImportXSales`, `AllowAdvanceWareHouse`, `AllowStressTest`. (Many are nvarchar(10/50) storing "Visible", "Collapsed", "Hidden" - likely controlling UI visibility).
        *   Processing Logic Flags: `GroupEX9`, `InvoicePerEntry`, `ItemDescriptionContainsAsycudaAttribute`, `RequirePOs`, `ExportNullTariffCodes`, `PreAllocateEx9s`, `NotifyUnknownMessages`, `ExportExpiredEntries`, `GroupShipmentInvoices`.
        *   Configuration Values: `MaxEntryLines`, `SoftwareName`, `OrderEntriesBy`, `OpeningStockDate`, `WeightCalculationMethod` (FK), `BondQuantum`, `DataFolder`, `Email`, `EmailPassword`, `AsycudaLogin`, `AsycudaPassword`, `BondTypeId` (FK), `AllocationsOpeningStockDate`.
        *   Execution Control: `AssessIM7`, `AssessEX`, `TestMode`.
    *   **Purpose:** This table is the central configuration hub for the application. Each row likely represents a specific client setup or operational mode. It controls which features are enabled/visible, defines processing rules (like grouping, allocation methods, date ranges), stores credentials for external systems (Email, ASYCUDA), specifies file paths, and sets flags for testing and specific assessment types (IM7, EX). The `Program.cs` loads these settings at startup to tailor the application's behavior.
    *   **Data:** Contains one sample entry for "Water Nut" / "Web Source", enabling many features and defining specific paths, credentials (masked/placeholder), and processing rules.
*   **Analysis: `dbo.FileTypes.Table.sql`**
    *   **Schema:**
        *   `Id` (int, PK, Identity): Unique identifier for the file type.
        *   `ApplicationSettingsId` (int, FK to `ApplicationSettings`): Links to the application settings.
        *   `Description` (nvarchar(50), nullable): Human-readable description (e.g., "Freight", "Manifest", "Unknown").
        *   `FilePattern` (nvarchar(255)): Regex pattern used to identify files belonging to this type. Crucial for matching files in email attachments or folders.
        *   `CreateDocumentSet` (bit): Flag indicating if processing this file type should trigger the creation of an `AsycudaDocumentSet`.
        *   `DocumentSpecific` (bit): Flag indicating if this file type relates to a specific document within a set (e.g., an invoice) vs. the set as a whole (e.g., a manifest).
        *   `DocumentCode` (nvarchar(50)): Code used when attaching this file type as a document (e.g., "IV05", "BL07", "NA").
        *   `ReplyToMail` (bit): Flag indicating if a reply email should be sent after processing.
        *   `FileGroupId` (int, nullable, FK to `FileGroups`): Optional grouping for related file types.
        *   `MergeEmails` (bit): Flag related to merging data from multiple emails?
        *   `CopyEntryData` (bit): Flag indicating if entry data should be copied.
        *   `ParentFileTypeId` (int, nullable, FK to `FileTypes`): Self-referencing key for hierarchical file types (e.g., a CSV derived from an XLSX).
        *   `OverwriteFiles` (bit, nullable): Flag to control overwriting existing files.
        *   `HasFiles` (bit, nullable): Flag indicating if this type represents actual files?
        *   `OldFileTypeId` (int, nullable): Legacy ID?
        *   `ReplicateHeaderRow` (bit, nullable): Flag for CSV/Excel processing?
        *   `IsImportable` (bit, nullable): Flag indicating if the file type can be imported directly.
        *   `MaxFileSizeInMB` (int, nullable): Maximum allowed file size.
        *   `FileInfoId` (int, nullable, FK to `FileTypes-FileImporterInfo`): Link to specific importer configuration details.
        *   `DocSetRefernece` (nvarchar(50), nullable): Reference string, often "Imports" or "Shipments", possibly used for context or filtering.
    *   **Purpose:** Defines how different files are identified (via `FilePattern`) and processed. Links file types to application settings, specifies document codes, controls behavior like document set creation, merging, and copying data, and links to specific importer configurations (`FileInfoId`). This table, along with `FileTypeActions`, forms the core of the file processing pipeline configuration.
    *   **Data:** Contains numerous pre-defined file types for ApplicationSettingId 3 ("Water Nut"). Includes patterns for XML (LIC, C71), CSV (Unclassified, Incomplete Suppliers, Fixed), PDF (Freight, Manifest, Invoice, BL, C14, Previous Decl), XLSX (PO, Summary), and TXT (Info). Notably includes ID 1183 for "Unknown" PDFs and ID 1186 (not shown in provided snippet, but referenced in `FolderProcessor`) likely for "ShipmentFolder" / `Info.txt`.
*   **Analysis: `dbo.EntryDataType.Table.sql`**
    *   **Schema:**
        *   `EntryDataType` (nvarchar(50), PK): The primary key, a short code representing the type (e.g., "INV", "PO", "ADJ").
        *   `Description` (nvarchar(50)): Human-readable description (e.g., "Shipment Invoice", "Purchase Order", "Adjustments").
        *   `RequirePreviousCNumber` (bit): Flag indicating if linking to a previous Customs Number is required for this type.
        *   `RequirePreviousInvoiceNumber` (bit): Flag indicating if linking to a previous Invoice Number is required for this type.
    *   **Purpose:** Defines the different categories of source data that can be processed (Invoices, POs, Sales, Adjustments, Discrepancies, Opening Stock). It also specifies validation rules regarding links to previous documents, which is particularly relevant for Discrepancy (`DIS`) entries. This table is referenced by `xcuda_Item` to classify the origin or nature of the item data.
    *   **Data:** Pre-populates with types: "ADJ" (Adjustments), "DIS" (Discrepancy - requires previous C# and Invoice#), "INV" (Shipment Invoice), "OPS" (Opening Stock), "PO" (Purchase Order), "Sales".
*   **Analysis: `dbo.xcuda_Valuation_item.Table.sql`**
    *   **Schema:**
        *   `Total_cost_itm` (float): Total cost of the item.
        *   `Total_CIF_itm` (float): Total CIF (Cost, Insurance, Freight) value of the item.
        *   `Rate_of_adjustement` (float, nullable): Rate of adjustment applied.
        *   `Statistical_value` (float): Statistical value of the item for customs purposes.
        *   `Alpha_coeficient_of_apportionment` (nvarchar(50), nullable): Coefficient used for apportionment calculations?
        *   `Item_Id` (int, PK, FK to `xcuda_Item`): Links to the specific item line. This is the primary key, implying a one-to-one relationship with `xcuda_Item`.
    *   **Purpose:** Stores the valuation details for each item in an ASYCUDA document, including cost, CIF, statistical value, and potentially adjustment/apportionment factors. This is directly linked to an `xcuda_Item`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.xcuda_Tarification.Table.sql`**
    *   **Schema:**
        *   `Extended_customs_procedure` (nvarchar(20), nullable): Extended customs procedure code.
        *   `National_customs_procedure` (nvarchar(20), nullable): National customs procedure code.
        *   `Item_price` (float): Price of the item.
        *   `Item_Id` (int, PK, FK to `xcuda_Item`): Links to the specific item line. This is the primary key, implying a one-to-one relationship with `xcuda_Item`.
        *   `Value_item` (nvarchar(20), nullable): Value item code/reference?
        *   `Attached_doc_item` (nvarchar(20), nullable): Attached document item code/reference?
    *   **Purpose:** Stores tarification details for each item, including customs procedure codes and the item price. It seems to be the parent table for supplementary units. Linked one-to-one with `xcuda_Item`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.xcuda_Supplementary_unit.Table.sql`**
    *   **Schema:**
        *   `Suppplementary_unit_quantity` (float, nullable): Quantity in the supplementary unit. (Typo in column name: `Suppplementary`)
        *   `Supplementary_unit_Id` (int, PK, Identity): Primary key for this supplementary unit record.
        *   `Tarification_Id` (int, FK to `xcuda_Tarification`): Links to the tarification record for the item. Note: FK links to `xcuda_Tarification.Item_Id`, effectively linking to the `xcuda_Item`.
        *   `Suppplementary_unit_code` (nvarchar(4), nullable): Code for the supplementary unit (e.g., 'L', 'KG'). (Typo in column name: `Suppplementary`)
        *   `Suppplementary_unit_name` (nvarchar(255), nullable): Name of the supplementary unit. (Typo in column name: `Suppplementary`)
        *   `IsFirstRow` (bit, nullable): Flag indicating if this is the first supplementary unit for the item?
    *   **Purpose:** Stores quantity information for an item in a unit of measure other than the primary statistical unit (e.g., Liters, Kilograms). An item can potentially have multiple supplementary units. Linked to `xcuda_Tarification`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.EntryDataDetails.Table.sql`**
    *   **Schema:**
        *   `EntryDataDetailsId` (int, PK, Identity): Primary key for the source document line item.
        *   `EntryData_Id` (int, FK to `EntryData`): Links to the parent `EntryData` header record.
        *   `EntryDataId` (nvarchar(50)): Likely the source document number (e.g., Invoice #, PO #). Used with `LineNumber` in a computed key.
        *   `LineNumber` (int, nullable): Line number within the source document.
        *   `ItemNumber` (nvarchar(20)): Item identifier from the source document.
        *   `Quantity` (float): Quantity of the item.
        *   `Units` (nvarchar(15), nullable): Unit of measure for the quantity.
        *   `ItemDescription` (nvarchar(255)): Description from the source document.
        *   `Cost` (float): Unit cost of the item.
        *   `TotalCost` (float, nullable): Total cost for the line (likely Quantity * Cost).
        *   `QtyAllocated` (float): Quantity already allocated from this line.
        *   `UnitWeight` (float): Weight per unit.
        *   `DoNotAllocate` (bit, nullable): Flag to prevent allocation from this line.
        *   `Freight`, `Weight`, `InternalFreight` (float, nullable): Cost/Weight details.
        *   `Status` (nvarchar(50), nullable): Status of the line item.
        *   `InvoiceQty`, `ReceivedQty` (float, nullable): Quantities related to invoicing/receiving.
        *   `PreviousInvoiceNumber`, `CNumber`, `Comment` (nvarchar(255), nullable): Fields for linking/comments.
        *   `EffectiveDate` (datetime2(7), nullable): Effective date for the line item?
        *   `IsReconciled` (bit, nullable): Reconciliation status flag.
        *   `TaxAmount`, `LastCost` (float, nullable): Tax and last cost information.
        *   `InventoryItemId` (int, FK to `InventoryItems`): Links to the master `InventoryItems` record.
        *   `FileLineNumber` (int, nullable): Line number from the original source file?
        *   `UpgradeKey` (int, nullable): Migration/versioning key.
        *   `VolumeLiters` (float, nullable): Volume in liters.
        *   `EntryDataDetailsKey` (Computed, Persisted): Composite key `EntryDataId | LineNumber`.
        *   `TotalValue` (Computed, Persisted): Calculated as `Quantity * Cost`.
        *   `CLineNumber` (int, nullable): Another line number field?
    *   **Purpose:** Represents the line items from source documents (Invoices, POs, etc.) before they are processed into ASYCUDA documents. It holds detailed information about each item, including quantity, cost, description, weight, allocation status, and links back to the source document header (`EntryData`) and the master inventory item (`InventoryItems`). This table acts as a staging area or source of truth for data that eventually populates `xcuda_Item` and related tables.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.EntryData.Table.sql`**
    *   **Schema:**
        *   `EntryDataId` (nvarchar(50)): Source document identifier (e.g., Invoice #, PO #). Part of computed key in `EntryDataDetails`.
        *   `ApplicationSettingsId` (int, FK to `ApplicationSettings`): Links to application settings.
        *   `EntryDataDate` (datetime2(7)): Date of the source document.
        *   `InvoiceTotal` (float, nullable): Total value of the invoice/document.
        *   `ImportedLines` (int, nullable): Number of lines imported from the source file.
        *   `SupplierCode` (nvarchar(100), nullable, FK to `Suppliers`): Links to the supplier.
        *   `TotalFreight`, `TotalInternalFreight`, `TotalWeight`, `TotalOtherCost`, `TotalInsurance`, `TotalDeduction` (float, nullable): Various total cost/weight components.
        *   `Currency` (nvarchar(4), nullable): Currency code for the document.
        *   `FileTypeId` (int, nullable, FK to `FileTypes`): Links to the `FileTypes` definition used to import this data.
        *   `SourceFile` (nvarchar(max), nullable): Path or name of the original source file.
        *   `EntryData_Id` (int, PK, Identity): Primary key for this header record.
        *   `Packages` (int, nullable): Number of packages.
        *   `UpgradeKey` (nvarchar(50), nullable): Migration/versioning key.
        *   `EntryType` (nvarchar(50), nullable, FK to `EntryDataType`): Type of entry (e.g., "INV", "PO").
        *   `EmailId` (nvarchar(255), nullable, FK to `Emails`): Links to the email record if imported via email.
    *   **Purpose:** Represents the header information for a source document (Invoice, PO, etc.) that has been imported into the system. It contains document-level details like date, totals, supplier, currency, and links to the application settings, the file type used for import, the supplier, the entry data type, and potentially the source email. It serves as the parent record for the line items stored in `EntryDataDetails`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.InventoryItems.Table.sql`**
    *   **Schema:**
        *   `ItemNumber` (nvarchar(20)): The unique identifier for the item within a specific `ApplicationSettingsId`.
        *   `ApplicationSettingsId` (int, FK to `ApplicationSettings`): Links to the application settings.
        *   `Description` (nvarchar(255)): Description of the item.
        *   `Category` (nvarchar(60), nullable): Item category.
        *   `TariffCode` (nvarchar(50), nullable, FK to `TariffCodes`): Links to the customs tariff code.
        *   `EntryTimeStamp` (datetime2(7), nullable, Default: sysutcdatetime()): Timestamp of record creation.
        *   `Id` (int, PK, Identity): Primary key for the inventory item record.
        *   `UpgradeKey` (nvarchar(20), nullable): Migration/versioning key.
    *   **Purpose:** Serves as the master table for inventory items. It defines the item's primary identifier (`ItemNumber`), description, category, and crucially, its customs `TariffCode`. It's linked per `ApplicationSettingsId`, suggesting item masters might be specific to a client/configuration. Referenced by `EntryDataDetails`.
    *   **Data:** No data inserted in this script.
*   **Analysis: `dbo.Suppliers.Table.sql`**
    *   **Schema:**
        *   `SupplierCode` (nvarchar(100), PK): The primary key, identifying the supplier. Part of a composite key with `ApplicationSettingsId`.
        *   `SupplierName` (nvarchar(510), nullable): Full name of the supplier.
        *   `Street`, `City` (nvarchar(100/38), nullable): Address components.
        *   `CountryCode` (nvarchar(3), nullable): ISO country code (e.g., 'US', 'CN', 'GB').
        *   `Country` (nvarchar(100), nullable): Full country name.
        *   `ApplicationSettingsId` (int, PK, FK to `ApplicationSettings`): Links to the application settings, forming a composite primary key with `SupplierCode`. This means supplier codes are unique *within* a specific application setting.
    *   **Purpose:** Stores master data for suppliers, including their code, name, address, and country. It's linked to `ApplicationSettings` and referenced by `EntryData` to associate source documents with their suppliers. Used in data validation (`SubmitIncompleteSuppliers`) and potentially other reporting/processing logic.
    *   **Data:** Contains numerous sample supplier entries, primarily for `ApplicationSettingsId` 2 and 3, with varying levels of detail (some only have code and ID).
*   **Analysis: `dbo.TariffCodes.Table.sql`**
    *   **Schema:**
        *   `TariffCode` (nvarchar(50), PK): The primary key, representing the customs tariff code (HS Code).
        *   `Description` (nvarchar(999), nullable): Description of the goods classified under this code.
        *   `RateofDuty`, `EnvironmentalLevy`, `CustomsServiceCharge`, `ExciseTax`, `VatRate`, `PetrolTax` (nvarchar(50), nullable): Fields storing various duty and tax rates applicable to this code, likely as strings representing percentages or fixed values.
        *   `Units` (nvarchar(50), nullable): Specifies the required unit(s) of measure (e.g., 'kg', 'kg and u').
        *   `SiteRev3` (nvarchar(50), nullable): Unknown purpose, possibly legacy or site-specific data.
        *   `TariffCategoryCode` (nvarchar(50), nullable): A higher-level category code (e.g., '0101', '0102').
        *   `LicenseRequired` (bit, nullable): Flag indicating if an import/export license is required.
        *   `Invalid` (bit, nullable): Flag indicating if the tariff code is currently invalid.
        *   `LicenseDescription` (nvarchar(50), nullable): Description related to the license requirement.
    *   **Purpose:** Acts as the master reference table for customs tariff codes (HS Codes). It provides descriptions, applicable duty/tax rates, required units, and licensing information for each code. This table is crucial for classifying items (`InventoryItems`) and calculating duties/taxes during customs declaration processing.
    *   **Data:** Contains a large number of pre-populated tariff codes with their descriptions and associated rates/flags, covering various categories like live animals, meat, fish, etc.
*   **Further Analysis Needed:** Specific scripts defining the structure of configuration tables (`FileType`, etc.) and the `TODO_` views are high priority. Understanding `PreProcessShipmentSP` is also important. Reading `dbo.FileTypes.Table.sql` is a high priority. Also need to analyze related `xcuda_` tables like `xcuda_Valuation_item`, `xcuda_Tarification`, `xcuda_Supplementary_unit`. **Priority: `dbo.Emails.Table.sql`**

## Object Relationships (Updated)

```mermaid
graph TD
    subgraph MainProcess [Program.cs]
        direction LR
        Main(Main) --> Loop(For Each appSetting)
        Loop --> PE(ProcessEmails)
        Loop --> CallEDSA(ExecuteDBSessionActions)
        Loop --> CallPFD(FolderProcessor.ProcessDownloadFolder)
        Loop --> CallELDA(ExecuteLastDBSessionAction)
        Main -- Catches --> EH(Email Error)
    end

    subgraph FolderProcessing [FolderProcessor.cs]
        direction LR
        PFD(ProcessDownloadFolder) --> LoopPDF(Loop *.pdf)
        LoopPDF --> PF(ProcessFile)
        PF --> CreateDocs(CreateDocumentsFolder)
        PF --> CopyFile(CopyFileToDocumentsFolder)
        PF --> GetUnkFT(GetUnknownFileTypes)
        PF --> ProcessFT(ProcessFileTypes)
        PF -- Success --> DeleteOrig(Delete Original PDF)
        ProcessFT --> CallImportPDF(PDFUtils.ImportPDF)
        ProcessFT -- Fail --> CallImportDeep(PDFUtils.ImportPDFDeepSeek)
        ProcessFT -- Fail --> Notify(NotifyUnknownPDF)
        ProcessFT -- Success --> CallCreateShipEmail(ShipmentUtils.CreateShipmentEmail)

        PSF(ProcessShipmentFolders) --> LoopSubfolders(Loop Subfolders)
        LoopSubfolders --> CheckInfo(Check Info.txt)
        LoopSubfolders --> GetShipFT(Get 'ShipmentFolder' FileType)
        LoopSubfolders --> ExtractBL(Extract BL Number)
        LoopSubfolders --> CreateEmail(Create Placeholder Email)
        LoopSubfolders --> CreateAttach(Create Attachments)
        LoopSubfolders --> CallImportUtilsActions(ImportUtils Actions)
        LoopSubfolders -- Success --> Archive(Move to Archive)
        LoopSubfolders -- Error --> ErrorMove(Move to Error)
    end

    subgraph EmailProcessing [ProcessEmails]
        direction LR
        PE --> EC(EmailDownloader.CheckEmails)
        PE --> CallEMA(ImportUtils.ExecuteEmailMappingActions)
        PE --> LoopEmailFiles(Loop Email FileTypes)
        LoopEmailFiles --> GFT(FileTypeManager.GetFileType)
        LoopEmailFiles --> CADS(Create AsycudaDocumentSet?)
        LoopEmailFiles --> SA(Utils.SaveAttachments)
        LoopEmailFiles --> CallEDSFA(ImportUtils.ExecuteDataSpecificFileActions)
        LoopEmailFiles --> CallENSFA(ImportUtils.ExecuteNonSpecificFileActions)
    end

    subgraph ActionExecution [ImportUtils.cs]
        direction LR
        EMA(ExecuteEmailMappingActions) --> GetEMActions(Get EmailMappingActions from DB)
        EMA --> ExecAct(ExecuteActions)

        EDSFA(ExecuteDataSpecificFileActions) --> GetDSFActions(Get FileTypeActions from DB - DataSpecific)
        EDSFA --> ExecAct

        ENSFA(ExecuteNonSpecificFileActions) --> GetNSFActions(Get FileTypeActions from DB - NonSpecific)
        ENSFA --> ExecAct

        ExecAct --> CheckNextStep(Check fileType.ProcessNextStep)
        CheckNextStep -- Yes --> LoopNextStep(Loop ProcessNextStep Actions)
        LoopNextStep --> LookupNextAction(Lookup Action in FileUtils.FileActions)
        LoopNextStep --> InvokeNextAction(Invoke Action)
        LoopNextStep -- Continue --> InvokeMainAction(Invoke Main Action)
        CheckNextStep -- No --> InvokeMainAction
        ExecAct --> LookupMainAction(Lookup Action in FileUtils.FileActions)
        LookupMainAction --> InvokeMainAction
    end

     subgraph TextProcessing [EmailTextProcessor in ImportUtils.cs]
        direction LR
        ETP_Exec(Execute) --> ReadLines(Read Text File Lines)
        ReadLines --> MatchRegex(Match EmailInfoMapping Regex)
        MatchRegex --> ExtractKV(Extract Key-Value)
        ExtractKV --> AddToData(Add to fileType.Data)
        ExtractKV --> BuildSQL(Build SQL UPDATE)
        BuildSQL --> ExecSQL(Execute SQL)
     end

    subgraph PDFProcessing [PDFUtils.cs]
        direction LR
        ImpPDF(ImportPDF) --> LookupContext(Lookup Context from DB)
        ImpPDF --> CallInvoiceReader(InvoiceReader.Import)

        ImpDeep(ImportPDFDeepSeek) --> GetTxt(InvoiceReader.GetPdftxt)
        ImpDeep --> CallDeepSeekAPI(DeepSeekInvoiceApi.ExtractShipmentInvoice)
        ImpDeep --> MapFT(Map Result to FileType)
        ImpDeep --> CallDFP(DataFileProcessor.Process)

        Link(LinkPDFs) --> ExecSP_TODO(Exec Stp_TODO_ImportCompleteEntries)
        Link --> CallBaseLink(BaseDataModel.LinkPDFs)

        ReLink(ReLinkPDFs) --> ScanDir(Scan Imports Dir)
        ReLink --> ExtractCN(Extract CNumber)
        ReLink --> FindDoc(Find AsycudaDocument)
        ReLink --> CreateAttachRecs(Create Attachment Records)

        Download(DownloadPDFs) --> ExecSP_TODO
        Download --> LoopDocs(Loop DocSets)
        LoopDocs --> CallSikuli(Utils.RunSiKuLi)

        AttachPDF(AttachEmailPDF) --> CallBaseAttach(BaseDataModel.AttachEmailPDF)
    end

    subgraph ShipmentProcessing [ShipmentUtils.cs]
        direction LR
        ImpShipInfo(ImportShipmentInfoFromTxt) --> ReadInfo(Read Info.txt)
        ImpShipInfo --> FindCreateDocSet(Find/Create AsycudaDocumentSet)
        ImpShipInfo --> UpdateDocSet(Update AsycudaDocumentSet Fields)
        ImpShipInfo --> FindCreateXcuda(Find/Create xcuda_ASYCUDA & related)
        ImpShipInfo --> UpdateXcuda(Update xcuda_Transport/Declarant)
        ImpShipInfo --> UpdateFTContext(Update FileType Context)

        CreateEmailReport(CreateShipmentEmail) --> ExecSP_PreProcess(Exec PreProcessShipmentSP)
        CreateEmailReport --> LoadShipment(Load Shipment Data)
        CreateEmailReport --> ProcessShipment(Shipment.ProcessShipment)
        CreateEmailReport --> SendEmail(EmailDownloader.SendEmail)
        CreateEmailReport --> SaveAttach(Save Attachments to DB)

        MapItems(MapUnClassifiedItems) --> ReadCSV(Read CSV)
        MapItems --> UpdateInventory(Update InventoryItem TariffCode)
        MapItems --> UpdateFTDocSetRef(Update FileType DocSet Reference)

        SubmitUnclassified(SubmitUnclassifiedItems/SubmitDocSetUnclassifiedItems) --> QueryTODO_Unclassified(Query TODO_SubmitUnclassifiedItems)
        SubmitUnclassified --> GenerateCSV(Generate CSV Report)
        SubmitUnclassified --> EmailReport(Email Report)

        SubmitSuppliers(SubmitIncompleteSuppliers) --> QueryTODO_Suppliers(Query TODO_SubmitIncompleteSupplierInfo)
        SubmitSuppliers --> GenerateCSV
        SubmitSuppliers --> EmailReport

        SubmitPackages(SubmitInadequatePackages) --> QueryTODO_Packages(Query TODO_SubmitInadequatePackages)
        SubmitPackages --> EmailNotification(Send Email Notification)

        UpdateSupp(UpdateSupplierInfo) --> ReadCSV
        UpdateSupp --> UpdateSuppliersDB(Update Suppliers Table)

        ImpUnattached(ImportUnAttachedSummary) --> ProcessXLSX(XlsxWriter.SaveUnAttachedSummary)
        ImpUnattached --> CreateEmailReport

        ClearData(ClearShipmentData) --> ExecSQLDelete(Execute SQL DELETE)

        SaveInfoToFile(SaveShipmentInfoToFile) --> WriteInfo(Write Info.txt)
    end


    subgraph DBSessions [ExecuteDBSessionActions / ExecuteLastDBSessionAction in Program.cs]
        direction LR
        EDSA(ExecuteDBSessionActions) --> FindSchedules(Find SessionSchedule)
        EDSA --> InvokeSessionAction(Invoke SessionsUtils.SessionActions)
        ELDA(ExecuteLastDBSessionAction) --> FindLastSchedule(Find Last SessionSchedule)
        ELDA --> InvokeSessionAction
    end

    subgraph SessionActionMapping [SessionsUtils.cs]
        SessionActionsDict[SessionActions Dictionary]
    end

    subgraph ActionMapping [FileUtils.cs]
        FileActionsDict[FileActions Dictionary]
    end

    subgraph DBConfig
       ActionsTable[dbo.Actions Table]
       FileTypeActionsTable[dbo.FileTypeActions Table]
       EmailMappingActionsTable[dbo.EmailMappingActions Table]
       SessionActionsTable[dbo.SessionActions Table]
       SessionsTable[dbo.Sessions Table]
       SessionScheduleTable[dbo.SessionSchedule Table]
       FileTypesTable[dbo.FileTypes Table]
       ParameterSetTable[dbo.ParameterSet Table]
       ParameterSetParamsTable[dbo.ParameterSetParameters Table]
       ParametersTable[dbo.Parameters Table]
       EmailMappingTable[dbo.EmailMapping Table]
       EmailInfoMappingsTable[dbo.EmailInfoMappings Table]
       InfoMappingTable[dbo.InfoMapping Table]
       InfoMappingRegExTable[dbo.InfoMappingRegEx Table]
       AsycudaDocumentSetTable[dbo.AsycudaDocumentSet Table]
       CustomsProceduresTable[dbo.Customs_Procedure Table]
       ConsigneesTable[dbo.Consignees Table]
       xcudaASYCUDATable[dbo.xcuda_ASYCUDA Table]
       xcudaDeclarantTable[dbo.xcuda_Declarant Table]
       xcudaTransportTable[dbo.xcuda_Transport Table]
       xcudaItemTable[dbo.xcuda_Item Table]
       EntryDataTypeTable[dbo.EntryDataType Table]
       ApplicationSettingsTable[dbo.ApplicationSettings Table]
       BondTypesTable[dbo.BondTypes Table]
       WeightCalculationMethodsTable[dbo.WeightCalculationMethods Table]
       FileGroupsTable[dbo.FileGroups Table]
       FileImporterInfoTable[dbo.FileTypes-FileImporterInfo Table]
       xcudaValuationItemTable[dbo.xcuda_Valuation_item Table]
       xcudaTarificationTable[dbo.xcuda_Tarification Table]
       xcudaSupplementaryUnitTable[dbo.xcuda_Supplementary_unit Table]
       EntryDataDetailsTable[dbo.EntryDataDetails Table]
       EntryDataTable[dbo.EntryData Table]
       InventoryItemsTable[dbo.InventoryItems Table]
       SuppliersTable[dbo.Suppliers Table]
       EmailsTable[dbo.Emails Table]
       TariffCodesTable[dbo.TariffCodes Table]


       ActionsTable -- Name --> FileActionsDict
       ActionsTable -- Name --> SessionActionsDict
       FileTypeActionsTable -- ActionId --> ActionsTable
       FileTypeActionsTable -- FileTypeId --> FileTypesTable
       EmailMappingActionsTable -- ActionId --> ActionsTable
       EmailMappingActionsTable -- EmailMappingId --> EmailMappingTable
       SessionActionsTable -- ActionId --> ActionsTable
       SessionActionsTable -- SessionId --> SessionsTable
       SessionScheduleTable -- SessionId --> SessionsTable
       SessionScheduleTable -- ActionId --> ActionsTable
       SessionScheduleTable -- ParameterSetId --> ParameterSetTable
       ParameterSetTable -- Id --> ParameterSetParamsTable
       ParameterSetParamsTable -- ParameterId --> ParametersTable
       EmailInfoMappingsTable -- EmailMappingId --> EmailMappingTable
       EmailInfoMappingsTable -- InfoMappingId --> InfoMappingTable
       InfoMappingTable -- Id --> InfoMappingRegExTable
       InfoMappingTable -- ApplicationSettingsId --> ApplicationSettingsTable
       AsycudaDocumentSetTable -- Customs_ProcedureId --> CustomsProceduresTable
       AsycudaDocumentSetTable -- ConsigneeName --> ConsigneesTable
       AsycudaDocumentSetTable -- ApplicationSettingsId --> ApplicationSettingsTable
       xcudaDeclarantTable -- ASYCUDA_Id --> xcudaASYCUDATable
       xcudaTransportTable -- ASYCUDA_Id --> xcudaASYCUDATable
       xcudaItemTable -- ASYCUDA_Id --> xcudaASYCUDATable
       xcudaItemTable -- EntryDataType --> EntryDataTypeTable
       xcudaItemTable -- EntryDataDetailsId --> EntryDataDetailsTable
       xcudaValuationItemTable -- Item_Id --> xcudaItemTable
       xcudaTarificationTable -- Item_Id --> xcudaItemTable
       xcudaSupplementaryUnitTable -- Tarification_Id --> xcudaTarificationTable
       ApplicationSettingsTable -- BondTypeId --> BondTypesTable
       ApplicationSettingsTable -- WeightCalculationMethod --> WeightCalculationMethodsTable
       EmailMappingTable -- ApplicationSettingsId --> ApplicationSettingsTable
       FileTypesTable -- ApplicationSettingsId --> ApplicationSettingsTable
       FileTypesTable -- FileGroupId --> FileGroupsTable
       FileTypesTable -- ParentFileTypeId --> FileTypesTable
       FileTypesTable -- FileInfoId --> FileImporterInfoTable
       EntryDataDetailsTable -- EntryData_Id --> EntryDataTable
       EntryDataDetailsTable -- InventoryItemId --> InventoryItemsTable
       EntryDataTable -- ApplicationSettingsId --> ApplicationSettingsTable
       EntryDataTable -- FileTypeId --> FileTypesTable
       EntryDataTable -- SupplierCode --> SuppliersTable
       EntryDataTable -- EntryType --> EntryDataTypeTable
       EntryDataTable -- EmailId --> EmailsTable
       InventoryItemsTable -- ApplicationSettingsId --> ApplicationSettingsTable
       InventoryItemsTable -- TariffCode --> TariffCodesTable
       SuppliersTable -- ApplicationSettingsId --> ApplicationSettingsTable


       EDSA -- Uses --> SessionsTable
       EDSA -- Uses --> SessionActionsTable
       EDSA -- Uses --> SessionScheduleTable
       EDSA -- Uses --> ParameterSetTable
       EDSA -- Uses --> ParameterSetParamsTable
       EDSA -- Uses --> ParametersTable
       ELDA -- Uses --> SessionScheduleTable
       EDSFA -- Uses --> FileTypeActionsTable
       ENSFA -- Uses --> FileTypeActionsTable
       EMA -- Uses --> EmailMappingActionsTable
       PE -- Uses --> EmailMappingTable
       PE -- Uses --> AsycudaDocumentSetTable
       PE -- Uses --> CustomsProceduresTable
       PE -- Uses --> FileTypesTable
       ETP_Exec -- Uses --> EmailInfoMappingsTable
       ETP_Exec -- Uses --> InfoMappingTable
       ETP_Exec -- Uses --> InfoMappingRegExTable
       ImpShipInfo -- Uses --> AsycudaDocumentSetTable
       ImpShipInfo -- Uses --> xcudaASYCUDATable
       ImpShipInfo -- Uses --> xcudaDeclarantTable
       ImpShipInfo -- Uses --> xcudaTransportTable
       Main -- Uses --> ApplicationSettingsTable
       GFT -- Uses --> FileTypesTable
    end


    subgraph Dependencies
        direction TB
        MainProcess --> CoreEntitiesContext
        MainProcess --> ApplicationSettingsTable
        MainProcess --> BaseDataModel
        MainProcess --> FolderProcessorClass[FolderProcessor Class]
        MainProcess --> SessionsUtilsClass[SessionsUtils Class]

        FolderProcessing --> CoreEntitiesContext
        FolderProcessing --> ApplicationSettingsTable
        FolderProcessing --> FileTypeManagerClass[FileTypeManager Class]
        FolderProcessing --> PDFUtilsClass[PDFUtils Class]
        FolderProcessing --> ShipmentUtilsClass[ShipmentUtils Class]
        FolderProcessing --> ImportUtilsClass[ImportUtils Class]
        FolderProcessing --> EmailDownloaderClass[EmailDownloader Class]
        FolderProcessing --> UtilsClass[Utils Class]
        FolderProcessing --> EmailsTable
        FolderProcessing --> AttachmentsTable[Attachments Table]
        FolderProcessing --> FileTypesTable


        EmailProcessing --> CoreEntitiesContext
        EmailProcessing --> ApplicationSettingsTable
        EmailProcessing --> BaseDataModel
        EmailProcessing --> EmailDownloaderClass
        EmailProcessing --> FileTypeManagerClass
        EmailProcessing --> ImportUtilsClass
        EmailProcessing --> UtilsClass
        EmailProcessing --> AsycudaDocumentSetTable
        EmailProcessing --> EmailMappingTable
        EmailProcessing --> CustomsProceduresTable
        EmailProcessing --> FileTypesTable

        ActionExecution --> CoreEntitiesContext
        ActionExecution --> FileUtilsClass[FileUtils Class]
        ActionExecution --> FileTypesTable
        ActionExecution --> FileTypeActionsTable
        ActionExecution --> EmailMappingActionsTable
        ActionExecution --> ActionsTable
        ActionExecution --> ApplicationSettingsTable

        TextProcessing --> CoreEntitiesContext
        TextProcessing --> FileTypesTable
        TextProcessing --> EmailInfoMappingsTable
        TextProcessing --> InfoMappingTable
        TextProcessing --> InfoMappingRegExTable

        PDFProcessing --> CoreEntitiesContext
        PDFProcessing --> BaseDataModel
        PDFProcessing --> InvoiceReaderClass[InvoiceReader Class]
        PDFProcessing --> DeepSeekApiClass[DeepSeekInvoiceApi Class]
        PDFProcessing --> FileTypeManagerClass
        PDFProcessing --> DataFileProcessorClass[DataFileProcessor Class]
        PDFProcessing --> UtilsClass
        PDFProcessing --> AsycudaDocAttachTable[AsycudaDocumentSet_Attachments Table]
        PDFProcessing --> AttachmentsTable
        PDFProcessing --> AsycudaDocumentsTable[AsycudaDocuments Table]
        PDFProcessing --> StoredProc_TODO[Stp_TODO_ImportCompleteEntries SP]
        PDFProcessing --> SikuliX

        ShipmentProcessing --> CoreEntitiesContext
        ShipmentProcessing --> EntryDataDSContext
        ShipmentProcessing --> InventoryDSContext
        ShipmentProcessing --> DocumentDSContext
        ShipmentProcessing --> CSVUtilsClass[CSVUtils Class]
        ShipmentProcessing --> XlsxWriterClass[XlsxWriter Class]
        ShipmentProcessing --> EmailDownloaderClass
        ShipmentProcessing --> BaseDataModel
        ShipmentProcessing --> EntryDocSetUtilsClass[EntryDocSetUtils Class]
        ShipmentProcessing --> ShipmentClass[Shipment Class & Extensions]
        ShipmentProcessing --> SuppliersTable
        ShipmentProcessing --> InventoryItemsTable
        ShipmentProcessing --> ShipmentTables[ShipmentBL/Invoice/Manifest/entrydata Tables]
        ShipmentProcessing --> TODOViews[TODO_ Views]
        ShipmentProcessing --> AsycudaDocumentSetTable
        ShipmentProcessing --> xcudaASYCUDATable
        ShipmentProcessing --> xcudaDeclarantTable
        ShipmentProcessing --> xcudaTransportTable
        ShipmentProcessing --> ContactsTable[Contacts Table]
        ShipmentProcessing --> ActionLogsTable[ActionDocSetLogs Table]
        ShipmentProcessing --> StoredProc_PreProcess[PreProcessShipmentSP SP]


        DBSessions --> CoreEntitiesContext
        DBSessions --> ApplicationSettingsTable
        DBSessions --> BaseDataModel
        DBSessions --> SessionsUtilsClass
        DBSessions --> SessionScheduleTable
        DBSessions --> SessionActionsTable

        EH --> EmailDownloaderClass
        EH --> UtilsClass

        FileUtilsClass --> DocumentUtilsClass[DocumentUtils Class]
        FileUtilsClass --> AllocateSalesUtilsClass[AllocateSalesUtils Class]
        FileUtilsClass --> CreateEX9UtilsClass[CreateEX9Utils Class]
        FileUtilsClass --> EX9UtilsClass[EX9Utils Class]
        FileUtilsClass --> CSVUtilsClass
        FileUtilsClass --> POUtilsClass[POUtils Class]
        FileUtilsClass --> XLSXProcessorClass[XLSXProcessor Class]
        FileUtilsClass --> EmailTextProcessorClass[EmailTextProcessor Class]
        FileUtilsClass --> EntryDocSetUtilsClass
        FileUtilsClass --> SubmitSalesXmlToCustomsUtilsClass[SubmitSalesXmlToCustomsUtils Class]
        FileUtilsClass --> ShipmentUtilsClass
        FileUtilsClass --> PDFUtilsClass
        FileUtilsClass --> DISUtilsClass[DISUtils Class]
        FileUtilsClass --> SubmitSalesToCustomsUtilsClass[SubmitSalesToCustomsUtils Class]
        FileUtilsClass --> C71UtilsClass[C71Utils Class]
        FileUtilsClass --> LICUtilsClass[LICUtils Class]
        FileUtilsClass --> UpdateInvoiceClass[UpdateInvoice Class]
        FileUtilsClass --> ImportWarehouseErrorsUtilsClass[ImportWarehouseErrorsUtils Class]
        FileUtilsClass --> UtilsClass

        SessionsUtilsClass --> ADJUtilsClass[ADJUtils Class]
        SessionsUtilsClass --> DISUtilsClass
        SessionsUtilsClass --> AllocateSalesUtilsClass
        SessionsUtilsClass --> CreateEX9UtilsClass
        SessionsUtilsClass --> EX9UtilsClass
        SessionsUtilsClass --> SubmitSalesXmlToCustomsUtilsClass
        SessionsUtilsClass --> EntryDocSetUtilsClass
        SessionsUtilsClass --> SalesUtilsClass[SalesUtils Class]
        SessionsUtilsClass --> DocumentUtilsClass
        SessionsUtilsClass --> PDFUtilsClass
        SessionsUtilsClass --> POUtilsClass
        SessionsUtilsClass --> C71UtilsClass
        SessionsUtilsClass --> LICUtilsClass
        SessionsUtilsClass --> UtilsClass
        SessionsUtilsClass --> ShipmentUtilsClass
        SessionsUtilsClass --> SubmitSalesToCustomsUtilsClass
        SessionsUtilsClass --> ImportAllAsycudaDocumentsInDataFolderUtilsClass[ImportAllAsycudaDocumentsInDataFolderUtils Class]
        SessionsUtilsClass --> ImportWarehouseErrorsUtilsClass
        SessionsUtilsClass --> SQLBlackBoxClass[SQLBlackBox Class]


    end

    CallPFD --> PFD
    CallImportUtilsActions --> EDSFA
    CallImportUtilsActions --> ENSFA
    CallEMA --> EMA
    CallEDSFA --> EDSFA
    CallENSFA --> ENSFA
    LookupMainAction --> FileActionsDict
    LookupNextAction --> FileActionsDict
    CallImportPDF --> ImpPDF
    CallImportDeep --> ImpDeep
    CallCreateShipEmail --> CreateEmailReport
    InvokeSessionAction --> SessionActionsDict
```

*   **Explanation:** Added `dbo.Emails` table and linked it from `EntryData`.

## Open Questions/Areas for Further Investigation

1.  **What is the exact logic within the methods referenced by `FileUtils.FileActions` and `SessionsUtils.SessionActions`?** (e.g., `DocumentUtils.ImportSalesEntries`, `AllocateSalesUtils.AllocateSales`, `ADJUtils.CreateAdjustmentEntries`, etc.) **Priority:** `InvoiceReader`, `DeepSeekInvoiceApi`, `DataFileProcessor`, `Shipment` class/extensions.
2.  **[Answered]** How does `FileTypeManager.GetFileType` and `FileTypeManager.GetImportableFileType` work? Need to analyze `WaterNut.Business.Services/Utils/FileTypeManager.cs`. (See Analysis below)
3.  **[Answered]** What is the implementation of `Utils.SaveAttachments` (called by `Program.cs`)? How does it relate to the attachment creation in `FolderProcessor.ProcessShipmentFolders` and `ShipmentUtils.CreateShipmentEmail`? Need to analyze `WaterNut.Business.Services/Utils/Utils.cs` more closely or find where `SaveAttachments` is defined. (See Analysis below)
4.  **[Answered]** What does `BaseDataModel` contain beyond `CurrentApplicationSettings`, `CurrentSessionSchedule`, `CurrentSessionAction`? What do `AttachEmailPDF`, `LinkPDFs`, and `EmailExceptionHandler` do? (Need to find its definition - likely in `CoreEntities` or `WaterNut.DataSpace`). (See Analysis below)
5.  **[Answered]** What is the exact schema for key configuration/data tables (`Attachments`, `AsycudaDocuments`, `AsycudaDocumentSet_Attachments`, `Customs_Procedure`, `Consignees`, `BondTypes`, `WeightCalculationMethods`, `FileGroups`, `FileTypes-FileImporterInfo`)? (Requires reading relevant SQL - **Priority: `dbo.Attachments.Table.sql`**) (See Analysis below)
6.  **[Partially Answered]** What is the exact schema for the `TODO_` views (`TODO_SubmitUnclassifiedItems`, `TODO_SubmitIncompleteSupplierInfo`, `TODO_SubmitInadequatePackages`) and the `Stp_TODO_ImportCompleteEntries` / `PreProcessShipmentSP` stored procedures? (Requires reading relevant SQL) (Scripts for `TODO_` views not found. See SP Analysis below)
7.  **[Answered]** What is the structure of the `Info.txt` file expected by `ProcessShipmentFolders` beyond the "BL" key? What other information might it contain? (Can infer somewhat from `ImportShipmentInfoFromTxt`). (See Analysis below)
8.  **[Answered]** How is `FolderProcessor.ProcessShipmentFolders` intended to be invoked if not by `Program.Main`? (See Analysis below)
9.  What is the logic within `InvoiceReader.Import` and `InvoiceReader.GetPdftxt`? Where is this class defined?
10. What is the `DeepSeekInvoiceApi`? Is it internal or a third-party service? Where is it defined?
11. What does `DataFileProcessor.Process` do? Where is it defined?
12. **[Answered]** What does `WaterNut.DataSpace.Utils.RunSiKuLi` do? What are the SikuliX scripts (e.g., "IM7-PDF")? (See Analysis below)
13. **[Answered]** What is the `Shipment` class and its extension methods (`LoadEmailPOs`, `LoadDBBL`, `AutoCorrect`, `ProcessShipment`)? Where is it defined (likely `AutoBot/ShipmentExtensions.cs`)? (See Analysis below)

## Analysis: `WaterNut.Business.Services/Utils/FileTypeManager.cs`

*   **Purpose:** Manages `FileTypes` definitions, using caching (`_fileTypes` static field) to store fully loaded `FileTypes` objects (including related entities like Mappings, Actions, ImporterInfos) for the current `ApplicationSettingsId`. Provides methods to retrieve these configurations.
*   **`FileTypes()` (Private Helper):** Loads and caches `FileTypes` from `CoreEntitiesContext` for the current `ApplicationSettingsId` if the cache is empty or stale. Eagerly loads numerous related entities (`FileTypeMappings`, `FileTypeActions`, `FileImporterInfos`, etc.).
*   **`GetFileType(FileTypes fileTypes)` / `GetFileType(int fileTypeId)`:** Retrieves a specific, fully loaded `FileTypes` object from the cache based on an input object or ID.
*   **`GetImportableFileType(string entryType, string fileFormat, string fileName)`:** Finds `FileTypes` suitable for data import. Filters the cached list based on `entryType`, `fileFormat`, matching `fileName` against `FilePattern`, and **requiring `FileTypeMappings` to exist** (unless it's an "Unknown" PDF). Sets the `AsycudaDocumentSetId` based on `DocSetRefernece`.
*   **`GetFileType(string entryType, string fileFormat, string fileName)`:** Similar to `GetImportableFileType` but finds *any* matching `FileType` based on entry type, format, and filename pattern, **without requiring `FileTypeMappings`**. Used for general type identification. Sets the `AsycudaDocumentSetId`.
*   **`GetHeadingFileType(IEnumerable<string> heading, FileTypes suggestedfileType)`:** Attempts to identify a `FileType` by matching provided `heading` strings against configured `FileTypeMappings.OriginalName`. It prioritizes the type with the most heading matches and matching format/entry type constraints. Can refine an initial guess based on content.
*   **`EntryTypes` / `FileFormats` (Inner Classes):** Provide constants for standard entry type strings (e.g., "PO", "INV", "Sales", "C71", "Lic") and file format strings (e.g., "CSV", "PDF", "XML").

## Analysis: `AutoBot/Utils.cs` (Partial - SaveAttachments)

*   **`SaveAttachments(FileInfo[] csvFiles, FileTypes fileType, Email email)`:**
    *   Called by `Program.ProcessEmails`.
    *   Ensures an `Emails` record exists in `CoreEntitiesContext` for the given `email.EmailId`, creating one if necessary.
    *   Iterates through the input `csvFiles` (attachments).
    *   Checks file size against `fileType.MaxFileSizeInMB` and sends a notification email if too large (using `FileTypeManager.SendBackTooBigEmail`).
    *   Generates a unique reference name for the attachment (`GetReference` helper).
    *   Finds or creates an `Attachments` record in `CoreEntitiesContext` for the file.
    *   Calls `AddUpdateEmailAttachments` helper to link the `Emails` record and the `Attachments` record in the `EmailAttachments` table, storing `FileTypeId` context.
    *   If `fileType.AsycudaDocumentSetId` is set, calls `EntryDocSetUtils.AddUpdateDocSetAttachement` to link the attachment to the document set.
*   **Relationship to other Attachment Logic:**
    *   This method centralizes saving email and attachment metadata within the `CoreEntitiesContext` specifically for the email processing workflow in `Program.cs`.
    *   `FolderProcessor.ProcessShipmentFolders` creates its own placeholder `Emails` and `Attachments` records in `CoreEntitiesContext`.
    *   `ShipmentUtils.CreateShipmentEmail` saves `Attachments` records (from the `Shipment` object) directly to `EntryDataDSContext`.
    *   Therefore, attachment handling is context-specific and not solely managed by this `SaveAttachments` method.

## Analysis: `WaterNut.Business.Services/.../BaseBusinessLayerDS.cs` (BaseDataModel)

*   **Purpose:** Acts as a central singleton (`BaseDataModel.Instance`) holding application-wide state and providing numerous data access, manipulation, and utility methods. It appears to be defined as a `partial class` across multiple files, including generated code.
*   **Key State Properties:**
    *   `CurrentApplicationSettings`: Holds the loaded `ApplicationSettings` for the current run.
    *   `CurrentSessionSchedule`: Holds the `SessionSchedule` records being processed.
    *   `CurrentSessionAction`: Holds the specific `SessionActions` being executed.
    *   Caches for `Customs_Procedure` and `ExportTemplates`.
*   **`AttachEmailPDF(int asycudaDocumentSetId, string emailId)`:**
    *   Called by `PDFUtils.AttachEmailPDF`.
    *   Links existing `Attachments` associated with the `emailId` to the specified `asycudaDocumentSetId` by creating records in the `AsycudaDocumentSet_Attachments` table within `CoreEntitiesContext`.
    *   Sets the `DocumentSpecific` flag based on the attachment's `DocumentCode`.
*   **`LinkPDFs(List<int> entries, string docCode = "NA")`:**
    *   Called by `PDFUtils.LinkPDFs`.
    *   Takes a list of `ASYCUDA_Id`s.
    *   For each `ASYCUDA_Id`, finds the corresponding `AsycudaDocument`.
    *   Searches the "Imports" directory for PDF files matching the document's CNumber and Registration Date.
    *   For each matching PDF file found:
        *   Creates an `Attachments` record if one doesn't exist for the file path.
        *   Creates an `AsycudaDocument_Attachments` record linking the `ASYCUDA_Id` and the `Attachments` record if the link doesn't already exist.
    *   Saves changes to `CoreEntitiesContext`.
*   **`EmailExceptionHandler(Exception e, bool sendOnlyOnce = true)`:**
    *   Called from various `catch` blocks (e.g., in `CreateShipmentEmail`).
    *   Sends an email notification containing the exception details to "Developer" contacts.
    *   Uses a static list (`ErrorLst`) to track recent error messages and avoid sending duplicate emails if `sendOnlyOnce` is true. (Dependency: `CoreEntitiesContext`, `Contacts`, `EmailDownloader`).
*   **Other Notable Methods:** Includes methods for creating ASYCUDA documents (`CreateDocumentCt`, `CreateNewAsycudaDocument`), creating line items (`CreateEntryItems`, `CreateItemFromEntryDataDetail`), importing data (`ImportDocuments`, `ImportC71`, `ImportLicense`), saving/deleting various entities, calculating freight (`CalculateDocumentSetFreight`), running SikuliX scripts (`RunSiKuLi` - though definition seems to be in `AutoBot/Utils.cs`), managing attachments (`SaveAttachedDocuments`, `AttachToExistingDocuments`, etc.), and more.

## Analysis: `dbo.Attachments.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key.
    *   `FilePath` (nvarchar(255), NOT NULL): Full path to the file.
    *   `DocumentCode` (nvarchar(50), NULL): Code indicating document type (e.g., "IV05", "NA"). Often derived from `FileTypes`.
    *   `Reference` (nvarchar(255), NULL): Reference string, often based on filename, potentially made unique.
    *   `EmailId` (nvarchar(255), NULL): Links to the source `dbo.Emails` record if applicable.
*   **Purpose:** Central registry for processed files, storing location and metadata. Linked to emails, document sets, and ASYCUDA documents via join tables.

## Analysis: `dbo.AsycudaDocumentSet_Attachments.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key for the link.
    *   `AttachmentId` (int, NOT NULL, FK to `Attachments.Id`): Links to the `Attachments` record.
    *   `AsycudaDocumentSetId` (int, NOT NULL, FK to `AsycudaDocumentSet.AsycudaDocumentSetId`): Links to the `AsycudaDocumentSet`.
    *   `DocumentSpecific` (bit, NOT NULL): Flag indicating if attachment relates to a specific document (true) or the whole set (false).
    *   `FileDate` (date, NOT NULL): Date associated with the file.
    *   `FileTypeId` (int, NULL, FK to `FileTypes.Id`): Links to the `FileTypes` record used for processing.
    *   `EmailId` (nvarchar(255), NULL, FK to `Emails.EmailId`): Links to the source `Emails` record.
*   **Purpose:** Many-to-many join table linking `Attachments` to `AsycudaDocumentSet`. Stores context about the relationship, including the `FileType` used and the source `EmailId`.

## Analysis: `dbo.Customs_Procedure.Table.sql`

*   **Schema:**
    *   `Document_TypeId` (int, NOT NULL, FK to `Document_Type`): Links to ASYCUDA document type.
    *   `Customs_ProcedureId` (int, PK, Identity): Primary key.
    *   `Extended_customs_procedure` (nvarchar(5), NOT NULL): Extended procedure code (e.g., "4000").
    *   `National_customs_procedure` (nvarchar(5), NOT NULL): National procedure code (e.g., "000", "800").
    *   `CustomsProcedure` (Computed, Persisted, NOT NULL): Combined code (e.g., "4000-000").
    *   `IsObsolete` (bit, NOT NULL): Flag for obsolete procedures.
    *   `IsPaid` (bit, NOT NULL): Flag indicating if duties are typically paid.
    *   `BondTypeId` (int, NULL, FK to `BondTypes`): Links to required bond type.
    *   `Stock` (bit, NOT NULL): Flag indicating if procedure affects bonded stock.
    *   `Discrepancy` (bit, NOT NULL): Flag for discrepancy processing.
    *   `Adjustment` (bit, NOT NULL): Flag for adjustment processing.
    *   `Sales` (bit, NOT NULL): Flag for sales processing.
    *   `CustomsOperationId` (int, NOT NULL, FK to `CustomsOperations`): Links to high-level operation (Import, Export, etc.).
    *   `SubmitToCustoms` (bit, NOT NULL): Flag indicating if documents should be submitted.
    *   `IsDefault` (bit, NOT NULL): Flag indicating default procedure for its operation.
    *   `ExportSupportingEntryData` (bit, NOT NULL): Flag to control export of supporting data.
*   **Purpose:** Defines valid customs procedures, linking codes to document types, operations, and flags that control application logic (stock impact, processing type, submission requirements). Referenced by `AsycudaDocumentSet`.

## Analysis: `dbo.Consignees.Table.sql`

*   **Schema:**
    *   `ConsigneeName` (nvarchar(100), PK): Primary key, the name of the consignee.
    *   `ConsigneeCode` (nvarchar(100), NULL): Optional code for the consignee.
    *   `Address` (nvarchar(300), NULL): Consignee address.
    *   `CountryCode` (nvarchar(3), NULL): Consignee country code.
    *   `ApplicationSettingsId` (int, NOT NULL): Links to `ApplicationSettings`.
*   **Purpose:** Stores master data for consignees. Referenced by `AsycudaDocumentSet`.

## Analysis: `dbo.BondTypes.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key.
    *   `Name` (nvarchar(50), NOT NULL): Name of the bond type.
*   **Purpose:** Defines the types of customs bonds (e.g., Warehouse, DutyFreeShop). Referenced by `Customs_Procedure` and `ApplicationSettings`.

## Analysis: `dbo.WeightCalculationMethods.Table.sql`

*   **Schema:**
    *   `Name` (nvarchar(50), PK): Primary key, the name of the method.
*   **Purpose:** Defines the available methods for calculating weight (e.g., MinimumWeight, Value, WeightEqualQuantity). Referenced by `ApplicationSettings`.

## Analysis: `dbo.FileGroups.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key.
    *   `Name` (nvarchar(50), NOT NULL): Name of the file group.
*   **Purpose:** Defines named groups for categorizing `FileTypes`. Referenced by `FileTypes`.

## Analysis: `dbo.FileTypes-FileImporterInfo.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key.
    *   `EntryType` (nvarchar(50), NOT NULL): Type of data (e.g., "PO", "Sales", "INV", "C71", "Lic").
    *   `Format` (nvarchar(50), NOT NULL): File format (e.g., "CSV", "XLSX", "PDF", "XML").
*   **Purpose:** Defines the valid combinations of `EntryType` and `Format`. Referenced by `FileTypes` (via `FileInfoId`) to categorize the file and determine processing logic.

## Analysis: `dbo.AsycudaDocument_Attachments.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key for the link.
    *   `AttachmentId` (int, NOT NULL, FK to `Attachments.Id`): Links to the `Attachments` record.
    *   `AsycudaDocumentId` (int, NOT NULL, FK to `xcuda_ASYCUDA.ASYCUDA_Id`): Links to the specific `xcuda_ASYCUDA` document record.
*   **Purpose:** Many-to-many join table linking `Attachments` directly to individual `xcuda_ASYCUDA` documents. Used for associating specific files (like downloaded assessment PDFs) with their corresponding ASYCUDA entry.

## Analysis: `dbo.AsycudaDocument_Attachments.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key for the link.
    *   `AttachmentId` (int, NOT NULL, FK to `Attachments.Id`): Links to the `Attachments` record.
    *   `AsycudaDocumentId` (int, NOT NULL, FK to `xcuda_ASYCUDA.ASYCUDA_Id`): Links to the specific `xcuda_ASYCUDA` document record.
*   **Purpose:** Many-to-many join table linking `Attachments` directly to individual `xcuda_ASYCUDA` documents. Used for associating specific files (like downloaded assessment PDFs) with their corresponding ASYCUDA entry.

## Analysis: `dbo.AsycudaDocument_Attachments.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key for the link.
    *   `AttachmentId` (int, NOT NULL, FK to `Attachments.Id`): Links to the `Attachments` record.
    *   `AsycudaDocumentId` (int, NOT NULL, FK to `xcuda_ASYCUDA.ASYCUDA_Id`): Links to the specific `xcuda_ASYCUDA` document record.
*   **Purpose:** Many-to-many join table linking `Attachments` directly to individual `xcuda_ASYCUDA` documents. Used for associating specific files (like downloaded assessment PDFs) with their corresponding ASYCUDA entry.

## Analysis: `dbo.AsycudaDocument_Attachments.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key for the link.
    *   `AttachmentId` (int, NOT NULL, FK to `Attachments.Id`): Links to the `Attachments` record.
    *   `AsycudaDocumentId` (int, NOT NULL, FK to `xcuda_ASYCUDA.ASYCUDA_Id`): Links to the specific `xcuda_ASYCUDA` document record.
*   **Purpose:** Many-to-many join table linking `Attachments` directly to individual `xcuda_ASYCUDA` documents. Used for associating specific files (like downloaded assessment PDFs) with their corresponding ASYCUDA entry.

## Analysis: `dbo.AsycudaDocument_Attachments.Table.sql`

*   **Schema:**
    *   `Id` (int, PK, Identity): Primary key for the link.
    *   `AttachmentId` (int, NOT NULL, FK to `Attachments.Id`): Links to the `Attachments` record.
    *   `AsycudaDocumentId` (int, NOT NULL, FK to `xcuda_ASYCUDA.ASYCUDA_Id`): Links to the specific `xcuda_ASYCUDA` document record.
*   **Purpose:** Many-to-many join table linking `Attachments` directly to individual `xcuda_ASYCUDA` documents. Used for associating specific files (like downloaded assessment PDFs) with their corresponding ASYCUDA entry.

## Analysis: Stored Procedures

*   **`dbo.Stp_TODO_ImportCompleteEntries`:**
    *   **Purpose:** Identifies ASYCUDA documents (`ASYCUDA_Id`) considered "import complete", likely replacing a complex view. Used by `PDFUtils` to find documents ready for PDF linking/downloading.
    *   **Logic:** Takes `@ApplicationSettingsId`. Uses temporary tables to join data from `[TODO-ImportCompleteEntries-New-Data]` view, `[Stp_TODO_ImportCompleteEntries-EntryDataDetailsEx]` view, `AsycudaDocumentSetEntryData`, and `AsycudaDocumentItemEntryDataDetails`. It identifies entries that have both newly imported data and corresponding assessed data (where `ImportComplete = 1`), returning key identifiers including the `ASYCUDA_Id` of both the new and assessed documents.
    *   **Dependencies:** Views `[TODO-ImportCompleteEntries-New-Data]`, `[Stp_TODO_ImportCompleteEntries-EntryDataDetailsEx]`; Tables `AsycudaDocumentSetEntryData`, `AsycudaDocumentItemEntryDataDetails`.
*   **`dbo.PreProcessShipmentSP`:**
    *   **Purpose:** Pre-populates the `ShipmentInvoicePOs` table, which links Purchase Orders (`EntryData`) to Shipment Invoices (`ShipmentInvoice`). Called by `ShipmentUtils.CreateShipmentEmail` before processing shipment data.
    *   **Logic:** Clears `ShipmentInvoicePOs`. Inserts links based on several matching strategies using various views: manual matches (`ShipmentInvoicePOManualMatches`), matching invoice numbers (`[Shipment Invoice PO Matches - Invoice No]`), matching items (`[Shipment Invoice PO Matches - Items]`), matching PO numbers (`ShipmentInvoicePOMatches`), and matching totals (`[Shipment Invoice PO Matches - Invoice Totals]`, `[Shipment Invoice PO Matches - Totals]`). It also updates `SalesFactor` in `ShipmentInvoiceDetails` and potentially adds new aliases to `InventoryItemAlias` based on item matches.
    *   **Dependencies:** Numerous views (see logic) and tables (`ShipmentInvoicePOs`, `EntryData`, `ShipmentInvoiceDetails`, `InventoryItemAlias`).

## Analysis: `Info.txt` Structure (Expected by `ImportShipmentInfoFromTxt`)

Based on the analysis of `ShipmentUtils.ImportShipmentInfoFromTxt`, the `Info.txt` file is expected to contain key-value pairs, one per line, separated by a colon (`:`). The parser ignores empty lines, lines without a colon, and specific placeholder values ("Consignee Address Not Found", "Not Found").

*   **Expected Keys:**
    *   `BL` (Required): Used as `Declarant_Reference_Number` to find/create `AsycudaDocumentSet`.
    *   `Currency` (Required if creating DocSet): Sets `AsycudaDocumentSet.Currency_Code`.
    *   `Exchange Rate` (Optional): Sets `AsycudaDocumentSet.Exchange_Rate` (defaults to 1.0).
    *   `Manifest` (Optional): Sets `AsycudaDocumentSet.Manifest_Number`.
    *   `Freight` (Optional): Sets `AsycudaDocumentSet.TotalFreight`.
    *   `Weight(kg)` (Optional): Sets `AsycudaDocumentSet.TotalWeight`.
    *   `Country of Origin` (Optional): Sets `AsycudaDocumentSet.Country_of_origin_code`.
    *   `Total Invoices` (Optional): Sets `AsycudaDocumentSet.TotalInvoices`.
    *   `Packages` (Optional): Sets `AsycudaDocumentSet.TotalPackages`.
    *   `Freight Currency` (Optional): Sets `AsycudaDocumentSet.FreightCurrencyCode` (defaults to "USD").
    *   `Office` (Optional): Sets `AsycudaDocumentSet.Office`.
    *   `Location of Goods` (Optional): Sets `xcuda_Transport.Location_of_goods`.
    *   `Consignee Code` (Optional): Sets `xcuda_Declarant.Declarant_code`.
    *   `Consignee Name` (Optional): Sets `xcuda_Declarant.Declarant_name`.

## Analysis: Invocation of `FolderProcessor.ProcessShipmentFolders`

A code search revealed that the only invocation of `FolderProcessor.ProcessShipmentFolders` within the provided project structure is inside the unit test file `AutoBotUtilities.Tests/FolderProcessorTests.cs`.

**Conclusion:** Based on the current code, this functionality is not called by the main application entry point (`Program.cs`). It might be legacy code, intended for separate execution, or not fully integrated. Its primary confirmed use is within the testing suite.

## Analysis: `WaterNut.DataSpace.Utils.RunSiKuLi` & SikuliX Scripts

*   **`RunSiKuLi` Method (Defined in `WaterNut.Business.Services/Utils/Utils.cs`):**
    *   **Purpose:** Executes external SikuliX GUI automation scripts (`.sikuli`) using Java. SikuliX uses image recognition to interact with application interfaces.
    *   **Functionality:**
        *   Constructs a command line to run `java.exe` with the SikuliX JAR (`sikulixide-2.0.5.jar`).
        *   Passes the path to the specified `.sikuli` script (located in `C:\Users\{User}\OneDrive\Clients\AutoBot\Scripts\`).
        *   Passes arguments to the script, including ASYCUDA login credentials (from `BaseDataModel`), the target `directoryName`, and optional parameters like `lastCNumber` or date ranges.
        *   Includes logic to wait for existing ASYCUDA processes and kill old Java processes.
        *   Starts the Java process, waits for completion (with timeout), and logs success/failure.
    *   **Usage:** Called by methods like `PDFUtils.DownloadPDFs`, `C71Utils.DownLoadC71`, `LICUtils.DownLoadLicence`, `C71Utils.AssessC71`, `LICUtils.AssessLicense` to automate interactions with likely the ASYCUDA World application.
*   **SikuliX Scripts (e.g., "IM7-PDF", "C71", "AssessC71", "LIC", "AssessLIC"):**
    *   These are separate scripts (likely Python/Jython) located outside the C# project.
    *   They contain image-based automation logic to control GUIs.
    *   They perform tasks like logging into ASYCUDA, navigating, entering data, clicking buttons (e.g., download, assess), and saving files, based on the arguments passed from `RunSiKuLi`.

## Analysis: `Shipment` Class and Extensions (`WaterNut.Business.Entities/.../Shipment.cs` & `AutoBot/ShipmentExtensions.cs`)

*   **`Shipment` Class:**
    *   Defined as a `partial class` in `WaterNut.Business.Entities` (likely `EntryDataDS` context).
    *   Acts as an aggregate root for shipment information.
    *   Contains properties for Manifest#, BL#, Name, Totals (Weight, Freight, Packages, Invoices), Currency, Origin, Consignee details, etc.
    *   Holds collections of related attached document entities (e.g., `ShipmentAttachedPOs`, `ShipmentAttachedInvoices`, `ShipmentAttachedBL`, `ShipmentAttachedRider`).
*   **`LoadEmailPOs(this Shipment shipment)` Extension Method:**
    *   Loads `PurchaseOrders` entities from `EntryDataDSContext` matching `shipment.EmailId`.
    *   Adds them to the `shipment.ShipmentAttachedPOs` collection.
*   **`LoadDBBL(this Shipment shipment)` Extension Method:**
    *   Loads `ShipmentBL` entities from `EntryDataDSContext` that are linked indirectly to the shipment via already loaded Invoices, Manifests, Freight Details, or Riders.
    *   Adds newly found, unique `ShipmentBL` entities to the `shipment.ShipmentAttachedBL` collection.
*   **`AutoCorrect(this Shipment masterShipment)` Extension Method:**
    *   Currently a placeholder/no-op method; returns the input shipment without modification.
*   **`ProcessShipment(this Shipment masterShipment)` Extension Method:**
    *   Orchestrates the processing and refinement of the aggregated data in `masterShipment`.
    *   Calls different private helper methods (`CreateShipmentFromBLsNoRider`, `CreateShipmentFromNoBLAndRider`, `CreateShipmentNoBLNoRider`) based on the presence/absence of attached BLs and Riders.
    *   These helpers contain complex logic to merge/split data, link invoices/POs, distribute costs/weights, and create one or more refined `Shipment` objects.
    *   Returns a `List<Shipment>` containing the processed shipment(s).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/SaveCSV.cs` (SaveCSVModel)

*   **Purpose:** Singleton class (`SaveCSVModel.Instance`) that orchestrates the processing and saving of CSV data, called by `CSVUtils.SaveCsv` and `CSVUtils.ReplaceCSV`.
*   **`ProcessDroppedFile(string droppedFilePath, FileTypes fileType, bool overWriteExisting)`:**
    *   Entry point called by `CSVUtils`.
    *   Retrieves relevant `AsycudaDocumentSet`(s) using `Utils.GetDocSets`.
    *   Calls private `SaveCSV` method.
*   **`SaveCSV(string droppedFilePath, FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting)`:**
    *   Calls `CreateRawDataFile` helper to read the CSV, find headers, and package info into a `RawDataFile` object. (Dependency: `CSVImporter`, `RawDataFile`).
    *   Looks up an `IRawDataExtractor` implementation in the `extractors` dictionary based on `fileType.FileImporterInfos.EntryType`.
    *   Most entry types map to `SaveCsvEntryData`, while `SubItems` maps to `SaveCsvSubItems`.
    *   Calls the `Extract` method of the selected extractor, passing the `RawDataFile`. This method contains the logic for mapping data based on `fileType.FileTypeMappings` and saving it to the database. (Dependency: `IRawDataExtractor` implementations like `SaveCsvEntryData`, `SaveCsvSubItems`).

## Analysis: `WaterNut.Business.Services/Importers/CSVImporter.cs`

*   **Purpose:** Handles the initial reading and preparation of CSV file data before it's processed by specific data extractors (like `SaveCsvEntryData`). Used by `SaveCSVModel`.
*   **`GetFileLines(string droppedFilePath)`:** Reads the raw text file, applies any regex replacements defined in `fileType.FileTypeReplaceRegex`, and splits the result into lines.
*   **`GetHeadings(IEnumerable<string> lines)`:** Takes the first line from the result of `GetFileLines`, splits it into columns using a `.CsvSplit()` extension method, and cleans the resulting header strings.
*   **(Note:** The `Import` method within this class defines a slightly different orchestration flow involving `CSVDataExtractor` and `EntryDataManager`, which doesn't seem to be the path taken by the `SaveCsv` action.)

## Analysis: `WaterNut.Business.Services/.../SaveCSV/SaveCsvEntryData.cs`

*   **Purpose:** Implements `IRawDataExtractor` for most standard CSV entry types (PO, Sales, INV, DIS, ADJ, etc.). Called by `SaveCSVModel`.
*   **`Extract(RawDataFile rawDataFile)`:**
    *   Calls `CreateCSVDataFile` helper:
        *   Optionally refines the `FileTypes` using `FileTypeManager.GetHeadingFileType`.
        *   Instantiates and executes `CSVDataExtractor` to parse lines and map data based on `fileType.FileTypeMappings`, returning structured data. (Dependency: `CSVDataExtractor`).
        *   Creates a new `DataFile` object containing the extracted data and context.
    *   Instantiates `DataFileProcessor` and calls its `Process` method, passing the new `DataFile`. This delegates the database saving logic back to the `DataFileProcessor`. (Dependency: `DataFileProcessor`).

## Analysis: `WaterNut.Business.Services/Importers/CSVDataExtractor.cs`

*   **Purpose:** Extracts structured data from raw CSV lines based on `FileTypeMappings`. Called by `SaveCsvEntryData`.
*   **`Execute()`:**
    *   Calls `GetMappings` to map `FileTypeMappings` to header column indices.
    *   Calls `GetCSVDataSummayList` to process data lines.
*   **`GetMappings` (Helper):** Creates a dictionary mapping `FileTypeMappings` (where `OriginalName` matches a header) to the column index.
*   **`GetCSVDataSummayList` (Helper):** Iterates through data lines (skipping header) and calls `GetCSVDataFromLine` for each.
*   **`GetCSVDataFromLine` (Helper):**
    *   Splits the line using `.CsvSplit()`.
    *   Creates a dynamic object (`BetterExpando`).
    *   Iterates through the `FileTypeMappings` applicable to the headers:
        *   Skips row if a required field is empty.
        *   Performs predefined validation checks (`ImportChecks` dictionary).
        *   Performs predefined data manipulation actions (`ImportActions` dictionary).
        *   For standard fields, calls `GetMappingValue` helper to get the processed value.
        *   Assigns the processed value to the corresponding property on the dynamic object.
    *   Calculates derived fields based on `fileType.ImportActions` using `System.Linq.Dynamic.ReplaceMacro`.
    *   Returns the populated dynamic object.
*   **`GetMappingValue` (Helper):** Applies regex replacements (`FileTypeMappingRegExs`) and converts the string value to the target data type (`Date`, `Number`, `string`) using `FileTypeManager` helpers.

## Analysis: `AutoBot/AllocateSalesUtils.cs`

*   **Purpose:** Provides the entry point for the sales allocation process.
*   **`AllocateSales()`:**
    *   Called by `FileActions` and `SessionActions`.
    *   Checks if `AssessEX` setting is true and if unallocated sales exist (via `HasUnallocatedSales` helper querying `TODO_UnallocatedSales` view). If so, returns early.
    *   Instantiates and calls `new AllocateSales().Execute(...)`, delegating the core allocation logic to the `AllocateSales` class (likely in `WaterNut.Business.Services/.../AllocatingSales`). (Dependency: `AllocateSales` class).
    *   Calls `EmailSalesErrorsUtils.EmailSalesErrors()` to report errors after allocation. (Dependency: `EmailSalesErrorsUtils`).

## Analysis: `WaterNut.Business.Services/.../AllocatingSales/AllocateSales.cs`

*   **Purpose:** Implements `IAllocateSalesProcessor` and orchestrates the multi-step process of allocating sales data to ASYCUDA entries. Called by `AllocateSalesUtils`.
*   **`Execute(...)` Methods:**
    *   The main entry point takes `ApplicationSettings`, flags (`allocateToLastAdjustment`, `onlyNewAllocations`), and an optional item list string (`lst`).
    *   Calls `BaseDataModel.GetItemSets` to group inventory items based on aliases, potentially filtering by `lst`.
    *   Calls the core `Execute` overload with the generated `itemSets`.
*   **Core `Execute` Logic:**
    *   Runs `SQLBlackBox.RunSqlBlackBox()`.
    *   Calls `AllocationsBaseModel.PrepareDataForAllocation`.
    *   Executes the following steps in parallel for each item set:
        1.  `ReAllocatedExistingXSales().Execute(x)`: Reallocates existing cross-border sales. (Dependency: `ReAllocatedExistingXSales`).
        2.  `ReallocateExistingEx9().Execute(x)`: Reallocates existing EX9 entries. (Dependency: `ReallocateExistingEx9`).
        3.  `AutoMatchSingleSetBasedProcessor().AutoMatch(...)`: Performs automatic matching between sales and ASYCUDA lines. (Dependency: `AutoMatchSingleSetBasedProcessor`).
        4.  `OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(...)`: Performs the main sales allocation based on item number matching. (Dependency: `OldSalesAllocator`).
        5.  `MarkErrors().Execute(x)`: Flags remaining unallocated quantities or other errors. (Dependency: `MarkErrors`).
    *   Logs progress throughout the process.

## Analysis: `AutoBot/ADJUtils.cs`

*   **Purpose:** Provides utilities for creating Adjustment ("ADJ") or Discrepancy ("DIS") entries, typically resulting in ASYCUDA IM9 documents.
*   **`CreateAdjustmentEntries(bool overwrite, string adjustmentType)`:**
    *   Called by `SessionActions` for `"CreateDiscpancyEntries"` (with `adjustmentType="DIS"`) and potentially other adjustment actions.
    *   Queries `TODO_AdjustmentsToXML` view filtered by `adjustmentType`. (Dependency: `CoreEntitiesContext`, `TODO_AdjustmentsToXML` view).
    *   Calls the core `CreateAdjustmentEntries` overload.
*   **Core `CreateAdjustmentEntries(..., List<IGrouping<int, TODO_AdjustmentsToXML>> lst, ...)`:**
    *   If `overwrite` is true, clears existing adjustment documents for the relevant `AsycudaDocumentSetId`s.
    *   Iterates through data grouped by `AsycudaDocumentSetId`.
    *   For each group, constructs a filter based on `EntryDataDetailsId`s.
    *   Starts tasks (potentially parallel) to:
        *   Create Duty Paid entries by calling `AdjustmentShortService().CreateIM9(...)` with a "Duty Paid" filter. (Dependency: `AdjustmentShortService`).
        *   Create Duty Free entries by calling `AdjustmentShortService().CreateIM9(...)` with a "Duty Free" filter. (Dependency: `AdjustmentShortService`).
        *   Create Duty Free Opening Stock entries by calling `AdjustmentOverService().CreateOPS(...)` with a "Duty Free" filter. (Dependency: `AdjustmentOverService`).
    *   Waits for tasks to complete.
    *   Calls `BaseDataModel.RenameDuplicateDocuments` to resolve potential naming conflicts.

## Analysis: `WaterNut.Business.Services/.../AllocatingSales/ReAllocatedExistingXSales.cs`

*   **Purpose:** Handles pre-allocation/reallocation of existing cross-border sales (xSales) data. Called by `AllocateSales.Execute`.
*   **`Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)`:**
    *   Returns early if `CurrentApplicationSettings.PreAllocateEx9s` is not true.
    *   Calls `GetPreAllocations` helper to retrieve potential pre-allocation data for the item sets (using either `GetUnAllocatedxSales` or `GetUnAllocatedxSalesMem` based on `AllocationsBaseModel.isDBMem` flag). (Dependency: `GetUnAllocatedxSales`, `GetUnAllocatedxSalesMem`).
    *   Calls `new ProcessPreAllocations().Execute(preAllocations)` to process and save the retrieved pre-allocations. (Dependency: `ProcessPreAllocations`).

## Analysis: `WaterNut.Business.Services/.../AutoMatching/AutoMatchProcessorSingleSetBased.cs`

*   **Purpose:** Performs automatic matching between discrepancy/adjustment data and existing entry data, primarily linking based on previous document numbers. Called by `AllocateSales.Execute`.
*   **`AutoMatch(..., List<(string ItemNumber, int InventoryItemId)> itemSet)`:**
    *   Retrieves `AdjustmentDetail` data for the `itemSet` (using `GetAllDiscrepancyDetails` helper).
    *   Clears existing allocations and resets quantities/status for the relevant items using complex SQL (`ClearDocSetAllocations` helper).
    *   Calls `DoAutoMatch` helper:
        *   Iterates through `AdjustmentDetail` records in parallel.
        *   For each record, calls `AutoMatchItemNumber` helper:
            *   Retrieves the corresponding `EntryDataDetail`.
            *   Applies a series of matching strategies (`PreviousInvoiceNumberMatcher`, `CNumberMatcher`, `EffectiveDatefProcessor`, `MissingCostProcessor`) by executing applicable `IAutoMatchProcessor` implementations. These likely update the `EntryDataDetail` with links or status changes. (Dependency: `IAutoMatchProcessor` implementations).
    *   Saves updated `EntryDataDetail` records and any generated `AdjustmentOversAllocations` or `AsycudaSalesAllocations` using `BulkMerge`.
    *   Calls `ProcessDISErrorsForAllocation` and `ProcessNoDataDISForAllocation` helpers to handle specific error conditions after matching. (Dependency: `ProcessDISErrorsForAllocation`, `ProcessDISErrorsForAllocationNoDB`).
    *   Saves any further changes made during error processing.

## Analysis: `WaterNut.Business.Services/.../Asycuda/SalesAllocator.cs`

*   **Purpose:** Contains the core algorithm for allocating quantities from sales/adjustments/discrepancies (`EntryDataDetails`) to matched ASYCUDA entry items (`xcuda_Item`). Called by `OldSalesAllocator`.
*   **`AllocateSales(..., List<AllocationsBaseModel.ItemSet> itemSets)`:**
    *   Iterates through prepared `itemSets`.
    *   Sorts the `SalesList` (by date, ID, line, qty) and `EntriesList` (by assessment/registration date, CNumber, etc.) within each set.
    *   Calls `AllocateSalestoAsycudaByKey` helper for each set.
    *   Returns the processed lists containing updated allocation quantities.
*   **`AllocateSalestoAsycudaByKey` (Core Allocation Loop):**
    *   Iterates through the sorted `saleslst`.
    *   For each `saleitm`, iterates through the sorted `asycudaEntries` list.
    *   Skips already fully allocated ASYCUDA entries.
    *   Handles "Early Sales" (sale date before ASYCUDA assessment date) by adding exception allocations.
    *   Handles returns (negative sale quantity) by attempting to find the previously allocated ASYCUDA entry (`GetPreviousAllocatedAsycudaItem`) and potentially moving backward in the ASYCUDA list.
    *   Calculates available quantity on the current ASYCUDA item (`GetAsycudaItmQtyToAllocate`), considering sub-items and Duty Free/Paid status.
    *   Calls `AllocateSaleItem` helper to perform the allocation:
        *   Determines the amount to allocate (minimum of remaining sale qty and available ASYCUDA qty).
        *   Creates an `AsycudaSalesAllocations` record linking the sale and ASYCUDA item with the allocated quantity.
        *   Updates `QtyAllocated` on both `EntryDataDetails` and `xcuda_Item`.
        *   Updates Duty Free/Paid specific allocated quantities (`DFQtyAllocated`, `DPQtyAllocated`) on `xcuda_Item`.
        *   Updates linked `xcuda_PreviousItem` / xBond quantities (`SetPreviousItemXbond`).
    *   Continues iterating through ASYCUDA entries until the sale item is fully allocated or all entries are checked.
    *   Adds exception allocations (`AddExceptionAllocation`) for remaining unallocated quantities or specific errors (e.g., "No Asycuda Entries Found", "Early Sales", "Returned More than Sold").

## Analysis: `WaterNut.Business.Services/.../Asycuda/OldSalesAllocator.cs`

*   **Purpose:** Prepares sales, adjustment, and discrepancy data and matches it with potential ASYCUDA entries based on item numbers and aliases, before delegating the final allocation logic. Called by `AllocateSales.Execute`.
*   **`AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(...)`:**
    *   Calls `MatchSalestoAsycudaEntriesOnItemNumber` helper to gather and prepare data.
    *   Instantiates `SalesAllocator` and calls its `AllocateSales` method, passing the prepared data and flags. This performs the core quantity allocation. (Dependency: `SalesAllocator`).
    *   Calls `SaveAllocations` helper (which uses `SaveAllocationSQL`) to persist the results. (Dependency: `SaveAllocationSQL`).
*   **`MatchSalestoAsycudaEntriesOnItemNumber` (Helper):**
    *   Asynchronously gathers ASYCUDA entries (`GeAsycudaEntriesWithItemNumber`), sales (`GetSaleslstWithItemNumber`), adjustments (`GetAdjustmentslstWithItemNumber`), and discrepancies (`GetDiscrepancieslstWithItemNumber`) in parallel for the item set.
    *   Combines sales, adjustments, and discrepancies.
    *   Calls `CreateItemSetsWithItemNumbers` helper to structure the data.
*   **`CreateItemSetsWithItemNumbers` (Helper):**
    *   Joins sales/adjustments/discrepancies with ASYCUDA entries based on `ItemNumber`.
    *   Groups the results into a dictionary keyed by (Date, EntryDataId, ItemNumber, InventoryItemId).
    *   Calls `AddLumpAndAliasData` helper to enrich the potential ASYCUDA `EntriesList` for each item set by adding entries related via aliases or lumped item configurations (using `AllocationsBaseModel.Instance.InventoryAliasCache`).
*   **(Data Retrieval Helpers):** Methods like `GetSaleslstWithItemNumber`, `GetAdjustmentslstWithItemNumber`, etc., query the database (using `GetEntryDataDetails` which selects between DB/Memory implementations) and format the results.

## Analysis: `WaterNut.Business.Services/.../Asycuda/MarkErrors.cs`

*   **Purpose:** Identifies and marks over-allocated and under-allocated entries after the main allocation process. Called last in the `AllocateSales.Execute` sequence.
*   **`Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)`:**
    *   Retrieves relevant `xcuda_Item` data (using `GetXcudaItemsMem`).
    *   Calls `MarkOverAllocatedEntries` helper.
    *   Calls `MarkUnderAllocatedEntries` helper.
*   **`MarkOverAllocatedEntries` (Helper):**
    *   Finds `xcuda_Item` IDs where `QtyAllocated > ItemQuantity` (using `GetOverAllocatedAsycudaEntries` helper).
    *   Retrieves associated `AsycudaSalesAllocations` (using `GetUOAllocations` helper).
    *   Generates SQL (`CreateOverAllocatedSQL` helper) to reduce `QtyAllocated` on existing allocations and potentially create new balancing negative allocations with "Over Allocated Entry" status.
    *   Executes the generated SQL.
    *   Calls `CleanupZeroAllocated` helper.
*   **`MarkUnderAllocatedEntries` (Helper):**
    *   Finds `xcuda_Item` IDs where `QtyAllocated < 0` (using `GetUnderAllocatedAsycudaItems` helper).
    *   Retrieves associated `AsycudaSalesAllocations` (using `GetUOAllocations` helper).
    *   Generates SQL (`CreateUnderAllocatedSql` helper) to increase `QtyAllocated` on existing negative allocations (cancelling them out) and potentially create new balancing positive allocations with "Under Allocated by" status.
    *   Executes the generated SQL.
    *   Calls `CleanupZeroAllocated` helper.
*   **`CleanupZeroAllocated` (Helper):** Deletes `AsycudaSalesAllocations` where `QtyAllocated` is 0 and `Status` is null.

## Analysis: `WaterNut.Business.Services/.../CreatingEx9/CreateEx9Mem.cs`

*   **Purpose:** Implements `ICreateEx9` and contains the core logic for generating EX9 `DocumentCT` objects based on filtered allocation data. Called via `AllocationsModel.Instance.CreateEx9`.
*   **`Execute(...)` / `ProcessEx9(...)`:**
    *   Orchestrates the EX9 creation process.
    *   Runs `SQLBlackBox`.
    *   Prepares necessary data asynchronously:
        *   Loads allocation data into memory (using `GetEx9DataMem`).
        *   Initializes universal caches (`InitializeUniversalData`).
        *   Creates monthly date filters (`CreateFilters`).
    *   Iterates through monthly filters (currently sequential despite `AsParallel`).
    *   Calls `CreateDutyFreePaidEntries` helper.
*   **`CreateDutyFreePaidEntries` Helpers (Multiple Overloads):**
    *   Groups allocation data using `CreateAllocationDataBlocks().Execute(...)` based on the current filter (monthly) and potentially other grouping flags (Invoice/IM7). (Dependency: `CreateAllocationDataBlocks`).
    *   Iterates through "Duty Paid" and "Duty Free" types (currently sequential).
    *   Sets the appropriate `Customs_Procedure` on the target `AsycudaDocumentSet`.
    *   The final overload (inferred) likely iterates through the filtered `allocationDataBlocks`.
    *   For each block, calls `CreateDocumentFromAllocationDataBlock().Execute(...)` to generate the `DocumentCT` object, including headers, items (`xcuda_Item`), and previous document links (`xcuda_PreviousItem`). (Dependency: `CreateDocumentFromAllocationDataBlock`).
    *   Collects the generated `DocumentCT` objects.
    *   Calls `BaseDataModel.RenameDuplicateDocuments` after processing all blocks.

## Analysis: `WaterNut.Business.Services/.../AdjustmentQS/AdjustmentShortService.cs` (Custom Part)

*   **Purpose:** Extends base service with custom logic, primarily orchestrating the creation of IM9 (Adjustment/Discrepancy) documents. Called by `ADJUtils`.
*   **`CreateIM9(...)`:**
    *   Retrieves the target `AsycudaDocumentSet` and sets the appropriate `Customs_Procedure` based on `dutyFreePaid` status.
    *   Calls `CreateAllocationDataBlocks` helper:
        *   Retrieves filtered allocation data using `GetIM9Data` helper (which calls `getEx9AllocationsList().Execute(...)`). (Dependency: `getEx9AllocationsList`).
        *   Groups the retrieved data into `AllocationDataBlock` objects, potentially per-invoice or per-month/duty-type.
    *   Calls `CreateDutyFreePaidDocument().Execute(...)`, passing the `AllocationDataBlock`s, `docSet`, `adjustmentType`, `dutyFreePaid` status, and numerous flags. This performs the core document generation. (Dependency: `CreateDutyFreePaidDocument`).
    *   Performs post-processing (stripping attachments, attaching email PDF, renaming duplicates) via `BaseDataModel`.
*   **AutoMatch Methods:** Provides wrappers calling `AutoMatchUtils` or `AutoMatchSetBasedProcessor`.

## Analysis: `WaterNut.Business.Services/.../SaveCSV/DataFileProcessor.cs`

*   **Purpose:** Dispatches structured data (contained in a `DataFile` object) to the appropriate database saving logic based on file format and entry type. Called by `SaveCsvEntryData` and potentially PDF processors.
*   **`_dataFileActions` Dictionary:** Nested dictionary mapping `[FileFormat][EntryType]` to specific saving functions (`Func<DataFile, Task<bool>>`). Initialized with `CSVDataFileActions.Actions` and `PDFDataFileActions.Actions`. (Dependency: `CSVDataFileActions`, `PDFDataFileActions`).
*   **`Process(DataFile dataFile)`:**
    *   Checks if the `dataFile`'s document set matches the current application settings.
    *   Looks up the appropriate saving function in `_dataFileActions` using the `dataFile`'s format and entry type.
    *   Invokes the found function, passing the `dataFile`. The invoked function (from `CSVDataFileActions` or `PDFDataFileActions`) contains the actual database persistence logic.

## Analysis: `WaterNut.Business.Services/.../AdjustmentQS/AdjustmentOverService.cs` (Custom Part)

*   **Purpose:** Extends base service with custom logic for creating OPS (Opening Stock) adjustment documents. Called by `ADJUtils`.
*   **`CreateOPS(...)`:**
    *   Retrieves the target `AsycudaDocumentSet` and sets the appropriate `Customs_Procedure` (Warehouse, Discrepancy, Duty Paid=true).
    *   Configures the `docSet` using `BaseDataModel.ConfigureDocSet`.
    *   Queries `TODO_AdjustmentOversToXML` view based on filters (`filterExpression`, `asycudaDocumentSetId`, `adjustmentType`, optional `entryDataDetailsIds`).
    *   Transforms the query results into `EntryDataDetails` objects, calculating the adjustment `Quantity` (Received - Invoice).
    *   Calls `BaseDataModel.Instance.CreateEntryItems(...)`, passing the prepared `EntryDataDetails` list, `docSet`, and flags. This performs the core document generation. (Dependency: `BaseDataModel.CreateEntryItems`).
    *   Performs post-processing (stripping attachments, attaching email PDF/blank C71, renaming duplicates) via `BaseDataModel` if `emailId` is provided.

## Analysis: `WaterNut.Business.Services/.../SaveCSV/CSVDataFileActions.cs`

*   **Purpose:** Defines the static `Actions` dictionary used by `DataFileProcessor` to map CSV `EntryType` strings to the specific functions that save the extracted data.
*   **`Actions` Dictionary:**
    *   Maps `EntryType` strings (e.g., "PO", "Sales", "Rider") to lambda expressions.
    *   Each lambda instantiates a specific processor/importer class and calls its `Process` or `ProcessAsync` method, passing the `DataFile`.
    *   **Key Mapping:** Multiple core types ("PO", "Sales", "Adj", "Dis", "Ops") map to `new EntryDataProcessor().Process(dataFile)`. (Dependency: `EntryDataProcessor`).
    *   Other types map to specialized classes like `RiderImporter`, `ExpiredEntriesImporter`, `CancelledEntriesImporter`, `CSVShipmentImporter`, `SaveItemHistory`. (Dependency: Respective Importer classes).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/EntryDataProcessor.cs`

*   **Purpose:** Orchestrates the saving of extracted data for core CSV entry types (PO, Sales, Adj, Dis, Ops). Called by `CSVDataFileActions`.
*   **`Process(DataFile dataFile)`:**
    *   Executes a sequence of import steps by calling methods on specialized importer classes:
        1.  `_inventoryImporter.ImportInventory(dataFile)`: Saves/updates inventory master data. (Dependency: `InventoryImporter`).
        2.  `_supplierImporter.ImportSuppliers(dataFile)`: Saves/updates supplier master data. (Dependency: `SupplierImporter`).
        3.  `_entryDataFileImporter.ImportEntryDataFile(dataFile)`: Saves file metadata/links. (Dependency: `EntryDataFileImporter`).
        4.  `_entryDataImporter.ImportEntryData(dataFile)`: Saves the main entry header/detail data. (Dependency: `EntryDataImporter`).
    *   The actual database persistence logic resides within these individual importer classes.

## Analysis: `WaterNut.Business.Services/.../SaveCSV/EntryDataImporter.cs`

*   **Purpose:** Handles the processing and saving of core entry data (PO, Sales, Adj, Dis, Ops) extracted from CSVs. Called by `EntryDataProcessor`.
*   **`ImportEntryData(DataFile dataFile)`:**
    *   Calls `RawEntryDataExtractor().GetRawEntryData(dataFile)` to transform the `List<dynamic>` data into structured `RawEntryData` objects (grouping details under headers). (Dependency: `RawEntryDataExtractor`).
    *   Calls `RawEntryDataProcessor().GetValidRawEntryData(...)` to validate the structured data. (Dependency: `RawEntryDataProcessor`).
    *   Calls `RawEntryDataProcessor().CreateEntryData(...)` asynchronously to convert the validated `RawEntryData` into `EntryData`/`EntryDataDetails` entities and save them to the database. (Dependency: `RawEntryDataProcessor`).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/RawEntryDataExtractor.cs`

*   **Purpose:** Transforms the flat `List<dynamic>` (representing CSV rows) extracted by `CSVDataExtractor` into a structured `List<RawEntryData>`. Called by `EntryDataImporter`.
*   **`GetRawEntryData(DataFile dataFile)` / `CreateRawEntryData(...)`:**
    *   Takes the `List<dynamic>` data.
    *   Groups the dynamic objects by header-level fields (`EntryDataId`, `EntryDataDate`, `SupplierInvoiceNo`, `CustomerName`).
    *   For each group, creates a `RawEntryData` object containing:
        *   `EntryDataValue`: Header information extracted from the group (using `.FirstOrDefault()` or `.Max()` for fields potentially repeated on detail lines).
        *   `List<EntryDataDetails>`: Detail lines created by mapping properties from each dynamic object in the group to an `EntryDataDetails` entity, performing type conversions and truncation.
        *   `List<TotalsValue>`: Summary total values extracted from the group.
        *   `List<InventoryItemsValue>`: Unique inventory items found in the group.
    *   Returns the list of structured `RawEntryData` objects.

## Analysis: `WaterNut.Business.Services/.../SaveCSV/RawEntryDataProcessing/RawEntryDataProcessor.cs`

*   **Purpose:** Performs validation on structured `RawEntryData` and dispatches the valid data for database persistence. Implements `IRawEntryDataProcessor`. Called by `EntryDataImporter`.
*   **`GetValidRawEntryData(List<RawEntryData> entryDataLst)`:**
    *   Filters the input list based on validation rules:
        *   Must have detail lines (`EntryDataDetails.Any()`).
        *   Must have `EntryDataId`.
        *   Must be unique based on (`EntryDataId`, `SupplierInvoiceNo`).
    *   Returns the filtered list of valid `RawEntryData`.
*   **`CreateEntryData(DataFile dataFile, List<RawEntryData> goodLst)`:**
    *   Takes the validated `RawEntryData` list (`goodLst`) and the original `DataFile` context.
    *   Calls `CreateEntryDataSelector().Execute(dataFile, goodLst)` asynchronously to handle the conversion to entities and database saving. (Dependency: `CreateEntryDataSelector`).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/CreatingEntryData/CreateEntryDataSelector.cs`

*   **Purpose:** Selects the concrete implementation for saving `EntryData` based on an internal flag. Implements `ICreateEntryDataProcessor`. Called by `RawEntryDataProcessor`.
*   **`Execute(DataFile dataFile, List<RawEntryData> goodLst)`:**
    *   Checks an `isDBMem` flag (currently hardcoded to `false`).
    *   Based on the flag, calls either `CreateEntryData().Execute(...)` or `CreateEntryDataSetBased().Execute(...)`.
    *   Currently always calls `CreateEntryDataSetBased().Execute(dataFile, goodLst)` asynchronously. (Dependency: `CreateEntryDataSetBased`).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/CreatingEntryData/CreateEntryDataSetBased.cs`

*   **Purpose:** Handles the final database persistence step for core CSV entry types (PO, Sales, Adj, Dis, Ops). Implements `ICreateEntryDataProcessor`. Called by `CreateEntryDataSelector`.
*   **`Execute(DataFile dataFile, List<RawEntryData> goodLst)`:**
    *   Ensures inventory items/aliases exist (`InventoryItemsAliasProcessor.UpdateInventoryItems`).
    *   Calls `EntryDataCreator().GetEntryData(...)` to get/create `EntryData` header entities corresponding to the `RawEntryData` headers. (Dependency: `EntryDataCreator`).
    *   Saves the `EntryData` headers using `BulkMerge` (for existing) and `BulkInsert` (for new). (Dependency: `EntryDataDSContext`, Bulk Extensions).
    *   Iterates through the results from `GetEntryData`. For each header entity and its associated raw details:
        *   Calls `EntryDataDetailsCreator().CreateEntryDataDetails(...)` to convert the raw details into `EntryDataDetails` entities linked to the header. (Dependency: `EntryDataDetailsCreator`).
    *   Flattens the list of created `EntryDataDetails`.
    *   Saves the `EntryDataDetails` using `BulkMerge` (for existing) and `BulkInsert` (for new).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/CreatingEntryData/EntryDataCreator.cs`

*   **Purpose:** Finds existing `EntryData` header entities or prepares new ones based on the incoming `RawEntryData`. Called by `CreateEntryDataSetBased`.
*   **`GetEntryData(...)`:**
    *   Calls `ExistingEntryDataMem().GetExistingEntryData(...)` to find existing `EntryData` entities matching the `RawEntryData` list. (Dependency: `ExistingEntryDataMem`).
    *   For `RawEntryData` items where no existing entity was found, it calls `NewEntryDataProcessorNoSave().Execute(...)` asynchronously to create new, unsaved `EntryData` entity objects based on the raw header data. (Dependency: `NewEntryDataProcessorNoSave`).
    *   Combines the existing and newly created `EntryData` entities.
    *   Returns a list of tuples, each containing an `EntryData` entity (either existing or new) and the corresponding list of raw `EntryDataDetails` from the original `RawEntryData`.

## Analysis: `WaterNut.Business.Services/.../SaveCSV/CreatingEntryData/EntryDataDetailsCreator.cs`

*   **Purpose:** Converts the raw detail lines associated with an `EntryData` header into `EntryDataDetails` entity objects. Called by `CreateEntryDataSetBased`.
*   **`CreateEntryDataDetails(...)`:**
    *   Takes a tuple containing the parent `EntryData` entity and the list of raw detail lines.
    *   Iterates through the raw detail lines.
    *   For each raw detail, creates a new `EntryDataDetails` entity.
    *   Maps properties from the raw detail to the entity, performing truncation and null checks.
    *   Sets the `EntryData_Id` foreign key on the detail entity to link it to the parent header.
    *   Includes specific logic for calculating `InvoiceQty` and `EffectiveDate` for "ADJ" type entries.
    *   Sets `TrackingState` to `Added`.
    *   Returns the list of newly created `EntryDataDetails` entities.

## Analysis: `WaterNut.Business.Services/.../BaseBusinessLayerDS.cs` (CreateEntryItems Method)

*   **Purpose:** Generic engine for creating ASYCUDA documents (`DocumentCT`, likely IM7 or similar) from various input data sources represented as `EntryDataDetails` (e.g., for OPS Adjustments). Called by `AdjustmentOverService.CreateOPS`.
*   **`CreateEntryItems(...)`:**
    *   Takes a list of `EntryDataDetails`, the target `AsycudaDocumentSet`, and numerous flags controlling grouping and behavior (e.g., `perInvoice`, `perIM7`, `applyPrevItmCheck`).
    *   Prepares the input data by grouping/transforming it into `IEntryLineData` objects (`CreateEntryLineDataList` helper).
    *   Iterates through the prepared `EntryLineData` (`pod`).
    *   Handles document splitting based on line count or grouping flags (`perInvoice`, `perIM7`). Creates new `DocumentCT` objects (`cdoc`) as needed using `CreateDocumentCt` and initializes headers using `IntCdoc`.
    *   For each `pod`, calls `CreateItemFromEntryDataDetail` helper to create the `xcuda_Item` entity and populate its sub-entities (Tarification, Valuation, Goods Description, Weight, Taxation, Packages) based on the `pod` data.
    *   If `applyPrevItmCheck` is true, calls `LinkPreviousDocuments` helper to create `xcuda_PreviousItem` links.
    *   Saves completed `DocumentCT` objects using `SaveDocumentCT` helper (which likely calls `SaveDocument`).
    *   Returns the list of generated `DocumentCT` objects.
*   **`CreateItemFromEntryDataDetail` (Helper):** Core mapping logic from `IEntryLineData` to the `xcuda_Item` object model and its sub-entities.

## Analysis: `WaterNut.Business.Services/Asycuda/C71ToDatabase.cs`

*   **Purpose:** Handles creation, import, and export of C71 (Valuation Form) data. Contains logic called by `C71Utils.CreateC71`.
*   **`CreateC71(...)`:**
    *   Takes supplier info, data from `TODO_C71ToXML` view (`lst`), document reference, and consignee details.
    *   Creates a new `xC71_Value_declaration_form` entity object (`CreateNewC71` helper).
    *   Populates Seller, Buyer, and Declarant segments based on input parameters.
    *   Iterates through the `lst` (data from the view).
    *   For each record, creates a new `xC71_Item` entity and maps properties (InvoiceNo, Date, ItemNo, Description, Quantity, Total, Currency, Rate, TariffCode) from the view data to the entity.
    *   Adds the created `xC71_Item` entities to the main `xC71_Value_declaration_form` entity.
    *   Returns the populated `xC71_Value_declaration_form` entity (which is then saved to DB and exported to XML by the caller).
*   **`SaveToDatabase(...)`:** Handles importing data from a parsed C71 XML file into the database entities (`Registered`, `xC71_Item`, etc.). Called by `BaseDataModel.ImportC71`.
*   **Export Methods (`ExportC71`, etc.):** Handles converting database entities back to the XML object model for serialization.

## Analysis: `WaterNut.Business.Services/Asycuda/LicenseToDatabase.cs`

*   **Purpose:** Handles creation, import, and export of License data. Contains logic called by `LICUtils.CreateLicence`.
*   **`CreateLicense(...)`:**
    *   Takes supplier info, contact info, data from `TODO_LicenseToXML` view (`lst`), document reference, and consignee details.
    *   Creates a new `xLIC_License` entity object (`CreateLicense` helper).
    *   Populates the `xLIC_General_segment` with Exporter (supplier) and Importer (consignee/contact) details.
    *   Iterates through the `lst` (data from the view), grouped by `TariffCode` and `LicenseDescription`.
    *   For each group, creates a new `xLIC_Lic_item_segment` entity and maps properties (Description, Commodity Code, Quantity Requested/Approved, Origin, UOM) from the view data. Quantity is summed, using VolumeLiters if available.
    *   Adds the created item segments to the main `xLIC_License` entity.
    *   Returns the populated `xLIC_License` entity (which is then saved to DB and exported to XML by the caller).
*   **`SaveToDatabase(...)`:** Handles importing data from a parsed License XML file into the database entities (`Registered`, `xLIC_Lic_item_segment`, etc.). Called by `BaseDataModel.ImportLicense`.
*   **Export Methods (`ExportLicense`, etc.):** Handles converting database entities back to the XML object model for serialization.

## Analysis: `WaterNut.Business.Services/.../SQLBlackBox.cs`

*   **Purpose:** Executes a predefined set of external SQL scripts for database maintenance tasks. Called by `AllocateSales` and `CreateEx9Mem`.
*   **`RunSqlBlackBox()`:**
    *   Defines a hardcoded list of script names (e.g., "CleanBackupHistory", "SmatIndexRebuild", "UpdateStats", "AdhocChange", "dropIndexDupes").
    *   Gets the connection string from `CoreEntitiesContext`.
    *   Iterates through the script names:
        *   Reads the content of the corresponding `.sql` file from the `SQLBlackBox\` subfolder. (Dependency: External `.sql` files).
        *   Executes the script content against the database using `SqlCommand.ExecuteNonQuery()` with an infinite timeout.

## Analysis: `AutoBot/SubmitSalesXmlToCustomsUtils.cs`

*   **Purpose:** Handles the submission of assessed Ex-Warehouse (Sales) documents (XMLs/PDFs) to Customs contacts via email.
*   **`SubmitSalesXMLToCustoms(int months)`:**
    *   Called by `SessionActions`.
    *   Determines the relevant date range and document set context (`BaseDataModel.CurrentSalesInfo`).
    *   Retrieves records from `TODO_SubmitXMLToCustoms` view for the period, grouped by `EmailId` (`GetSalesXmls` helper).
    *   For each group, calls `ProcessSalesData` helper.
*   **`ProcessSalesData(...)` (Helper):**
    *   Generates an email body summarizing the documents (`CreateEmailBody` helper).
    *   Finds associated attachment file paths (`GetAttachments` helper querying `AsycudaDocument_Attachments`).
    *   Sends the email (forwarding original if possible, otherwise new) with body and attachments to "Customs" contacts (`SendEmails` helper using `EmailDownloader`).
    *   Logs the submission action in `AttachmentLog` (`UpdateEmailLog` helper).

## Analysis: `WaterNut.Business.Services/.../CreatingEx9/CreateAllocationDataBlocks.cs`

*   **Purpose:** Groups filtered allocation data (`EX9Allocations`) into logical blocks (`AllocationDataBlock`) suitable for generating individual ASYCUDA documents. Used by `CreateEx9Mem` and `AdjustmentShortService`.
*   **`Execute(...)`:**
    *   Retrieves `EX9Allocations` data using `GetEx9DataMem.Execute(...)` based on input filters. (Dependency: `GetEx9DataMem`).
    *   Filters out allocations related to items in the `errors` list.
    *   Calls `CreateWholeAllocationDataBlocks` helper to perform grouping.
    *   Yields the resulting `AllocationDataBlock` objects.
*   **`CreateWholeAllocationDataBlocks` (Helper):** Selects grouping strategy based on `perIM7` flag.
*   **`CreateWholeNonIM7AllocationDataBlocks` (Helper):** Groups allocations by `DutyFreePaid`, `Type`, `MonthYear` (fixed), and `PreviousItem_Id`.
*   **`CreateWholeIM7AllocationDataBlocks` (Helper):** Groups allocations by `DutyFreePaid`, `Type`, `MonthYear` (fixed), and `pCNumber`.
*   Each yielded `AllocationDataBlock` contains the grouping keys and the list of associated `EX9Allocations`.

## Analysis: `WaterNut.Business.Services/.../SaveCSV/InventoryImporter.cs`

*   **Purpose:** Orchestrates the import/update of inventory item master data based on data found in the `DataFile`. Called by `EntryDataProcessor`.
*   **`ImportInventory(DataFile dataFile)`:**
    *   Calls `InventoryItemDataUtils.CreateItemGroupList(...)` to extract unique inventory items and descriptions from `dataFile.Data`. (Dependency: `InventoryItemDataUtils`).
    *   Calls `InventorySourceFactory.GetInventorySource(...)` to determine the data source context based on `dataFile.FileType`. (Dependency: `InventorySourceFactory`).
    *   Calls `InventoryProcessorSelector().Execute(...)` asynchronously, passing the extracted item list and source context. This delegates the database persistence logic (finding/creating/updating `InventoryItem` records). (Dependency: `InventoryProcessorSelector`).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/InventoryProcessing/InventoryProcessorSelector.cs`

*   **Purpose:** Selects the concrete implementation for saving inventory data based on an internal flag. Implements `IInventoryProcessor`. Called by `InventoryImporter`.
*   **`Execute(...)`:**
    *   Checks an `isDbMem` flag (currently hardcoded to `false`).
    *   Based on the flag, calls either `InventoryProcessor().Execute(...)` or `InventoryProcessorSet().Execute(...)`.
    *   Currently always calls `InventoryProcessorSet().Execute(...)` asynchronously. (Dependency: `InventoryProcessorSet`).

## Analysis: `WaterNut.Business.Services/.../SaveCSV/InventoryProcessing/InventoryProcessorSet.cs`

*   **Purpose:** Handles the core logic for finding, creating, and updating `InventoryItem` master data and related sources/aliases based on input `InventoryData`. Implements `IInventoryProcessor`. Called by `InventoryProcessorSelector`.
*   **`Execute(...)`:**
    *   Finds existing `InventoryItem` entities matching the input data (`InventoryItemDataUtils.GetExistingInventoryItemFromData`).
    *   Updates `TariffCode` and `Description` on existing items if changed in the input data.
    *   Creates and saves `InventoryItemSource` links for existing items if missing (`InventorySourceProcessor`).
    *   Saves changes to existing `InventoryItem` entities (delegating to `SaveInventoryItemsSelector().Execute(...)`). (Dependency: `SaveInventoryItemsSelector`).
    *   Updates `InventoryItemId` on the original dynamic data objects for existing items.
    *   Processes and saves standard codes/aliases for existing items (`InventoryCodesProcessor`, `InventoryAliasCodesProcessor`).
    *   Identifies and creates new `InventoryItem` entities for items not found in the database (`InventoryItemDataUtils.GetNewInventoryItemFromData`).
    *   Updates `InventoryItemId` on the original dynamic data objects for new items.
    *   Processes and saves standard codes/aliases for new items.
    *   *(Note: Saving of the newly created `InventoryItem` entities themselves might happen within `GetNewInventoryItemFromData` or require a separate call not explicitly shown here).*

## Analysis: `WaterNut.Business.Services/.../SavingInventoryItems/SaveInventoryItemSelector.cs`

*   **Purpose:** Selects the concrete implementation for saving processed `InventoryItem` data based on an internal flag. Implements `ISaveInventoryItemsProcessor`. Called by `InventoryProcessorSet`.
*   **`Execute(List<InventoryDataItem> processedInventoryItems)`:**
    *   Checks an `isDbMem` flag (currently hardcoded to `false`).
    *   Based on the flag, calls either `SaveInventoryItems().Execute(...)` or `SaveInventoryItemsBulk().Execute(...)`.
    *   Currently always calls `SaveInventoryItemsBulk().Execute(...)`. (Dependency: `SaveInventoryItemsBulk`).

## Analysis: `WaterNut.Business.Services/.../SavingInventoryItems/SaveInventoryItemsBulk.cs`

*   **Purpose:** Saves lists of new or updated `InventoryItem` entities to the database using efficient bulk operations. Implements `ISaveInventoryItemsProcessor`. Called by `SaveInventoryItemsSelector`.
*   **`Execute(List<InventoryDataItem> processedInventoryItems)`:**
    *   Extracts `InventoryItem` entities from the input list.
    *   Separates entities into `newItems` (`Id == 0`) and `existingItems` (`Id != 0`).
    *   Calls `BulkInsert` on `InventoryDSContext` for `newItems`. (Dependency: `InventoryDSContext`, Bulk Extensions).
    *   Calls `BulkMerge` on `InventoryDSContext` for `existingItems` (handles inserts/updates efficiently). (Dependency: `InventoryDSContext`, Bulk Extensions).

## Analysis: `WaterNut.Business.Services/.../CreatingEx9/CreateDutyFreePaidDocument.cs`

*   **Purpose:** Core class responsible for generating the actual ASYCUDA document structure (`DocumentCT`) for EX9 and IM9 (Adjustments/Discrepancies) based on processed allocation data. Called by `CreateEx9Mem` and `AdjustmentShortService`.
*   **`Execute(...)`:**
    *   Takes grouped allocation data (`AllocationDataBlock` list), target `AsycudaDocumentSet`, document type, duty status (`dfp`), PI summary, and numerous boolean flags controlling generation logic.
    *   Initializes inventory cache and document list.
    *   Checks for multi-month data if required.
    *   Iterates through `AllocationDataBlock`s (representing months or invoices).
    *   Calls `PrepareAllocationsData` helper to further group allocations within the block (e.g., by PreviousItem_Id).
    *   Iterates through the processed allocation groups (`mypod`).
    *   Handles document splitting based on line count, `perInvoice` flag, or `PerIM7` flag. Creates and initializes new `DocumentCT` objects (`cdoc`) as needed using `BaseDataModel.Instance.CreateDocumentCt` and `Ex9InitializeCdoc` helper.
    *   Calls `CreateEx9EntryAsync` helper asynchronously for each allocation group (`mypod`) to generate `xcuda_Item` and related entities within the current `cdoc`.
    *   Saves completed `DocumentCT` objects asynchronously using `SaveDocumentCT` helper (which calls `BaseDataModel.Instance.SaveDocumentCt`).
    *   Returns the list of generated `DocumentCT` objects.
*   **`CreateEx9EntryAsync` (Helper - Core Item Creation):**
    *   Takes processed allocation data (`mypod`), current document (`cdoc`), item count, flags.
    *   Iterates through previous items (`pitm`) linked to the allocations.
    *   Performs complex checks and quantity calculations based on flags (Historic/Current, PI checks, EX9 Bucket).
    *   Creates `xcuda_Item` entity.
    *   Creates `xcuda_PreviousItem` entity linking to the source item.
    *   Populates detailed fields for Tarification, Goods Description, Valuation, Weight, Taxation, Packages, Attached Documents based on allocation data and source item (`pitm`).
    *   Updates PI summary data.
    *   Generates SQL for potential status updates.
    *   Returns the count of items created and the generated SQL.

## Analysis: `WaterNut.Business.Services/.../AllocationsModelDS.cs`

*   **Purpose:** Singleton class (`AllocationsModel.Instance`) acting as a facade/service locator for various allocation-related operations.
*   **`CreateEx9` Property:** Returns `CreateEx9Mem.Instance`. This indicates the core EX9 creation logic called by `CreateEX9Utils` resides in the `CreateEx9Mem` class. (Dependency: `CreateEx9Mem`).
*   **Other Methods:** Provides methods for retrieving allocations (`GetAsycudaSalesAllocations`), translating filter expressions (`TranslateAllocationWhereExpression`), performing manual allocations (`ManuallyAllocate`), clearing existing allocations at various scopes (`ClearAllocations`, `ClearAllAllocations`, etc. - uses direct SQL execution), and exporting allocations to Excel (`Send2Excel`).

## Analysis: `WaterNut.Business.Services/.../Asycuda/ReallocateExistingEx9.cs`

*   **Purpose:** Handles pre-allocation/reallocation based on existing EX9 (Export/Ex-Warehouse) document data. Called by `AllocateSales.Execute`.
*   **`Execute(List<(string ItemNumber, int InventoryItemId)> itemSetLst)`:**
    *   Returns early if `CurrentApplicationSettings.PreAllocateEx9s` is not true.
    *   Calls `GetExistingEx9s` helper to retrieve potential pre-allocation data for the item sets (using either `GetExistingEx9s` or `GetExistingEx9sMem` based on `AllocationsBaseModel.isDBMem` flag). (Dependency: `GetExistingEx9s`, `GetExistingEx9sMem`).
    *   Calls `new ProcessPreAllocations().Execute(preAllocations)` to process and save the retrieved pre-allocations. (Dependency: `ProcessPreAllocations`).

## Analysis: `AutoBot/CreateEX9Utils.cs`

*   **Purpose:** Handles the creation of EX9 (Export/Ex-Warehouse) documents based on allocated sales data.
*   **`CreateEx9(bool overwrite, int months)`:**
    *   Called by `FileActions` and `SessionActions`.
    *   Runs `SQLBlackBox.RunSqlBlackBox()`.
    *   Gets date range and document set context using `BaseDataModel.CurrentSalesInfo`.
    *   Checks if relevant allocated sales data exists for the period using a complex SQL query (`HasData` helper querying `EX9AsycudaSalesAllocations` view).
    *   If data exists and `overwrite` is true, clears existing documents in the target document set using `BaseDataModel.Instance.ClearAsycudaDocumentSet`.
    *   Creates a dynamic LINQ filter string based on the date range.
    *   Calls `AllocationsModel.Instance.CreateEx9.Execute(...)`, passing the filter, flags, and target document set. This external method performs the core logic of generating EX9 `DocumentCT` objects from the filtered allocation data. (Dependency: `AllocationsModel.Instance.CreateEx9`).
    *   Returns the list of created `DocumentCT` objects.

## Analysis: `WaterNut.Business.Services/.../SaveCSV/SaveCsvSubItems.cs`

*   **Purpose:** Implements `IRawDataExtractor` specifically for CSV files containing sub-item definitions (`EntryType` = "SubItems"). Called by `SaveCSVModel`.
*   **`Extract(RawDataFile rawDataFile)`:**
    *   Maps expected sub-item headers ("Precision_4", "pCNumber", "RegistrationDate", "ItemNumber", "ItemDescription", "Quantity", "QtyAllocated") to column indices.
    *   Parses CSV lines into `SubItemData` objects.
    *   Calls `ImportInventory` helper to ensure all listed sub-items exist in the `InventoryItems` master table, creating them if necessary. (Dependency: `InventoryItemService`).
    *   Calls `ImportSubItems` helper:
        *   Groups sub-items by their parent item identifier (Precision_4, CNumber, RegistrationDate).
        *   For each parent group, finds the parent `xcuda_Item` record using `AsycudaDocumentItemService`.
        *   For each sub-item in the group, finds or creates a `SubItems` entity linked to the parent `xcuda_Item`.
        *   Updates the `SubItems` entity properties with data from the CSV.
        *   Saves changes to the parent `xcuda_Item` (which includes the updated `SubItems` collection) using `xcuda_ItemService`.