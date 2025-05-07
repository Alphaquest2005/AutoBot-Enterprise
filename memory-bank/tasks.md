# Tasks
[2025-05-03 16:47:47] - ## Task: Setup Advanced Memory Bank

**Complexity Level:** 2 (Simple Enhancement)

**Description:** Setup the memory bank from https://github.com/enescingoz/roo-advanced-memory-bank. Involves cloning, configuring modes, and initializing the VAN workflow.

[2025-05-03 16:53:31] - ## Implementation Plan (Level 2)
- [x] Clone `advanced-memory-bank` repository.
- [x] Move `.roomodes` to workspace root.
- [x] Update paths within `.roomodes`.
- [x] Switch to VAN mode.
- [x] Run VAN initialization (create `tasks.md`, determine complexity).
- [x] Switch to PLAN mode.
- [x] Perform Technology Validation (verify modes load, essential files exist).
- [x] Update `tasks.md` with final status.
- [x] Verify plan completeness.

## Technology Validation
- **Stack:** Roo Code custom modes (`.roomodes`), rule files (`advanced-memory-bank/.roo/rules/`).
- **Validation:**
  - [x] Custom modes (VAN, PLAN) load successfully.
  - [x] Essential files (`.roomodes`, `tasks.md`) exist in correct locations.
  - [x] Core rule files appear present (though specific Level 2 files were missing).
  - [x] Technology validation complete.

## Status
- [x] Initialization complete (VAN)
- [x] Planning complete (PLAN)
- [x] Technology validation complete (PLAN)
- [x] Implementation complete (Setup task finished)

## Challenges & Mitigations
- **Challenge:** Missing Level 2 rule files (`enhancement-planning.mdc`, `task-tracking-basic.mdc`).
  - **Mitigation:** Followed general Level 2 steps from `plan-mode-map.mdc` and adapted.
- **Challenge:** `.roomodes` file initially in subdirectory.
  - **Mitigation:** Moved file to root and updated internal paths.

[2025-05-05 08:49:31] - ## New Task: Comprehensive Codebase Analysis (autobot1/autobot)

**Request:** Execute a comprehensive analysis of the `autobot1\autobot` project codebase. The analysis must encompass the primary project and identify/include all directly related sub-projects, libraries, and data stores referenced within its configuration files or build scripts. Thoroughly examine the project's architecture, key functionalities, component interactions, data processing pipelines, dependencies (internal and external), and technology stack. Synthesize these findings into a structured summary document suitable for a knowledge base. This summary must explicitly detail: high-level architecture, core modules/components and their responsibilities, key data structures/schemas, critical execution flows/algorithms, mapping of functionalities to code locations, a list of identified technologies/frameworks/libraries with versions, location/purpose of configuration files, and external service dependencies. This summary will provide actionable context to enhance logging systems with component information, validate file/resource path accuracy, maintain an up-to-date technology inventory, improve project documentation, and refactor the project to follow SOLID principles.

[2025-05-05 13:31:30] - ## Task Update: Scope Broadened to Full Workspace Analysis

**Reason:** Initial target directory `autobot1/autobot` not found. User requested a full workspace scan (file listing and definition summary) to build comprehensive context before proceeding with detailed analysis and refactoring.

[2025-05-05 16:54:48] - ## New Task: Create TEMU Invoice Import SQL Entries

**Request:** Based on the memory bank, create a plan for database entries required to import TEMU invoices into the `WebSource-AutoBot` database. Analyze existing invoice import mechanisms (e.g., Amazon) and the database schema (`Actions`, `FileTypes`, `EmailInfoMappings`, `InfoMapping`, `InfoMappingRegEx`, etc.) to determine the necessary SQL `INSERT` statements. Use `AutoBotUtilities.Tests\PDFUtilsTests.cs::CanImportTemuInvoice_07252024_TEMU()` for verification. Add learnings to the memory bank.

**Complexity Level:** 3 (Moderate Complexity)

[2025-05-05 17:06:09] - ## Implementation Plan: TEMU Invoice Import (Level 3)

**Requirements Analysis:**
- Core Requirements:
  - [ ] Create a mechanism to import TEMU PDF invoices.
  - [ ] Extract key header fields (e.g., InvoiceNo, Date, Total) and line item details (e.g., Item Description, Quantity, Price) from TEMU PDFs.
  - [ ] Store extracted data in the `WebSource-AutoBot` database (`ShipmentInvoice`, `ShipmentInvoiceDetails` tables).
  - [ ] Utilize the existing `InvoiceReader` framework by defining a new OCR template in the `OCR` database.
  - [ ] Ensure import works for sample TEMU PDFs from tests.
  - [ ] Verify results using `CanImportTemuInvoice_07252024_TEMU` test.
- Technical Constraints:
  - [ ] Must integrate with the existing `PDFUtils` -> `InvoiceReader` flow.
  - [ ] Requires access to and ability to modify the `OCR` database (separate from `WebSource-AutoBot`).

