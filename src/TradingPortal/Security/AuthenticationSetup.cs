using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace TradingPortal.Security;

public static class AuthenticationSetup {
    public static IServiceCollection AddTradingPortalAuthentication(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment) {

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, ClaimsCurrentUser>();

        if (environment.IsDevelopment()) {
            services
                .AddAuthentication(DevelopmentAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                    DevelopmentAuthenticationHandler.SchemeName,
                    options => { });

            services.AddAuthorization();

            return services;
        }

        services
            .AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.Cookie.Name = "__Host-TradingPortal.Session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Path = "/";

                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/access-denied";
            })
            .AddOpenIdConnect(options => {
                var oidc = configuration.GetSection("Oidc");

                options.Authority = Required(oidc, "Authority");
                options.ClientId = Required(oidc, "ClientId");
                options.ClientSecret = Required(oidc, "ClientSecret");

                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                options.SaveTokens = true;

                options.GetClaimsFromUserInfoEndpoint = true;
                options.MapInboundClaims = false;

                options.Scope.Clear();

                foreach (var scope in oidc.GetSection("Scopes").Get<string[]>() ?? []) {
                    options.Scope.Add(scope);
                }

                options.TokenValidationParameters = new TokenValidationParameters {
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                };
            });

        services.AddAuthorization();

        return services;
    }

    private static string Required(IConfiguration configuration, string key) => configuration[key]
            ?? throw new InvalidOperationException($"Missing OIDC configuration: Oidc:{key}");
}