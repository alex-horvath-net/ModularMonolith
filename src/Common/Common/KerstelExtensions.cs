using System.Security.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Common; 
public static class KerstelExtensions {


    // Extended hardened transport configuration with certificate & port binding
    public static ConfigureWebHostBuilder UseKestrel(this ConfigureWebHostBuilder webHost, IConfiguration config, IHostEnvironment env) {
        webHost.ConfigureKestrel(options => {
            options.AddServerHeader = false;
           
            options.Limits.MaxRequestBodySize = 10_000_000; // 10MB cap
            options.Limits.MinRequestBodyDataRate = new MinDataRate(240, TimeSpan.FromSeconds(5));
            options.Limits.MinResponseDataRate = new MinDataRate(240, TimeSpan.FromSeconds(5));
            
            options.ConfigureHttpsDefaults(https => {
                https.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                https.CheckCertificateRevocation = true;
                https.ClientCertificateMode = ClientCertificateMode.NoCertificate;
            });
            
            if (env.IsDevelopment()) {
                // Dev HTTPS listener (self-signed dev cert handled automatically by ASP.NET)
                options.ListenAnyIP(5001, lo => lo.UseHttps(https => {
                    https.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                }));
            } else {
                var cert = LoadServerCertificate(config);
                options.ListenAnyIP(443, lo => lo.UseHttps(https => {
                    https.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                    https.ServerCertificate = cert;
                    https.HandshakeTimeout = TimeSpan.FromSeconds(10);
                    https.CheckCertificateRevocation = true;
                }));
                if (config.GetValue<bool>("Http:EnablePort80")) {
                    options.ListenAnyIP(80); // optional plain HTTP for external redirect logic
                }
            }
        });
        return webHost;
    }

    private static X509Certificate2 LoadServerCertificate(IConfiguration cfg) {
        // Generic service certificate keys allow reuse; fallback to WebApi-specific keys for backward compatibility
        var path = cfg["Certificates:Service:Path"] ?? cfg["Certificates:WebApi:Path"];
        var password = cfg["Certificates:Service:Password"] ?? cfg["Certificates:WebApi:Password"];
        if (!string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path)) {
#pragma warning disable SYSLIB0057
            return new X509Certificate2(path!, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
#pragma warning restore SYSLIB0057
        }
        var thumbprint = cfg["Certificates:Service:Thumbprint"] ?? cfg["Certificates:WebApi:Thumbprint"];
        if (!string.IsNullOrWhiteSpace(thumbprint)) {
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var cert = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true)
                .OfType<X509Certificate2>()
                .FirstOrDefault();
            if (cert != null) return cert;
        }
        throw new InvalidOperationException("Server certificate not found. Configure Certificates:Service:(Path|Thumbprint) or Certificates:WebApi:(Path|Thumbprint).");
    }
}
