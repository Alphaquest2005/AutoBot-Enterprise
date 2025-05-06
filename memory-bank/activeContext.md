# Active Context

This file tracks the project's current status...

*
## Debugging and Investigation Logs

[2025-05-03 16:58:53] - ## Debugging ShipmentInvoice and PDFUtilsTests (Task Continuation: 2025-05-03 12:58 PM)

**Objective:** Configure TEMU PDF invoice import (DB OCR rules + AutoBotUtilities.Tests tests) ensuring `TotalsZero == 0`.

**Investigation Path & Findings:**
1.  Started with `AutoBotUtilities.Tests\PDFUtilsTests.cs`.
2.  Encountered compiler errors in new TEMU tests related to `EntryDataDS.Business.Entities.ShipmentInvoice` properties (`InvoiceNumber`, `Freight`, `Tax`, `TotalsZero`) and missing `InvoiceReader.GetPdftxt` method.
3.  Investigated `EntryDataDS` and `CoreEntities` projects; found T4 templates but seemingly empty generated `.cs` files.
4.  Analyzed project references (`AutoBotUtilities.Tests.csproj`, `AutoBotUtilities.csproj`) identifying dependencies on `WaterNut.*` projects.
5.  Located the likely active `ShipmentInvoice.cs` definition within `WaterNut.Business.Entities\Generated Business Entities\EntryDataDS\`.
6.  Read `ShipmentInvoice.cs`:
    *   Confirmed `InvoiceNo` property exists (corrected test code).
    *   Confirmed `Freight` and `Tax` properties do *not* exist directly (removed corresponding assertions from test code).
    *   Confirmed `TotalsZero` property is *not* defined in this file.
7.  Identified `ShipmentInvoice` inherits from `Core.Common.Business.Entities.BaseEntity<ShipmentInvoice>`.
8.  Searched for `BaseEntity` definition:
    *   Checked `Core.Common\Core.Common\BusinessObjectBase.cs` (incorrect namespace/definition).
    *   Checked `Core.Common\Core.Common.Contracts` (only service interfaces).
    *   Checked `Core.Common\Core.Common.Data` and found multiple potential `BaseEntity.cs` files.

**Current Status:**
*   Need to identify and read the correct `BaseEntity.cs` (likely `Core.Common\Core.Common.Data\Business.Entities\BaseEntity.cs`) to find the `TotalsZero` property definition.
*   Still need to resolve the `InvoiceReader.GetPdftxt` method error.
*   Database configuration for TEMU OCR rules has not yet started.
[2025-05-03 17:02:52] - **Update (2025-05-03 1:01 PM):**
*   Located the `TotalsZero` property definition in the partial class `WaterNut.Business.Entities\Custom Entities\EntryDataDS\ShipmentInvoice.cs`. It is a calculated property verifying header/detail total consistency.
*   Next step: Resolve the `InvoiceReader.GetPdftxt` compiler error.
[2025-05-03 17:05:47] - **Update (2025-05-03 1:04 PM):**
*   Investigated `InvoiceReader\InvoiceReader\PipelineInfrastructure\GetPdfTextStep.cs`.
*   Confirmed the PDF text extraction logic is part of this pipeline step class (`WaterNut.DataSpace.PipelineInfrastructure.GetPdfTextStep`) and requires an `InvoiceProcessingContext`.
*   There is no static `InvoiceReader.GetPdftxt` method as used in the test code.
*   Hypothesis: The test code should be calling a method within the `PDFUtils` class itself.
*   Next step: Examine `AutoBot\AutoBotUtilities\PDFUtils.cs` to find the correct text extraction method.
[2025-05-03 17:08:56] - **Update (2025-05-03 1:08 PM):**
*   Examined `AutoBot\PDFUtils.cs`.
*   Found the method call `InvoiceReader.InvoiceReader.GetPdftxt()` within the `ImportPDFDeepSeek` method (line 302).
*   This confirms the method exists but is part of the `InvoiceReader` class within the `InvoiceReader` namespace.
*   The test code in `PDFUtilsTests.cs` is missing the class name in the call.
*   Next step: Correct the method call in `PDFUtilsTests.cs`.
[2025-05-03 17:50:55] - **Correction (2025-05-03 1:50 PM):**
*   User clarified that `ApplicationSettingsId=3` is the only valid setting.
*   Therefore, the existing `OCR-Invoices` record with `Id=164` (linked to `ApplicationSettingsId=3`) is the correct one to use for TEMU configuration.
*   The record I created (`Id=168` with `ApplicationSettingsId=1`) is incorrect and needs to be deleted.

[2025-05-05 13:33:23] - ## Code Definition Analysis: AutoBot/ Directory

Scanned `AutoBot/` directory using `list_code_definition_names`. Key findings:

*   **Customs Forms:** Contains classes like `C71Utils`, `EX9Utils`, `LICUtils`, `POUtils`, `DISUtils` for handling specific customs documents (import, create, assess, submit, download, error handling).
*   **Sales/Adjustments:** Includes `AllocateSalesUtils`, `EmailSalesErrorsUtils`, `Ex9SalesReportUtils`, `SalesUtils`, `SubmitSalesToCustomsUtils`, `SubmitSalesXmlToCustomsUtils` for sales allocation, reporting, error management, and submission.
*   **Shipment Processing:** `ShipmentExtensions` and `ShipmentUtils` provide extensive logic for loading shipment data (POs, Invoices, Riders, Manifests, Freight, BLs) from email/DB, processing complex shipment scenarios, managing attachments, handling unclassified items/suppliers, and import/export.
*   **PDF/File Handling:** `PDFCreator`, `PDFUtils`, `FileUtils`, `ImportUtils`, `XLSXProcessor` offer utilities for PDF creation/processing/linking/import (mentions DeepSeek, suggesting OCR), CSV/XLSX handling, and general file operations.
*   **Core Logic:** A large, partial `Utils` class seems central, defining numerous actions (`SessionActions`, `FileActions`) and orchestrating workflows related to the above areas.
*   **Error Management:** Specific classes like `ImportWarehouseErrors`, `SubmitDiscrepancyErrorReport` focus on error handling and reporting.

This directory appears crucial for the application's core business logic related to customs, sales, and shipment processing.
