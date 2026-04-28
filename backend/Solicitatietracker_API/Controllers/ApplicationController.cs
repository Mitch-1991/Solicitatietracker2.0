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
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }


        [HttpGet("archive")]
        public async Task<ActionResult<List<ApplicationDto>>> GetArchive()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var applications = await _applicationService.GetArchivedAsync(userId.Value);
            return Ok(applications);
        }

        [HttpGet("archive/{id:int}")]
        public async Task<ActionResult<ApplicationDto>> GetArchivedById(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var application = await _applicationService.GetArchivedByIdAsync(id, userId.Value);

            if (application == null)
            {
                return NotFound();
            }

            return Ok(application);
        }

        [ActionName("FindByIdAsync")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApplicationDto>> FindByIdAsync(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            var application = await _applicationService.FindByIdAsync(id, userId.Value);

            if(application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }



        [HttpPost]
        public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody]CreateApplicationDto createApplicationDto)
        {
            var userId = GetCurrentUserId();

            if(userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            try
            {
                var createdApplication = await _applicationService.CreateAsync(createApplicationDto, userId.Value);

                return Created($"/api/application/{createdApplication.Id}", createdApplication);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the application." });

            }
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApplicationDto>> UpdateApplication(int id, [FromBody] UpdateApplicationDto updateApplicationDto)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }
            try
            {
                var updatedApplication = await _applicationService.UpdateAsync(id, updateApplicationDto, userId.Value);

                if (updatedApplication == null)
                {
                    return NotFound();
                }

                return Ok(updatedApplication);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the application.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> ArchiveApplication(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var archived = await _applicationService.ArchiveAsync(id, userId.Value);

            if (!archived)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if(string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out int userId)){
                return null;
            }
            return userId;
        }
    }
}
