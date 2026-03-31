using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SolicitatieTracker.App.DTOs;
using SolicitatieTracker.App.Services;
using SolicitatieTracker.Domain.Entities;

namespace Solicitatietracker_API.Controllers
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

                return CreatedAtAction(
                    nameof(FindByIdAsync),
                    new { id = createdApplication.Id },
                    createdApplication
                );
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the application.");

            }
        }
    }
}
