# Orders Module Design – Implementation?ready Specification

This document specifies the Orders module (API, application, infrastructure) with Problem/Solution/Service registration/Middleware mapping/Config keys/Verification sections to guide implementation.

---

## 1) Purpose
- Problem:<br>
  Provide APIs to create and read orders with strong validation, transactional integrity, and auditability.

- Solution:<br>
  Implement minimal API endpoints mapped under `/v{version}/orders` with application handlers, validators (FluentValidation), and EF Core persistence with checks and row versioning.

- Service registration:<br>
  - Register handlers: `services.AddScoped<GetOrdersQueryHandler>()`, `AddScoped<CreateOrderCommandHandler>()`, etc.
  - Register `IValidator<CreateOrderCommand>` and `IReadOrderService`.
  - Register `OrdersDbContext` with `UseSqlServer(configuration.GetConnectionString("AppDB"), sql => sql.EnableRetryOnFailure(...));` and environment?specific logging flags.

- Middleware mapping:<br>
  - `app.MapOrders()` which executes migrations in development and maps `MapOrdersEndpoints()`.

- Config keys:<br>
  - Uses `ConnectionStrings:AppDB` and common configuration keys.

- Verification:<br>
  - Integration tests for `GET /v1/orders`, `POST /v1/orders` with validation, DB persistence, and concurrency handling.

---

## 2) Validation & Input Handling
- Problem:<br>
  Invalid input may reach business logic or DB causing data corruption.

- Solution:<br>
  Use FluentValidation validators and return RFC7807 ProblemDetails for validation errors.

- Service registration:<br>
  - `services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>()` and call validator in endpoint before handler.

- Middleware mapping:<br>
  - Endpoint handlers invoke validator and return `BadRequest` with structured errors on invalid input.

- Verification:<br>
  - Submit invalid orders and assert 400 with structured error list.

---

## 3) Persistence & Migrations
- Problem:<br>
  DB schema must enforce integrity and support migrations in CI/CD.

- Solution:<br>
  Use EF Core with migrations and check constraints; apply migrations in dev during MapOrders if desired.

- Service registration:<br>
  - `services.AddDbContext<OrdersDbContext>(...)` with `sql.MigrationsHistoryTable("__EFMigrationsHistory", "orders")`.

- Verification:<br>
  - Migrations apply and tables created with expected constraints; seeded sample data present in dev.

---

## 4) Authorization
- Problem:<br>
  Need role/scope based access to endpoints.

- Solution:<br>
  Define authorization policies `orders.read` and `orders.write` and apply via `.RequireAuthorization(OrdersConstants.Read)` etc.

- Service registration:<br>
  - `services.AddAuthorizationBuilder().AddPolicy(OrdersConstants.Read, p => p.RequireClaim("scope","orders.read"))...`

- Verification:<br>
  - Protected endpoints require tokens containing required scopes; invalid tokens receive 403/401.

---

## 5) Rate Limiting for Writes
- Problem:<br>
  High write concurrency can stress DB and cause contention.

- Solution:<br>
  Apply `.RequireRateLimiting("writes")` to POST endpoints to limit concurrency.

- Middleware mapping:<br>
  - Ensure MapOrdersEndpoints decorates create endpoints accordingly.

- Verification:<br>
  - Simulate concurrent creates and assert limited concurrency and correct status codes under overload.

---

## 6) Observability
- Problem:<br>
  Need domain-level traces for order operations.

- Solution:<br>
  Emit activities using `ActivitySource` and rely on Common's OpenTelemetry configuration; add `AddSource("Orders")` in observability.

- Verification:<br>
  - Traces in OTLP collector show `Orders` operations and include correlation IDs.

---

## 7) Tests
- Problem:<br>
  Ensure correctness of handler logic, validation, and DB interactions.

- Solution:<br>
  Unit tests for handlers and validators; integration tests with TestServer or containerized DB for end-to-end validation.

- Verification:<br>
  - Unit tests pass; integration tests validate DB persistence and endpoint contracts.

---

This file guides implementers to wire Orders module correctly within the Common orchestrator.
