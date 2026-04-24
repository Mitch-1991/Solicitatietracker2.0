using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolicitatieTracker.App.DTOs.Auth;
using SolicitatieTracker.App.Services.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace Solicitatietracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new {message = ex.Message});
            }catch(ArgumentException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if(string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            try
            {
                var user = await _authService.GetCurrentUserAsync(userId);
                return Ok(user);
            }catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
