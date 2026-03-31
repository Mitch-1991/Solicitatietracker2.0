using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.Infrastructure.Data.Repos
{
    public interface IDashboardRepository
    {
        Task<int> GetLopendeSollicitatiesCountAsync();
        Task<int> GetGesprekkenGeplandCountAsync();
        Task<int> GetAfgewezenCountAsync();
        Task<int> GetAanbiedingenCountAsync();
        Task<IEnumerable<Application>> GetAllLopendeSollicitatiesAsync();
        Task<IEnumerable<Interview>> GetAllIntervieuwApplicationsAsync();
    }
}
