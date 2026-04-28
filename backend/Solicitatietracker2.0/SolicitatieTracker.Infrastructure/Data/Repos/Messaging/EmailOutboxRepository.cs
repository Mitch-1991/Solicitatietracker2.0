using Microsoft.EntityFrameworkCore;
using SolicitatieTracker.Infrastructure.Messaging;
using SollicitatieTracker.Infrastructure.Data;
using SollicitatieTracker.Domain.Entities;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Data.Repos.Messaging;

public class EmailOutboxRepository : IEmailOutboxRepository
{
    private readonly SollicitatietrackerDbContext _context;

    public EmailOutboxRepository(SollicitatietrackerDbContext context)
    {
        _context = context;
    }

    public async TaskSystem AddAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _context.EmailOutboxMessages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<EmailOutboxMessage>> GetPendingAsync(
        DateTime utcNow,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmailOutboxMessages
            .Where(message =>
                message.Status == EmailOutboxStatus.Pending &&
                message.NextAttemptAt != null &&
                message.NextAttemptAt <= utcNow)
            .OrderBy(message => message.NextAttemptAt)
            .ThenBy(message => message.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async TaskSystem MarkAsSentAsync(
        EmailOutboxMessage message,
        DateTime sentAtUtc,
        CancellationToken cancellationToken = default)
    {
        message.Status = EmailOutboxStatus.Sent;
        message.SentAt = sentAtUtc;
        message.LastError = null;
        message.NextAttemptAt = null;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async TaskSystem MarkAsFailedAttemptAsync(
        EmailOutboxMessage message,
        int attempts,
        string error,
        DateTime? nextAttemptAtUtc,
        bool finalFailure,
        CancellationToken cancellationToken = default)
    {
        message.Attempts = attempts;
        message.LastError = error;
        message.NextAttemptAt = nextAttemptAtUtc;
        message.Status = finalFailure ? EmailOutboxStatus.Failed : EmailOutboxStatus.Pending;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
