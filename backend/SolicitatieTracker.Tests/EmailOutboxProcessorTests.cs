using SolicitatieTracker.Infrastructure.Messaging;
using SollicitatieTracker.Domain.Entities;
using TaskSystem = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Tests;

public class EmailOutboxProcessorTests
{
    [Fact]
    public async TaskSystem ProcessPendingAsync_MarksSuccessfulMessageAsSent()
    {
        var message = CreatePendingMessage();
        var repository = new FakeEmailOutboxRepository(message);
        var processor = new EmailOutboxProcessor(repository, new FakeEmailSender());

        await processor.ProcessPendingAsync(DateTime.UtcNow, batchSize: 10, maxAttempts: 5);

        Assert.Equal(EmailOutboxStatus.Sent, message.Status);
        Assert.NotNull(message.SentAt);
        Assert.Null(message.LastError);
    }

    [Fact]
    public async TaskSystem ProcessPendingAsync_RetriesFailedMessage()
    {
        var now = new DateTime(2026, 4, 27, 10, 0, 0, DateTimeKind.Utc);
        var message = CreatePendingMessage();
        message.NextAttemptAt = now;
        var processor = new EmailOutboxProcessor(
            new FakeEmailOutboxRepository(message),
            new FakeEmailSender(new InvalidOperationException("SMTP offline")));

        await processor.ProcessPendingAsync(now, batchSize: 10, maxAttempts: 5);

        Assert.Equal(EmailOutboxStatus.Pending, message.Status);
        Assert.Equal(1, message.Attempts);
        Assert.Equal("SMTP offline", message.LastError);
        Assert.Equal(now.AddMinutes(1), message.NextAttemptAt);
    }

    [Fact]
    public async TaskSystem ProcessPendingAsync_MarksMessageFailedAfterMaxAttempts()
    {
        var message = CreatePendingMessage();
        message.Attempts = 4;
        var processor = new EmailOutboxProcessor(
            new FakeEmailOutboxRepository(message),
            new FakeEmailSender(new InvalidOperationException("SMTP offline")));

        await processor.ProcessPendingAsync(DateTime.UtcNow, batchSize: 10, maxAttempts: 5);

        Assert.Equal(EmailOutboxStatus.Failed, message.Status);
        Assert.Equal(5, message.Attempts);
        Assert.Equal("SMTP offline", message.LastError);
        Assert.Null(message.NextAttemptAt);
    }

    private static EmailOutboxMessage CreatePendingMessage()
    {
        return new EmailOutboxMessage
        {
            Id = 11,
            ToEmail = "test@example.com",
            Subject = "Reset",
            HtmlBody = "<p>Reset</p>",
            TextBody = "Reset",
            Status = EmailOutboxStatus.Pending,
            Attempts = 0,
            NextAttemptAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        private readonly Exception? _exception;

        public FakeEmailSender(Exception? exception = null)
        {
            _exception = exception;
        }

        public TaskSystem SendAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default)
        {
            return _exception == null ? TaskSystem.CompletedTask : TaskSystem.FromException(_exception);
        }
    }

    private sealed class FakeEmailOutboxRepository : IEmailOutboxRepository
    {
        private readonly List<EmailOutboxMessage> _messages;

        public FakeEmailOutboxRepository(params EmailOutboxMessage[] messages)
        {
            _messages = messages.ToList();
        }

        public TaskSystem AddAsync(EmailOutboxMessage message, CancellationToken cancellationToken = default)
        {
            _messages.Add(message);
            return TaskSystem.CompletedTask;
        }

        public Task<List<EmailOutboxMessage>> GetPendingAsync(DateTime utcNow, int batchSize, CancellationToken cancellationToken = default)
        {
            var messages = _messages
                .Where(message =>
                    message.Status == EmailOutboxStatus.Pending &&
                    message.NextAttemptAt <= utcNow)
                .Take(batchSize)
                .ToList();

            return System.Threading.Tasks.Task.FromResult(messages);
        }

        public TaskSystem MarkAsSentAsync(EmailOutboxMessage message, DateTime sentAtUtc, CancellationToken cancellationToken = default)
        {
            message.Status = EmailOutboxStatus.Sent;
            message.SentAt = sentAtUtc;
            message.LastError = null;
            message.NextAttemptAt = null;
            return TaskSystem.CompletedTask;
        }

        public TaskSystem MarkAsFailedAttemptAsync(
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
            return TaskSystem.CompletedTask;
        }
    }
}
