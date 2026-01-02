# ApplicationPortal Design – Implementation?ready Specification

This document specifies the ApplicationPortal host (server?side Blazor) behaviour, security, and operational requirements. It follows the same structured Problem/Solution/Service registration/Middleware mapping/Config keys/Verification format as the Common design.

---

## 1) Purpose
- Problem:<br>
  Web UI must render interactive components, maintain secure user sessions, and communicate with backend WebApi without exposing secrets or weakening CSP.

- Solution:<br>
  Implement server?side Blazor host that configures DataProtection, cookie authentication, CSP nonce injection for Blazor boot script, and a typed HttpClient for WebApi calls.

- Service registration:<br>
  - `builder.Services.AddDataProtection().SetApplicationName("ModularMonolith").SetDefaultKeyLifetime(TimeSpan.FromDays(90));` with key persistence configured (filesystem in dev; encrypted store and shared directory in prod).
  - `builder.Services.AddRazorComponents().AddInteractiveServerComponents();` and `builder.Services.AddHttpClient("WebApi")` bound to `WebApi:BaseUrl`.

- Middleware mapping:<br>
  - Call `app.MapRazorComponents<App>().AddInteractiveServerRenderMode();` and ensure `UseAntiforgery`, `UseAuthentication`, `UseAuthorization` applied by Common via `MapCommon`.

- Config keys:<br>
  - `WebApi:BaseUrl`, `DataProtection:KeyDirectory`, `DataProtection:CertificatePath|CertificateThumbprint`.

- Verification:<br>
  - Blazor app loads in browser; boot script uses CSP nonce; user login sets cookie with HttpOnly/Secure and DataProtection keys persist to configured store.

---

## 2) Data Protection
- Problem:<br>
  Cookie integrity and protection of cryptographic keys require secure storage and rotation.

- Solution:<br>
  Configure DataProtection with application name, key lifetime, and persist keys to shared secure directory; protect keys with certificate in non?dev.

- Service registration:<br>
  - `AddDataProtection()` with `PersistKeysToFileSystem(new DirectoryInfo(keyDir))` and `ProtectKeysWithCertificate(cert)` in non?dev.

- Verification:<br>
  - Restart host and confirm cookies remain valid; keys persisted to configured `DataProtection:KeyDirectory`.

---

## 3) CSP Nonce Injection
- Problem:<br>
  Blazor server requires a boot script; CSP must allow it without enabling unsafe-inline globally.

- Solution:<br>
  Use SecurityHeaders middleware generating per-request nonce stored in `HttpContext.Items["csp-nonce"]` and Razor injects nonce attribute on `<script>` tag.

- Implementation:<br>
  - `App.razor` includes `<script src="_framework/blazor.web.js" nonce="@Nonce">` where `Nonce` retrieved from `HttpContext.Items`.

- Verification:<br>
  - Load page and confirm CSP header contains `nonce-...` and script loads without CSP violation in browser console.

---

## 4) Cookie Authentication
- Problem:<br>
  Need secure session cookies for UI while avoiding CSRF and session stealing.

- Solution:<br>
  Cookie authentication configured with `HttpOnly=true`, `SecurePolicy=Always`, `SameSite=Strict`, sliding expiration and short timeouts.

- Service registration:<br>
  - `AddAuthentication().AddCookie(o => { o.LoginPath = "/login"; o.SlidingExpiration = true; o.ExpireTimeSpan = TimeSpan.FromMinutes(30); o.Cookie.HttpOnly = true; o.Cookie.SecurePolicy = CookieSecurePolicy.Always; o.Cookie.SameSite = SameSiteMode.Strict; });`

- Verification:<br>
  - After login, browser cookie has correct flags; CSRF token required for state-changing POSTs.

---

## 5) HttpClient to WebApi
- Problem:<br>
  UI needs to call WebApi reliably with correct base address and resilient policies.

- Solution:<br>
  Register named HttpClient "WebApi" with BaseAddress from `WebApi:BaseUrl`; set timeout and accept headers; resilience via Polly if needed.

- Service registration:<br>
  - `builder.Services.AddHttpClient("WebApi", c => { c.BaseAddress = new Uri(baseUrl); ... });`

- Verification:<br>
  - Component uses `IHttpClientFactory.CreateClient("WebApi")` and API calls succeed in dev and prod with correct base URL.

---

## 6) Observability & Security
- Problem:<br>
  UI actions must be observable and not leak sensitive tokens.

- Solution:<br>
  Use Common's observability and security features; do not log PII; ensure correlation ID forwarded on requests to backend.

- Verification:<br>
  - Requests from UI show correlation id in backend traces; no PII in logs.

---

## 7) Deployment & Data Protection
- Problem:<br>
  DP keys and certificates must be provisioned securely in production.

- Solution:<br>
  Use secret stores (KeyVault) or mounted files; configure `DataProtection:KeyDirectory` and `Certificates:*` via environment or key vault.

- Verification:<br>
  - Production environment starts without errors; DP keys persisted and encrypted as configured.

---

## 8) Tests
- Problem:<br>
  Need to validate UI host behaviours (nonce injection, cookie flags, DP key persistence).

- Solution:<br>
  Integration tests using TestServer/WebApplicationFactory to validate CSP nonce presence, cookie flags, and HttpClient base URL.

- Verification:<br>
  - Automated tests confirm expected behavior.

---

This specification ensures ApplicationPortal is secure and interoperable with Common and WebApi; follow coding guidance closely to avoid runtime surprises.
