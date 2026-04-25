using Accounts;
using Accounts.Core.Infrastructure.Data;
using Accounts.CreateToken;
using Accounts.Register.BlazorTrigger;
using Billing;
using Core;
using Core.Domain.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Orders;
using TradingPortal;
using TradingPortal.Components;
using TradingPortal.Security;
using TradingPortal.Trader;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(sp => {
    var userStory = sp.GetRequiredService<Accounts.CreateVisitor.UserStory>();
    var request = new Accounts.CreateVisitor.UserStory.Request("TradingPortal", "1.0", Guid.Parse("10000000-0000-0000-0000-000000000001"));
    var response = userStory.Run(request).ToSynch();

    return new UserContext {
        User = response.ApplicationUser
    };
});

builder.Services.AddCore(builder.Configuration, builder.Environment);
builder.Services.AddAccounts(builder.Configuration);

//builder.Services.AddCommon(builder.Configuration, builder.Environment);
builder.Services.AddOrderBusinessExpert(builder.Configuration);
builder.Services.AddBilling(builder.Configuration);
builder.Services.AddScoped<CreateTokenCommandHandler>();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddTradingPortalAuthentication(
    builder.Configuration,
    builder.Environment);

builder.Services.AddScoped<PlaceTradeUserStory>();

builder.Services
    // Configure authentication to use cookies for maintaining the Blazor server-side session and
    // OpenID Connect for handling user login with an external identity provider.
    // This setup allows for secure authentication while providing a seamless user experience in the Blazor UI.
    .AddAuthentication(options => {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookie authentication for the Blazor UI session.
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Use OpenID Connect for login challenges, which will redirect to the external provider when an unauthenticated user tries to access a protected resource.
    })
    // Configure cookie settings for the Blazor server-side session cookie, ensuring it's secure and properly scoped.
    .AddCookie(options => {
        options.Cookie.Name = "_Host-TradingPortal.Session"; // "__Host-" prefix enforces Secure and SameSite=Strict by browser rules.
        options.Cookie.HttpOnly = true; // Not accessible to JavaScript.
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Only sent over HTTPS.
        options.Cookie.SameSite = SameSiteMode.Strict; // Not sent on cross-site requests.
        options.Cookie.Path = "/";  // Cookie valid for entire site; adjust if API is on different path/subdomain.       

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Absolute expiration time.
        options.SlidingExpiration = true; // Reset expiration on activity.

        options.LoginPath = "/login"; // Redirect here on unauthorized access; ensure this endpoint is accessible without auth.
        options.LogoutPath = "/logout"; // Optional: endpoint to clear auth cookie on logout; ensure this endpoint is accessible without auth.
        options.AccessDeniedPath = "/access-denied"; // Optional: endpoint to show access denied message; ensure this endpoint is accessible without auth.
    })
    // Configure OpenID Connect settings to integrate with the external identity provider, enabling secure authentication for users accessing the Blazor UI.
    .AddOpenIdConnect(options => {
        options.Authority = builder.Configuration["Oidc:Authority"]; // URL of the identity provider (e.g., IdentityServer, Azure AD).
        options.ClientId = builder.Configuration["Oidc:ClientId"]; // Client ID registered with the identity provider for this application.
        options.ClientSecret = builder.Configuration["Oidc:ClientSecret"]; // Client secret for confidential client flows; ensure this is stored securely (e.g., in Key Vault or user-secrets).

        // Bank-grade normal browser login flow.
        options.ResponseType = OpenIdConnectResponseType.Code; // Authorization code flow is recommended for server-side applications for better security (tokens are exchanged server-side, not exposed to the browser).
        options.UsePkce = true; // Use Proof Key for Code Exchange (PKCE) to mitigate authorization code interception attacks.

        // Tokens are stored inside the protected server-side authentication ticket.
        options.SaveTokens = true; // Save the tokens (id_token, access_token, refresh_token) in the authentication properties after a successful login. These can be retrieved later for API calls or token refresh.

        options.GetClaimsFromUserInfoEndpoint = true; // After receiving the id_token, the middleware will call the UserInfo endpoint to retrieve additional claims about the user. This is useful if the id_token does not contain all necessary claims.
        options.MapInboundClaims = false; // Prevent automatic mapping of standard OIDC claims to Microsoft-specific claim types (e.g., "sub" remains "sub" instead of being mapped to ClaimTypes.NameIdentifier).

        options.Scope.Clear(); // Clear default scopes to specify only the ones needed for this application.
        options.Scope.Add("openid"); // Required for OpenID Connect; indicates that the application intends to use OIDC for authentication.
        options.Scope.Add("profile"); // Optional: request access to the user's profile information (e.g., name, email) from the UserInfo endpoint.
        options.Scope.Add("trading-api"); // Custom scope for accessing the trading API; ensure this scope is defined in the identity provider and included in the user's consent.

        // Configure claim type mappings to ensure that the claims received from the identity provider are correctly interpreted by the application. This is especially important for role-based authorization, as it ensures that role claims are recognized properly.
        options.TokenValidationParameters = new TokenValidationParameters {
            NameClaimType = "name",
            RoleClaimType = "roles"
        };
    });

