# Project Structure Analysis

This document provides an overview of the project structure for the AutoBot-Enterprise solution, based on analysis performed during Iteration 7.

*   **Solution File:** `AutoBot-Enterprise.sln`

*   **Projects:**

    *   **AutoBotUtilities** (`AutoBot\AutoBotUtilities.csproj`): Contains core utility classes, including PDF processing (`PDFUtils.cs`), email processing (`EmailTextProcessor.cs`), and various other utilities.
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** x64
        *   **Key Package References:**
            *   EntityFramework (6.4.4)
            *   Serilog (2.12.0)
            *   Serilog.Sinks.Console (4.1.0)
            *   MoreLinq (2.10.21)
            *   TrackableEntities (2.5.7)
            *   Newtonsoft.Json (13.0.3)
            *   Microsoft.Office.Interop.Excel (15.0.4795.1000)
            *   NUnit (3.13.3)
            *   NUnit3TestAdapter (4.5.0)
            *   System.Runtime.InteropServices.WindowsRuntime (4.3.0)
            *   System.ValueTuple (4.5.0)
            *   Serilog.Context (5.0.0)
        *   **Project References:**
            *   `Core.Common\Core.Common.csproj`
            *   `CoreEntities\CoreEntities.csproj`
            *   `DocumentDS\DocumentDS.csproj`
            *   `WaterNut.Business.Services\WaterNut.Business.Services.csproj`
            *   `WaterNut.DataSpace\WaterNut.DataSpace.csproj`
            *   `pdf-ocr\pdf-ocr.csproj`
        *   **Source Files Analyzed (Relevant to Iteration 7):**
            *   `AutoBot\PDFUtils.cs` (Contains `ImportPDF`, `ImportPDFDeepSeek`, `ImportSuccessState`)
            *   `AutoBot\EmailTextProcessor.cs`
            *   `AutoBot\FileUtils.cs`
            *   `AutoBot\ImportUtils.cs`
            *   `AutoBot\Utils.cs`
            *   `AutoBot\EntryDocSetUtils.cs`
            *   `AutoBot\ShipmentUtils.cs`

    *   **Core.Common** (`Core.Common\Core.Common.csproj`): Contains common utilities and extensions.
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** AnyCPU
        *   **Key Package References:**
            *   EntityFramework (6.4.4)
            *   MoreLinq (2.10.21)
            *   TrackableEntities (2.5.7)
            *   Newtonsoft.Json (13.0.3)
            *   Serilog (2.12.0)
        *   **Project References:** None listed.
        *   **Source Files Analyzed:** (To be detailed as needed)

    *   **CoreEntities** (`CoreEntities\CoreEntities.csproj`): Contains Entity Framework context and business entities.
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** AnyCPU
        *   **Key Package References:**
            *   EntityFramework (6.4.4)
            *   TrackableEntities (2.5.7)
        *   **Project References:** None listed.
        *   **Source Files Analyzed:** (To be detailed as needed)

    *   **InvoiceReader** (`InvoiceReader\InvoiceReader.csproj`): Contains the core invoice reading logic, including the `InvoiceReader` class and the `PipelineInfrastructure`.
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** x64
        *   **Key Package References:**
            *   EntityFramework (6.4.4)
            *   Serilog (2.12.0)
            *   Serilog.Sinks.Console (4.1.0)
            *   MoreLinq (2.10.21)
            *   TrackableEntities (2.5.7)
            *   Newtonsoft.Json (13.0.3)
            *   System.Runtime.InteropServices.WindowsRuntime (4.3.0)
            *   System.ValueTuple (4.5.0)
        *   **Project References:**
            *   `Core.Common\Core.Common.csproj`
            *   `CoreEntities\CoreEntities.csproj`
            *   `DocumentDS\DocumentDS.csproj`
            *   `WaterNut.Business.Services\WaterNut.Business.Services.csproj`
            *   `WaterNut.DataSpace\WaterNut.DataSpace.csproj`
            *   `pdf-ocr\pdf-ocr.csproj`
        *   **Source Files Analyzed (Relevant to Iteration 7):**
            *   `InvoiceReader\InvoiceReader.cs` (Contains `Import`, `GetPdftxt`)
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/InvoiceProcessingPipeline.cs` (Partial class)
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/InvoiceProcessingContext.cs` (Contains `InvoiceProcessingContext` class)
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/PipelineRunner.cs` (Contains `PipelineRunner` class)
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/GetPdfTextStep.cs` (Partial class)
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/GetPdfTextStep.GetPdfSparseTextAsync.cs` (Partial class)
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/GetPdfTextStep.GetSingleColumnPdfText.cs` (Partial class)
        *   **Missing Files (Not Found):**
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/Steps/HandleErrorStateStep.cs` (Confirmed not found in .csproj or directory listing)
            *   `InvoiceReader/InvoiceReader/PipelineInfrastructure/Steps/UpdateImportStatusStep.cs` (Confirmed not found in .csproj or directory listing)
            *   *(Note: Other pipeline step implementations may exist and need to be identified and analyzed.)*

    *   **pdf-ocr** (`pdf-ocr\pdf-ocr.csproj`): Contains the PDF OCR functionality.
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** x64
        *   **Key Package References:**
            *   Tesseract (5.0.0-alpha20201128)
            *   Serilog (2.12.0)
            *   Serilog.Sinks.Console (4.1.0)
        *   **Project References:** None listed.
        *   **Source Files Analyzed:**
            *   `PdfOcr\PdfOcr.cs` (Contains `PdfOcr` class, `Ocr`, `GetTextFromImage`)
            *   `PdfOcr\Program.cs` (Contains `Main` method)

    *   **WaterNut.Business.Services** (`WaterNut.Business.Services\WaterNut.Business.Services.csproj`): Contains business services, including data processing logic.
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** AnyCPU
        *   **Key Package References:**
            *   EntityFramework (6.4.4)
            *   TrackableEntities (2.5.7)
            *   Serilog (2.12.0)
        *   **Project References:**
            *   `Core.Common\Core.Common.csproj`
            *   `CoreEntities\CoreEntities.csproj`
            *   `DocumentDS\DocumentDS.csproj`
            *   `WaterNut.DataSpace\WaterNut.DataSpace.csproj`
        *   **Source Files Analyzed (Relevant to Iteration 7):**
            *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\SaveCSV\DataFile.cs` (Contains `DataFile` class)
            *   `WaterNut.Business.Services\Custom Services\DataModels\Custom DataModels\SaveCSV\DataFileProcessor.cs` (Contains `Process` method)

    *   **WaterNut.Data** (`WaterNut.Data\WaterNut.Data.csproj`): Contains Entity Framework DbContexts, mappings, and data access utilities.
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** AnyCPU
        *   **Key Package References:**
            *   EntityFramework (6.5.1)
            *   Newtonsoft.Json (13.0.3)
            *   Serilog (4.2.0)
            *   Serilog.Enrichers.Context (4.6.5)
            *   Serilog.Enrichers.GlobalLogContext (3.0.0)
            *   Serilog.Extensions.Logging (9.0.1)
            *   System.ValueTuple (4.6.1)
            *   ValueInjecter (3.2.0)
        *   **Project References:**
            *   `DomainInterfaces\InterfacesModel.csproj`
            *   `WaterNut.Business.Entities\WaterNut.Business.Entities.csproj`
        *   **Source Files Analyzed (Relevant to Iteration 7):**
            *   `CustomDBContexts\AllocationDS.Context.cs`
            *   `CustomDBContexts\AllocationQS.Context.cs`
            *   `CustomDBContexts\CoreEntities.Context.cs`
            *   `CustomDBContexts\EntryDataQS.Context.cs`
            *   `CustomDBContexts\SalesDataQS.Context.cs`
            *   `DBConfiguration.cs`
            *   `DbContextExtentions.cs`
            *   `DBExecutionStrategy.cs`
            *   `DbSetExtensions\AsycudaDocumentSetExDTO.cs`
            *   `CustomDBContexts\DocumentItemDS.Context.cs`
            *   `ObjectContexts\AdjustmentQS.Context.cs`
            *   `ObjectContexts\AllocationDS.Context.cs`
            *   `ObjectContexts\AllocationQS.Context.cs`
            *   `ObjectContexts\CoreEntities.Context.cs`
            *   `ObjectContexts\CounterPointQS.Context.cs`
            *   `ObjectContexts\DocumentDS.Context.cs`
            *   `ObjectContexts\DocumentItemDS.Context.cs`
            *   `ObjectContexts\EntryDataDS.Context.cs`
            *   `ObjectContexts\EntryDataQS.Context.cs`
            *   `ObjectContexts\InventoryDS.Context.cs`
            *   `ObjectContexts\InventoryQS.Context.cs`
            *   `ObjectContexts\LicenseDS.Context.cs`
            *   `ObjectContexts\OCR.Context.cs`
            *   `ObjectContexts\PreviousDocumentDS.Context.cs`
            *   `ObjectContexts\PreviousDocumentQS.Context.cs`
            *   `ObjectContexts\QuickBooksDS.Context.cs`
            *   `ObjectContexts\SalesDataQS.Context.cs`
            *   `ObjectContexts\ValuationDS.Context.cs`
            *   `Properties\AssemblyInfo.cs`
            *(Note: DbContext Mappings files were also listed in the .csproj but are omitted here for brevity unless they become directly relevant.)*

    *   **WaterNut.DataSpace** (`WaterNut.Data\WaterNut.Data.csproj`): Contains data access logic and utilities. *(Note: This project path has been corrected based on file system analysis. The namespace WaterNut.DataSpace is used by classes within this project and potentially others.)*
        *   **Framework:** .NET Framework 4.8
        *   **Platform Target:** AnyCPU
        *   **Key Package References:**
            *   EntityFramework (6.4.4)
            *   TrackableEntities (2.5.7)
            *   Serilog (2.12.0)
        *   **Project References:**
            *   `Core.Common\Core.Common.csproj`
            *   `CoreEntities\CoreEntities.csproj`
            *   `DocumentDS\DocumentDS.csproj`
        *   **Source Files Analyzed (Relevant to Iteration 7):**
            *   `WaterNut.DataSpace\Utils.cs` (Contains `GetDocSets`)
            *   `WaterNut.DataSpace\BaseDataModel.cs`
            *   `WaterNut.DataSpace\CoreEntitiesContext.cs` (Entity Framework DbContext)
            *   `WaterNut.DataSpace\EntryDataDSContext.cs` (Entity Framework DbContext)
            *(Note: These files were listed in the previous ProjectStructure.md under the incorrect path. Their actual location and content need to be verified if they become relevant to the objective.)*


*   **Missing Projects (Not Found in .sln):** None identified so far.

*   **Potential Areas for Further Detailing:**
    *   Specific pipeline step implementations within `InvoiceReader\InvoiceReader\PipelineInfrastructure`.
    *   Database interaction logic within `WaterNut.Data` (previously listed under `WaterNut.DataSpace`).
    *   DeepSeekInvoiceApi implementation details.
    *   Verification of source file locations listed under the corrected `WaterNut.DataSpace` entry.