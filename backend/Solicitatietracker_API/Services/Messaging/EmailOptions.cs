namespace Solicitatietracker_API.Services.Messaging;

public class FrontendSettings
{
    public string BaseUrl { get; set; } = "http://localhost:5173";
}

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool UseStartTls { get; set; } = true;

    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = "SollicitatieTracker";
}

public class EmailOutboxSettings
{
    public int PollIntervalSeconds { get; set; } = 10;

    public int BatchSize { get; set; } = 20;

    public int MaxAttempts { get; set; } = 5;
}
