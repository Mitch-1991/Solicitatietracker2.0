using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolicitatieTracker.App.DTOs;

namespace Solicitatietracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService service) { _dashboardService = service; }

        [HttpGet("kpis")]
        public async Task<ActionResult<DashboardKPIDto>> GetKpis()
        {
            var kpis = await _dashboardService.GetKPIAsync();
            return Ok(kpis);
        }
        [HttpGet("overview")]
        public async Task<ActionResult<List<DashboardOverviewDto>>> GetOverview()
        {
            var overview = await _dashboardService.GetDashboardOverview();
            return Ok(overview);
        }
    }
}
