# WebApi Design

## Purpose
Exposes HTTP API endpoints for Orders and Billing modules with cross-cutting concerns (security, versioning, observability, error handling) integrated via Common library.

## Responsibilities
- Host minimal API endpoints (`/v{version}/orders`, `/v{version}/billing`).
- Enforce transport security (TLS 1.2/1.3) and request limits through Common `UseKestrel`.
- Apply global middleware pipeline from `CommonExtensions`.
- Provide structured logging & OpenTelemetry export.
- Surface OpenAPI (dev only) and health probes.

## Startup Flow
1. Create builder.
2. Harden Kestrel using `UseKestrel(config, env)`.
3. Configure logging via `UseLogger`.
4. Register cross-cutting services `AddCommon` then module services `AddOrders`, `AddBilling`.
5. Build app.
6. Map common endpoints (security headers, error handling, health, docs).
7. Map module endpoints.
8. Run.

## Security
- TLS enforced on 443 (prod); optional port 80 only if `Http:EnablePort80`.
- HSTS via `AddHttps` / `UseHttps`.
- CSP with nonce & security headers in Common.
- JWT bearer auth via Common `AddAuthentication` (dev token endpoint for development only).
- Rate limiting: global sliding window + write concurrency.

## Versioning
- URL segment `v{version:apiVersion}` with default version 1.0.
- Future versions supported by adding ApiVersionSet in module endpoint definitions.

## Observability
- Serilog structured logs enriched with application, environment, machine.
- OpenTelemetry tracing & metrics (AspNetCore, HttpClient, runtime) exported to OTLP endpoint.
- CorrelationId middleware attaches `X-Correlation-ID` header.

## Error Handling
- Central RFC7807 ProblemDetails responses via `AddErrorHandling` / `UseErrorHandling`.
- Safe logging of unhandled exceptions (PII excluded).

## Performance & Resiliency
- Request/response data rate limits; max body size 10MB.
- Polly policies on outbound HttpClients (retry, circuit-breaker, timeout) consumed by other hosts.

## Configuration
- `appsettings.json` includes Auth, Cors, Health, Observability, Http, Certificates, DataProtection, WebApi, RateLimiting sections.
- Fail-fast if required settings missing (e.g., certificates, OTLP endpoint in prod).

## Deployment
- Container-ready: single entry point; externalized secrets (cert paths, DP keys).
- Health probes: `/health/live` & `/health/ready` with IP allowlist control.

## Extensibility
- New modules add `ModuleNameExtensions` with `AddModuleName` + `MapModuleName` pattern.
- Additional API versions: duplicate endpoint groups with new `ApiVersionSet`.

## Non-Goals
- UI rendering (handled by WebPortal).
- Direct business logic: delegated to module application handlers.
