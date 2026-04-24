using SollicitatieTracker.App.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.App.Services
{
    public interface IDashboardService
    {
        Task<DashboardKPIDto> GetKPIAsync(int userId);
        Task<List<DashboardOverviewDto>> GetDashboardOverview(int userId);
        Task<List<UpcomingInterviewDto>> GetUpcomingInterviews(int userId);
    }
}
