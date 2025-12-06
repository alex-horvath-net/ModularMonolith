using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Configuration;

public static class SecretValidationExtensions
{
    public static IServiceCollection ValidateSecretsOnStart(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {
        // Perform fail-fast checks for production; warn in development
        var missing = new List<string>();

        // Connection string presence
        var conn = config.GetConnectionString("AppDB");
        if (string.IsNullOrWhiteSpace(conn))
        {
            missing.Add("ConnectionStrings:AppDB");
        }
        else
        {
            // If connection string contains Password= ensure it's not empty or a placeholder
            var lower = conn.ToLowerInvariant();
            var pwdIndex = lower.IndexOf("password=", StringComparison.OrdinalIgnoreCase);
            if (pwdIndex >= 0)
            {
                var after = conn.Substring(pwdIndex + "password=".Length);
                var end = after.IndexOf(';');
                var pwd = end >= 0 ? after.Substring(0, end) : after;
                if (string.IsNullOrWhiteSpace(pwd))
                {
                    missing.Add("ConnectionStrings:AppDB (missing password)");
                }
            }
        }

        // DataProtection: KeyDirectory or certificate
        var dpKeyDir = config["DataProtection:KeyDirectory"]; 
        var dpCertPath = config["DataProtection:CertificatePath"]; 
        var dpCertThumb = config["DataProtection:CertificateThumbprint"]; 
        if (string.IsNullOrWhiteSpace(dpKeyDir) && string.IsNullOrWhiteSpace(dpCertPath) && string.IsNullOrWhiteSpace(dpCertThumb))
        {
            missing.Add("DataProtection:KeyDirectory or DataProtection:CertificatePath/DataProtection:CertificateThumbprint");
        }

        // Signing certificate / auth key: do not allow DevKey in non-dev
        var authDevKey = config["Authentication:DevKey"]; // development-only
        var certThumb = config["Certificates:Service:Thumbprint"] ?? config["Certificates:WebApi:Thumbprint"] ?? config["DataProtection:CertificateThumbprint"];
        if (!env.IsDevelopment())
        {
            if (string.IsNullOrWhiteSpace(certThumb))
            {
                missing.Add("Certificates:Service:Thumbprint or DataProtection:CertificateThumbprint (signing certificate/thumbprint)");
            }
            // DB connection string password already checked above.
        }

        if (missing.Count > 0)
        {
            var msg = "Missing critical configuration/secrets: " + string.Join(", ", missing);
            if (env.IsDevelopment())
            {
                // In development warn but do not fail
                Console.WriteLine("WARNING: " + msg);
                return services;
            }

            throw new InvalidOperationException(msg);
        }

        return services;
    }
}
