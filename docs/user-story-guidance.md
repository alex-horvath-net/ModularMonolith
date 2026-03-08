# User Story Guidance

## Goal
Define `UserStory` records in a clear, business-focused, and implementation-agnostic way.

This document standardizes:
- business workflow orchestration,
- decomposition of workflow steps into `AcceptanceCriteria`,
- separation of `Business AcceptanceCriteria` and `UI Visibility AcceptanceCriteria`.

## User Story Survey Protocol
- Each `UserStory` must capture exactly one user action and one related business workflow.
- Keep `UserStory` content business-level only.
- Do not include technical implementation details.
- Record every `UserStory` in `docs\user-stories.md` using this guidance.
- Development cannot start until the story is complete and `Status` is `Accepted`.
- Use the same rules for reverse-engineered and new stories.

## Quality Gates (Mandatory)
A `UserStory` is valid only if all gates pass:
- Single action: one trigger from one application user.
- Business clarity: clear business issue and clear aimed business outcome.
- Workflow orchestration clarity: business workflow is described as ordered business work steps.
- Decomposition completeness: each business workflow step is decomposed into acceptance criteria.
- Traceability: each criterion uses stable ID format `AcceptanceCriteria-<storyId>-NN`.

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
- `Aimed Business Outcome`: Intended business capability to be achieved by this `UserStory`.
- `Business Setup`: Existing user interaction sequence that brings the user to the step immediately before the new or changed action.
- `User Action`: Concrete action performed by the application user that triggers the workflow.
- `Business workflow`: Ordered, non-technical business work steps that address the business issue.
- `AcceptanceCriteria`: Explicit, objective, and verifiable criteria.

### Business Workflow Orchestration Rules (Mandatory)
- Describe the workflow as ordered business steps (`Step 1`, `Step 2`, ...). Use both hierarchical bullet points and mermaid flowchart.
- Keep steps non-technical and value-oriented.
- Each step must describe business intent and expected state transition.
- Prefer hierarchical bullets when sub-steps are needed.

### AcceptanceCriteria Decomposition Rules (Mandatory)
- Every business workflow step must be decomposed into one or more `AcceptanceCriteria`.
- Each `AcceptanceCriteria` must represent exactly one specific validation point.
- Use stable IDs:
  - `AcceptanceCriteria-001-01`
  - `AcceptanceCriteria-001-02`

- Criteria categories:
  - `Business AcceptanceCriteria` (mandatory): validates business behavior or business outcome.
  - `UI Visibility AcceptanceCriteria` (optional): validates what the user must be able to see or confirm in UI.

- Preferred criterion format:
  - `AcceptanceCriteria-001-01` (`Business`): Given `<business context>`, When `<user action or business event>`, Then `<business outcome>`.
  - `AcceptanceCriteria-001-02` (`UI Visibility`): Given `<state>`, When `<user reaches UI state>`, Then `<visible outcome>`.

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

### Out of Scope for UserStory Documentation (Mandatory)
- API/class/component design
- internal decomposition and technical architecture
- test implementation details
- TDD/BDD execution planning

### Definition of Ready (Mandatory)
Story is `Accepted` only when:
- all required fields are filled,
- business workflow is clearly orchestrated as ordered business work steps,
- each workflow step has decomposed `AcceptanceCriteria`,
- both `Business AcceptanceCriteria` and `UI Visibility AcceptanceCriteria` are captured where applicable,
- no implementation detail is needed to understand business intent.

## Practical Writing Rules
- Use short, concrete sentences.
- Prefer domain words over technical words.
- Avoid ambiguous terms such as "handle", "process", "manage" without context.
- Keep one idea per bullet.
- If information is unknown, write `TBD` and continue.

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
