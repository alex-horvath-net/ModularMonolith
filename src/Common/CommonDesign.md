# Common Project – Implementation‑ready Concern Registry

This document lists each cross‑cutting concern in the exact order they are registered/applied by `AddCommon` and `MapCommon`. Each concern is presented in a consistent, implementation‑ready format: Problem, Solution, Service registration, Middleware mapping, Config keys, Verification.

---

## 1) HTTPS / HSTS
- Problem:<br>
  Users and intermediaries may access the service over plaintext (HTTP) or accept downgraded TLS ciphers, enabling SSL‑strip and downgrade attacks.

- Solution:<br>
  Enforce HTTPS transport and send Strict‑Transport‑Security header in production environments only.

- Service registration:<br>
  Call `services.AddHsts(options => { options.MaxAge = TimeSpan.FromDays(365); options.IncludeSubDomains = true; options.Preload = true; });` in `AddCommon` (via `services.AddHttps()`).

- Middleware mapping:<br>
  Call `app.UseHttps()` in `MapCommon` which will call `app.UseHsts()` when not in development. Host must configure TLS (Kestrel) via `UseKestrel(config, env)` so HTTPS terminates on the app. Ensure `UseHttpsRedirection()` is applied (it is mapped later in the pipeline).

- Config keys:<br>
  None required. Host must provide certificate configuration under `Certificates:*`.

- Verification:<br>
  - In production mode, request `http://` endpoint → expect redirect to `https://` and response contains `Strict-Transport-Security` header with configured values.
  - Run TLS scan (SSL Labs or equivalent) and confirm TLS 1.2+ only and no weak ciphers.

---

## 2) Client Request Headers (Forwarded Headers Preservation)
- Problem:<br>
  Reverse proxies/load balancers can hide original client IP and scheme, breaking rate limiting, logs, and IP‑based controls.

- Solution:<br>
  Configure forwarded header processing early so the app sees original `RemoteIpAddress` and scheme.

- Service registration:<br>
  Implement `services.AddClientHeadersInProxy()` which configures `ForwardedHeadersOptions` (e.g. `X-Forwarded-For`, `X-Forwarded-Proto`) and optionally loads known proxies/networks from config.

- Middleware mapping:<br>
  Call `app.UseClientHeadersInProxy()` before any component relying on client IP or scheme (rate limiting, auth, logging).

- Config keys:<br>
  Optional: `ForwardedHeaders:KnownProxies`, `ForwardedHeaders:KnownNetworks`.

- Verification:<br>
  - Send a proxied request with `X-Forwarded-For`/`X-Forwarded-Proto` and confirm `HttpContext.Connection.RemoteIpAddress` and `Request.Scheme` reflect forwarded values.

---

## 3) Rate Limiting
- Problem:<br>
  Unbounded traffic or repeated write requests can exhaust CPU/memory or database resources (DoS) and degrade availability.

- Solution:<br>
  Apply a global sliding‑window limiter partitioned by identity and a named concurrency limiter for write endpoints.

- Service registration:<br>
  `services.AddRateLimiting()` registers `AddRateLimiter` with:
  - Global partitioned sliding window keyed by identity (claims or remote IP fallback).
  - Named concurrency limiter `writes` (e.g., PermitLimit=10, QueueLimit=10).

- Middleware mapping:<br>
  Call `app.UseRateLimiting()` early (after logging) so rejections are recorded. Apply `.RequireRateLimiting("writes")` to write endpoints.

- Config keys:<br>
  Optional `RateLimiting:Writes:PermitLimit`, `RateLimiting:Writes:QueueLimit`, `RateLimiting:Global:*` for future tuning.

- Verification:<br>
  - Run load tests to exceed limits and observe 429 responses and optional `Retry-After` header; verify write concurrency is limited.

---

## 4) API Documentation (OpenAPI)
- Problem:<br>
  Developers need machine‑readable API contracts and an interactive UI for local testing; docs must not be exposed in production.

- Solution:<br>
  Generate OpenAPI documents and expose UI only in development environments.

