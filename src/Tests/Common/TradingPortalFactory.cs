using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tests.Common;

public sealed class TradingPortalFactory : WebApplicationFactory<TradingPortal.UserContext> {
    public string BaseAddress { get; private set; } = string.Empty;

    protected override IHost CreateHost(IHostBuilder builder) {
        builder.UseEnvironment("Development");
        builder.ConfigureWebHost(webHost => {
            webHost.UseKestrel(options => options.ListenLocalhost(0));
        });

        var host = builder.Build();
        host.Start();

        var addresses = host.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses;
        BaseAddress = addresses?.FirstOrDefault() ?? "http://localhost:0";

        return host;
    }
}
