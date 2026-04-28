using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.Infrastructure.Data.Repos
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly SollicitatietrackerDbContext _context;

        public DashboardRepository(SollicitatietrackerDbContext context) { _context = context; }

        public async Task<int> GetAanbiedingenCountAsync(int userId)
        {
            return await _context.Applications
                .Where(a => a.UserId == userId && !a.IsArchived)
                .CountAsync(a => a.Status == Status.Aanbieding);
        }

        public async Task<int> GetAfgewezenCountAsync(int userId)
        {
            return await _context.Applications
                .Where(a => a.UserId == userId && !a.IsArchived)
                .CountAsync(a => a.Status == Status.Afgewezen);
        }

        public async Task<IEnumerable<Interview>> GetAllIntervieuwApplicationsAsync(int userId)
        {
            return await _context.Interviews
                .Where(i => i.Application.UserId == userId && !i.Application.IsArchived && i.ScheduledStart >= DateTime.Now)
                .Include(i => i.Application)
                    .ThenInclude(a => a.Company)
                .OrderBy(i => i.ScheduledStart)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetAllLopendeSollicitatiesAsync(int userId)
        {
            return await _context.Applications
                .Where(a => a.UserId == userId && !a.IsArchived &&  a.Status != Status.Aanbieding)
                .Include(a => a.Company)
                .ToListAsync();
        }

        public async Task<int> GetGesprekkenGeplandCountAsync(int userId)
        {
            return await _context.Interviews
                .CountAsync(i => i.Application.UserId == userId && !i.Application.IsArchived && i.ScheduledStart >= DateTime.Now);
        }

        public async Task<int> GetLopendeSollicitatiesCountAsync(int userId)
        {
            return await _context.Applications
                .Where(a => a.UserId == userId && !a.IsArchived)
                .CountAsync(a => a.Status == Status.Verzonden);
        }
    }
}
