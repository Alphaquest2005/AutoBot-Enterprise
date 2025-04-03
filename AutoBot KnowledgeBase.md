# AutoBot Knowledge Base

## Overview

*   **Objective:** This application appears to automate processing based on application settings. It primarily focuses on checking emails, downloading attachments, processing files based on predefined types and rules, interacting with a database (likely for customs/shipping documents - Asycuda), and executing scheduled or triggered database actions. It also processes files dropped into specific folders (`Downloads`, `ShipmentInput`). Processing involves parsing files (PDFs, text, Excel), extracting data using regex or external tools/APIs (like `InvoiceReader`, `DeepSeekInvoiceApi`), executing configurable actions stored in the database (mapped to C# methods via `FileUtils.FileActions` for file-based triggers and `SessionsUtils.SessionActions` for session/schedule-based triggers), and updating database records. It also includes functionality for linking documents, downloading files (potentially via UI automation like SikuliX), reporting on data issues (unclassified items, incomplete suppliers, inadequate packages), and processing shipment information from various sources.
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
        d.  Process individual PDF files in a `Downloads` folder (`FolderProcessor.ProcessDownloadFolder`). This involves:
            i.  Copying the PDF to a structured `Documents` folder.
            ii. Identifying "Unknown" `FileTypes` (`FileTypeManager`).
            iii. Attempting PDF import (`PDFUtils.ImportPDF` -> `InvoiceReader.Import`).
            iv. If import fails, attempting deep seek import (`PDFUtils.ImportPDFDeepSeek` -> `DeepSeekInvoiceApi` -> `DataFileProcessor.Process`).
            v. If successful, potentially creating a shipment email/report (`ShipmentUtils.CreateShipmentEmail`).
            vi. Deleting the original PDF if processing succeeded.
        e.  *(Implied)* Potentially process shipment folders in `ShipmentInput` (`FolderProcessor.ProcessShipmentFolders`). This involves:
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

## Analysis: `AutoBot1/Program.cs`

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
    *   **Line 59:** Sets the `EmailId` property of the retrieved `FileTypes` to the original PDF filename (likely for context/linking, though seems odd as it's not an email).
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
    *   **Lines 152-164:** If the import (either initial or deep seek) was successful for the `fileType`, attempts to create a shipment email using `ShipmentUtils.CreateShipmentEmail`. Errors during this step also mark processing as failed. *Dependency: `ShipmentUtils.CreateShipmentEmail`.*
    *   **Line 167:** Returns `true` only if *all* file types were processed without critical errors.
*   **Method: `NotifyUnknownPDF(...)` (private void)**
    *   **Lines 174-189:** Sends an email notification using `EmailDownloader.EmailDownloader.SendEmail` to "Developer" contacts, indicating an unknown/unparseable PDF was found. Includes error details and attaches relevant files. *Dependency: `Utils.Client`, `EmailDownloader`.*
*   **Method: `ProcessShipmentFolders(ApplicationSettings appSetting)` (public async Task)**
    *   **Called by:** *Not called by `Program.Main` in the provided code. Might be called elsewhere or intended for a separate utility/process.*
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
        *   Adds extracted key-value pairs to `fileType.Data` (a `List<KeyValuePair<string, string>>`).
        *   Constructs SQL `UPDATE` statements based on `InfoMapping` properties (`EntityType`, `Field`, `EntityKeyField`) and the extracted data. Uses `fileType.Data` to find the key value for the `WHERE` clause.
        *   Executes the aggregated SQL statements using `CoreEntitiesContext`. *Dependency: `CoreEntitiesContext`.*
    *   **Helper Methods:** `GetEmailMappings`, `GetMappingData`, `GetDbStatement`, `ReplaceSpecialChar`.
*   **Class: `ImportUtils`**
    *   **Purpose:** Orchestrates the execution of actions defined in the database (`FileTypeActions`, `EmailMappingActions`) based on the current context (`FileType`, `EmailMapping`, `ApplicationSettings`). It acts as the bridge between database configuration and C# code execution.
    *   **Dependencies:** `FileTypes`, `FileInfo`, `ApplicationSettings`, `EmailMapping`, `FileTypeActions`, `EmailMappingActions`, `Actions` entity, `CoreEntitiesContext`, `FileUtils.FileActions` (dictionary), `BaseDataModel`, `EmailDownloader`.
    *   **Method: `ExecuteDataSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)` (static)**
        *   Retrieves `FileTypeActions` from the DB where `Actions.IsDataSpecific == true`, filtered by `appSetting` flags (`AssessIM7`, `AssessEX`) and `TestMode`. Ordered by `FileTypeAction.Id`.
        *   Looks up the C# implementation for each action name in `FileUtils.FileActions`.
        *   Calls `ExecuteActions` for each found action delegate.
    *   **Method: `ExecuteEmailMappingActions(EmailMapping emailMapping, FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)` (static)**
        *   Retrieves `EmailMappingActions` associated with the `emailMapping`, filtered by `TestMode`. Ordered by `EmailMappingAction.Id`.
        *   Looks up the C# implementation in `FileUtils.FileActions`.
        *   Calls `ExecuteActions` for each found action delegate.
    *   **Method: `ExecuteNonSpecificFileActions(FileTypes fileType, FileInfo[] files, ApplicationSettings appSetting)` (static)**
        *   Retrieves `FileTypeActions` from the DB where `Actions.IsDataSpecific == null` or `Actions.IsDataSpecific != true`, filtered by `appSetting` flags and `TestMode`. Ordered by `FileTypeAction.Id`.
        *   Looks up the C# implementation in `FileUtils.FileActions`.
        *   Calls `ExecuteActions` for each found action delegate.
    *   **Method: `ExecuteActions(FileTypes fileType, FileInfo[] files, (string Name, Action<FileTypes, FileInfo[]> Action) x)` (static)**
        *   **Core Action Executor:** Wraps the invocation of a specific action delegate (`x.Action`).
        *   **Logging:** Logs start, success, failure, and duration of actions.
        *   **ProcessNextStep Logic:** Before executing the main action (`x.Action`), it checks if `fileType.ProcessNextStep` (a `List<string>`) contains action names.
            *   If it does, it iterates through this list, looking up and invoking actions from `FileUtils.FileActions` sequentially.
            *   If an action named "Continue" is encountered in `ProcessNextStep`, it stops processing the list and proceeds to execute the main action (`x.Action`).
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
    *   **Values:** Lambda expressions or method group references pointing to static methods in various utility classes. These methods take `FileTypes` (for context) and `FileInfo[]` (for the files being processed) as parameters.
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
    *   `ImportPDF(FileInfo[] pdfFiles, FileTypes fileType)` (async Task):
        *   Orchestrates the primary PDF import process.
        *   Looks up context (`EmailId`, `FileTypeId`) from `AsycudaDocumentSet_Attachments`.
        *   Delegates the core parsing and data extraction to `InvoiceReader.Import`. **Dependency: `InvoiceReader` class.**
        *   Returns success/failure status.
    *   `ImportPDFDeepSeek(FileInfo[] fileInfos, FileTypes fileType)` (async Task):
        *   Alternative import method, likely used as a fallback.
        *   Extracts text using `InvoiceReader.GetPdftxt`.
        *   Calls an external (?) `DeepSeekInvoiceApi` to extract structured data. **Dependency: `DeepSeekInvoiceApi` class.**
        *   Maps results to internal `FileTypes` using `FileTypeManager`.
        *   Sets default values based on `FileTypeMappings`.
        *   Processes the extracted data using `DataFileProcessor.Process`. **Dependency: `DataFileProcessor` class.**
        *   Returns success/failure status.
    *   `AttachEmailPDF(FileTypes ft, FileInfo[] fs)`:
        *   Calls `BaseDataModel.AttachEmailPDF` to handle the attachment logic, passing `AsycudaDocumentSetId` and `EmailId`. **Dependency: `BaseDataModel` class.**
    *   `LinkPDFs()`:
        *   Retrieves completed entries using `Stp_TODO_ImportCompleteEntries` stored procedure.
        *   Calls `BaseDataModel.LinkPDFs` to perform the linking logic. **Dependency: `BaseDataModel` class, `Stp_TODO_ImportCompleteEntries` SP.**
    *   `ReLinkPDFs()`:
        *   Scans the "Imports" directory for recently modified PDFs.
        *   Uses regex to extract a CNumber (Customs Number?).
        *   Finds the corresponding `AsycudaDocuments` record.
        *   Creates `Attachments` and `AsycudaDocument_Attachments` records in the database to link the file if not already linked.
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
*   **Further Analysis Needed:** Specific scripts defining the structure of configuration tables (`FileType`, etc.) and the `TODO_` views are high priority. Understanding `PreProcessShipmentSP` is also important. Reading `dbo.FileTypes.Table.sql` is a high priority. Also need to analyze related `xcuda_` tables like `xcuda_Valuation_item`, `xcuda_Tarification`, `xcuda_Supplementary_unit`. **Priority: `dbo.Suppliers.Table.sql`**

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

*   **Explanation:** Added `dbo.TariffCodes` table and linked it from `InventoryItems`.

## Open Questions/Areas for Further Investigation

1.  **What is the exact logic within the methods referenced by `FileUtils.FileActions` and `SessionsUtils.SessionActions`?** (e.g., `DocumentUtils.ImportSalesEntries`, `AllocateSalesUtils.AllocateSales`, `ADJUtils.CreateAdjustmentEntries`, etc.) **Priority:** `InvoiceReader`, `DeepSeekInvoiceApi`, `DataFileProcessor`, `Shipment` class/extensions.
2.  How does `FileTypeManager.GetFileType` and `FileTypeManager.GetImportableFileType` work? Need to analyze `WaterNut.Business.Services/Utils/FileTypeManager.cs`.
3.  What is the implementation of `Utils.SaveAttachments` (called by `Program.cs`)? How does it relate to the attachment creation in `FolderProcessor.ProcessShipmentFolders` and `ShipmentUtils.CreateShipmentEmail`? Need to analyze `WaterNut.Business.Services/Utils/Utils.cs` more closely or find where `SaveAttachments` is defined.
4.  What does `BaseDataModel` contain beyond `CurrentApplicationSettings`, `CurrentSessionSchedule`, `CurrentSessionAction`? What do `AttachEmailPDF`, `LinkPDFs`, and `EmailExceptionHandler` do? (Need to find its definition - likely in `CoreEntities` or `WaterNut.DataSpace`).
5.  What is the exact schema for key configuration/data tables (`Emails`, `Attachments`, `AsycudaDocuments`, `AsycudaDocumentSet_Attachments`, `Customs_Procedure`, `Consignees`, `BondTypes`, `WeightCalculationMethods`, `FileGroups`, `FileTypes-FileImporterInfo`)? (Requires reading relevant SQL - **Priority: `dbo.Emails.Table.sql`, `dbo.Attachments.Table.sql`**)
6.  What is the exact schema for the `TODO_` views (`TODO_SubmitUnclassifiedItems`, `TODO_SubmitIncompleteSupplierInfo`, `TODO_SubmitInadequatePackages`) and the `Stp_TODO_ImportCompleteEntries` / `PreProcessShipmentSP` stored procedures? (Requires reading relevant SQL)
7.  What is the structure of the `Info.txt` file expected by `ProcessShipmentFolders` beyond the "BL" key? What other information might it contain? (Can infer somewhat from `ImportShipmentInfoFromTxt`).
8.  How is `FolderProcessor.ProcessShipmentFolders` intended to be invoked if not by `Program.Main`?
9.  What is the logic within `InvoiceReader.Import` and `InvoiceReader.GetPdftxt`? Where is this class defined?
10. What is the `DeepSeekInvoiceApi`? Is it internal or a third-party service? Where is it defined?
11. What does `DataFileProcessor.Process` do? Where is it defined?
12. What does `WaterNut.DataSpace.Utils.RunSiKuLi` do? What are the SikuliX scripts (e.g., "IM7-PDF")?
13. What is the `Shipment` class and its extension methods (`LoadEmailPOs`, `LoadDBBL`, `AutoCorrect`, `ProcessShipment`)? Where is it defined (likely `AutoBot/ShipmentExtensions.cs`)?