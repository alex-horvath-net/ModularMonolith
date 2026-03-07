# Copilot Instructions

## General Guidelines
- You must always apply repository-level instructions and referenced documents,</br> 
  but only re-send them when necessary (e.g., to refresh memory or when changed).
- You must follow documentation formatting and naming rules defined in: `#file:'docs/documentation-style.md'`.
- You must always follow the development model defined in: `#file:'docs/lead-development-guidence.md'`.</br>
  Read this file only once per Visual Studio session, not on every message.
- Development guidance is strict black-and-white with no optional steps.

## Command Protocol
- When user says `dev start`, clear content of `#file:'docs/log.md'` first, then append exactly in this format: `dev started - dd.MM.yyyy hh:mm:ss`.
- When user says `dev end`, append to `#file:'docs/log.md'` exactly in this format: `dev ended - dd.MM.yyyy hh:mm:ss`.

## User Story Survey Guidance
- When guiding a user story survey, follow: `#file:'docs/user-story-guidance.md'`.
- Always apply all required fields, optional fields, scales, and logging rules from that document.
