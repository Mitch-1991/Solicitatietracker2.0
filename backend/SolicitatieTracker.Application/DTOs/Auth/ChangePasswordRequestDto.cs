using System.ComponentModel.DataAnnotations;

namespace SolicitatieTracker.App.DTOs.Auth
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;

        [Required]
        public string ConfirmPassword { get; set; } = null!;
    }
}
