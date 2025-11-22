# Common Project – Implementation?ready Concern Registry

This document is an implementation specification for the `Common` project. It lists each cross?cutting concern in the exact order they are registered/applied by `AddCommon` and `MapCommon`. Each concern includes:
- Problem: the concrete issue to address.
- Solution: precise behavioral solution.
- Implementation: exact service and middleware calls, configuration keys, and minimal pseudocode / notes so a developer can implement the code without surprises.
- Verification: how to validate correctness.

---

## 1) HTTPS / HSTS
- Problem:
  - Browsers and intermediaries may access the service over plaintext (HTTP) or accept downgraded TLS ciphers, enabling SSL?strip and downgrade attacks; lack of HSTS means users can be tricked into using HTTP even after an admin intended to enforce HTTPS.

- Solution:
  - Enforce HTTPS transport and send Strict?Transport?Security header in production environments only.

- Implementation:
  - Service registration: call `services.AddHsts(options => { options.MaxAge = TimeSpan.FromDays(365); options.IncludeSubDomains = true; options.Preload = true; });` inside `services.AddHttps()`.
  - Middleware mapping: in `MapCommon` call `app.UseHttps()` which executes `if (!env.IsDevelopment()) app.UseHsts();`
  - Host must configure Kestrel TLS at host level (see `UseKestrel(config, env)` implementation in Common.KerstelExtensions) so that HTTPS actually terminates on the app.
  - Ensure `UseHttpsRedirection()` is present (it is applied later in pipeline) to redirect HTTP ? HTTPS.

- Config keys: none required; environment determines behavior. Host must provide certificate config under `Certificates:*`.

- Verification:
  - Start app in production mode; curl HTTP endpoint ? expect 301/307 redirect to HTTPS; final HTTPS response must include header `Strict-Transport-Security` with configured max-age and includeSubDomains.
  - TLS scan (sslscan/ssllabs) reports TLS 1.2+ only and no weak ciphers.

---

## 2) Client Headers (Forwarded Headers Preservation)
- Problem:
  - When running behind reverse proxies or load balancers, `HttpContext.Connection.RemoteIpAddress` and `Request.Scheme` may reflect the proxy not the original client; this breaks rate limiting, accurate logging, and ip?based access control.

- Solution:
  - Configure forwarded header processing and normalize client address/scheme early in the pipeline.

- Implementation:
  - Service registration: `services.AddClientHeadersInProxy()` should configure `ForwardedHeadersOptions` (e.g., `ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto`) and optionally set KnownProxies/KnownNetworks from configuration if running in restricted infra.
  - Middleware mapping: `app.UseClientHeadersInProxy()` must be invoked before any component that uses `RemoteIpAddress`, especially before rate limiting and security checks.
  - Example code:
    - services.AddClientHeadersInProxy() { services.Configure<ForwardedHeadersOptions>(opts => { opts.ForwardedHeaders = ...; /* optionally add KnownProxies from config */ }); }
    - app.UseForwardedHeaders(); // inside UseClientHeadersInProxy

- Config keys: optional `ForwardedHeaders:KnownProxies` / `ForwardedHeaders:KnownNetworks`

- Verification:
  - Send request via reverse proxy adding `X-Forwarded-For` and `X-Forwarded-Proto`; inside request handler, log `HttpContext.Connection.RemoteIpAddress` and ensure it matches the forwarded value.

---

## 3) Rate Limiting
- Problem:
  - Unbounded client traffic or repeated write requests can exhaust resources (DoS) and degrade availability.

- Solution:
  - Global sliding-window limiter partitioned by identity + concurrency limiter for critical write endpoints.

- Implementation:
  - Service registration: `services.AddRateLimiting()` registers `AddRateLimiter` with:
    - Global `PartitionedRateLimiter.Create<HttpContext, string>(...)` keyed by identity derived from claims or remote IP (fall back to IP).
    - `options.GlobalLimiter = <sliding-window>` configured with PermitLimit, Window and QueueLimit.
    - Named `options.AddConcurrencyLimiter("writes", o => { o.PermitLimit = 10; o.QueueLimit = 10; });`
  - Middleware mapping: `app.UseRateLimiting()` placed early, after HTTP logging, so rejections are logged.
  - Endpoint binding: apply `.RequireRateLimiting("writes")` on POST/create endpoints.

- Config keys: externalize thresholds in `RateLimiting:*` for future tuning.

- Verification:
  - Run load tests to exceed limits and confirm 429 responses and `Retry-After` header when provided. Confirm concurrent write limiter rejects beyond permit+queue.

---

## 4) API Documentation (OpenAPI)
- Problem:
  - Developers need machine-readable API contracts and a secured Swagger UI for local testing; however, exposing docs in production increases attack surface.

- Solution:
  - Generate OpenAPI documents and expose UI only in development.