- Service registration:<br>
  `services.AddApiDocumentation()` registers OpenAPI generation and injects a Bearer security scheme into the document.

- Middleware mapping:<br>
  `app.MapApiDocumentation()` maps `/openapi/{document}.json` and UI only when `env.IsDevelopment()`.

- Config keys:<br>
  None.

- Verification:<br>
  - In dev, open Swagger UI and confirm the Bearer security scheme and that endpoints/DTOs are documented.

---

## 5) API Admin (Swagger UI / Admin)
- Problem:<br>
  Ops and developers need an interactive, read‑only view for troubleshooting; must be restricted from production.

- Solution:<br>
  Provide an admin UI mapped conditionally (dev or internal network) and gate it as needed.

- Service registration:<br>
  `services.AddApiAdmin()` configures the Swagger UI endpoints.

- Middleware mapping:<br>
  `app.MapApiAdmin()` maps the UI when allowed by environment/config; protect with rate limiting/auth if exposed to internal networks.

- Config keys:<br>
  Optional gating keys for environment or network restrictions.

- Verification:<br>
  - UI accessible in dev; inaccessible in production or remote networks as configured.

---

## 6) API Versioning
- Problem:<br>
  Breaking changes require a versioning strategy to support clients concurrently and allow deprecation.

- Solution:<br>
  Configure API Versioning with default, URL segment and header readers, and ApiExplorer grouping.

- Service registration:<br>
  `services.AddApiVersioning(...)` with DefaultApiVersion 1.0, `AssumeDefaultVersionWhenUnspecified=true`, `ReportApiVersions=true`, and combined `UrlSegmentApiVersionReader` + `HeaderApiVersionReader("X-API-Version")`. Chain `.AddApiExplorer(...)` for grouping.

- Middleware mapping / Endpoint usage:<br>
  Modules create `versionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1,0)).Build();` and map groups under `/v{version:apiVersion}/...`.

- Config keys:<br>
  None.

- Verification:<br>
  - Confirm `/v1/orders` works, `/orders` falls back to default, and response headers include `api-supported-versions`.

---

## 7) Error Handling (RFC7807)
- Problem:<br>
  Inconsistent error shapes and leaked stack traces make client handling and auditing unreliable and insecure.

- Solution:<br>
  Centralize exception handling to return RFC7807 ProblemDetails with safe fields and audit linkage (traceId, correlationId).

- Service registration:<br>
  `services.AddErrorHandling()` configures ProblemDetails mapping for known exceptions (validation → 400, unauthorized → 401/403, etc.).

- Middleware mapping:<br>
  `app.UseErrorHandling()` placed early to convert exceptions from any layer into ProblemDetails. Ensure it enriches `extensions` with `traceId` and `correlationId`.

- Config keys:<br>
  None.

- Verification:<br>
  - Trigger exceptions and confirm responses follow RFC7807, contain `traceId`/`correlationId`, and do not include stack traces in production builds.

---

## 8) Browser Request Restrictions (CORS)
- Problem:<br>
  Unrestricted CORS can allow malicious webpages to call backend APIs from other origins.

- Solution:<br>
  Provide a configurable `DefaultCors` policy using `Cors:AllowedOrigins`.

- Service registration:<br>
  `services.AddBrowserRequestRestrictions(config)` reads `Cors:AllowedOrigins` and registers `DefaultCors` with `WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()` when origins exist.

- Middleware mapping:<br>
  `app.UseBrowserRequestRestrictions()` calls `app.UseCors("DefaultCors")` early enough to affect incoming requests.

- Config keys:<br>
  `Cors:AllowedOrigins` (array).

- Verification:<br>
  - Requests from allowed origins succeed; browser blocks requests from disallowed origins (CORS preflight fails).

---

## 9) JSON Hardening
- Problem:<br>
  Malicious or malformed JSON (comments, extreme depth, case ambiguity) can cause resource exhaustion or incorrect binding.

- Solution:<br>
  Harden System.Text.Json options: disallow comments, limit depth, enforce case sensitivity.

