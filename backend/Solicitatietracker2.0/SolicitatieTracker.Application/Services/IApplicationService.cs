using SolicitatieTracker.App.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.Services
{
    public interface IApplicationService
    {
        Task<ApplicationDto> CreateAsync(CreateApplicationDto createApplicationDto);
        Task<ApplicationDto?> FindByIdAsync(int id);
    }
}
