# Refactoring Plan v5.3 - Refined Error Handling

This plan outlines the refactoring of email and folder-based shipment processing to promote code reuse, separation of concerns, and includes a defined error handling strategy.

**Core Idea:** Standardize processing by having the `EmailProcessor` create a folder structure identical to manually dropped folders. A single `ShipmentFolderProcessor` then handles all folders found in the input directory, delegating core logic to shared methods. State propagation between file type processing steps is explicitly handled.

**Components & Workflow:**

1.  **Testing Prerequisite:**
    *   **Sample Data:** Use the prepared sample data folder located at `AutoBotUtilities.Tests/Test Data/HAWB9595443` (relative path from solution root). This folder simulates the output of `EmailProcessor`.
    *   **Enhance `AutoBotUtilities.Tests/FolderProcessorTests.cs::ProcessShipmentFolders_GeneratesXml_Test`** (or create a new test).
    *   Modify the test setup to copy the `HAWB9595443` sample data folder into the test's `ShipmentInput` directory.
    *   Add assertions to verify the database state after processing the sample folder against the *current* codebase. This includes checking for the correct `AsycudaDocumentSet`, associated `EntryData`, placeholder `Emails`, and `Attachments` records.
    *   **Goal:** Ensure this enhanced test passes reliably *before* starting the refactoring steps below.

2.  **New Class `EmailProcessor`:**
    *   **Responsibility:** Fetch relevant emails. For each:
        *   Parse/extract the **`referenceNumber`** (shipment name) from the email subject/body.
        *   Determine the `shipmentId` (e.g., `EmailId`).
        *   **Create Folder:** Create a subfolder within `ShipmentInput` named using the `referenceNumber`. Handle potential naming collisions.
        *   Save the email body as `Info.txt` inside this folder.
        *   Save all relevant attachments inside this folder.
    *   Stops after creating the folder structure.

3.  **Rename `FolderProcessor` -> `ShipmentFolderProcessor`:**
    *   Reflects its specific responsibility.

4.  **`ShipmentFolderProcessor`:**
    *   **Responsibility:** Single pipeline for processing folders in `ShipmentInput`.
    *   **Workflow:**
        *   Scan `ShipmentInput`.
        *   For each subfolder:
            *   Initialize `bool overallFolderSuccess = true;`
            *   Use `subfolder.Name` as `referenceNumber` and `shipmentId`.
            *   **Try-Catch Block (Outer):** Wrap folder processing.
                *   Find `Info.txt` (`infoTextFileInfo`).
                *   Initialize `shipmentContextData = new Dictionary<string, string>();`.
                *   **If `infoTextFileInfo` exists:**
                    *   Get the "Info" `FileType` (`infoFileType`). Handle DB error (critical).
                    *   Set `infoFileType.EmailId = shipmentId`. Initialize `infoFileType.Data`.
                    *   Call `InfoFileProcessor.ProcessInfoTextFile(infoFileType, infoTextFileInfo, ctx)`.
                        *   On `false`: Log error, `overallFolderSuccess = false;`. Continue processing other files.
                        *   On exception: Caught by outer block.
                    *   Copy key results from `infoFileType.Data` into `shipmentContextData`.
                *   Get the "ShipmentFolder" `FileType`. Handle DB error (critical).
                *   Find child `FileTypes` linked to "ShipmentFolder". Handle DB error (critical).
                *   **Loop through `childFileType`s:**
                    *   Find `matchingFiles`.
                    *   If files found:
                        *   Set `childFileType.EmailId = shipmentId`. Initialize `childFileType.Data`.
                        *   Copy context from `shipmentContextData` into `childFileType.Data`.
                        *   Call `ImportUtils.ProcessFileTypeActionsWorkflow(...)`.
                            *   On `false`: Log error, `overallFolderSuccess = false;`. Continue to next `childFileType`.
                            *   On exception: Caught by outer block.
            *   **Catch Block (Outer):** Log critical exception, `overallFolderSuccess = false;`.
            *   **Finally/After Try-Catch:** If `overallFolderSuccess`, `ArchiveProcessedFolder(...)`. Else, `HandleProcessingError(...)`.

5.  **New Class `InfoFileProcessor`:**
    *   **Responsibility:** Processes `Info.txt` using mappings from the "Info" `FileType`. Populates the passed `fileType.Data` collection. May handle direct DB updates.
    *   **Internal Structure:**
        *   `public static bool ProcessInfoTextFile(FileTypes infoFileType, FileInfo infoTextFile, CoreEntitiesContext ctx)`: Main entry point, orchestrates steps, top-level error handling.
        *   `private static bool ValidateInputs(...)`: Performs null checks and basic validation on inputs.
        *   `private static string ProcessLines(...)`: Iterates through lines of the file, calls processing for each line, aggregates results (like SQL statements).
        *   `private static string ProcessSingleLine(...)`: Processes one line, finds applicable mappings.
        *   `private static string ExtractDataAndGenerateSqlForMapping(...)`: Handles a single mapping for a line, extracts data, populates `fileType.Data`, generates SQL chunk.
        *   `private static bool ExecuteAggregatedSql(...)`: Executes the combined SQL statements generated from the file.
        *   `private static (List<InfoMapping> im, string line) GetEmailMappings(...)`: **Placeholder** - Needs implementation based on existing logic to find relevant mappings for a line.
        *   `private static (KeyValuePair<string, string> InfoData, InfoMapping Map) GetMappingData(...)`: **Placeholder** - Needs implementation based on existing logic to extract data using a mapping rule.
        *   `private static string GetDbStatement(...)`: **Placeholder** - Needs implementation based on existing logic to generate SQL from extracted data.
    *   **Error Handling:** Internal try-catch within methods. Log specific errors. Return `false` for recoverable errors. Throw exceptions for critical issues.

6.  **Shared Method `ImportUtils.ProcessFileTypeActionsWorkflow`:**
    *   **Responsibility:** Handles AsycudaDocumentSet association (if needed, checking `fileType.Data` first) and runs Data/Non-Specific Actions for a *non-Info.txt* `FileType`. Relies on caller preparing context in `FileType.Data`.
    *   **Signature:** `public static async Task<bool> ProcessFileTypeActionsWorkflow(FileTypes fileType, FileInfo[] matchingFiles, string referenceNumber, ApplicationSettings appSetting, CoreEntitiesContext ctx)`
    *   **Error Handling:** Internal try-catch. Log specific errors. Return `false` for non-critical action/DB failures. Throw exceptions for critical issues.

7.  **Refactor `Program.cs`:**
    *   Remove email processing logic.
    *   Instantiate `EmailProcessor` and `ShipmentFolderProcessor`.
    *   Call `emailProcessor.ProcessEmailsAsync(...)`.
    *   Call `shipmentFolderProcessor.ProcessShipmentFolders(...)`.

8.  **Post-Refactoring Testing:**
    *   Re-run the enhanced test suite (including the modified `ProcessShipmentFolders_GeneratesXml_Test`) against the refactored code to verify functionality is preserved.

**Error Handling Strategy Summary:**
*   Critical errors halt processing for the current folder, log, and move to Error.
*   Non-critical errors within `InfoFileProcessor` or `ProcessFileTypeActionsWorkflow` are logged, mark the folder as failed, but allow processing of subsequent file types within that folder to continue.
*   A folder is Archived only if all steps complete successfully; otherwise, it's moved to Error.
*   File system errors during Archive/Error moves are logged.