using System.Text.Json.Serialization;

namespace MonAppK8s.Payloads;

public class MonApiPayload
{
    public string? VERSION { get; set; }

    public string? RUNNING_IN_CONTAINER { get; set; }

    public string? PATH { get; set; }

    public string? MONAPI_SERVICE_SERVICE_PORT { get; set; }

    public string? MONAPI_SERVICE_SERVICE_HOST { get; set; }

    public string? MONAPI_SERVICE_PORT_80_TCP_PROTO { get; set; }

    public string? MONAPI_SERVICE_PORT_80_TCP_PORT { get; set; }

    public string? MONAPI_SERVICE_PORT_80_TCP_ADDR { get; set; }

    public string? MONAPI_SERVICE_PORT_80_TCP { get; set; }

    public string? MONAPI_SERVICE_PORT { get; set; }

    [JsonPropertyName("Logging:LogLevel:Microsoft.AspNetCore")]
    public string? Logging_LogLevel_MicrosoftAspNetCore { get; set; }

    [JsonPropertyName("Logging:LogLevel:Default")]
    public string? Logging_LogLevel_Default { get; set; }

    public string? KUBERNETES_SERVICE_PORT_HTTPS { get; set; }

    public string? KUBERNETES_SERVICE_PORT { get; set; }

    public string? KUBERNETES_SERVICE_HOST { get; set; }

    public string? KUBERNETES_PORT_443_TCP_PROTO { get; set; }

    public string? KUBERNETES_PORT_443_TCP_PORT { get; set; }

    public string? KUBERNETES_PORT_443_TCP_ADDR { get; set; }

    public string? KUBERNETES_PORT_443_TCP { get; set; }

    public string? KUBERNETES_PORT { get; set; }

    public string? HTTP_PORTS { get; set; }

    public string? HOSTNAME { get; set; }

    public string? HOME { get; set; }

    public string? ENVIRONMENT { get; set; }

    public string? DOTNET_VERSION { get; set; }

    public string? DOTNET_RUNNING_IN_CONTAINER { get; set; }

    public string? contentRoot { get; set; }

    [JsonPropertyName("Azure:ClientTenantId")]
    public string? Azure_ClientTenantId { get; set; }

    [JsonPropertyName("Azure:ClientSecret")]
    public string? Azure_ClientSecret { get; set; }

    [JsonPropertyName("Azure:ClientId")]
    public string? Azure_ClientId { get; set; }

    public string? ASPNET_VERSION { get; set; }

    public string? ASPNETCORE_HTTP_PORTS { get; set; }

    public string? ASPNETCORE_ENVIRONMENT { get; set; }

    public string? APP_UID { get; set; }

    public string? applicationName { get; set; }

    public string? AllowedHosts { get; set; }
}
