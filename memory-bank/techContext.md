
## Build & Test Procedures

[2025-05-03 20:03:22] - ## AutoBot-Enterprise Build & Test Instructions (from BUILD_INSTRUCTIONS.md)

**Build Specific Project (Debug, x64):**
```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "<ProjectFile>.csproj" /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
```

**Run Tests (vstest.console):**
```powershell
# Ensure test project is built first
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\<TestProjectFolder>\bin\x64\Debug\net48\<TestProjectName>.dll" /TestCaseFilter:"<Filter>" "/Logger:console;verbosity=detailed"
```

**Common Issues:**
*   **RuntimeIdentifier Errors:** Add `<RuntimeIdentifiers>win;win-x64</RuntimeIdentifiers>` to `.csproj` and rebuild.
*   **Architecture Mismatch (MSB3270):** Ensure all projects target x64.
*   **NuGet Restore/Downgrade (NU1605):** Check sources, resolve version conflicts in `.csproj` files, re-run `Clean,Restore,Rebuild`.

*(Refer to BUILD_INSTRUCTIONS.md for full details and prerequisites.)*
[2025-05-04 08:33:54] - 
**Rule:** Always clean, restore, and rebuild the relevant test project using MSBuild before running tests with vstest.console.exe to ensure the latest changes are included.

## NuGet Package Management

[2025-05-04 14:15:11] - All projects in this solution utilize `<PackageReference>` within their `.csproj` files for managing NuGet dependencies. `packages.config` files are not used.

[2025-05-05 07:12:19] - ## Data Structures/Schemas (autobot1/autobot Analysis)

- **Database Schema:** Primarily defined by Entity Framework models within `CoreEntities.Business.Entities` and `WaterNut.DataSpace`. Key entities observed or inferred include:
    - `Actions`: Represents tasks to be executed.
    - `FileTypes`: Defines configurations for processing specific file types, including associated actions (`FileTypeActions`) and email mappings (`EmailInfoMappings`).
    - `EmailMapping`: Defines configurations for processing emails, including associated actions (`EmailMappingActions`).
    - `FileTypeActions`: Links `FileTypes` to specific `Actions`, defining the workflow for a file type.
    - `EmailMappingActions`: Links `EmailMapping` to specific `Actions`, defining the workflow for an email mapping.
    - `EmailInfoMappings`: Defines how to extract data from emails/text using regex (`InfoMappingRegEx`).
    - `InfoMapping`: Specifies target entity (`EntityType`), field (`Field`), and key (`EntityKeyField`) for database updates based on extracted data.
    - `InfoMappingRegEx`: Contains regular expressions (`KeyRegX`, `FieldRx`, `KeyReplaceRx`, `FieldReplaceRx`) for data extraction.
    - Domain-specific entities related to customs processes (e.g., likely tables for Adjustments, Discrepancies, EX9, POs, C71, Licenses, Shipments, Sales Data, Entry Documents, etc., managed by the respective utility classes).
- **File Structures:** 
    - Input files are often CSV or text-based, processed by `EmailTextProcessor` based on regex defined in the database.
    - PDF files are processed (`DownloadPDFs`, `LinkPDFs`, `ImportPDF`, `ConvertPNG2PDF`).
    - XML is likely used for customs submissions (`SubmitSalesXmlToCustomsUtils`).
    - XLSX files are processed (`XLSXProcessor.cs`).
- **In-Memory Structures:**
    - `FileUtils.FileActions`: `Dictionary<string, Action<FileTypes, FileInfo[]>>` mapping action names to delegates.
    - `SessionsUtils.SessionActions`: `Dictionary<string, Action>` mapping session action names to delegates.
    - `FileTypes.Data`: `List<KeyValuePair<string, string>>` used temporarily by `EmailTextProcessor` to hold extracted data for a file.
    - `FileTypes.ProcessNextStep`: `List<string>` holding a sequence of action names to execute.

[2025-05-05 07:14:16] - ## Technology Stack (autobot1/autobot Analysis)

