using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Common;

public static class SecretConfigurationExtensions
{
    public static IConfigurationBuilder AddSecretsFromStore(this IConfigurationBuilder builder, IHostEnvironment env)
    {
        // Preserves existing configuration then adds secrets depending on environment
        if (env.IsDevelopment())
        {
            // Use user secrets when running locally (if available)
            try
            {
                builder.AddUserSecrets<System.Object>(optional: true);
            }
            catch
            {
                // ignore if user secrets not configured
            }

            // Environment variables always supported
            builder.AddEnvironmentVariables();
            return builder;
        }

        // Non-dev: require a KeyVault URI to be configured
        var temp = builder.Build();
        var keyVaultUri = temp["Secrets:KeyVault:Uri"];
        var allowFallback = bool.TryParse(temp["Secrets:AllowLocalFallback"], out var b) && b;

        if (string.IsNullOrWhiteSpace(keyVaultUri))
        {
            if (allowFallback)
            {
                // Fallback to environment vars only (dangerous; gated by explicit flag)
                builder.AddEnvironmentVariables();
                return builder;
            }

            throw new InvalidOperationException("Secrets:KeyVault:Uri must be configured in non-development environments or set Secrets:AllowLocalFallback=true for temporary CI fallback.");
        }

        // Use DefaultAzureCredential to authenticate to Key Vault (managed identity / service principal)
        var credential = new DefaultAzureCredential();
        builder.AddAzureKeyVault(new Uri(keyVaultUri), credential);
        return builder;
    }
}
