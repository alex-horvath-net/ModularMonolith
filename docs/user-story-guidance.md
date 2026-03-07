# User Story Guidance

## User Story Survey Protocol
- Each user story must be captured in a structured format, following the required and optional fields defined below.
- When guiding a user story survey, always ask and capture all required fields below.

## Survey Logging Protocol
- Record every collected survey information in `docs/log.md`.
- Log each field/value as soon as the value is provided.
- Keep appending, do not overwrite existing `docs/log.md` content.

### Required Fields
- `Title`
- `Status`
- `Application`
- `Application User`
- `Business Issue`: Describe the missing business capability for the application user.
- `Business Setup`: Describe the simplest existing user action sequence that brings the user to the step just before the action to be changed or added.
- `User Action`: Describe how the application user triggers the business workflow (for example, by opening a page or submitting a form).
- `Business workflow`: Describe the sequence of non-technical business work steps performed behind the scenes to deliver value.
- `Achieved Business Outcome`: Describe the business capability that is actually achieved.
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