- Implementation:
  - Service registration: `services.AddApiDocumentation()` adds OpenAPI/NSwag/Swashbuckle registration and a document transformer that injects a Bearer security scheme.
  - Middleware mapping: `app.MapApiDocumentation()` only maps the `/openapi/{document}.json` and related UI when `env.IsDevelopment()`.

- Config keys: none required.

- Verification:
  - In dev, navigate to `/swagger` or configured UI and confirm Bearer scheme present and docs reflect endpoints and DTOs.

---

## 5) API Admin (Swagger Admin UI)
- Problem:
  - Ops/developers need read?only interactive interface for debugging; must not be enabled in prod.

- Solution:
  - Provide a gated admin UI mapped by `MapApiAdmin()` only in non?production or behind internal network.

- Implementation:
  - Service registration: `services.AddApiAdmin()` configures the UI endpoint.
  - Mapping: `app.MapApiAdmin()` executed in `MapCommon` but conditional on `env.IsDevelopment()` (or further gating via config). Place behind rate limiting and auth in non?dev scenarios if required.

- Verification:
  - Confirm UI accessible in dev and inaccessible in prod environment.

---

## 6) API Versioning
- Problem:
  - Breaking changes require a versioning strategy to allow side?by?side support and graceful deprecation.

- Solution:
  - Configure API versioning with default version, URL segment and header readers, and API explorer grouping.

- Implementation:
  - Service registration: `services.AddApiVersioning(o => { o.DefaultApiVersion = new ApiVersion(1,0); o.AssumeDefaultVersionWhenUnspecified = true; o.ReportApiVersions = true; o.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader("X-API-Version")); }).AddApiExplorer(o => { o.GroupNameFormat = "'v'VVV"; o.SubstituteApiVersionInUrl = true; });`
  - Endpoint usage: modules create `var versionSet = app.NewApiVersionSet().HasApiVersion(new ApiVersion(1,0)).Build();` and map groups under `/v{version:apiVersion}/...`.

- Verification:
  - Call `/v1/orders` and verify success; call `/orders` without version honors default; check response headers `api-supported-versions`.

---

## 7) Error Handling (RFC7807)
- Problem:
  - Applications returning inconsistent error shapes complicate client handling and auditing; raw stack traces may leak sensitive data.

- Solution:
  - Centralize exception handling to normalize errors to RFC7807 ProblemDetails with safe fields (status, title, traceId, correlationId, details when safe).

- Implementation:
  - Service registration: `services.AddErrorHandling()` registers ProblemDetails options and maps known exceptions (ValidationException ? 400, UnauthorizedAccessException ? 403, etc.).
  - Middleware mapping: `app.UseErrorHandling()` placed early so it can format exceptions from any middleware or endpoint.
  - Include `traceId` (HttpContext.TraceIdentifier) and correlation id in ProblemDetails `extensions` for audit linkage.

- Verification:
  - Force an exception; examine response body JSON conforms to RFC7807 and includes `traceId`/`correlationId` but no stack trace in prod.

---

## 8) Browser Request Restrictions (CORS)
- Problem:
  - Unrestricted cross?origin requests may allow malicious client pages to call APIs.

- Solution:
  - Provide a configurable CORS policy named `DefaultCors` using `Cors:AllowedOrigins`.

- Implementation:
  - Service registration: `services.AddBrowserRequestRestrictions(configuration)` reads `Cors:AllowedOrigins` and registers policy: `builder.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()` only when origins present.
  - Middleware mapping: `app.UseBrowserRequestRestrictions()` invokes `app.UseCors("DefaultCors")`.

- Verification:
  - Browser front?end served from configured origin can call API; calls from other origins are blocked by browser (CORS preflight fails).

---

## 9) JSON Hardening
- Problem:
  - Comment?rich or extremely nested JSON bodies can be abused to cause excessive CPU/memory usage or ambiguous binding.

- Solution:
  - Harden System.Text.Json options: disallow comments, set MaxDepth=32, enforce property name case sensitivity.

- Implementation:
  - In `AddJson()` call `services.ConfigureHttpJsonOptions(o => { o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Disallow; o.SerializerOptions.MaxDepth = 32; o.SerializerOptions.PropertyNameCaseInsensitive = false; });`

- Verification:
  - Submit a JSON payload with comments ? request rejected; very deep nested payload truncated or rejected per MaxDepth.

---

## 10) Authentication (JWT)
- Problem:
  - Need robust token validation (issuer, audience, signature) and support for development tokens without weakening prod security.

- Solution:
  - Configure JWT Bearer authentication with strict validation parameters and provide a dev-only token issuance endpoint.

