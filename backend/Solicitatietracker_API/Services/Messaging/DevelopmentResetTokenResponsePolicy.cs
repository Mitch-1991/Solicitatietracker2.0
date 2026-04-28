using Microsoft.Extensions.Options;
using SolicitatieTracker.App.Services.Auth;

namespace Solicitatietracker_API.Services.Messaging;

public class DevelopmentResetTokenResponsePolicy : IResetTokenResponsePolicy
{
    private readonly IWebHostEnvironment _environment;
    private readonly SmtpSettings _smtpSettings;

    public DevelopmentResetTokenResponsePolicy(
        IWebHostEnvironment environment,
        IOptions<SmtpSettings> smtpOptions)
    {
        _environment = environment;
        _smtpSettings = smtpOptions.Value;
    }

    public bool ShouldExposeResetUrl()
    {
        return _environment.IsDevelopment() &&
            (string.IsNullOrWhiteSpace(_smtpSettings.Host) ||
             string.IsNullOrWhiteSpace(_smtpSettings.FromEmail));
    }
}
