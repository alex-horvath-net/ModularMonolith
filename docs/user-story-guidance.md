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
- `Current business process`
  - `Start`
  - `Business workflow`
  - `End`
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

## 4-Step Scales
- `Status`
  - `1` Forming
  - `2` Accepted
  - `3` In Progress
  - `4` Done
- `Work`
  - `1` 2 hours
  - `2` 6 hours
  - `3` 2 days
  - `4` 5 days
- `Frequency`
  - `1` Rarely
  - `2` Occasionally
  - `3` Often
  - `4` Very often
- `Importance` (MoSCoW-aligned)
  - `1` Won't have (this cycle)
  - `2` Could have (nice to have)
  - `3` Should have
  - `4` Must have
- `Urgency`
  - `1` No rush
  - `2` Soon
  - `3` Urgent
  - `4` Immediate
- `Impact`
  - `1` Minor
  - `2` Moderate
  - `3` Major
  - `4` Business Critical
- `Certainty`
  - `1` Guess
  - `2` Low confidence
  - `3` High confidence
  - `4` Confirmed
- `Stable`
  - `1` Forming
  - `2` Evolving
  - `3` Stable
  - `4` Never change
