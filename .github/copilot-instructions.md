# Copilot Instructions

## General Guidelines
- You must follow the development model defined in: `#file:'docs/LEAD-DEVELOPMENT.md'`. Read this file only once per Visual Studio session, not on every message.
- Development guidance is strict black-and-white with no optional steps.

## Command Protocol
- When user says `dev start`, clear content of `#file:'docs/LEAD-DEVELOPMENT.md'` first, then append exactly in this format: `dev started - dd.MM.yyyy hh:mm:ss`.
- When user says `dev end`, append to `#file:'docs/log.md'` exactly in this format: `dev ended - dd.MM.yyyy hh:mm:ss`.

## User Story Survey Protocol
- When guiding a user story survey, always ask and capture all required fields below.
- Required fields:
  - `Application`
  - `Role`
  - `Title`
  - `Wished business process`
    - `Start`
    - `Business workflow`
    - `End`
  - `Acceptance criteria`
- Optional fields:
  - `Current business process`
    - `Start`
    - `Business workflow`
    - `End`
  - `Story Points`
  - `Frequency`
  - `Importance`
  - `Urgency`
  - `Impact`
  - `Certainty`
  - `Stable`

### 4-Step Scales
- `Story Points`
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

## Survey Logging Protocol
- Record every collected survey information in `#file:'docs/log.md'`.
- Log each field/value as soon as the value is provided.
- Keep appending, do not overwrite existing `#file:'docs/log.md'` content.
