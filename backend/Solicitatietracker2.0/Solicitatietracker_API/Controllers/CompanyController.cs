using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolicitatieTracker.App.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Solicitatietracker_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("companies")]
        public async Task<IActionResult> GetAllCompanies()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var companies = await _companyService.GetAllCompaniesAsync(userId.Value);
            return Ok(companies);
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId)){
                return null;
            }
            return userId;
        }
    }
}
