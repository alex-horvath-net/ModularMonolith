# User Story Guidance

## User Story Survey Protocol
- Each user story capture only a single user action and its related business workflow, not multiple user actions or workflows.
- No userstory should claim technical implementation details, such as "add a button" or "create an API endpoint". Instead, focus on the business need and outcome.
- Each user story must be recorded in a structured format in `docs\user-stories.md` defined below.
- No development phase can be started without a fully captured and accepted user story.

`docs\user-stories.md` should have the following structure:


# User Stories

## Index
| ID | Title | Status | Application | Role | Updated |
|---|---|---|---|---|---|
|   |   |   |   |   |   |

- Id should be a 3-digit autoincrement number, for example: `001`.
- Title should be link to the user story section, for example: `[US-001](#US-001-Title)`.

---

## US-ID-Title

### Required Fields
- `Title`
- `Status`
- `Application`
- `Application User`
- `Business Issue`: Describe the missing business capability for the application user.
- `Business Setup`: Describe the simplest existing user action sequence that brings the user to the step just before the action to be changed or added.
- `User Action`: Describe how the application user triggers the business workflow (for example, by opening a page or submitting a form).
- `Business workflow`: Suggested the sequenced orchestration of non-technical business work steps, which address the business issue. 
   It should be described with hierarchical bullet points or mermaid sequence diagram or mermaid flowchart.
- `Achieved Business Outcome`: Describe the business capability that is actually achieved. 
   This might be different from the originally wished business outcome, so further user stories may be needed to achieve it.
- `Acceptance criteria`

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

- Note: Each field, even required fields like `Wished business process`, can be initially captured at a high level and refined later as details become known.

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