- **Language:** C#
- **Framework:** .NET Framework (Specific version not determined, but likely 4.x given WCF and Entity Framework usage)
- **Runtime:** .NET CLR
- **Host:** WCF Console Host (`WCFConsoleHost` project)
- **Data Access:** Entity Framework (likely EF 6, based on `DbContext` usage in `CoreEntitiesContext`)
- **Database:** SQL Server (Inferred from MCP server connection string: `MINIJOE\SQLDEVELOPER2022`, Database: `WebSource-AutoBot`)
- **Libraries (Observed/Inferred):**
    - `Core.Common.Converters` (Likely internal library)
    - `AdjustmentQS.Business.Services` (Likely internal library)
    - `EntryDataDS.Business.Entities` (Likely internal library)
    - `SalesDataQS.Business.Services` (Likely internal library)
    - `System.IO`
    - `System.Linq`
    - `System.Text.RegularExpressions`
    - `System.Threading.Tasks`
    - `System.Diagnostics`
- **Build/Dependencies:** Uses `packages.config` (implies NuGet package management, common in older .NET Framework projects).

## Configuration (autobot1/autobot Analysis)

- **Primary Configuration Source:** Database tables (`Actions`, `FileTypes`, `EmailMapping`, `FileTypeActions`, `EmailMappingActions`, `EmailInfoMappings`, `InfoMapping`, `InfoMappingRegEx`). These tables define the workflows, actions, regex patterns, and database targets.
- **Application Settings:** `ApplicationSettings` class (referenced in `ImportUtils`), likely populated from a database table or configuration file. Contains settings like `AssessIM7`, `AssessEX`, `TestMode`.
- **Connection Strings:** Likely stored in `App.config` within `WCFConsoleHost` and potentially `AutoBotUtilities` project (standard .NET Framework practice).
- **File Paths:** Input/output file paths might be configured in the database or potentially `App.config`, although specific locations weren't explicitly identified in the analyzed code.
- **Action Definitions:** Action names (strings) are keys in `SessionsUtils.SessionActions` and `FileUtils.FileActions` dictionaries, linking names to code implementations.

[2025-05-05 07:15:41] - ## Dependencies (autobot1/autobot Analysis)

- **Internal Project Dependencies:**
    - `WCFConsoleHost` depends on `AutoBotUtilities` (the `AutoBot` project).
    - `AutoBot` project likely depends on other internal libraries/projects within the solution (e.g., `CoreEntities.Business.Entities`, `WaterNut.DataSpace`, `Core.Common.Converters`, `AdjustmentQS.Business.Services`, `EntryDataDS.Business.Entities`, `SalesDataQS.Business.Services`).
- **External Service Dependencies:**
    - **Database:** SQL Server instance `MINIJOE\SQLDEVELOPER2022`, database `WebSource-AutoBot`.
    - **Email Server:** An external email server is required for `EmailDownloader` to send notifications.
    - **Customs Systems (Inferred):** Actions like `SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms` imply interaction with external customs authority systems (likely via APIs or specific file formats/protocols).
- **File System:** Relies heavily on the local file system for reading input files (CSV, text, PDF, XLSX, PNG) and writing output/log files.

[2025-05-06 19:44:46] - ## IMAP MCP Server (non-dirty/imap-mcp)

**Purpose:** Provides MCP access to an IMAP email account.
**Configured for:** `websource@auto-brokerage.com`
**Location:** `c:\Insight Software\AutoBot-Enterprise\imap-mcp`

**Key Configuration (`config.yaml`):**
- IMAP Host: `mail.auto-brokerage.com`
- IMAP Port: `993`
- Username: `websource@auto-brokerage.com`
- Authentication: Password (stored in `config.yaml`)
- SSL: `true`

**To Start the Server:**
1. Open a terminal.
2. Navigate to the directory: `cd c:\Insight Software\AutoBot-Enterprise\imap-mcp`
3. Run: `.venv\Scripts\Activate.ps1; C:\Users\josep\.local\bin\uv.exe run imap_mcp.server --config config.yaml`

**To Test (List Inbox Emails):**
1. Open a terminal.
2. Navigate to the directory: `cd c:\Insight Software\AutoBot-Enterprise\imap-mcp`
3. Run: `.venv\Scripts\Activate.ps1; C:\Users\josep\.local\bin\uv.exe run list_inbox.py --config config.yaml --folder INBOX --limit 10`

**Installation Notes:**
- Installed using `uv` package manager.
- Source cloned from `https://github.com/non-dirty/imap-mcp.git`.
- Dependencies managed in a virtual environment (`.venv`).

## Logging Framework

[2025-05-12 11:38:27] - The AutoBot project utilizes the Serilog library for logging, not NLog.

## Build & Test Process

[2025-05-12 12:18:22] - Always refer to and use the instructions specified in [`BUILD_INSTRUCTIONS.md`](BUILD_INSTRUCTIONS.md) when building or testing any part of the AutoBot-Enterprise project.
