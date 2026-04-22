using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.Infrastructure.Data;
using SollicitatieTracker.Infrastructure.Data.Repos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.App.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepo = dashboardRepository;
        }

        public async Task<List<DashboardOverviewDto>> GetDashboardOverview()
        {
            var Sollicitaties = await _dashboardRepo.GetAllLopendeSollicitatiesAsync();
            var overzicht = new List<DashboardOverviewDto>();

            foreach (var sollicitatie in Sollicitaties)
            {
                
                overzicht.Add(new DashboardOverviewDto
                {
                    Id = sollicitatie.Id,
                    CompanyName = sollicitatie.Company.Name,
                    JobTitle = sollicitatie.JobTitle,
                    Status = sollicitatie.Status.ToString(),
                    AppliedDate = (DateOnly)sollicitatie.AppliedDate,
                    NextStep = sollicitatie.NextStep
                });
            }
            return overzicht;
        }

        public async Task<DashboardKPIDto> GetKPIAsync()
        {
           var afgewezen = await _dashboardRepo.GetAfgewezenCountAsync();
           var lopende = await _dashboardRepo.GetLopendeSollicitatiesCountAsync();
           var geplandeGesprekken = await _dashboardRepo.GetGesprekkenGeplandCountAsync();
           var aanbiedingen = await _dashboardRepo.GetAanbiedingenCountAsync();

            return new DashboardKPIDto
            {
                LopendeSollicitaties = lopende,
                GesprekkenGepland = geplandeGesprekken,
                Afgewezen = afgewezen,
                Aanbiedingen = aanbiedingen
            };
        }

        public async Task<List<UpcomingInterviewDto>> GetUpcomingInterviews()
        {
            var interviews = await _dashboardRepo.GetAllIntervieuwApplicationsAsync();
            var upcomingInterviews = new List<UpcomingInterviewDto>();

            foreach (var interview in interviews)
            {
                upcomingInterviews.Add(new UpcomingInterviewDto
                {
                    Id = interview.Id,
                    CompanyName = interview.Application.Company.Name,
                    JobTitle = interview.Application.JobTitle,
                    InterviewDate = interview.ScheduledStart.ToString("dd MM yyyy", new CultureInfo("nl-BE")),
                    Hour = interview.ScheduledStart.ToString("HH:mm")
                });
            }
            return upcomingInterviews;

        }
    }
}