- Service registration:<br>
  In `AddJson()` call `services.ConfigureHttpJsonOptions(o => { o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Disallow; o.SerializerOptions.MaxDepth = 32; o.SerializerOptions.PropertyNameCaseInsensitive = false; });`

- Middleware mapping:<br>
  N/A (applies at formatter level).

- Config keys:<br>
  None (tunable constants may be externalized later).

- Verification:<br>
  - Send JSON with comments or extreme depth and confirm rejection or controlled failure.

---

## 10) Authentication (JWT)
- Problem:<br>
  Need reliable token validation and developer convenience of local tokens without weakening production security.

- Solution:<br>
  Configure strict JWT Bearer validation and expose a dev-only token endpoint when in development.

- Service registration:<br>
  `services.AddAuthentication(config, env)` binds `Auth:Issuer`, `Auth:Audience`, configures `TokenValidationParameters` (issuer, audience, signing key). In dev, support `Auth:DevKey` for token issuance.

- Middleware mapping:<br>
  `app.UseAuthentication()` and `app.UseAuthorization()` are invoked in MapCommon. `app.MapDevToken()` is mapped only in development.

- Config keys:<br>
  `Auth:Audience`, `Auth:Issuer`, `Auth:DevKey`, `Auth:DevScopes`.

- Verification:<br>
  - Obtain dev token in dev and call protected endpoints; in prod dev token endpoint should not exist and calls without valid tokens return 401.

---

## 11) Request Logging (HTTP Logging)
- Problem:<br>
  Need structured request/response logs for auditing but must avoid logging secrets.

- Solution:<br>
  Use HttpLogging middleware to record fields (method, path, duration, selected headers) and explicitly exclude sensitive headers.

- Service registration:<br>
  `services.AddRequestLogging()` configures `HttpLoggingFields` and adds request/response headers to capture; ensure `Authorization`, `Cookie`, `Set-Cookie` are not captured.

- Middleware mapping:<br>
  `app.UseHttpLogging()` placed early to capture raw request/response before modifications.

- Config keys:<br>
  Logging levels under `Logging:*`.

- Verification:<br>
  - Inspect logs for request/response metadata and `X-Correlation-ID`, confirm no Authorization header values are logged.

---

## 12) Observability (OpenTelemetry)
- Problem:<br>
  Missing distributed traces and metrics impede investigations and SLA measurement.

- Solution:<br>
  Instrument ASP.NET Core and HttpClient, collect runtime metrics, and export to OTLP collector; enforce fail‑fast when OTLP endpoint is required in production.

- Service registration:<br>
  `services.AddObservability(config, env)` constructs a ResourceBuilder, registers `AddAspNetCoreInstrumentation()`, `AddHttpClientInstrumentation()`, `AddRuntimeInstrumentation()`, and configures OTLP exporter with `Observability:OtlpEndpoint`.

- Middleware mapping:<br>
  Ensure correlation ID middleware enriches spans; no extra middleware needed for basic instrumentation.

- Config keys:<br>
  `Observability:OtlpEndpoint` (required in non‑dev).

- Verification:<br>
  - Start app with OTLP collector configured and confirm traces/metrics reach the collector. Missing endpoint in prod should cause startup error.

---

## 13) Health Checks
- Problem:<br>
  Orchestrators require liveness and readiness endpoints to manage lifecycle and routing.

- Solution:<br>
  Register health checks and map `/health/live` and `/health/ready` (readiness includes dependency checks).

- Service registration:<br>
  `services.AddFullHealthCheck()` registers base health checks; modules may add DB/cache checks in their own `Add*` methods.

- Middleware mapping:<br>
  `app.MapFullHealthCheck()` maps endpoints; apply IP filter if configured.

- Config keys:<br>
  None for base checks; modules may add their own keys.

- Verification:<br>
  - `/health/live` returns 200 when app running; `/health/ready` returns 200 only when all registered checks are healthy.

---

