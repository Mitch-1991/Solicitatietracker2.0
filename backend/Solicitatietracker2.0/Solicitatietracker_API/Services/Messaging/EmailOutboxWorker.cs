using Microsoft.Extensions.Options;
using SolicitatieTracker.Infrastructure.Messaging;

namespace Solicitatietracker_API.Services.Messaging;

public class EmailOutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<EmailOutboxSettings> _options;
    private readonly ILogger<EmailOutboxWorker> _logger;

    public EmailOutboxWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<EmailOutboxSettings> options,
        ILogger<EmailOutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var settings = _options.Value;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<EmailOutboxProcessor>();

                var processed = await processor.ProcessPendingAsync(
                    DateTime.UtcNow,
                    Math.Max(1, settings.BatchSize),
                    Math.Max(1, settings.MaxAttempts),
                    stoppingToken);

                if (processed > 0)
                {
                    _logger.LogInformation("Processed {Count} e-mail outbox messages.", processed);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "E-mail outbox worker failed while processing messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(Math.Max(1, settings.PollIntervalSeconds)),
                stoppingToken);
        }
    }
}
