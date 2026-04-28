namespace SolicitatieTracker.App.DTOs.Auth
{
    public class ForgotPasswordResponseDto
    {
        public string Message { get; set; } = "Als dit e-mailadres bestaat, ontvang je zo meteen een resetlink.";
        public string? ResetUrl { get; set; }
    }
}