builder.Services.AddHttpContextAccessor(); // Needed for accessing HttpContext to pull user claims in components if required
builder.Services.AddScoped<ICurrentUser, ClaimsCurrentUser>(); // Abstraction for accessing current user information (e.g., user id, roles) in a way that can be easily mocked for testing and decoupled from the underlying authentication mechanism.

builder.Services.AddAuthorization();
builder.Services.AddApiVersioning(o => o.ReportApiVersions = true)
                .AddApiExplorer(o => {
                    o.GroupNameFormat = "'v'VVV";
                    o.SubstituteApiVersionInUrl = true;
                });

var app = builder.Build();

Core.Infrastructure.Log.LoggerFactory.Factory = app.Services.GetRequiredService<ILoggerFactory>();

SeedAccounts(app);

//app.MapCommon();
app.MapDevToken();
app.MapOrders();
app.MapBilling();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

if (app.Environment.IsProduction()) {
    app.UseHttpsRedirection();
}
app.UseAuthentication(); // Must be before Authorization and any endpoint that requires auth
app.UseAuthorization(); // Must be after Authentication and before any endpoint that requires auth
app.UseAntiforgery(); // Global antiforgery middleware; ensure endpoints that need it are properly decorated with [ValidateAntiForgeryToken] and that tokens are included in requests (e.g., via hidden fields or headers in Blazor components)

app.MapGet("/login", async (HttpContext http, IWebHostEnvironment environment) => {
    if (environment.IsDevelopment()) {
        return Results.Redirect("/");
    }

    await http.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
    return Results.Empty;
});

app.MapPost("/logout", async (HttpContext http, IWebHostEnvironment environment) => {
    if (environment.IsDevelopment()) {
        return Results.Redirect("/");
    }

    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

    return Results.Redirect("/");
});

app.MapGet("/me", (ICurrentUser currentUser) => {
    return Results.Ok(new {
        currentUser.IsAuthenticated,
        currentUser.ExternalUserId,
        currentUser.UserName,
        currentUser.DisplayName,
        currentUser.Email,
        currentUser.Desk,
        currentUser.Roles,
        currentUser.Scopes
    });
}).RequireAuthorization();

app.MapPost("/trades/place-demo", (PlaceTradeUserStory userStory) => {
    var request = new PlaceTradeRequest(
        Desk: "FX",
        Symbol: "EUR/USD",
        Quantity: 1_000_000m);

    return Results.Ok(userStory.Execute(request));
}).RequireAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

static void SeedAccounts(WebApplication app) {
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    var register = scope.ServiceProvider.GetRequiredService<IRegisterAdapter>();

    db.Database.EnsureCreated();

    var request = new RegisterBlazorRequest() {
        Email = "aladar.horvath@outlook.com",
        UserName = "Aladar Horvath",
        Password = "Sup3r$ecretPwd!",
        Roles = { "Trader" }
    };

    var existingAccount = db.Accounts.FirstOrDefault(a => a.Email == request.Email);
    if (existingAccount is not null) {
        if (!string.Equals(existingAccount.UserName, request.UserName, StringComparison.Ordinal)) {
            existingAccount.UserName = request.UserName;
            existingAccount.UserNameNormalized = request.UserName.ToLowerInvariant();
            db.SaveChanges();
        }

        return;
    }

    register.Run(request, CancellationToken.None).GetAwaiter().GetResult();
}

