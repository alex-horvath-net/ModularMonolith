# Orders Module Design

## Purpose
Manage customer orders lifecycle: creation, retrieval, and line items; expose versioned REST endpoints; publish domain data to other modules (e.g., Billing).

## Architecture
- Layered: API (Minimal endpoints) -> Application (Handlers, Validators, Query Services) -> Infrastructure (EF Core DbContext, configurations).
- Decoupled DTO boundary (`OrderDto`) used for external contracts; entities internal to infrastructure.

## Data Model
- `Order` aggregate with owned collection `OrderLine` (EF Core owned entity mapping to `OrderLines` table).
- Concurrency via RowVersion and auditing timestamps `CreatedUtc`, `UpdatedUtc`.
- Check constraints enforce valid state (non-empty CustomerId, temporal ordering, positive Quantity, non-negative UnitPrice).

## Persistence
- SQL Server with retry policy (`EnableRetryOnFailure`), migrations history in schema `orders`.
- Configuration classes encapsulate mappings (`OrderConfiguration`).

## API Endpoints
- GET `/v1/orders` list orders.
- GET `/v1/orders/{id}` retrieve single order.
- POST `/v1/orders` create order (validation + rate limiting `writes`).

## Validation
- FluentValidation for `CreateOrderCommand` ensures command integrity before persistence.

## Security
- Authorization policies: `orders.read`, `orders.write` via scope claims.
- Rate limiting concurrency policy on POST.

## Observability
- Activities traced (OpenTelemetry instrumentation auto-captures ASP.NET Core + HttpClient).

## Error Handling
- Application exceptions bubble to global handler producing RFC7807 responses.

## Extensibility
- New commands/queries added through handler classes and endpoint mapping updates.
- Additional versions: duplicate endpoint group with new ApiVersionSet.

## Non-Goals
- UI concerns.
- Direct billing logic (handled in Billing module).
