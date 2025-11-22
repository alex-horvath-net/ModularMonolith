# WebPortal Design

## Purpose
Server-side Blazor host providing interactive UI consuming WebApi. Implements secure session management and browser security controls.

## Responsibilities
- Render Razor Components and layouts.
- Manage user authentication (cookie-based) and authorization policies.
- Provide HttpClient for calling WebApi endpoints.
- Enforce CSP nonce on Blazor boot script.
- Persist DataProtection keys (encrypted + rotated) for cookie integrity.

## Security
- Cookie auth with HttpOnly, Secure, SameSite=Strict.
- Antiforgery middleware enabled.
- DataProtection key rotation (90 days) with external encrypted store in non-dev.
- CSP nonce injection via SecurityHeadersMiddleware.
- HSTS enforced in non-development.

## Observability
- Leverages shared logging in future (optional) and correlation via HttpClient headers.

## Configuration
- Requires `WebApi:BaseUrl`, `DataProtection:*` values.
- Certificate settings for DP at-rest encryption in production.

## Error Handling
- Central exception handler page `/Error` (scope for errors enabled in production).

## Extensibility
- Additional modules add components and pages referencing shared DTOs via API calls.

## Non-Goals
- Exposing REST API (handled by WebApi).
- Direct DB access.
