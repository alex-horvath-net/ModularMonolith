using System.Text;
using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Common {
    internal static class AuthenticationExtensions {

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env) {
            var audience = configuration["Auth:Audience"]!;
            var issuer = configuration["Auth:Issuer"]!;
            var devKey = configuration["Auth:DevKey"]!;

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.MapInboundClaims = false;
                    options.RequireHttpsMetadata = !env.IsDevelopment();
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(devKey)),
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidTypes = new[] { "JWT", "at+jwt" },
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                    };
                });

            services
                .AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

            return services;
        }
    }
}