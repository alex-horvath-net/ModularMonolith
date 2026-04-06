# Copilot Instructions

## General Guidelines
- Follow repository-level instruction documents and their references.
- Follow documentation formatting and naming rules in: `#file:'docs/documentation-style.md'`.
- Follow the development model in: `#file:'docs/lead-development-guidence.md'`.
- Provide responses in English.

## User Story Survey Guidance
- For user story survey work, use: `#file:'docs/user-story-guidance.md'` as the single source of truth, focusing on UserStory structure and business orchestration logic.
- Separate Business AcceptanceCriteria and UI Visibility AcceptanceCriteria within the user story documentation.
- If required story fields or mappings are missing, stop and ask for missing business information before proposing implementation work.
- Testing guidance will be moved to a separate future instruction file.

## Naming Conventions
- Name the internal user-story execution result as `Product<Response>` instead of `RegisterExecution` when carrying a Product Owner-facing response plus internal workflow metadata.

## Testing Structure
- Each test class must have a dedicated same-file `(TestClassName)DSL` class that inherits from `FeatureDSL`, except for Register demos.
- For Register demos, place workstep-specific DSL/helper methods directly in the `{WorkStepName}Demo` class, and let each demo class inherit from `FeatureDSL`.
- `FeatureDSL` should inherit from `ModuleDSL` and remain the shared base for all test classes.
- Omit redundant `Register` prefixes from test class names when the enclosing folder/namespace already provides that context.
- Refer to test classes as `Demo`, treating them as demonstrations of what the Product Owner receives from the Product increment.
