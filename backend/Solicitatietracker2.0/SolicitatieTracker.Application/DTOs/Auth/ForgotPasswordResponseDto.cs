namespace SolicitatieTracker.App.DTOs.Auth
{
    public class ForgotPasswordResponseDto
    {
        public string Message { get; set; } = "Als dit e-mailadres bestaat, is er een resetlink aangemaakt.";
        public string? ResetToken { get; set; }
        public string? ResetUrl { get; set; }
    }
}
