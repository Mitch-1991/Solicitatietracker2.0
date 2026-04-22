using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.App.Services;
using SollicitatieTracker.Domain.Entities;

namespace Sollicitatietracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }


        [ActionName("FindByIdAsync")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApplicationDto>> FindByIdAsync(int id)
        {
            var application = await _applicationService.FindByIdAsync(id);

            if(application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }



        [HttpPost]
        public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody]CreateApplicationDto createApplicationDto)
        {
            try
            {
                var createdApplication = await _applicationService.CreateAsync(createApplicationDto);

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
            try
            {
                var updatedApplication = await _applicationService.UpdateAsync(id, updateApplicationDto);

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
    }
}
