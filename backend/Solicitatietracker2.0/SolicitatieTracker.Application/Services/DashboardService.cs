using SolicitatieTracker.App.DTOs;
using SolicitatieTracker.Infrastructure.Data;
using SolicitatieTracker.Infrastructure.Data.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.DTOs
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepo = dashboardRepository;
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
    }
}
