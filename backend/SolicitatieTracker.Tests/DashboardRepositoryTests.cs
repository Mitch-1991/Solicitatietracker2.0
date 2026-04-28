using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data;
using SollicitatieTracker.Infrastructure.Data.Repos;
using TaskSystem = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Tests;

public class DashboardRepositoryTests
{
    [Fact]
    public async TaskSystem RepositoryCountsMatchSeededDashboardData()
    {
        await using var context = CreateContext();

        context.Applications.AddRange(
            new Application { Id = 1, UserId = 1, CompanyId = 1, JobTitle = "Backend Developer", Status = Status.Verzonden, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { Id = 2, UserId = 1, CompanyId = 1, JobTitle = "Frontend Developer", Status = Status.Gesprek, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { Id = 3, UserId = 1, CompanyId = 1, JobTitle = "Cloud Engineer", Status = Status.Afgewezen, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { Id = 4, UserId = 1, CompanyId = 1, JobTitle = "API Developer", Status = Status.Aanbieding, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { Id = 5, UserId = 1, CompanyId = 1, JobTitle = "Archived Developer", Status = Status.Verzonden, IsArchived = true, ArchivedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { Id = 6, UserId = 1, CompanyId = 1, JobTitle = "Archived Interview", Status = Status.Gesprek, IsArchived = true, ArchivedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        context.Interviews.AddRange(
            new Interview { ApplicationId = 1, InterviewType = "HR", ScheduledStart = DateTime.Now.AddDays(2), CreatedAt = DateTime.UtcNow },
            new Interview { ApplicationId = 2, InterviewType = "Technisch", ScheduledStart = DateTime.Now.AddDays(-2), CreatedAt = DateTime.UtcNow },
            new Interview { ApplicationId = 6, InterviewType = "Technisch", ScheduledStart = DateTime.Now.AddDays(3), CreatedAt = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();

        var repository = new DashboardRepository(context);

        var lopende = await repository.GetLopendeSollicitatiesCountAsync(userId: 1);
        var gesprekkenGepland = await repository.GetGesprekkenGeplandCountAsync(userId: 1);
        var afgewezen = await repository.GetAfgewezenCountAsync(userId: 1);
        var aanbiedingen = await repository.GetAanbiedingenCountAsync(userId: 1);

        Assert.Equal(1, lopende);
        Assert.Equal(1, gesprekkenGepland);
        Assert.Equal(1, afgewezen);
        Assert.Equal(1, aanbiedingen);
    }

    [Fact]
    public async TaskSystem RepositoryActiveQueriesExcludeArchivedApplications()
    {
        await using var context = CreateContext();

        var company = new Company { Id = 1, UserId = 1, Name = "Acme", CreatedAt = DateTime.UtcNow };
        context.Companies.Add(company);
        context.Applications.AddRange(
            new Application { Id = 1, UserId = 1, CompanyId = 1, Company = company, JobTitle = "Active", Status = Status.Verzonden, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { Id = 2, UserId = 1, CompanyId = 1, Company = company, JobTitle = "Archived", Status = Status.Verzonden, IsArchived = true, ArchivedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();

        var repository = new DashboardRepository(context);

        var applications = await repository.GetAllLopendeSollicitatiesAsync(userId: 1);

        var application = Assert.Single(applications);
        Assert.Equal("Active", application.JobTitle);
    }

    [Fact]
    public void StatusPropertyIsConfiguredAsStringConversion()
    {
        using var context = CreateContext();

        var statusProperty = context.Model
            .FindEntityType(typeof(Application))!
            .FindProperty(nameof(Application.Status))!;

        var converter = statusProperty.GetTypeMapping().Converter;

        Assert.NotNull(converter);
        Assert.Equal(typeof(string), converter!.ProviderClrType);
    }

    private static SollicitatietrackerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SollicitatietrackerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SollicitatietrackerDbContext(options);
    }
}
