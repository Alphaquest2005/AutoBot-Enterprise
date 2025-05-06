# Decision Log

This file records architectural and implementation decisions...

*
[2025-05-05 19:05:48] - 🎨🎨🎨 ENTERING CREATIVE PHASE: Algorithm Design (Regex) 🎨🎨🎨

**Focus:** TEMU Invoice PDF Extraction Regex Patterns
**Objective:** Design robust .NET Regex patterns to identify TEMU invoices and extract key header fields from their text representation for use within the `InvoiceReader` framework.
**Requirements:** Extract Order ID, Order Date, Item(s) Total, Subtotal, Shipping, Sales Tax, Order Total. Provide an identification regex.

**PROBLEM STATEMENT**
The system needs to process TEMU PDF invoices. This requires defining regex patterns within the `OCR` database template to identify these invoices and extract relevant data fields from the text obtained via OCR/text ripping.

**OPTIONS ANALYSIS**

Based on the extracted text from `07252024_TEMU.pdf` (specifically the `SparseText` section):

### Option 1: Specific Label Matching Regex
*   **Description**: Use regex patterns anchored to the specific text labels found preceding the data (e.g., `Order ID:`, `Subtotal:`).
*   **Pros**:
    *   Relatively simple and clear.
    *   Targets specific, consistent labels found in the sample.
*   **Cons**:
    *   Sensitive to exact label text (e.g., requires matching the typo "Salos tax").
    *   Not suitable for complex, multi-line data like line item descriptions based on the sample text.
*   **Complexity**: Low (for headers)
*   **Implementation Time**: Low (for headers)

### Option 2: Positional/Generic Regex (Not Recommended)
*   **Description**: Attempt to extract data based on position or more generic patterns without relying on exact labels.
*   **Pros**: Might be more resilient to minor label changes (if structure is consistent).
*   **Cons**: Much less robust, prone to errors if layout changes, harder to develop and maintain.
*   **Complexity**: Medium-High
*   **Implementation Time**: Medium-High

### Option 3: Framework-Based Extraction (for Line Items)
*   **Description**: Utilize the existing `InvoiceReader` framework's `Part`/`Line`/`Field` structure defined in the `OCR` database to handle potentially multi-line or complex structures like line items, rather than relying solely on single regex patterns per field.
*   **Pros**: Designed to handle complex document structures, leverages existing system capabilities.
*   **Cons**: Requires configuration in the `OCR` database beyond simple regex (defining parts, lines, relationships).
*   **Complexity**: Medium (requires DB configuration)
*   **Implementation Time**: Medium

**DECISION**
*   **Selected Approach**: Use **Option 1 (Specific Label Matching Regex)** for header fields and invoice identification. Use **Option 3 (Framework-Based Extraction)** principles when configuring the `OCR` database template for line items.
*   **Rationale**: Option 1 provides reliable extraction for the clearly labeled header fields found in the sample text. The framework approach (Option 3) is necessary to handle the complexity of line item data which isn't easily captured by single regex patterns in the sample.

**IMPLEMENTATION GUIDELINES (Regex Patterns - Option 1)**
*   **Identification Regex** (to be used in `InvoiceIdentificatonRegEx` table, requires `RegexOptions.Singleline`):
    ```regex
    TEMU\s+@ As we're committed.*?Order Summary
    ```
*   **Header Field Extraction Regex** (to be used in `RegularExpressions` table and linked via `Fields`):
    *   `InvoiceID`: `Order ID:\s*(PO-\d{3}-\d{19})`
    *   `OrderDate`: `Order time:\s*(.+?)\n`
    *   `ItemsTotal`: `Item\(s\) total:\s*\$([\d,]+\.\d{2})`
    *   `Subtotal`: `Subtotal:\s*\$([\d,]+\.\d{2})`
    *   `Shipping`: `Shipping:\s*\$([\d,]+\.\d{2})`
    *   `SalesTax`: `Salos tax:\s*\$([\d,]+\.\d{2})`
    *   `OrderTotal`: `Order total:\s*\$([\d,]+\.\d{2})`
*   **Line Items**: Requires configuration of `Parts` and `Lines` within the `OCR` database template, potentially using simpler regex within those structures to capture individual item details. Detailed regex depends on analyzing the text structure within the item section more closely during template implementation.

**VERIFICATION**
*   These regex patterns are based on the single provided sample (`07252024_TEMU.pdf`).
*   Verification requires implementing these patterns in an `OCR` database template and running the `CanImportTemuInvoice_07252024_TEMU` test.
*   Refinement will be necessary during the IMPLEMENT phase based on test results and potentially other TEMU invoice samples.

🎨🎨🎨 EXITING CREATIVE PHASE - DECISION MADE 🎨🎨🎨

