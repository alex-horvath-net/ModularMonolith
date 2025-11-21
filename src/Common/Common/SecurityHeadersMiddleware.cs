using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using System.Text;

namespace Common;

public static class SecurityHeadersMiddleware {
    public static IApplicationBuilder UseSecurityHeaders(this WebApplication app) => app.Use(async (context, next) => {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        headers["X-XSS-Protection"] = "0"; // modern browsers ignore, use CSP
        headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

/* Dev/Local: relax CSP for hot reload and tooling
   Prod: enforce strict CSP by removing unsafe-inline, requiring nonce, and enabling HSTS */
        var isDev = app.Environment.IsDevelopment();
        
        // Generate per-request nonce for CSP
        var nonceBytes = RandomNumberGenerator.GetBytes(16);
        var nonce = Convert.ToBase64String(nonceBytes);
        context.Items["csp-nonce"] = nonce;

        string csp;
        if (isDev) {
            // Relaxed in dev for tooling/hot-reload
            csp = $"default-src 'self' http://localhost:* https://localhost:*; script-src 'self' 'unsafe-inline' 'unsafe-eval' http://localhost:* https://localhost:*; style-src 'self' 'unsafe-inline' http://localhost:* https://localhost:*; img-src 'self' data: blob:; connect-src 'self' ws://localhost:* wss://localhost:* http://localhost:* https://localhost:*; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
        } else {
            // Strict in prod: remove unsafe-inline, require nonce
            csp = $"default-src 'self'; script-src 'self' 'nonce-{nonce}'; style-src 'self' 'nonce-{nonce}'; img-src 'self' data:; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
            headers["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains; preload";
        }
        headers["Content-Security-Policy"] = csp;

        // Cache control for sensitive content
        headers["Cache-Control"] = "no-store";
        headers["Pragma"] = "no-cache";

        await next();
    });
}
