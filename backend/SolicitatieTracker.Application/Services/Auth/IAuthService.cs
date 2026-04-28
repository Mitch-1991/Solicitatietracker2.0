using SolicitatieTracker.App.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.App.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<CurrentUserDto> GetCurrentUserAsync(int userId);
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequest);
        TaskSystem ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest);
        TaskSystem ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordRequest);
    }
}
