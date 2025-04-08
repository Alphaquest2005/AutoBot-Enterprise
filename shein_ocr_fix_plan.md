# Shein OCR Template Bug Fix Plan

## Task

Fix a bug with the "Shein" OCR_Invoice template (ID 163, FileTypeId 1147) where `invoicedetails` are only created for the first invoice instance in a multi-invoice PDF. The issue stems from the template having a recurring header part and a non-recurring details part.

## Analysis Summary

1.  **Template Location:** OCR templates are defined as data in database tables managed by `OCRContext` (Schema in `AAll-EDMX/WaterNutDB.edmx`). Key tables: `OCR-Invoices`, `OCR-Parts`, `OCR-RecuringPart`, `OCR-ChildPart`, `OCR-Lines`, `OCR-Fields`, `OCR-RegularExpressions`.
2.  **Processing Code:** `WaterNut.DataSpace.Invoice` class (`Invoice.cs`) loads templates and parses PDF text via its `Read()` and `SetPartLineValues` methods.
3.  **Shein Template Structure (ID 163):**
    *   **Header (Part ID 2410):** Defined as **Recurring**. Parent part. Uses Regex ID `2334` for start. Contains lines for header fields (InvoiceNo, Date, Totals, etc.).
    *   **Details (Part ID 2412):** Defined as **Non-Recurring**. Child of Part 2410. Uses Regex ID `2335` for start and `2336` for end. Contains Line ID `2101` (using Regex ID `2342`) for detail fields (Quantity, ItemNumber, Description).
4.  **Root Cause:** The `Invoice.SetPartLineValues` method, when processing the non-recurring child (Details) of the recurring parent (Header), incorrectly uses `FirstOrDefault()` on the results obtained by processing the *entire* document text within the child's start/end markers. It fails to scope the child part processing to the specific text segment of the *current* parent instance. This results in only the details from the first header instance being captured repeatedly.

## Proposed Solution

Modify the C# code (likely within `Part.Read` or how `Invoice.SetPartLineValues` invokes child processing) to correctly scope the processing of a non-recurring child part to the text segment belonging *only* to the current instance of its recurring parent part.

**Conceptual Steps:**

1.  When a recurring parent part instance starts, determine its text boundaries (e.g., from its start marker to the next start marker or end of document/section).
2.  When processing the non-recurring child part for that parent instance, pass *only* the parent's specific text segment to the child's processing logic.
3.  Ensure the child's regex matching operates solely within this limited text segment.

## Plan Visualization (Mermaid)

```mermaid
graph TD
    A[Start Processing Shein Invoice] --> B{Invoice.Read(FullText)};
    B --> C{Identify All Parts (Header=2410, Details=2412)};
    C --> D{Process Header Part (2410)};
    D -- Recurring=True --> E{Loop through Header Instances};
    E --> F[Identify Text Segment for Current Header Instance];

    subgraph Current Flawed Logic
        F --> G1[Process Details Part (2412)];
        G1 -- NonRecurring=True --> H1{Read Details from FullText (using Details Regex)};
        H1 --> I1{Take FirstOrDefault() Match};
        I1 --> J[Add Details to Result (Only First Ever Found)];
    end

    subgraph Proposed Corrected Logic
        F --> G2[Process Details Part (2412)];
        G2 -- NonRecurring=True --> H2{Read Details from Current Header Instance Text Segment (using Details Regex)};
        H2 --> I2{Find Match within Segment};
        I2 --> J;
    end

    E -- Next Instance --> F;
    E -- No More Instances --> K[Assemble Final Result];

    style G1 fill:#f9f,stroke:#333,stroke-width:2px
    style H1 fill:#f9f,stroke:#333,stroke-width:2px
    style I1 fill:#f9f,stroke:#333,stroke-width:2px

    style G2 fill:#9cf,stroke:#333,stroke-width:2px
    style H2 fill:#9cf,stroke:#333,stroke-width:2px
    style I2 fill:#9cf,stroke:#333,stroke-width:2px
```

## Next Step

Switch to "Code" mode to implement the fix in the relevant C# classes (`Invoice.cs` / `Part.cs`).