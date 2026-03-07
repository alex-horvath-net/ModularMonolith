# Documentation Style

## Goal
Keep all repository documentation consistent, easy to scan, and predictable.

## File Naming
- Use lowercase `kebab-case` for document file names in `docs/`.
- Use `.md` extension.
- Use only lowercase letters (`a-z`), numbers (`0-9`), and `-`.
- Examples:
  - `lead-development-guidence.md`
  - `userstories.md`
  - `documentation-style.md`

## Heading Style
- Use `Title Case` for headings.
- Keep heading hierarchy consistent:
  - `#` for document title
  - `##` for major sections
  - `###` for subsection details

## Terminology
Use canonical terms consistently:
- `Acceptance criteria`
- `Wished business process`
- `Current business process`
- `Estimated Work`
- `Actual Work`
- `Remaining Work`
- `Frequency`
- `Importance`
- `Urgency`
- `Impact`
- `Certainty`
- `Stable`

## List Formatting
- Use `-` for bullet lists.
- Use backticks for field names and command literals.
- For nested process fields, always use:
  - `Start`
  - `Business workflow`
  - `End`

## Scales Formatting
- Keep scales in ascending numeric order.
- Use the exact value format:
  - `` `N` Label ``
- Reuse one shared definition for each scale across documents.

## Tables
- Use tables only for index/overview sections.
- Keep column names stable across documents.

## Survey Logging
- `docs/log.md` is append-only.
- Log entries must not overwrite existing content.
- Use timestamp format: `dd.MM.yyyy hh:mm:ss`.

## Change Discipline
- Update related docs together when terminology or scales change.
- Keep examples aligned with the canonical survey field set.
