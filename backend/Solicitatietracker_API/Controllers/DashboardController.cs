using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.App.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Sollicitatietracker_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService service) { _dashboardService = service; }

        [HttpGet("kpis")]
        public async Task<ActionResult<DashboardKPIDto>> GetKpis()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var kpis = await _dashboardService.GetKPIAsync(userId.Value);
            return Ok(kpis);
        }
        [HttpGet("overview")]
        public async Task<ActionResult<List<DashboardOverviewDto>>> GetOverview()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var overview = await _dashboardService.GetDashboardOverview(userId.Value);
            return Ok(overview);
        }
        [HttpGet("upcoming-interviews")]
        public async Task<ActionResult<List<UpcomingInterviewDto>>> GetUpcomingInterviews()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var interviews = await _dashboardService.GetUpcomingInterviews(userId.Value);
            return Ok(interviews);
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
