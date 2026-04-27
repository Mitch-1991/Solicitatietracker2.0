using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SolicitatieTracker.Domain.Entities;
using SollicitatieTracker.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace SolicitatieTracker.App.Services.Auth
{
    public class JwtTokenService : IJwtTokenService
    {

        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
        public string GenerateToken(User user, bool rememberMe)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = GetExpiration(rememberMe);

            var Token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }


        public DateTime GetExpiration(bool rememberMe)
        {
            return rememberMe
                ? DateTime.UtcNow.AddDays(_jwtSettings.RememberMeExpirationDays)
                : DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
        }
    }
}
