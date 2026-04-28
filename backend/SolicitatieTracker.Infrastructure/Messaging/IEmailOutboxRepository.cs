using SollicitatieTracker.Domain.Entities;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Messaging;

public interface IEmailOutboxRepository
{
    TaskSystem AddAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default);

    Task<List<EmailOutboxMessage>> GetPendingAsync(
        DateTime utcNow,
        int batchSize,
        CancellationToken cancellationToken = default);

    TaskSystem MarkAsSentAsync(
        EmailOutboxMessage message,
        DateTime sentAtUtc,
        CancellationToken cancellationToken = default);

    TaskSystem MarkAsFailedAttemptAsync(
        EmailOutboxMessage message,
        int attempts,
        string error,
        DateTime? nextAttemptAtUtc,
        bool finalFailure,
        CancellationToken cancellationToken = default);
}
