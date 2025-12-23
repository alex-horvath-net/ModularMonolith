# WebApi Design – Implementation?ready Specification

This document describes the WebApi host implementation details, responsibilities, configuration, and verification steps. It follows the same Problem/Solution/Service registration/Middleware mapping/Config keys/Verification structure used in `CommonDesign.md` so engineers can implement or audit code without surprise.

---

## 1) Purpose
- Problem:<br>
  Need a dedicated host to expose module APIs (Orders, Billing) with centralized cross?cutting concerns but host-specific configurations (ports, certificates, data protection persistence).

- Solution:<br>
  Implement a lean Program.cs that: configures Kestrel via `Common.KerstelExtensions.UseKestrel(builder.Configuration, builder.Environment)`; calls `builder.Host.UseLogger()`; invokes `builder.Services.AddCommon(...)` then modules `AddOrders`, `AddBilling`; maps common and module endpoints and runs the app.

- Implementation details:<br>
  - Program.cs uses only orchestration calls. Host?specific responsibilities include certificate provisioning for Kestrel, data protection key persistence (DataProtection:KeyDirectory), and environment-specific overrides.
  - Ensure `builder.WebHost.UseKestrel(builder.Configuration, builder.Environment)` is invoked before `AddCommon` so transport is hardened early.

- Config keys:<br>
  - `Certificates:Service:Path|Password|Thumbprint` or `Certificates:WebApi:*` for backward compatibility.
  - `DataProtection:KeyDirectory`, `DataProtection:CertificatePath|CertificateThumbprint` for DP key encryption in prod.

- Verification:<br>
  - Start host in development and production-like modes; confirm Kestrel binds expected ports, HSTS header presence in prod, and DataProtection keys persisted to configured directory when set.

---

## 2) Module Registration
- Problem:<br>
  Module services and DB contexts must be registered after Common to ensure policies (auth, logging) are available to their configuration.

- Solution:<br>
  Register modules via `builder.Services.AddOrders(configuration); builder.Services.AddBilling(configuration);` ensuring their DI registrations and EF contexts are configured.

- Implementation details:<br>
  - Modules should configure their DbContexts with retry policies and environment-specific logging (sensitive data logging only in dev).
  - Migrations can be applied automatically in development (controlled by `env.IsDevelopment()` in module Map extension).

- Verification:<br>
  - Run migrations and ensure DB schema created and seeded in dev; integration tests hit module endpoints and confirm expected behaviours.

---

## 3) Middleware & Endpoint Mapping
- Problem:<br>
  Pipeline ordering matters for security and observability.

- Solution:<br>
  After building, call `app.MapCommon(); app.MapOrders(); app.MapBilling(); app.Run();` honoring Common's pipeline order.

- Implementation details:<br>
  - Module Map methods should use versioned route groups and apply module-level policies (e.g., `.RequireAuthorization(OrdersConstants.Read)`).

- Verification:<br>
  - Confirm endpoints return expected status codes; protected endpoints require tokens; CSP nonce present in responses for Blazor assets if applicable.

---

## 4) Observability & Logging
- Problem:<br>
  Host must emit structured logs and OTel traces with service-specific resource attributes.

- Solution:<br>
  Use Serilog via `builder.Host.UseLogger()` and ensure `AddObservability` in Common is called; set resource name `service.name=WebApi` in Observability config.

- Verification:<br>
  - Check logs contain `Application=WebApi`, `Environment`, and correlation ids; traces appear in OTLP collector if configured.

---

## 5) Deployment
- Problem:<br>
  Production deployment must provide certificates and DP key dir securely.

- Solution:<br>
  Use secure secret stores (KeyVault/AKV) or mount read-only PFX and set env vars for `Certificates` and `DataProtection:KeyDirectory`.

- Verification:<br>
  - CI/CD pipeline passes secrets via env or secret mounts; runtime starts without throwing missing config exceptions.

---

## 6) Tests
- Problem:<br>
  Need to verify host-level guarantees (TLS binding, HSTS, DP persistence).

- Solution:<br>
  Integration tests using WebApplicationFactory and test certificates for TLS; smoke tests for endpoints and health checks.

- Verification:<br>
  - Integration pipeline validates TLS/HSTS and endpoint behaviour.

---

This specification keeps the WebApi host minimal and predictable; all cross?cutting behaviors are delegated to Common and modules.
