using SollicitatieTracker.App.Services;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data.Repos;
using TaskSystem = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Tests;

public class CalendarServiceTests
{
    [Fact]
    public async TaskSystem GetInterviewsAsync_MapsInterviewDetailsToCalendarDto()
    {
        var scheduledStart = new DateTime(2026, 4, 20, 14, 0, 0);
        var scheduledEnd = scheduledStart.AddHours(1);
        var repository = new FakeCalendarRepository
        {
            Interviews =
            [
                new Interview
                {
                    Id = 9,
                    ApplicationId = 4,
                    InterviewType = "Online",
                    ScheduledStart = scheduledStart,
                    ScheduledEnd = scheduledEnd,
                    MeetingLink = "https://meet.example.com",
                    ContactPerson = "Sofie Janssens",
                    ContactEmail = "sofie@example.com",
                    Notes = "Case bespreken",
                    Application = new Application
                    {
                        Id = 4,
                        UserId = 1,
                        JobTitle = "Backend Developer",
                        Company = new Company { Id = 2, UserId = 1, Name = "TechCorp", CreatedAt = DateTime.UtcNow },
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    CreatedAt = DateTime.UtcNow
                }
            ]
        };

        var service = new CalendarService(repository);

        var result = await service.GetInterviewsAsync(1, new DateTime(2026, 4, 1), new DateTime(2026, 4, 30));

        var interview = Assert.Single(result);
        Assert.Equal(9, interview.Id);
        Assert.Equal(4, interview.ApplicationId);
        Assert.Equal("TechCorp", interview.CompanyName);
        Assert.Equal("Backend Developer", interview.JobTitle);
        Assert.Equal("Online", interview.InterviewType);
        Assert.Equal(scheduledStart, interview.ScheduledStart);
        Assert.Equal(scheduledEnd, interview.ScheduledEnd);
        Assert.Equal("https://meet.example.com", interview.MeetingLink);
        Assert.Equal("Sofie Janssens", interview.ContactPerson);
        Assert.Equal("sofie@example.com", interview.ContactEmail);
        Assert.Equal("Case bespreken", interview.Notes);
    }

    [Fact]
    public async TaskSystem GetInterviewsAsync_RejectsRangeWhenToIsBeforeFrom()
    {
        var service = new CalendarService(new FakeCalendarRepository());

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetInterviewsAsync(1, new DateTime(2026, 4, 30), new DateTime(2026, 4, 1)));

        Assert.Equal("Einddatum mag niet voor startdatum liggen.", exception.Message);
    }

    private sealed class FakeCalendarRepository : ICalendarRepository
    {
        public List<Interview> Interviews { get; init; } = [];

        public System.Threading.Tasks.Task<List<Interview>> GetInterviewsAsync(int userId, DateTime from, DateTime toExclusive)
        {
            return TaskSystem.FromResult(Interviews);
        }
    }
}
