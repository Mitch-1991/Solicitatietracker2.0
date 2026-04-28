using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SolicitatieTracker.Infrastructure.Messaging;
using SollicitatieTracker.Domain.Entities;
using TaskSystem = System.Threading.Tasks.Task;

namespace Solicitatietracker_API.Services.Messaging;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async TaskSystem SendAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default)
    {
        ValidateSettings();

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        mimeMessage.To.Add(MailboxAddress.Parse(message.ToEmail));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody
        }.ToMessageBody();

        using var client = new SmtpClient();
        var secureSocketOptions = _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await client.ConnectAsync(_settings.Host, _settings.Port, secureSocketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        }

        await client.SendAsync(mimeMessage, cancellationToken);

        try
        {
            await client.DisconnectAsync(quit: true, CancellationToken.None);
        }
        catch
        {
            // The server has already accepted the message; disconnect is best effort.
        }
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            throw new InvalidOperationException("SMTP host is niet geconfigureerd.");
        }

        if (string.IsNullOrWhiteSpace(_settings.FromEmail))
        {
            throw new InvalidOperationException("SMTP afzender is niet geconfigureerd.");
        }
    }
}
