using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Common.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using WebPortal; // added for WebApiOptions
using WebPortal.Components;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap configuration: load secrets from Key Vault or user-secrets before any service registration
builder.Configuration.AddSecretsFromStore(builder.Environment);

// Bind & validate WebApi options (fail fast if BaseUrl missing or invalid)
builder.Services.AddOptions<WebApiOptions>()
    .Bind(builder.Configuration.GetSection("WebApi"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "WebApi:BaseUrl missing")
    .Validate(o => Uri.IsWellFormedUriString(o.BaseUrl!, UriKind.Absolute), "WebApi:BaseUrl invalid URI")
    .ValidateOnStart();

// Hardened Data Protection configuration
var dataProtection = builder.Services.AddDataProtection()
    .SetApplicationName("ModularMonolith")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // rotation policy

// Load encryption certificate (non-development) from configured path or store thumbprint
if (!builder.Environment.IsDevelopment()) {
    var certPath = builder.Configuration["DataProtection:CertificatePath"];
    var certPassword = builder.Configuration["DataProtection:CertificatePassword"];
    X509Certificate2? cert = null;
    if (!string.IsNullOrWhiteSpace(certPath) && File.Exists(certPath)) {
        cert = new X509Certificate2(certPath, certPassword, X509KeyStorageFlags.MachineKeySet);
    } else {
        var thumbprint = builder.Configuration["DataProtection:CertificateThumbprint"];
        if (!string.IsNullOrWhiteSpace(thumbprint)) {
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            cert = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false)
                .OfType<X509Certificate2>()
                .FirstOrDefault();
        }
    }
    if (cert is null) {
        throw new InvalidOperationException("Data Protection encryption certificate missing. Configure DataProtection:CertificatePath or DataProtection:CertificateThumbprint.");
    }
    dataProtection.ProtectKeysWithCertificate(cert);

    // Persist keys to external shared directory (configure via env/secret); fallback retains existing path for dev only
    var keyDir = builder.Configuration["DataProtection:KeyDirectory"];
    if (string.IsNullOrWhiteSpace(keyDir)) {
        throw new InvalidOperationException("Shared key directory not configured. Set DataProtection:KeyDirectory to a secured, shared location.");
    }
    Directory.CreateDirectory(keyDir);
    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(keyDir));
} else {
    // Development: local unencrypted store (acceptable only for dev)
    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "dpkeys")));
}

// Policies
IAsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 50)));

IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

IAsyncPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(4));

// Register named resilient HttpClient without silent fallback
builder.Services.AddHttpClient("WebApi", (sp, client) => {
    var opts = sp.GetRequiredService<IOptions<WebApiOptions>>().Value;
    client.BaseAddress = new Uri(opts.BaseUrl!); // validated
    client.Timeout = TimeSpan.FromSeconds(5);
    client.DefaultRequestVersion = HttpVersion.Version20;
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
})
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy)
    .AddPolicyHandler(timeoutPolicy);

// Needed for accessing HttpContext to pull CSP nonce in components if required
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// AuthN/Z for Blazor server-side endpoints and auth pipeline parity
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(o => {
    o.LoginPath = "/login";
    o.SlidingExpiration = true;
    o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
