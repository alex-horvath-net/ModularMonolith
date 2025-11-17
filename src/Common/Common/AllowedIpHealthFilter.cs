using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

// AllowedIpHealthFilter acts as an endpoint filter enforcing an IP allowlist for health endpoints.
public sealed class AllowedIpHealthFilter : IEndpointFilter {
    private readonly HashSet<string> _allowed = new(StringComparer.OrdinalIgnoreCase);

    public AllowedIpHealthFilter(IEnumerable<string>? ips) {
        if (ips != null) {
            foreach (var ip in ips) {
                if (string.IsNullOrWhiteSpace(ip)) continue;
                var s = ip.Trim();

                // Parse and store both canonical and mapped forms so "::ffff:x.x.x.x" matches "x.x.x.x"
                if (IPAddress.TryParse(s, out var parsed)) {
                    _allowed.Add(parsed.ToString());
                    if (parsed.AddressFamily == AddressFamily.InterNetwork) {
                        _allowed.Add(parsed.MapToIPv6().ToString()); // "::ffff:x.x.x.x"
                    }
                } else if (string.Equals(s, "localhost", StringComparison.OrdinalIgnoreCase)) {
                    _allowed.Add(IPAddress.Loopback.ToString());            // "127.0.0.1"
                    _allowed.Add(IPAddress.IPv6Loopback.ToString());        // "::1"
                    _allowed.Add(IPAddress.Loopback.MapToIPv6().ToString()); // "::ffff:127.0.0.1"
                }
            }
        }
    }

    private bool IsAllowed(HttpContext context) {
        // If no allowlist configured, allow all (dev-friendly).
        if (_allowed.Count == 0) return true;

        var remote = context.Connection.RemoteIpAddress;
        if (remote is null) return false;

        // Check raw representation
        if (_allowed.Contains(remote.ToString())) return true;

        // If remote is IPv6-mapped IPv4, also check its IPv4 form
        if (remote.IsIPv4MappedToIPv6 && _allowed.Contains(remote.MapToIPv4().ToString())) return true;

        return false;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next) {
        if (!IsAllowed(invocationContext.HttpContext)) {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }
        return await next(invocationContext);
    }
}
