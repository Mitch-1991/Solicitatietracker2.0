using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.Infrastructure.Data.Repos
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly SollicitatietrackerDbContext _context;

        public DashboardRepository(SollicitatietrackerDbContext context) { _context = context; }

        public async Task<int> GetAanbiedingenCountAsync()
        {
            return await _context.Applications.CountAsync(a => a.Status == Status.Aanbieding);
        }

        public async Task<int> GetAfgewezenCountAsync()
        {
            return await _context.Applications.CountAsync(a => a.Status == Status.Afgewezen);
        }

        public async Task<int> GetGesprekkenGeplandCountAsync()
        {
            return await _context.Interviews.CountAsync(i => i.ScheduledStart > DateTime.Now);
        }

        public async Task<int> GetLopendeSollicitatiesCountAsync()
        {
            return await _context.Applications.CountAsync(a => a.Status != Status.Afgewezen && a.Status != Status.Aanbieding);
        }
    }
}
