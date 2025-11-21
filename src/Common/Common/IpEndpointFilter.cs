using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

// AllowedIpHealthFilter acts as an endpoint filter enforcing an IP allowlist for health endpoints.
public sealed class IpEndpointFilter : IEndpointFilter {
    private readonly HashSet<string> _allowed = new(StringComparer.OrdinalIgnoreCase);

    public IpEndpointFilter(IEnumerable<string>? ip_v4_List) {
        if (ip_v4_List != null) {
            foreach (var ip_v4 in ip_v4_List) {
               
                
                if (string.IsNullOrWhiteSpace(ip_v4)) 
                    continue;

                if (string.Equals(ip_v4.Trim(), "localhost", StringComparison.OrdinalIgnoreCase)) {
                    _allowed.Add(IPAddress.Loopback.ToString());             // "127.0.0.1"
                    _allowed.Add(IPAddress.Loopback.MapToIPv6().ToString()); // "::ffff:127.0.0.1"
                    _allowed.Add(IPAddress.IPv6Loopback.ToString());         // "::1"
                    continue;
                }

                // Parse and store both canonical and mapped forms so "::ffff:x.x.x.x" matches "x.x.x.x"
                if (IPAddress.TryParse(ip_v4.Trim(), out var ip_v4_ipaddress)) {
                    _allowed.Add(ip_v4_ipaddress.ToString());
                    if (ip_v4_ipaddress.AddressFamily == AddressFamily.InterNetwork) {
                        var ip_v6_ipaddress= ip_v4_ipaddress.MapToIPv6();  // "::ffff:x.x.x.x"
                        _allowed.Add(ip_v6_ipaddress.ToString());
                    }
                } 
            }
        }
    }

    private bool IsAllowed(HttpContext context) {
        // If no allowlist configured, allow all (dev-friendly).
        if (_allowed.Count == 0) return true;

        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp is null) return false; 

        // Check raw representation
        if (_allowed.Contains(remoteIp.ToString())) return true;

        // If remoteIp is IPv6-mapped IPv4, also check its IPv4 form
        if (remoteIp.IsIPv4MappedToIPv6 && _allowed.Contains(remoteIp.MapToIPv4().ToString())) return true;

        return false;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext invocationContext, EndpointFilterDelegate next) {
        if (!IsAllowed(invocationContext.HttpContext)) {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }
        return await next(invocationContext);
    }
}
