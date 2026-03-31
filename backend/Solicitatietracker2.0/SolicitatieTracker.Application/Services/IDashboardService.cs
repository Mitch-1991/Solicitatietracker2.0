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
        Task<DashboardKPIDto> GetKPIAsync();
        Task<List<DashboardOverviewDto>> GetDashboardOverview();
        Task<List<UpcomingInterviewDto>> GetUpcomingInterviews();
    }
}
