using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.Infrastructure.Data.Repos
{
    public interface IDashboardRepository
    {
        Task<int> GetLopendeSollicitatiesCountAsync();
        Task<int> GetGesprekkenGeplandCountAsync();
        Task<int> GetAfgewezenCountAsync();
        Task<int> GetAanbiedingenCountAsync();
    }
}
