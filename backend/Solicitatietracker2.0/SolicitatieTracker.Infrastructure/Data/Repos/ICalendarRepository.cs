using SollicitatieTracker.Domain.Entities;

namespace SollicitatieTracker.Infrastructure.Data.Repos
{
    public interface ICalendarRepository
    {
        Task<List<Interview>> GetInterviewsAsync(int userId, DateTime from, DateTime toExclusive);
    }
}
