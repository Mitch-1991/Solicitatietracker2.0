using SolicitatieTracker.App.DTOs.Auth;
using SolicitatieTracker.Infrastructure.Data.Repos.Auth;
using SollicitatieTracker.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.App.Services.Auth
{
    public class AuthService : IAuthService
    {
        private const int MinimumPasswordLength = 6;
        private const int ResetTokenValidityMinutes = 30;

        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IResetTokenResponsePolicy _resetTokenResponsePolicy;

        public AuthService(
            IUserRepository authRepository,
            IJwtTokenService jwtTokenService,
            IResetTokenResponsePolicy resetTokenResponsePolicy)
        {
            _userRepository = authRepository;
            _jwtTokenService = jwtTokenService;
            _resetTokenResponsePolicy = resetTokenResponsePolicy;
        }

        public async Task<CurrentUserDto> GetCurrentUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.", nameof(userId));
            }

            return new CurrentUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            if (loginRequest == null)
            {
                throw new ArgumentNullException(nameof(loginRequest));
            }

            var email = loginRequest.Email?.Trim();
            var password = loginRequest.Password?.Trim();

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email is required.", nameof(loginRequest.Email));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password is required.", nameof(loginRequest.Password));
            }

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return new AuthResponseDto
            {
                Token = _jwtTokenService.GenerateToken(user, loginRequest.RememberMe),
                User = MapCurrentUser(user),
                ExpiresAt = _jwtTokenService.GetExpiration(loginRequest.RememberMe)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (registerRequest == null)
            {
                throw new ArgumentNullException(nameof(registerRequest));
            }

            var firstName = registerRequest.FirstName?.Trim();
            var lastName = registerRequest.LastName?.Trim();
            var email = registerRequest.Email?.Trim();
            var password = registerRequest.Password?.Trim();

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("First name is required.", nameof(registerRequest.FirstName));
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("Last name is required.", nameof(registerRequest.LastName));
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email is required.", nameof(registerRequest.Email));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password is required.", nameof(registerRequest.Password));
            }

            if (password.Length < MinimumPasswordLength)
            {
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(registerRequest.Password));
            }

            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
            {
                throw new ArgumentException("Email is already in use.", nameof(registerRequest.Email));
            }

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);

            return new AuthResponseDto
            {
                Token = _jwtTokenService.GenerateToken(createdUser, rememberMe: true),
                User = MapCurrentUser(createdUser),
                ExpiresAt = _jwtTokenService.GetExpiration(rememberMe: true)
            };
        }

        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequest)
        {
            if (forgotPasswordRequest == null)
            {
                throw new ArgumentNullException(nameof(forgotPasswordRequest));
            }

            var email = forgotPasswordRequest.Email?.Trim();
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email is required.", nameof(forgotPasswordRequest.Email));
            }

            var response = new ForgotPasswordResponseDto();
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return response;
            }

            var token = GenerateResetToken();
            user.PasswordResetTokenHash = HashResetToken(token);
            user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddMinutes(ResetTokenValidityMinutes);

            await _userRepository.UpdateAsync(user);

            if (_resetTokenResponsePolicy.ShouldExposeResetToken())
            {
                response.ResetToken = token;
                response.ResetUrl = $"/reset-password?token={Uri.EscapeDataString(token)}";
            }

            return response;
        }

        public async TaskSystem ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest)
        {
            if (resetPasswordRequest == null)
            {
                throw new ArgumentNullException(nameof(resetPasswordRequest));
            }

            var token = resetPasswordRequest.Token?.Trim();
            var newPassword = resetPasswordRequest.NewPassword?.Trim();
            var confirmPassword = resetPasswordRequest.ConfirmPassword?.Trim();

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Reset token is required.", nameof(resetPasswordRequest.Token));
            }

            ValidateNewPassword(newPassword, confirmPassword);

            var user = await _userRepository.GetByResetTokenHashAsync(HashResetToken(token));
            if (user == null || user.PasswordResetTokenExpiresAt == null || user.PasswordResetTokenExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Reset token is invalid or expired.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetTokenHash = null;
            user.PasswordResetTokenExpiresAt = null;

            await _userRepository.UpdateAsync(user);
        }

        public async TaskSystem ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordRequest)
        {
            if (changePasswordRequest == null)
            {
                throw new ArgumentNullException(nameof(changePasswordRequest));
            }

            var currentPassword = changePasswordRequest.CurrentPassword?.Trim();
            var newPassword = changePasswordRequest.NewPassword?.Trim();
            var confirmPassword = changePasswordRequest.ConfirmPassword?.Trim();

            if (string.IsNullOrEmpty(currentPassword))
            {
                throw new ArgumentException("Current password is required.", nameof(changePasswordRequest.CurrentPassword));
            }

            ValidateNewPassword(newPassword, confirmPassword);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.", nameof(userId));
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordResetTokenHash = null;
            user.PasswordResetTokenExpiresAt = null;

            await _userRepository.UpdateAsync(user);
        }

        private static CurrentUserDto MapCurrentUser(User user)
        {
            return new CurrentUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        private static string GenerateResetToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-", StringComparison.Ordinal)
                .Replace("/", "_", StringComparison.Ordinal)
                .TrimEnd('=');
        }

        private static string HashResetToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        private static void ValidateNewPassword(string? newPassword, string? confirmPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("New password is required.", nameof(newPassword));
            }

            if (newPassword.Length < MinimumPasswordLength)
            {
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(newPassword));
            }

            if (newPassword != confirmPassword)
            {
                throw new ArgumentException("Passwords do not match.", nameof(confirmPassword));
            }
        }
    }
}
