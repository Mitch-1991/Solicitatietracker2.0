using Microsoft.Extensions.Options;
using SolicitatieTracker.Infrastructure.Messaging;

namespace Solicitatietracker_API.Services.Messaging;

public class ConfiguredResetPasswordLinkBuilder : IResetPasswordLinkBuilder
{
    private readonly FrontendSettings _settings;

    public ConfiguredResetPasswordLinkBuilder(IOptions<FrontendSettings> options)
    {
        _settings = options.Value;
    }

    public string BuildResetPasswordLink(string token)
    {
        var baseUrl = string.IsNullOrWhiteSpace(_settings.BaseUrl)
            ? "http://localhost:5173"
            : _settings.BaseUrl.Trim().TrimEnd('/');

        return $"{baseUrl}/reset-password?token={Uri.EscapeDataString(token)}";
    }
}
