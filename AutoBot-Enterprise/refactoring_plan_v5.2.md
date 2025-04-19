    # Refactoring Plan v5.2 - Explicit State Propagation & Pre-Refactor Testing

This plan outlines the refactoring of email and folder-based shipment processing to promote code reuse and separation of concerns.

**Core Idea:** Standardize processing by having the `EmailProcessor` create a folder structure identical to manually dropped folders. A single `ShipmentFolderProcessor` then handles all folders found in the input directory, delegating core logic to shared methods.

**Components & Workflow:**

1.  **Testing Prerequisite:**
    *   **Enhance `AutoBotUtilities.Tests/FolderProcessorTests.cs::ProcessShipmentFolders_GeneratesXml_Test`**.
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
            *   Use `subfolder.Name` as `referenceNumber` and `shipmentId`.
            *   Find `Info.txt` (`infoTextFileInfo`).
            *   Initialize `shipmentContextData = new Dictionary<string, string>();`.
            *   **If `infoTextFileInfo` exists:**
                *   Get the "Info" `FileType` (`infoFileType`).
                *   Set `infoFileType.EmailId = shipmentId`.
                *   Initialize `infoFileType.Data`.
                *   Call `InfoFileProcessor.ProcessInfoTextFile(infoFileType, infoTextFileInfo, ctx)`.
                *   Copy key results (e.g., `AsycudaDocumentSetId`) from `infoFileType.Data` into `shipmentContextData`.
            *   Get the "ShipmentFolder" `FileType`.
            *   Find child `FileTypes` linked to "ShipmentFolder".
            *   **Loop through `childFileType`s:**
                *   Find `matchingFiles`.
                *   If files found:
                    *   Set `childFileType.EmailId = shipmentId`.
                    *   Initialize `childFileType.Data`.
                    *   Copy context from `shipmentContextData` into `childFileType.Data`.
                    *   Call `ImportUtils.ProcessFileTypeActionsWorkflow(...)`.
                    *   Track success/failure.
            *   Handle overall folder success/failure (Archive/Error).

5.  **New Class `InfoFileProcessor`:**
    *   **Responsibility:** Processes `Info.txt` using mappings from the "Info" `FileType`. Populates the passed `fileType.Data` collection. May handle direct DB updates.
    *   **Method:** `public static bool ProcessInfoTextFile(FileTypes infoFileType, FileInfo infoTextFile, CoreEntitiesContext ctx)`

6.  **Shared Method `ImportUtils.ProcessFileTypeActionsWorkflow`:**
    *   **Responsibility:** Handles AsycudaDocumentSet association (if needed, checking `fileType.Data` first) and runs Data/Non-Specific Actions for a *non-Info.txt* `FileType`. Relies on caller preparing context in `FileType.Data`.
    *   **Signature:** `public static async Task<bool> ProcessFileTypeActionsWorkflow(FileTypes fileType, FileInfo[] matchingFiles, string referenceNumber, ApplicationSettings appSetting, CoreEntitiesContext ctx)`

7.  **Refactor `Program.cs`:**
    *   Remove email processing logic.
    *   Instantiate `EmailProcessor` and `ShipmentFolderProcessor`.
    *   Call `emailProcessor.ProcessEmailsAsync(...)`.
    *   Call `shipmentFolderProcessor.ProcessShipmentFolders(...)`.

8.  **Post-Refactoring Testing:**
    *   Re-run the enhanced test suite (including the modified `ProcessShipmentFolders_GeneratesXml_Test`) against the refactored code to verify functionality is preserved.

**State Propagation:** The `shipmentContextData` dictionary in `ShipmentFolderProcessor` ensures context derived from `Info.txt` (like `AsycudaDocumentSetId`) is explicitly passed to the processing of subsequent file types within the same folder.