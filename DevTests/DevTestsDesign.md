# DevTests Project Design

## Purpose
Provide a sandbox for local experimentation, spike solutions, or prototype tests without affecting production-quality test suite.

## Scope
- Temporary exploratory code.
- Does not deploy.
- May host integration harnesses pointing to local WebApi / WebPortal.

## Guidelines
- Keep throwaway code isolated; migrate stable tests into future dedicated test projects (e.g., UnitTests, IntegrationTests) under a structured test hierarchy.
- Avoid committing secrets or long-lived data.

## Non-Goals
- Formal regression testing.
- Performance/load testing.
