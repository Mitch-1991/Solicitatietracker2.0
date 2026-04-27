using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data;
using SollicitatieTracker.Infrastructure.Data.Repos;
using TaskSystem = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Tests;

public class CalendarRepositoryTests
{
    [Fact]
    public async TaskSystem GetInterviewsAsync_FiltersByUserAndDateRangeAndSorts()
    {
        await using var context = CreateContext();

        var company = new Company { Id = 1, UserId = 1, Name = "TechCorp", CreatedAt = DateTime.UtcNow };
        var otherCompany = new Company { Id = 2, UserId = 2, Name = "OtherCorp", CreatedAt = DateTime.UtcNow };
        var application = new Application { Id = 1, UserId = 1, CompanyId = 1, Company = company, JobTitle = "Backend Developer", Status = Status.Gesprek, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        var otherApplication = new Application { Id = 2, UserId = 2, CompanyId = 2, Company = otherCompany, JobTitle = "Frontend Developer", Status = Status.Gesprek, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

        context.Companies.AddRange(company, otherCompany);
        context.Applications.AddRange(application, otherApplication);
        context.Interviews.AddRange(
            new Interview { Id = 1, ApplicationId = 1, Application = application, InterviewType = "Online", ScheduledStart = new DateTime(2026, 4, 20, 14, 0, 0), CreatedAt = DateTime.UtcNow },
            new Interview { Id = 2, ApplicationId = 1, Application = application, InterviewType = "Op locatie", ScheduledStart = new DateTime(2026, 4, 10, 9, 0, 0), CreatedAt = DateTime.UtcNow },
            new Interview { Id = 3, ApplicationId = 1, Application = application, InterviewType = "Online", ScheduledStart = new DateTime(2026, 5, 1, 9, 0, 0), CreatedAt = DateTime.UtcNow },
            new Interview { Id = 4, ApplicationId = 2, Application = otherApplication, InterviewType = "Online", ScheduledStart = new DateTime(2026, 4, 12, 9, 0, 0), CreatedAt = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();

        var repository = new CalendarRepository(context);

        var result = await repository.GetInterviewsAsync(
            userId: 1,
            from: new DateTime(2026, 4, 1),
            toExclusive: new DateTime(2026, 5, 1));

        Assert.Equal(new[] { 2, 1 }, result.Select(interview => interview.Id));
        Assert.All(result, interview => Assert.Equal(1, interview.Application.UserId));
        Assert.All(result, interview => Assert.Equal("TechCorp", interview.Application.Company.Name));
    }

    private static SollicitatietrackerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SollicitatietrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SollicitatietrackerDbContext(options);
    }
}