//// Bootstrap configuration: load secrets from Key Vault or user-secrets before any service registration
//builder.Configuration.AddSecretsFromStore(builder.Environment);

//// Bind & validate WebApi options (fail fast if BaseUrl missing or invalid)
//builder.Services.AddOptions<ApplicationApiOptions>()
//    .Bind(builder.Configuration.GetSection("ApplicationApi"))
//    .Validate(options => !string.IsNullOrWhiteSpace(options.BaseUrl), "ApplicationApi:BaseUrl is missing")
//    .Validate(options => Uri.IsWellFormedUriString(options.BaseUrl!, UriKind.Absolute), "ApplicationApi:BaseUrl is invalid URI")
//    .ValidateOnStart();

//// Hardened Data Protection configuration
//var dataProtection = builder.Services.AddDataProtection()
//    .SetApplicationName("ModularMonolith")
//    .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // rotation policy

//// Load encryption certificate (non-development) from configured path or store thumbprint
//if (!builder.Environment.IsDevelopment()) {
//    var certPath = builder.Configuration["DataProtection:CertificatePath"];
//    var certPassword = builder.Configuration["DataProtection:CertificatePassword"];
//    X509Certificate2? cert = null;
//    if (!string.IsNullOrWhiteSpace(certPath) && File.Exists(certPath)) {
//        cert = new X509Certificate2(certPath, certPassword, X509KeyStorageFlags.MachineKeySet);
//    } else {
//        var thumbprint = builder.Configuration["DataProtection:CertificateThumbprint"];
//        if (!string.IsNullOrWhiteSpace(thumbprint)) {
//            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
//            store.Open(OpenFlags.ReadOnly);
//            cert = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: false)
//                .OfType<X509Certificate2>()
//                .FirstOrDefault();
//        }
//    }
//    if (cert is null) {
//        throw new InvalidOperationException("Data Protection encryption certificate missing. Configure DataProtection:CertificatePath or DataProtection:CertificateThumbprint.");
//    }
//    dataProtection.ProtectKeysWithCertificate(cert);

//    // Persist keys to external shared directory (configure via env/secret); fallback retains existing path for dev only
//    var keyDir = builder.Configuration["DataProtection:KeyDirectory"];
//    if (string.IsNullOrWhiteSpace(keyDir)) {
//        throw new InvalidOperationException("Shared key directory not configured. Set DataProtection:KeyDirectory to a secured, shared location.");
//    }
//    Directory.CreateDirectory(keyDir);
//    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(keyDir));
//} else {
//    // Development: local unencrypted store (acceptable only for dev)
//    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "dpkeys")));
//}

//// Policies
//IAsyncPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
//    .HandleTransientHttpError()
//    .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
//    .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)) + TimeSpan.FromMilliseconds(Random.Shared.Then(0, 50)));

//IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = HttpPolicyExtensions
//    .HandleTransientHttpError()
//    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

//IAsyncPolicy<HttpResponseMessage> timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(4));

//// Register named resilient HttpClient without silent fallback
//builder.Services.AddHttpClient("WebApi", (sp, client) => {
//    var opts = sp.GetRequiredService<IOptions<ApplicationApiOptions>>().Value;
//    client.BaseAddress = new Uri(opts.BaseUrl!); // validated
//    client.Timeout = TimeSpan.FromSeconds(5);
//    client.DefaultRequestVersion = HttpVersion.Version20;
//    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
//    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//})
//    .AddPolicyHandler(retryPolicy)
//    .AddPolicyHandler(circuitBreakerPolicy)
//    .AddPolicyHandler(timeoutPolicy);

//// Needed for accessing HttpContext to pull CSP nonce in components if required
//builder.Services.AddHttpContextAccessor();

//// Add services to the container.
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

//// AuthN/Z for Blazor server-side endpoints and auth pipeline parity
//builder.Services.AddBasicAuthentication(options => {
//    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
//}).AddCookie(options => {
//    options.LoginPath = "/login";
//    options.SlidingExpiration = true;
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.Cookie.SameSite = SameSiteMode.Strict;
//});

//builder.Services.AddAuthorization();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment()) {
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
//app.UseAntiforgery();

//app.MapStaticAssets();
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();

//app.Run();
