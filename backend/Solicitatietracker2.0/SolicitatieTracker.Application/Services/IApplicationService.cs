using SollicitatieTracker.App.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.App.Services
{
    public interface IApplicationService
    {
        Task<ApplicationDto> CreateAsync(CreateApplicationDto createApplicationDto, int userId);
        Task<ApplicationDto?> FindByIdAsync(int id, int userId);
        Task<ApplicationDto?> UpdateAsync(int id, UpdateApplicationDto updateApplicationDto, int userId);
    }
}
