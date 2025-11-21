using System.Security.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace Common; 
public static class KerstelExtensions {


    public static ConfigureWebHostBuilder UseKestrel(this ConfigureWebHostBuilder webHost) {
        
        webHost.ConfigureKestrel(options => {
            options.AddServerHeader = false;
            options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
            options.Limits.MinRequestBodyDataRate = new MinDataRate(240, TimeSpan.FromSeconds(5));
            options.Limits.MinResponseDataRate = new MinDataRate(240, TimeSpan.FromSeconds(5));

            options.ConfigureHttpsDefaults(https => {
                https.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;
                https.CheckCertificateRevocation = true;
                https.ClientCertificateMode = ClientCertificateMode.NoCertificate;
            });
        });

        return webHost;
    }
}