- Implementation:
  - Service registration: `services.AddAuthentication(config, env)` binds `Auth:Issuer`, `Auth:Audience` and uses `TokenValidationParameters` with valid issuer, audience and issuer signing key (production: from cert/keystore; development: `Auth:DevKey`).
  - Middleware mapping: `app.UseAuthentication(); app.UseAuthorization();` (already present in MapCommon).
  - Dev helper: `MapDevToken()` only when `env.IsDevelopment()` issues short lived tokens (10 min) signed with `Auth:DevKey`.

- Config keys: `Auth:Audience`, `Auth:Issuer`, `Auth:DevKey`, `Auth:DevScopes`.

- Verification:
  - Acquire dev token in dev and call protected endpoint; call same in prod should fail (endpoint not mapped).

---

## 11) Request Logging (HTTP Logging)
- Problem:
  - Need structured request/response telemetry for audit while preventing sensitive data leakage.

- Solution:
  - Use `HttpLoggingMiddleware` capturing request/response metadata and selected headers; explicitly exclude Authorization, Cookie and Set-Cookie.

- Implementation:
  - Service registration: `services.AddRequestLogging()` configures `HttpLoggingFields` and `RequestHeaders`/`ResponseHeaders` to capture; add `X-Correlation-ID` to captured headers.
  - Middleware mapping: `app.UseHttpLogging()` early to capture raw request details before other transformations.

- Verification:
  - Make requests and confirm logs include method/path/duration and `X-Correlation-ID`, but do not include Authorization header values.

---

## 12) Observability (OpenTelemetry)
- Problem:
  - Lack of unified traces and metrics across services impairs root-cause analysis.

- Solution:
  - Register OpenTelemetry tracing and metrics instrumentation and export to OTLP collector; fail startup if OTLP endpoint required but missing.

- Implementation:
  - Service registration: `services.AddObservability(config, env)` builds `ResourceBuilder`, adds `AddAspNetCoreInstrumentation`, `AddHttpClientInstrumentation`, `AddRuntimeInstrumentation`, and `AddOtlpExporter` using `Observability:OtlpEndpoint`.
  - Ensure spans add attributes for `traceId` and correlation id via middleware/enrichers.

- Config keys: `Observability:OtlpEndpoint` (required when not development).

- Verification:
  - Start app with OTLP endpoint and confirm traces appear in collector for requests and errors; missing endpoint in prod causes startup exception.

---

## 13) Health Checks
- Problem:
  - Orchestrators need quick liveness/readiness checks.

- Solution:
  - Register health checks; map `/health/live` and `/health/ready` endpoints.

- Implementation:
  - Service registration: `services.AddFullHealthCheck()` adds `AddHealthChecks()` and basic self check; allow modules to register their own checks (DB, cache) in their `Add*` methods.
  - Mapping: `app.MapFullHealthCheck()` maps endpoints with optional IP filter.

- Verification:
  - GET `/health/live` returns 200 when app alive; `/health/ready` returns 200 only when all registered checks pass.

---

## 14) Allowed IPs for Health Probes
- Problem:
  - Readiness endpoints may expose internal application state to the public internet.

- Solution:
  - Use `IpEndpointFilter` on health endpoints to allow only configured IPs (supports "localhost"), otherwise deny with 403.

- Implementation:
  - Service registration: `services.AddAllowedIPsForHealthProbes(config)` reads `Health:AllowedIps` and registers an `IpEndpointFilter` instance for health routes.
  - Mapping: `MapFullHealthCheck()` applies the filter to readiness endpoints.

- Verification:
  - Requests to health endpoints from non?allowed IPs return 403; allowed IPs succeed.

---

## 15) Business Event Publisher
- Problem:
  - Modules need to publish domain events without tight coupling or direct references.

- Solution:
  - Provide an in?process `IBusinessEventPublisher` implementation registered scoped so modules can publish events; later replaceable by distributed bus.

- Implementation:
  - Service registration: `services.AddScoped<IBusinessEventPublisher, InProcessBusinessEventPublisher>();`
  - Publisher interface: `Task PublishAsync<T>(T evt);` that dispatches to in?process handlers.

- Verification:
  - Publish an event in Orders module and have Billing handler receive it in same process (unit/integration test).

---

## Mapping Order (MapCommon) – exact sequence
1. `app.UseHttps()` (? HSTS in non?dev)
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

## How to extend / add a new cross?cutting concern
1. Create `XFeatureExtensions` with `AddXFeature(IServiceCollection, IConfiguration)` and `UseXFeature(WebApplication)` if middleware is needed.
2. Add a single line call inside `CommonExtensions.AddCommon` at the correct ordering point.
3. Add `app.UseXFeature()` in `CommonExtensions.MapCommon` where ordering matters.
4. Add configuration keys under `XFeature:*` and a `ValidateOnStart` check if critical.
5. Add unit/integration tests verifying behavior and failure modes.

---

This specification is intentionally prescriptive: follow the Implementation sections to make changes in code with minimal surprises for maintainers and auditors.
