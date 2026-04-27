using System.ComponentModel.DataAnnotations;

namespace SolicitatieTracker.App.DTOs.Auth
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
