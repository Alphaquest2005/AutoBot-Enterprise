# Plan: SikuliX Enhancement (Debugging, Git, Testing)

**Overall Goal:** Enhance the SikuliX automation process with screenshot debugging, ensure scripts are version controlled using Git, and create robust integration tests for key scripts, starting with `SaveIM7`.

---

## Phase 1: Preparation & Version Control

*   [ ] **Task 1:** Navigate to the script directory: `D:\OneDrive\Clients\AutoBot\Scripts (2)`.
*   [ ] **Task 2:** Initialize Git repository: Run `git init` in the script directory.
*   [ ] **Task 3:** Stage all existing files: Run `git add .`.
*   [ ] **Task 4:** Create initial commit: Run `git commit -m "Initial commit of SikuliX scripts"`.

*   **Notes/Learnings (Phase 1):**
    *   _Add notes here as tasks are completed or issues arise._

---

## Phase 2: Screenshot Debugging Implementation

**Focus Scripts:** `SaveIM7`, `C71`, `AssessC71`, `License`, `AssessLicense`

*   [ ] **Task 5:** Create `debug_utils.py` in the script directory (`D:\OneDrive\Clients\AutoBot\Scripts (2)`) with the `capture_debug_screenshot` function (handles directory creation, timestamping, conditional execution based on flag/argument).
*   [ ] **Task 6:** Modify `SaveIM7.sikuli/*.py`: Import `debug_utils` and add conditional calls to `capture_debug_screenshot` at key action points.
*   [ ] **Task 7:** Modify `C71.sikuli/*.py`: Import `debug_utils` and add conditional calls.
*   [ ] **Task 8:** Modify `AssessC71.sikuli/*.py`: Import `debug_utils` and add conditional calls.
*   [ ] **Task 9:** Modify `License.sikuli/*.py`: Import `debug_utils` and add conditional calls.
*   [ ] **Task 10:** Modify `AssessLicense.sikuli/*.py`: Import `debug_utils` and add conditional calls.
*   [ ] **Task 11:** Modify C# `RunSiKuLi` Method(s) (in `AutoBot/Utils.cs` and/or `AutoBot/Services/SikuliAutomationService.cs`) to accept an `enableDebugging` flag and pass a corresponding argument (e.g., `--debug-screenshots`) to the SikuliX process.
*   [ ] **Task 12:** Commit debugging changes: Stage `debug_utils.py`, modified `.py` files, and C# changes. Commit with message like `feat: Add screenshot debugging capability`.

*   **Notes/Learnings (Phase 2):**
    *   _Document decisions on specific action points for screenshots._
    *   _Note any challenges integrating the debug flag._

---

## Phase 3: Integration Testing (Focus: `SaveIM7`)

*   [ ] **Task 13:** Confirm Test Project (`AutoBotUtilities.Tests`) and Framework (NUnit). (Done during planning).
*   [ ] **Task 14:** Design `SaveIM7` Test:
    *   Objective: Verify `SaveIM7.sikuli` saves XML in Asycuda GUI.
    *   Prerequisites: Running Asycuda, valid input XML, necessary DB state.
    *   Execution: Trigger C# code path for "SaveIM7".
    *   Verification: Check Asycuda GUI (potentially using SikuliX within test), DB records, logs.
    *   Environment: Requires active desktop session.
*   [ ] **Task 15:** Implement `SaveIM7` Test: Create/update NUnit test class/method in `AutoBotUtilities.Tests`.
*   [ ] **Task 16:** Run & Refine `SaveIM7` Test: Execute, debug, and adjust the test until reliable.
*   [ ] **Task 17:** Commit `SaveIM7` Test Code: Add and commit the new test files/changes.

*   **Notes/Learnings (Phase 3):**
    *   _Detail the specific C# trigger method identified._
    *   _Document verification strategies used (GUI check, DB check, etc.)._
    *   _Note any environment setup required for the test._

---

## Phase 4: Extend Testing (Future)

*   [ ] **Task 18:** Design, Implement, Refine, and Commit `C71` Integration Test.
*   [ ] **Task 19:** Design, Implement, Refine, and Commit `AssessC71` Integration Test.
*   [ ] **Task 20:** Design, Implement, Refine, and Commit `License` Integration Test.
*   [ ] **Task 21:** Design, Implement, Refine, and Commit `AssessLicense` Integration Test.

*   **Notes/Learnings (Phase 4):**
    *   _Note similarities or differences in testing these scripts compared to SaveIM7._

---

## General Notes/Learnings

*   _Record any overarching observations or decisions made during the process._