# User Story Guidance

## Goal
Capture `UserStory` records that are unambiguous, business-focused, and directly testable.

## User Story Survey Protocol
- Capture exactly one user action and one related business workflow per `UserStory`.
- Keep `UserStory` content business-level only.
- Do not include implementation details.
- Record every `UserStory` in `docs\user-stories.md` using this guidance.
- Do not start development until the story is complete and `Status` is `Accepted`.
- Use the same rules for reverse-engineered and new stories.

## Quality Gates (Mandatory)
A `UserStory` is valid only if all gates pass:
- Single action: one trigger from one application user.
- Business clarity: clear business issue and clear targeted business outcome.
- Workflow decomposition: each business workflow step is represented by `AcceptanceCriteria`.
- Testability: each `AcceptanceCriteria` is objective and pass/fail verifiable.
- Traceability: each `AcceptanceCriteria` uses a stable ID: `AcceptanceCriteria-<storyId>-NN`.
- Mapping consistency:
  - each business `AcceptanceCriteria` maps 1:1 to one `BDD` scenario,
  - each UI Visibility `AcceptanceCriteria` maps to `E2E UI` tests,
  - do not create a separate `Acceptance Tests` layer when `BDD` already verifies business acceptance.

`docs\user-stories.md` must use the structure below:

# User Stories

## Index
| ID | Title | Status | Application | Role | Updated |
|---|---|---|---|---|---|
|   |   |   |   |   |   |

- ID must be 3-digit autoincrement format, for example: `001`.
- Title must link to the story section, for example: `[Title](#UserStory-001-Title)`.
- Updated format is `dd.MM.yyyy hh:mm:ss`, for example: `07.03.2026 14:30:00`.

---

## UserStory-ID-Title

### Required Fields
- `Title`
- `Status`
- `Application`
- `Application User`
- `Business Issue`: Missing business capability for the application user.
- `Aimed Business Outcome`: Aimed Business capability by this story.
- `Business Setup`: Simplest existing interaction sequence that reaches the step just before the new/changed action.
- `User Action`: How the user triggers the workflow.
- `Business workflow`: Sequenced non-technical, business work steps that resolve the business issue (hierarchical bullets or mermaid).
- `AcceptanceCriteria`: Must use explicit, testable rules.

### AcceptanceCriteria Format (Mandatory)
- Use numbered criteria with stable IDs.
- Preferred format:
  - `AcceptanceCriteria-001-01`: Given `<context>`, When `<action>`, Then `<observable outcome>`.
  - `AcceptanceCriteria-001-02`: Given ...
- Criteria must be measurable and binary (pass/fail).
- Each criterion must represent exactly one business workflow step.
- Criteria types:
  - `Business AcceptanceCriteria` (mandatory)
  - `UI Visibility AcceptanceCriteria` (optional)

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
  - Map each business `AcceptanceCriteria` to exactly one BDD scenario ID (`BDD-001-01`, ...).
- `E2E UI Tests`
  - Map each UI visibility `AcceptanceCriteria` to one or more E2E UI test IDs (`E2E-001-01`, ...).

- Mapping rule:
  - Business workflow step: `Business AcceptanceCriteria` -> `BDD`
  - UI-visible workflow step: `UI Visibility AcceptanceCriteria` -> `E2E UI`

- Clarification: Do not add a separate `Acceptance Tests` layer when `BDD` already validates business acceptance criteria.

### Out of Scope for UserStory Documentation (Mandatory)
- Internal unit decomposition
- Unit-test targets and unit-test case design
- TDD RED-GREEN slice planning
- Technical implementation steps (for example, API shape, class decomposition, button/control details)

These belong to implementation-focused guidance, not `docs\user-stories.md`.

### Definition of Ready (Mandatory)
Story is `Accepted` only when:
- all required fields are filled,
- all `AcceptanceCriteria` are testable,
- every business criterion has 1:1 `BDD` mapping,
- every UI visibility criterion has `E2E UI` mapping,
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
