using Microsoft.AspNetCore.Builder;

namespace Common;

public static class SecurityHeadersMiddleware {
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, bool isDev) => app.Use(async (context, next) => {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        headers["X-XSS-Protection"] = "0"; // modern browsers ignore, use CSP
        headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

        // Basic CSP; in dev we relax for hot reload
        var csp = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'";
        if (isDev) {
            csp = "default-src 'self' http://localhost:* https://localhost:*; script-src 'self' 'unsafe-inline' 'unsafe-eval' http://localhost:* https://localhost:*; style-src 'self' 'unsafe-inline' http://localhost:* https://localhost:*; img-src 'self' data: blob:; connect-src 'self' ws://localhost:* wss://localhost:* http://localhost:* https://localhost:*; frame-ancestors 'none'; base-uri 'self'";
        }
        headers["Content-Security-Policy"] = csp;

        await next();
    });
}
