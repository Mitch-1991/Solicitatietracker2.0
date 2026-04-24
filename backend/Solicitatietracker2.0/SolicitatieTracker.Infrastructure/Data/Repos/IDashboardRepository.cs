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
        Task<int> GetLopendeSollicitatiesCountAsync(int userId);
        Task<int> GetGesprekkenGeplandCountAsync(int userId);
        Task<int> GetAfgewezenCountAsync(int userId);
        Task<int> GetAanbiedingenCountAsync(int userId);
        Task<IEnumerable<Application>> GetAllLopendeSollicitatiesAsync(int userId);
        Task<IEnumerable<Interview>> GetAllIntervieuwApplicationsAsync(int userId);
    }
}
