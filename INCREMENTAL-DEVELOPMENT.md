# Incremental Development 

This is the primary development instruction, <br>
for building well architected solution for real business need <br>
in form of small, iterative, incremental and verified work steps.

---

## 1. The Three Nested Development Loops

Development proceeds in three strictly ordered, nested loops:

- User action loop
- Business scope loop 
- Solution scope loop

---

## 2. Init User Action (IUA)

This step need only in case of business need a new user action <br>
Based on user story suggest the
- Name of Application
- Name of Application User
- Name of Business Expert
- Name of Business Workflow

RED: 
- Create an UserAction PlayWright UI test which 
  - placed at `Tets\{BusinessExpertName}\{WorkflowName}\UserAction.Tests.cs Do_Nothing`
  - verifies nothing, but the user action triggers an empty workflow.
  - It needs no visible input fields nor output fields yet
- Verify if test really fails: `dotnet test Tests.csproj --filter "FullyQualifiedName=Tests.Trader.PlaceTrade.PlaceTrade_UserAction"`
- Close the pahase: `git commit -am "user action red: init"` 

GREEN
- Create a Blazor UI, which makes init UI test passed
  - place at `Expert\{BusinessExpertName}\{WorkflowName}\UserAction.\{ApplicationName}.cs`  
- Announce: `git commit -m "Iuser action green" - a`

REFACTOR
- inmprove name if needed
- once the test is green again: `git commit -m "user action blue: init" - a`

## 3. Init Business Workflow (IBW)

This step need only in case of a new Business Action is introduced 

RED
- Create an init Business Workflow test which
    - has an input request without fields
    - has an output response without fields
	- has no business work steps
	
GREEN
- Create the workk flow which makes the workflow test pass 

REFACTOR
- inmprove name if needed

## 2. User Action Loop

User Action Loop Artifact:
 - Production ready Application
 - Executable Application User Manual in form of UI tests or Integration tests.

Open the User Action loops when:
- Business need to create, update or delete a user action and
- Solution scope loop is closed
- Business scope loop is closed and

Close the loops when:
- No more business need for the user action is identified.

Identify the current state of the User Action:
- at the very first loop, we can know only what the user story implicates
    - what is the application
    - where is the application segment to use(page, screen ...)
    - who is the application user
	- what is the user activity (visit a page, submit ...)
	- which business workflow is triggered by the user action

- - what visible user inputs are required (fields)
- what visible action outputs are expected (fields, messages, navigation ...)

Shrink the user action
- Shrink original user action that still delivers value to the user.
- It should be small enough to be implemented, tested and delivered in max 3 hours.

Start the loop with the shrinked user action.

RED phase:
- Define the shrinked User Action with a UI test that fails, becuse of the missing implementation.
- This UI test must verify the visible behaviour of the user action.
- This UI test must verify if the business workflow is triggered.
- This UI test must not verify business workflow behavior.
- This UI test must not verify Solution behavior.
- This UI test will be the Executable Application User Manual
- Prefer to use Playwright.
- Also acceptable: Cypress, Selenium, ...
- So this UI test addss a verified increment to the previous delivered user action.
- Close the phase with: git commit -m "R-UA-001: <short description>" -a

GREEN phase:
- Once the Business Scope loops
- This loop has no own GREEN phase, instead of the inner Business scope loop and Solution scope loop
  provide the needed implementation to make the UI test pass.

REFACTOR phase:
- Improve the UI without changing the behaviour.
- Improve the visual appearance.
- Improve the user experience.
- Close the phase with: git commit -m "B-UA-001: <short description>" -a

---

## 3. Business Scope Loop

Business Scope Loop Artifact:
 - Executable Business Workflow Specification in form of BDD tests.

Open the Business Scope loops when:
- User Action need to create, update or delete a Business Workflow and
- Solution scope loop is closed

Close the loops when:
- No more business work step for the business workflow is identified.

Recap knowlage about the Workflow:
- at the very first loop, we can only
	- Identify which application user needs the business workflow
	- Identify which business expert holds the business workflow
	- Identify what request needs to trigger the business workflow (it comes from the user action input)
	- Identify what is the name of the business workflow (it should reflect the user action intent)
	- Identify what response will the business workflow provide (it feeds the user action output)
    
- Identify business work steps of the business workflow
- Identify the orchestration of business work steps

Shrink the Business Scope
- Shrink original Business Scope that still delivers value to the user.
- It should be small enough to be implemented, tested and delivered in max 3 hours.

Start the loop with the shrinked Business Scope.

RED phase:
- Define the shrinked Business Scope with a UI test that fails, becuse of the missing implementation.
- This UI test must verify only the visible behaviour of the Business Scope.
- This UI test must not verify business workflow or solution details.
- This UI test will be the Executable Application User Manual
- Prefer to use Playwright.
- Also acceptable: Cypress, Selenium, ...
- So this UI test addss a verified increment to the previous delivered user action.
- Close the phase with: git commit -m "R-UA-001: <short description>" -a

GREEN phase:
- Once the Business Scope loops
- This loop has no own GREEN phase, instead of the inner Business scope loop and Solution scope loop
  provide the needed implementation to make the UI test pass.

REFACTOR phase:
- Improve the UI without changing the behaviour.
- Improve the visual appearance.
- Improve the user experience.
- Close the phase with: git commit -m "B-UA-001: <short description>" -a



Purpose:
- Define and verify business truth

Focus:
- Business rules
- Invariants
- Negative guarantees
- Compliance and auditability
- Workflow correctness

Key unit:
- Business workstep (atomic business responsibility)

Rules:
- Every business workstep MUST have exactly one BDD test
- BDD tests are the source of business truth
- BDD tests are usually API / application-boundary tests
- UI visibility is irrelevant to whether a BDD test is required

BDD tests must exist even if the user never sees the behaviour.

---

## 4. TDD — Test Driven Development

Purpose:
- Implement the smallest solution pieces correctly

Focus:
- Components
- Algorithms
- Validators
- Calculators
- Infrastructure adapters

Rules:
- TDD exists only to satisfy BDD
- TDD may never change business rules
- TDD is executed in strict RED → GREEN → REFACTOR cycles

---

## 5. Nested Execution Flow

Outer loop:
- BDD RED: one failing test for a single business workstep

Inner loop (repeated as needed):
- TDD RED: smallest missing behaviour
- TDD GREEN: minimal implementation
- TDD REFACTOR: cleanup without behaviour change

Stop TDD cycles when the BDD test becomes GREEN.

Final step:
- BDD REFACTOR (clarity only, no semantic change)

---

## 6. UI Visibility Rule

Business worksteps fall into two categories:

1. UI-visible worksteps
   - Require BDD test
   - Require UI test

2. UI-invisible worksteps
   - Require BDD test
   - Must NOT rely on UI tests

Examples of UI-invisible business capabilities:
- Atomicity
- No side effects on failure
- Audit logging
- Idempotency
- Authorization enforcement
- Retry and fallback behaviour
- Event emission correctness

---

## 7. Governance Rules

- UDD may influence BDD, never code
- BDD may influence TDD, never UI
- TDD may influence code only
- UI tests are additive, never authoritative

---

## 8. One-Sentence Doctrine

UDD proves usability.  
BDD proves correctness.  
TDD proves implementation.