**Component Analysis:**
- **`OCR` Database:**
  - Changes needed: Add new rows to `Invoices`, `Parts`, `Lines`, `Fields`, `RegularExpressions`, `InvoiceIdentificatonRegEx` tables for TEMU template.
  - Dependencies: Schema understanding, example regex from other templates.
- **`WebSource-AutoBot` Database:**
  - Changes needed: Verify `FileType` ID 1147 configuration. No schema changes expected.
  - Dependencies: `FileType` 1147 exists and triggers the correct action.
- **`InvoiceReader` Code (`WaterNut.Business.Services/.../InvoiceReader.cs`):**
  - Changes needed: None anticipated initially.
  - Dependencies: `PdfPig`, Tesseract (`PdfOcr`).
- **`PDFUtils` Code (`AutoBot/PDFUtils.cs`):**
  - Changes needed: None anticipated.
  - Dependencies: Calls `InvoiceReader`.
- **`AutoBotUtilities.Tests` Code (`AutoBotUtilities.Tests/PDFUtilsTests.cs`):**
  - Changes needed: Update `// TODO:` assertions with correct expected values.
  - Dependencies: Test PDF files accessible.

**Architecture Considerations:**
- Leverages existing `InvoiceReader` framework and OCR database for template-driven extraction.
- No significant architectural changes required.

**Implementation Strategy:**
1.  **Phase 1: Analysis & Template Design**
    - [ ] Analyze text structure of sample TEMU PDFs.
    - [ ] Identify key fields and patterns.
    - [ ] Design OCR template structure (Parts, Lines, Fields).
    - [ ] Draft initial regex patterns (identification and extraction).
2.  **Phase 2: Database Implementation (OCR DB)**
    - [ ] Write SQL `INSERT` statements for the TEMU template.
    - [ ] Execute SQL to add the template.
3.  **Phase 3: Testing & Refinement**
    - [ ] Run `CanImportTemuInvoice_07252024_TEMU` test.
    - [ ] Debug and refine regex in `OCR` database based on results.
    - [ ] Repeat until test passes with correct data.
4.  **Phase 4: Final Verification**
    - [ ] Update assertions in `PDFUtilsTests.cs`.
    - [ ] Run all TEMU tests.
    - [ ] Document template in memory bank.

**Detailed Steps:** (Covered within Implementation Strategy Phases)

**Dependencies:**
- Access to `OCR` database (schema and data modification).
- Access to sample TEMU PDF files used in tests.
- Understanding of the `InvoiceReader` framework and `OCR` DB schema (potentially requiring examination of existing templates).

**Challenges & Mitigations:**
- **Challenge:** Complex/Inconsistent TEMU PDF structure.
  - **Mitigation:** Detailed analysis, specific regex anchors. Escalate if regex proves insufficient (potential code change/rely on fallback AI).
- **Challenge:** `OCR` database access/permissions.
  - **Mitigation:** Confirm access during implementation phase. Task blocked if unavailable.
- **Challenge:** Understanding `OCR` DB schema for template creation.
  - **Mitigation:** Query existing templates in `OCR` DB during implementation.
- **Challenge:** Test PDF file accessibility.
  - **Mitigation:** Ensure files are accessible or copy to workspace during implementation.

**Creative Phase Components:**
- [X] ⚙️ Algorithm Design (Specifically, Regex pattern design for TEMU PDF structure)

[2025-05-07 17:10:57] - ## Task: Debug NullReferenceException in EmailShipment

**ID:** TASK_VAN_001
**Status:** Fix Implemented (Testing Pending)
**Priority:** High
**Assigned Mode:** IMPLEMENT
**Description:** A `NullReferenceException` occurs in the `EmailShipment` method in `AutoBot/ShipmentUtils.cs` at line 152. The error is `Value cannot be null. Parameter name: source`, originating from `System.Linq.Enumerable.Sum` when called on `shipment.Invoices`. Suspected cause is `shipment.Invoices` being null.
**Files Involved:**
- `AutoBot/ShipmentUtils.cs`
**Acceptance Criteria:**
- The `EmailShipment` method no longer throws a `NullReferenceException` when `shipment.Invoices` is null.
- The logic correctly handles cases where `shipment.Invoices` might be null or empty.
**Complexity Assessment (VAN):** Level 1 (Direct fix anticipated)
**Implementation Notes:**
- Applied fix to `AutoBot/ShipmentUtils.cs` at line 152.
- Changed `shipment.Invoices.Sum(x => x.TotalsZero)` to `(shipment.Invoices?.Sum(x => x.TotalsZero) ?? 0)`.
- This ensures that if `shipment.Invoices` is null, the expression evaluates to `0`, preventing the `NullReferenceException`.
**Next Step:** Manual testing or integration testing is required to verify the fix in a runtime environment. Transition to REFLECT mode.
