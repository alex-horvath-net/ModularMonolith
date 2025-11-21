using System.Security.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace Common {
    public static class KerstelExtensions {

        public static ConfigureWebHostBuilder UseSecureKerstel(this ConfigureWebHostBuilder wbHost) {

            wbHost.UseKestrel(options => {
                options.AddServerHeader = false; // Hide Server header
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB

                // Enforce TLS 1.2/1.3 only when HTTPS is enabled
                options.ConfigureHttpsDefaults(https => {
                    https.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                    https.CheckCertificateRevocation = true;
                    https.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                });
            });

            return wbHost;
        }
    }
}