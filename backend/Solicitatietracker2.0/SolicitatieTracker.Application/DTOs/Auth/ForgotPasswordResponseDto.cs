namespace SolicitatieTracker.App.DTOs.Auth
{
    public class ForgotPasswordResponseDto
    {
        public string Message { get; set; } = "E-mail verzonden.";
        public string? ResetUrl { get; set; }
    }
}
