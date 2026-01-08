# Billing Module Design – Implementation?ready Specification

This document specifies the Billing module with explicit Problem/Solution/Service registration/Middleware mapping/Config keys/Verification sections to guide implementation and tests.

---

## 1) Purpose
- Problem:<br>
  Provide invoice retrieval and financial reporting APIs decoupled from order creation while maintaining data integrity and auditability.

- Solution:<br>
  Expose minimal API endpoints under `/v{version}/billing` that return DTOs and rely on EF Core persistence with appropriate constraints and precision for monetary values.

- Service registration:<br>
  - Register `BillingDbContext` with migrations, precise decimal mapping (19,4), and retry policies.
  - Register `GetInvoiceQueryHandler` and query services.

- Middleware mapping:<br>
  - `app.MapBilling()` which maps versioned endpoints and applies `.RequireAuthorization("Billing.Read")`.

- Config keys:<br>
  - Uses common `ConnectionStrings:AppDB` and Observability/RateLimiting from Common.

- Verification:<br>
  - Integration tests for `GET /v1/billing/invoices/{id}` returning `InvoiceDto` and correct 404 for missing invoices.

---

## 2) Data Model & Constraints
- Problem:<br>
  Monetary values and foreign keys need precision and integrity checks to avoid inconsistent invoices.

- Solution:<br>
  Use EF Core with `decimal(19,4)` mapping, check constraints ensuring non-negative totals and non-empty GUIDs, and DB default timestamp for `CreatedAtUtc`.

- Service registration:<br>
  - `modelBuilder.HasDefaultSchema("billing")` and entity configurations in `Billing.Infrastructure.Data.Configurations`.

- Verification:<br>
  - DB schema contains correct constraints; attempts to insert invalid data are rejected by DB.

---

## 3) Authorization
- Problem:<br>
  Billing data is sensitive; require explicit scope to read invoices.

- Solution:<br>
  Require `billing.read` scope via policy and annotate endpoints with `.RequireAuthorization("Billing.Read")`.

- Service registration:<br>
  - Register authorization policy in module extensions.

- Verification:<br>
  - Calls without `billing.read` scope receive 403.

---

## 4) DTO Mapping
- Problem:<br>
  Entities may contain internal fields that should not be exposed in API contracts.

- Solution:<br>
  Map entities to `InvoiceDto` before returning; document DTOs in OpenAPI.

- Implementation:<br>
  - Endpoint returns `TypedResults.Ok(new InvoiceDto(...))` and uses `.Produces<InvoiceDto>(200)`.

- Verification:<br>
  - Swagger shows `InvoiceDto`; responses do not leak internal DB fields (e.g., RowVersion).

---

## 5) Observability
- Problem:<br>
  Billing operations need traceability into OTLP collector for audit and latency analysis.

- Solution:<br>
  Use `ActivitySource` with `AddSource("Billing")` configured in Common observability.

- Verification:<br>
  - Billing spans appear in collector with appropriate attributes and correlation IDs.

---

## 6) Tests
- Problem:<br>
  Ensure correctness of query handlers and mapping.

- Solution:<br>
  Unit tests for handlers and DTO mapping; integration tests with DB verifying data retrieval and constraints.

- Verification:<br>
  - Tests validate expected results and error handling.

---

This specification instructs implementers how to wire Billing correctly within Common and ensures API contracts are safe and auditable.
