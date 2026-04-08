using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// In Docker: HTTP/1.1 on 8080 (REST, metrics); optional gRPC uses HTTP/2 cleartext on 8082 (h2c cannot share a port with HTTP/1.1 without TLS).
/// Outside containers: Http1AndHttp2 on launch URLs (HTTPS dev or single HTTP port).
/// </summary>
public static class EshopKestrelExtensions
{
    public static WebApplicationBuilder ConfigureEshopDockerKestrel(
        this WebApplicationBuilder builder,
        bool grpcOnDedicatedPort8082 = false)
    {
        if (grpcOnDedicatedPort8082)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        // Keep HTTP :8080 from ASPNETCORE_URLS and add dedicated gRPC :8082 in container when needed.
        builder.WebHost.UseKestrel((_, options) =>
        {
            if (RunningInContainer())
            {
                if (grpcOnDedicatedPort8082)
                {
                    // Explicitly bind both ports for gRPC services:
                    // - 8080 for HTTP/1.1 REST endpoints (gateway + probes)
                    // - 8082 for HTTP/2 cleartext gRPC calls
                    options.ListenAnyIP(8080, lo => lo.Protocols = HttpProtocols.Http1);
                    options.ListenAnyIP(8082, lo => lo.Protocols = HttpProtocols.Http2);
                }
                else
                {
                    // Always bind :8080 in containers to avoid host/runtime differences in URL-based binding.
                    options.ListenAnyIP(8080, lo => lo.Protocols = HttpProtocols.Http1);
                }
            }
            else
            {
                options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2);
            }
        });

        return builder;
    }

    private static bool RunningInContainer() =>
        string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Environment.GetEnvironmentVariable("ESHOP_FORCE_CONTAINER_KESTREL"), "true", StringComparison.OrdinalIgnoreCase)
        || File.Exists("/.dockerenv");
}
