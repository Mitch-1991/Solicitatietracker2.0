using SolicitatieTracker.App.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.Services.Auth
{
    public class AuthService : IAuthService
    {
        public Task<CurrentUserDto> GetCurrentUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            throw new NotImplementedException();
        }
    }
}
