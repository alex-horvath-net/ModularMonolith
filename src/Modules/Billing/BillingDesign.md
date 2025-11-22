# Billing Module Design

## Purpose
Generate and manage invoices derived from order data; provide read endpoints for financial reporting and downstream integration.

## Architecture
- API layer: Minimal endpoints under `/v{version}/billing`.
- Application layer: Query handlers (e.g., `GetInvoiceQueryHandler`).
- Infrastructure layer: EF Core `BillingDbContext` + migrations.

## Data Model
- `Invoice` entity with immutable identity (GUID), foreign keys to `Order` and `Customer`, monetary `Total` with precision (19,4), timestamp `CreatedAtUtc` defaulted via database.
- Check constraints ensure non-negative totals and valid GUIDs.

## API Endpoints
- GET `/v1/billing/invoices/{id}` returns `InvoiceDto`.

## Security
- Authorization policy: `billing.read` scope required.
- Global rate limiting plus read limiter `fixed` per Common configuration.

## Observability
- Included in OpenTelemetry resource (`AddSource("Billing")`) for tracing.

## Error Handling
- Null/missing invoice results in 404; exceptions handled globally via Common error middleware.

## Extensibility
- Additional endpoints (list, search, create) can be added with new handlers and DTOs.
- Future API version: add ApiVersionSet and new route group `/v{version}/billing` while retaining v1.

## Non-Goals
- Payment processing, tax calculation (outside scope).
