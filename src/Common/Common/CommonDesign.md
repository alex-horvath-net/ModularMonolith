# Common Design

## Purpose
Provides cross-cutting infrastructure and middleware extensions (security headers, auth, error handling, rate limiting, health, versioning, logging, observability, Kestrel hardening) shared across hosts and modules.

## Key Components
- `CommonExtensions`: Orchestrates DI + pipeline wiring.
- Security: HTTPS/HSTS, CSP with nonce, CorrelationId, SecurityHeaders.
- Auth: JWT bearer validation + development token issuance.
- Error Handling: RFC7807 via centralized exception handler.
- Rate Limiting: Sliding window global + concurrency limiter for writes.
- API Versioning: Minimal API groups using ApiVersionSet.
- Health: Live/ready endpoints with IP allowlist filter.
- Observability: OpenTelemetry tracing/metrics exporters + Serilog logging.
- Logging: Http logging (selected headers only).
- Transport: Extended `UseKestrel` for TLS + limits.

## Design Principles
- Extension methods keep `Program.cs` lean.
- Internal classes hide implementation details; only orchestrator methods are public.
- Fail-fast validation for critical configuration (OTLP endpoint, certificate).

## Security Controls
- Strict TLS (1.2/1.3), HSTS, CSP, sensitive header exclusion.
- Dev-only token endpoint guarded by environment check.

## Extensibility
- New cross-cutting concerns added as separate extension classes (`XyzExtensions`).
- Modules reference Common for shared behaviors without tight coupling.

## Non-Goals
- Business domain logic.
- UI rendering.
