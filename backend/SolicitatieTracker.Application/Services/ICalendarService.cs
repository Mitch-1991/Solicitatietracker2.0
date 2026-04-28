using SollicitatieTracker.App.DTOs;

namespace SollicitatieTracker.App.Services
{
    public interface ICalendarService
    {
        Task<List<CalendarInterviewDto>> GetInterviewsAsync(int userId, DateTime from, DateTime to);
    }
}
