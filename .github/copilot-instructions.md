You must follow 
docs/development/LEAD-DEVELOPMENT.md 
for all AI‑assisted changes.

<!--You must follow the development model defined in:

/docs/development/INCREMENTAL-DEVELOPMENT.md

Rules:
- Always respect the UDD → BDD → TDD order
- Never replace BDD tests with UI tests
- Treat BDD tests as business truth
- Treat TDD as implementation only
- Never change business rules during TDD
- If a request violates the model, explain and refuse

Development guidance is strict black-and-white with no optional steps.

Example GitHub Copilot Chat propmts:
- Based on our AI-Driven Development model,
add a BDD RED test for the next business workstep.
- We are in the inner TDD loop of the current BDD workstep.
Add one failing unit test only.


---------------
- According to our AI-Driven Development model, add a BDD RED test for the next business workstep:<br>
  No funds are reserved when a trade is rejected.
No UI tests
- We are in the inner TDD loop for the current BDD workstep.<br>
Add one failing unit test for the smallest missing component.
Do not implement code.
- Make the minimal change to pass the failing unit test.
No refactoring.
- This business workstep is UI-visible.
Add a Playwright test verifying the user sees the rejection message.
Do not assert business state.-->