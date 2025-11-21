using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common;

public static class ObservabilityExtensions {
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration config, IHostEnvironment env) {


        // OpenTelemetry
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: "WebApi", 
                serviceVersion: "1.0.0",
                serviceInstanceId: Environment.MachineName)
            .AddAttributes(new[] { new KeyValuePair<string, object>("deployment.environment", env.EnvironmentName) });

        var otlpEndpoint = config["Observability:OtlpEndpoint"];
        if (!env.IsDevelopment() && string.IsNullOrWhiteSpace(otlpEndpoint)) {
            throw new InvalidOperationException("Observability:OtlpEndpoint missing for non-development environment");
        }

        services.AddOpenTelemetry()
            .WithTracing(t => {
                t.SetResourceBuilder(resourceBuilder);
                t.AddAspNetCoreInstrumentation(o => { o.RecordException = true; });
                t.AddHttpClientInstrumentation();
                t.AddSource("Orders", "Billing");
                if (!string.IsNullOrWhiteSpace(otlpEndpoint)) {
                    t.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                }
            })
            .WithMetrics(m => {
                m.SetResourceBuilder(resourceBuilder);
                m.AddAspNetCoreInstrumentation();
                m.AddHttpClientInstrumentation();
                m.AddRuntimeInstrumentation();
                if (!string.IsNullOrWhiteSpace(otlpEndpoint)) {
                    m.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
                }
            });
        return services;
    }
}   