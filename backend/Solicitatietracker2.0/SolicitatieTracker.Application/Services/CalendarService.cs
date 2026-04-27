using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data.Repos;

namespace SollicitatieTracker.App.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly ICalendarRepository _calendarRepository;

        public CalendarService(ICalendarRepository calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }

        public async Task<List<CalendarInterviewDto>> GetInterviewsAsync(int userId, DateTime from, DateTime to)
        {
            var fromDate = from.Date;
            var toDate = to.Date;

            if (toDate < fromDate)
            {
                throw new ArgumentException("Einddatum mag niet voor startdatum liggen.");
            }

            var interviews = await _calendarRepository.GetInterviewsAsync(userId, fromDate, toDate.AddDays(1));

            return interviews
                .Select(MapToDto)
                .ToList();
        }

        private static CalendarInterviewDto MapToDto(Interview interview)
        {
            return new CalendarInterviewDto
            {
                Id = interview.Id,
                ApplicationId = interview.ApplicationId,
                CompanyName = interview.Application.Company.Name,
                JobTitle = interview.Application.JobTitle,
                InterviewType = interview.InterviewType,
                ScheduledStart = interview.ScheduledStart,
                ScheduledEnd = interview.ScheduledEnd,
                Location = interview.Location,
                MeetingLink = interview.MeetingLink,
                ContactPerson = interview.ContactPerson,
                ContactEmail = interview.ContactEmail,
                Notes = interview.Notes
            };
        }
    }
}
