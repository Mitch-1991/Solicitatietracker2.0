using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Domain.Entities;

namespace SollicitatieTracker.Infrastructure.Data.Repos
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly SollicitatietrackerDbContext _context;

        public CalendarRepository(SollicitatietrackerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Interview>> GetInterviewsAsync(int userId, DateTime from, DateTime toExclusive)
        {
            return await _context.Interviews
                .Where(i =>
                    i.Application.UserId == userId &&
                    i.ScheduledStart >= from &&
                    i.ScheduledStart < toExclusive)
                .Include(i => i.Application)
                    .ThenInclude(a => a.Company)
                .OrderBy(i => i.ScheduledStart)
                .ToListAsync();
        }
    }
}
