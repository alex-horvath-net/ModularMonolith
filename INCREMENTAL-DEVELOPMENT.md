# Incremental Development Model 

This is the primary development workflow, <br>
for building well architected solution for business need <br>
in form of small, iterative, verified increments.

---

## 1. The Three Nested Development Loops

Development proceeds in three strictly ordered, nested loops:

- Business need loops explore the user manual of the application.
- Business scope loops explore business workflow behind the business need.
- Solution scope loops explore the solution behind the busness scope.

---

## 2. Business need loop

Artifact:
 - User Manual
 - User Story
 - Demo

Purpose of each loop:
- Add a tiny verified increment to the previous busness need,
  .
Final loop:
- Complites the altimate vision which satisfies the User Story.

Focus:
- UI behaviour
- User journeys
- Error messages
- Accessibility
- Permissions as experienced by the user

Verification:
- UI / E2E tests (e.g. Playwright)

Rules:
- UDD never defines business truth
- UDD may not bypass BDD
- UI tests are never authoritative for business rules

---

## 3. BDD — Business Driven Development

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
