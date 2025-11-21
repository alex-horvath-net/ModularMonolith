using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Common;
internal static class ClientHeaderExtensions {

    internal static IServiceCollection AddClientHeadersInProxy(this IServiceCollection services) {


        services.Configure<ForwardedHeadersOptions>(options => {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear(); // Explicit trust config handled at infrastructure level
            options.KnownProxies.Clear();  // Avoid implicit trusts 
                                           // NOTE: If required, you can add:
                                           // options.KnownProxies.Add(IPAddress.Parse("10.0.0.10"));
                                           // or:
                                           // options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
        });
        return services;
    }

    internal static IEndpointRouteBuilder UseClientHeadersInProxy(this WebApplication app) {
        // use before  auth, redirects,link generation midleware

        app.UseForwardedHeaders();      // Preserve client headers across proxy,  ratelimiting and redirects

        return app;
    }
}