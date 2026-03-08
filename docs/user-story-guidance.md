# User Story Guidance

## Goal
Capture user stories in a format that is unambiguous, testable, and directly usable for BDD, TDD, E2E UI tests, acceptance tests, and unit tests.

## User Story Survey Protocol
- Each user story must capture exactly one user action and one related business workflow.
- User stories must stay business-focused and must not prescribe implementation details.
- Every user story must be recorded in `docs\user-stories.md` using the required structure.
- Development cannot start until the story is fully captured and has `Status: Accepted`.
- Reverse-engineered stories must use the same structure and quality gates as new stories.

## Quality Gates (Mandatory)
A story is valid only if all gates pass:
- Single action: one trigger from one application user.
- Business clarity: clear business issue and achieved business outcome.
- Testability: every acceptance criterion is objective and verifiable.
- Traceability: each criterion has a stable `AC-<storyId>-NN` identifier.
- Coverage mapping: each story includes explicit mapping to BDD, acceptance, E2E UI, and unit-level tests.

`docs\user-stories.md` must use the structure below:

# User Stories

## Index
| ID | Title | Status | Application | Role | Updated |
|---|---|---|---|---|---|
|   |   |   |   |   |   |

- ID must be 3-digit autoincrement format, for example: `001`.
- Title must link to the story section, for example: `[Title](#US-001-Title)`.
- Updated format is `dd.MM.yyyy hh:mm:ss`, for example: `07.03.2026 14:30:00`.

---

## US-ID-Title

### Required Fields
- `Title`
- `Status`
- `Application`
- `Application User`
- `Business Issue`: Missing business capability for the application user.
- `Business Setup`: Simplest existing interaction sequence that reaches the step just before the new/changed action.
- `User Action`: How the user triggers the workflow.
- `Business workflow`: Sequenced non-technical work steps that resolve the business issue (hierarchical bullets or mermaid).
- `Achieved Business Outcome`: Capability actually achieved by this story.
- `Acceptance criteria`: Must use explicit, testable rules.

### Acceptance Criteria Format (Mandatory)
- Use numbered criteria with stable IDs.
- Preferred format:
  - `AC-001-01`: Given `<context>`, When `<action>`, Then `<observable outcome>`.
  - `AC-001-02`: Given ...
- Criteria must be measurable and binary (pass/fail).

### Optional Fields
- `Original Estimated Work`
- `Remaining Work`
- `Applied Work`
- `Frequency`
- `Importance`
- `Urgency`
- `Impact`
- `Certainty`
- `Stable`

### Test Mapping (Mandatory)
Each story must include this section:
- `BDD Scenarios`
  - Map each `AC` to one BDD scenario ID (`BDD-001-01`, ...)
- `Acceptance Tests`
  - Map each `AC` to one acceptance test case ID (`AT-001-01`, ...)
- `E2E UI Tests`
  - List only UI-visible outcomes with IDs (`E2E-001-01`, ...)
- `Unit Test Targets`
  - List business rules/components to verify in isolation (`UT-001-01`, ...)
- `TDD Plan`
  - Define smallest implementation slices in expected RED-GREEN sequence

### Definition of Ready (Mandatory)
Story is `Accepted` only when:
- all required fields are filled,
- all acceptance criteria are testable,
- test mapping section is complete,
- no implementation detail is required to understand business intent.

## Scales
- `Status`
  - `Forming`
  - `Accepted`
  - `In Progress`
  - `Done`
- `Work`
  - `2 hours`
  - `6 hours`
  - `16 hours`
  - `32 hours`
- `Frequency`
  - `Rarely`
  - `Occasionally`
  - `Often`
  - `Very often`
- `Importance`
  - `Won't have`
  - `Could have`
  - `Should have`
  - `Must have`
- `Urgency`
  - `No rush`
  - `Soon`
  - `Urgent`
  - `Immediate`
- `Impact`
  - `Minor`
  - `Moderate`
  - `Major`
  - `Business Critical`
- `Certainty`
  - `Guess`
  - `Low confidence`
  - `High confidence`
  - `Confirmed`
- `Stable`
  - `Forming`
  - `Evolving`
  - `Stable`
  - `Never change`
