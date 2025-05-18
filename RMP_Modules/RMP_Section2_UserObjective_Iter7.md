## 2. User Objective Refinement (Iteration 7)

**Initial User Objective:** Fix the failing NUnit test `CanImportAmazoncomOrder11291264431163432()` in `AutoBotUtilities.Tests\PDFImportTests.cs`.

**Refined Objective:** Implement structured Serilog logging within the `InvoiceReader` project (focusing on `InvoiceProcessingPipeline`, `PDFUtils`, `DataFileProcessor`) to diagnose the root cause of the test failure and enable its correction. Establish the LTM/STM framework featuring:
    1.  LTM as a directory (`LTM/`) of immutable Markdown files, each representing a specific learning concept.
    2.  STM (Section 10) as an index of immutable entries, providing "seeds" for deterministic LTM filename construction.
    3.  New LTM/STM entries are created for new learning, versioned, and inter-linked; existing entries are never modified.
    4.  Reactive LTM consultation triggered by internal logging (`META_LOG_DIRECTIVE`s) to continuously inform the process, in addition to deliberate LTM consultation during planning and failure analysis.
    5.  Integration of comprehensive task history analysis into the reflection phase for deeper learning and LTM population.

**Priority:** High - This is the primary focus of the current iteration.