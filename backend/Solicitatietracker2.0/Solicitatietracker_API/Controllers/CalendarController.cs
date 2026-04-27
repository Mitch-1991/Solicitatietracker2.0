using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.App.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Sollicitatietracker_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet("interviews")]
        public async Task<ActionResult<List<CalendarInterviewDto>>> GetInterviews([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            if (from == default || to == default)
            {
                return BadRequest(new { message = "From en to zijn verplicht." });
            }

            try
            {
                var interviews = await _calendarService.GetInterviewsAsync(userId.Value, from, to);
                return Ok(interviews);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return userId;
        }
    }
}
