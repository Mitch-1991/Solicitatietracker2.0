using SollicitatieTracker.Domain.Entities;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Messaging;

public interface IEmailSender
{
    TaskSystem SendAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default);
}
