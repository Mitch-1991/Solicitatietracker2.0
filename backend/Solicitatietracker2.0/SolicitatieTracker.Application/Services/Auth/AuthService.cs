using SolicitatieTracker.App.DTOs.Auth;
using SolicitatieTracker.Infrastructure.Data.Repos.Auth;
using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUserRepository authRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = authRepository;
            _jwtTokenService = jwtTokenService;
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
            if(loginRequest == null)
            {
                throw new ArgumentNullException(nameof(loginRequest));
            }
            var email = loginRequest.Email?.Trim();
            var password = loginRequest.Password?.Trim();

            if(string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email is required.", nameof(loginRequest.Email));
            }
            if(string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password is required.", nameof(loginRequest.Password));
            }

            var user = await _userRepository.GetByEmailAsync(email!);

            if(user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash); 

            if(!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            return new AuthResponseDto
            {
                Token = _jwtTokenService.GenerateToken(user),
                User = new CurrentUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                },
                ExpiresAt = _jwtTokenService.GetExpiration()
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
            if (password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(registerRequest.Password));
            }

            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
            {
                throw new ArgumentException("Email is already in use.", nameof(registerRequest.Email));
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);

            var token = _jwtTokenService.GenerateToken(createdUser);
            var expiresAt = _jwtTokenService.GetExpiration();

            return new AuthResponseDto
            {
                Token = token,
                User = new CurrentUserDto
                {
                    Id = createdUser.Id,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    Email = createdUser.Email
                },
                ExpiresAt = expiresAt
            };

        }
    }
}
