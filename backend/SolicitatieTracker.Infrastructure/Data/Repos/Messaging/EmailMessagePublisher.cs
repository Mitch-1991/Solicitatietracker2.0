using SolicitatieTracker.Infrastructure.Messaging;
using SollicitatieTracker.Domain.Entities;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Data.Repos.Messaging;

public class EmailMessagePublisher : IEmailMessagePublisher
{
    private readonly IEmailOutboxRepository _emailOutboxRepository;

    public EmailMessagePublisher(IEmailOutboxRepository emailOutboxRepository)
    {
        _emailOutboxRepository = emailOutboxRepository;
    }

    public async TaskSystem PublishAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var outboxMessage = new EmailOutboxMessage
        {
            ToEmail = message.ToEmail.Trim(),
            Subject = message.Subject.Trim(),
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody,
            Status = EmailOutboxStatus.Pending,
            Attempts = 0,
            NextAttemptAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _emailOutboxRepository.AddAsync(outboxMessage, cancellationToken);
    }
}
