using System;

namespace SollicitatieTracker.Domain.Entities;

public partial class EmailOutboxMessage
{
    public int Id { get; set; }

    public string ToEmail { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string HtmlBody { get; set; } = null!;

    public string TextBody { get; set; } = null!;

    public EmailOutboxStatus Status { get; set; } = EmailOutboxStatus.Pending;

    public int Attempts { get; set; }

    public DateTime? NextAttemptAt { get; set; }

    public string? LastError { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SentAt { get; set; }
}