## 14) Allowed IPs for Health Probes
- Problem:<br>
  Readiness endpoints may leak internal state if publicly accessible.

- Solution:<br>
  Use `IpEndpointFilter` to restrict health endpoints to an allowlist.

- Service registration:<br>
  `services.AddAllowedIPsForHealthProbes(config)` reads `Health:AllowedIps` and registers the filter.

- Middleware mapping:<br>
  Apply `IpEndpointFilter` to `/health/ready` routes during `MapFullHealthCheck()`.

- Config keys:<br>
  `Health:AllowedIps` (array supporting `localhost`, IPv4, IPv6, mapped IPv4).

- Verification:<br>
  - Requests from non‑allowed IPs receive 403; allowed IPs succeed.

---

## 15) Business Event Publisher
- Problem:<br>
  Modules need to publish domain events without compile‑time coupling to each other.

- Solution:<br>
  Offer an in‑process `IBusinessEventPublisher` for decoupled dispatch; later replace with an external message bus.

- Service registration:<br>
  `services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();` where `InProcessBusinessEventPublisher.PublishAsync<T>(T evt)` dispatches to local handlers.

- Middleware mapping:<br>
  Not applicable (service only).

- Config keys:<br>
  None.

- Verification:<br>
  - Unit/integration test: publish an event in Orders and assert Billing handler receives it within the same process.

---

## 16) Secrets bootstrap
- Problem:<br>
  Centralized secret management is required for production (Azure Key Vault) and development (user-secrets) to avoid sensitive data in source control and app settings.

- Solution:<br>
  Add `SecretConfigurationExtensions.AddSecretsFromStore` to bootstrap configuration from user-secrets in dev and Azure Key Vault in non-dev.

- Service registration:<br>
  N/A (configuration bootstrap).

- Middleware mapping:<br>
  N/A (configuration bootstrap).

- Config keys:<br>
  `Secrets:KeyVault:Uri` (production) and user-secrets (development).

- Verification:<br>
  - Run in dev with user-secrets set and in production simulation without KeyVault to confirm fail-fast; configure `Secrets:KeyVault:Uri` and confirm secrets retrieved from Key Vault.

---

## Mapping Order (MapCommon) – exact sequence
1. `app.UseHttps()` (→ HSTS in non‑dev)
2. `app.UseClientHeadersInProxy()` (forwarded headers)
3. `app.UseCorrelationId()` (establish correlation)
4. `app.UseSecurityHeaders()` (CSP + security headers)
5. `app.UseHttpLogging()` (request/response logging)
6. `app.UseRateLimiting()` (global & named policies)
7. `app.UseErrorHandling()` (ProblemDetails)
8. `app.UseHttpsRedirection()` (redirect to HTTPS)
9. `app.UseBrowserRequestRestrictions()` (CORS)
10. `app.UseAuthentication()`
11. `app.UseAuthorization()`
12. `app.MapDevToken()` (dev only)
13. `app.MapFullHealthCheck()` (health endpoints + IP filter)
14. `app.MapApiDocumentation()` (OpenAPI JSON in dev)
15. `app.MapApiAdmin()` (Swagger UI in dev/test)

---

## How to extend / add a new cross‑cutting concern
1. Create `XFeatureExtensions` with `AddXFeature(IServiceCollection, IConfiguration)` and `UseXFeature(WebApplication)` if middleware is needed.
2. Add a single line call inside `CommonExtensions.AddCommon` at the correct ordering point.
3. Add `app.UseXFeature()` in `CommonExtensions.MapCommon` where ordering matters.
4. Add configuration keys under `XFeature:*` and a `ValidateOnStart` check if critical.
5. Add unit/integration tests verifying behavior and failure modes.

---

## Operational note: Design.md updates
- Rule:<br>
  When implementing changes described in this document, update the related `*Design.md` for the affected project(s) with a short change note: What changed, Why, Files modified, How to verify.

---

This specification is prescriptive and implementation‑ready. Follow the Implementation sections to make changes in code with minimal surprises for maintainers and auditors.
