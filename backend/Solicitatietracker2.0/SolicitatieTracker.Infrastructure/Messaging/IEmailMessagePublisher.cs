using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Messaging;

public interface IEmailMessagePublisher
{
    TaskSystem PublishAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
