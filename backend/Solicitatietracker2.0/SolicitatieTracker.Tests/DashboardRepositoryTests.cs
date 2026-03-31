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
            new Application { UserId = 1, CompanyId = 1, JobTitle = "Backend Developer", Status = Status.Verzonden, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { UserId = 1, CompanyId = 1, JobTitle = "Frontend Developer", Status = Status.Gesprek, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { UserId = 1, CompanyId = 1, JobTitle = "Cloud Engineer", Status = Status.Afgewezen, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Application { UserId = 1, CompanyId = 1, JobTitle = "API Developer", Status = Status.Aanbieding, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        context.Interviews.AddRange(
            new Interview { ApplicationId = 1, InterviewType = "HR", ScheduledStart = DateTime.Now.AddDays(2), CreatedAt = DateTime.UtcNow },
            new Interview { ApplicationId = 2, InterviewType = "Technisch", ScheduledStart = DateTime.Now.AddDays(-2), CreatedAt = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();

        var repository = new DashboardRepository(context);

        var lopende = await repository.GetLopendeSollicitatiesCountAsync();
        var gesprekkenGepland = await repository.GetGesprekkenGeplandCountAsync();
        var afgewezen = await repository.GetAfgewezenCountAsync();
        var aanbiedingen = await repository.GetAanbiedingenCountAsync();

        Assert.Equal(2, lopende);
        Assert.Equal(1, gesprekkenGepland);
        Assert.Equal(1, afgewezen);
        Assert.Equal(1, aanbiedingen);
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
