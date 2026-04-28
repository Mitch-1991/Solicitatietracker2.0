namespace SolicitatieTracker.Infrastructure.Messaging;

public class EmailMessage
{
    public string ToEmail { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string HtmlBody { get; set; } = null!;

    public string TextBody { get; set; } = null!;
}
