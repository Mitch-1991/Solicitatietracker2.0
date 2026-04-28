namespace SolicitatieTracker.Infrastructure.Messaging;

public class EmailOutboxProcessor
{
    private readonly IEmailOutboxRepository _emailOutboxRepository;
    private readonly IEmailSender _emailSender;

    public EmailOutboxProcessor(
        IEmailOutboxRepository emailOutboxRepository,
        IEmailSender emailSender)
    {
        _emailOutboxRepository = emailOutboxRepository;
        _emailSender = emailSender;
    }

    public async Task<int> ProcessPendingAsync(
        DateTime utcNow,
        int batchSize,
        int maxAttempts,
        CancellationToken cancellationToken = default)
    {
        var messages = await _emailOutboxRepository.GetPendingAsync(utcNow, batchSize, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await _emailSender.SendAsync(message, cancellationToken);
                await _emailOutboxRepository.MarkAsSentAsync(message, DateTime.UtcNow, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                var attempts = message.Attempts + 1;
                var finalFailure = attempts >= maxAttempts;
                DateTime? nextAttemptAt = finalFailure ? null : utcNow.Add(GetRetryDelay(attempts));

                await _emailOutboxRepository.MarkAsFailedAttemptAsync(
                    message,
                    attempts,
                    TruncateError(ex.Message),
                    nextAttemptAt,
                    finalFailure,
                    cancellationToken);
            }
        }

        return messages.Count;
    }

    private static TimeSpan GetRetryDelay(int attempts)
    {
        var minutes = Math.Min(60, Math.Pow(2, Math.Max(0, attempts - 1)));
        return TimeSpan.FromMinutes(minutes);
    }

    private static string TruncateError(string message)
    {
        return message.Length <= 2000 ? message : message[..2000];
    }
}
