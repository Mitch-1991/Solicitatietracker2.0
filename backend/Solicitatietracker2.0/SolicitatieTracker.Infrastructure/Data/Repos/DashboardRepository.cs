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
            return await _context.Applications.Where(a => a.UserId == userId).CountAsync(a => a.Status == Status.Aanbieding);
        }

        public async Task<int> GetAfgewezenCountAsync(int userId)
        {
            return await _context.Applications.Where(a => a.UserId == userId).CountAsync(a => a.Status == Status.Afgewezen);
        }

        public async Task<IEnumerable<Interview>> GetAllIntervieuwApplicationsAsync(int userId)
        {
            var applicationsWithInterviews = await _context.Applications
                                                                .Where(a => a.UserId == userId && a.Interviews != null)
                                                                .Include(a => a.Interviews)
                                                                .ToListAsync();
            var Interviews = new List<Interview>();

            foreach(var application in applicationsWithInterviews)
            {
                foreach (var interview in application.Interviews)
                {
                    Interviews.Add(interview);
                }
            }
            return Interviews;

        }

        public async Task<IEnumerable<Application>> GetAllLopendeSollicitatiesAsync(int userId)
        {
            return await _context.Applications
                .Where(a => a.UserId == userId &&  a.Status != Status.Aanbieding)
                .Include(a => a.Company)
                .ToListAsync();
        }

        public async Task<int> GetGesprekkenGeplandCountAsync(int userId)
        {
            return await _context.Applications.Where(a => a.UserId == userId).CountAsync(a => a.Status == Status.Gesprek);
        }

        public async Task<int> GetLopendeSollicitatiesCountAsync(int userId)
        {
            return await _context.Applications.Where(a => a.UserId == userId).CountAsync(a => a.Status == Status.Verzonden);
        }
    }
}
