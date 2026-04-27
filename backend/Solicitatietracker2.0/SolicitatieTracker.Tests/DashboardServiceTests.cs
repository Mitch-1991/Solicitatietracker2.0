using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.App.Services;
using SollicitatieTracker.Infrastructure.Data.Repos;
using InterviewEntity = SollicitatieTracker.Domain.Entities.Interview;
using TaskSystem = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Tests;

public class DashboardServiceTests
{
    [Fact]
    public async TaskSystem GetKPIAsync_MapsRepositoryCountsToDto()
    {
        var repository = new FakeDashboardRepository
        {
            LopendeSollicitaties = 5,
            GesprekkenGepland = 2,
            Afgewezen = 1,
            Aanbiedingen = 3
        };

        var service = new DashboardService(repository);

        var result = await service.GetKPIAsync(userId: 1);

        Assert.Equal(5, result.LopendeSollicitaties);
        Assert.Equal(2, result.GesprekkenGepland);
        Assert.Equal(1, result.Afgewezen);
        Assert.Equal(3, result.Aanbiedingen);
    }

    private sealed class FakeDashboardRepository : IDashboardRepository
    {
        public int LopendeSollicitaties { get; init; }
        public int GesprekkenGepland { get; init; }
        public int Afgewezen { get; init; }
        public int Aanbiedingen { get; init; }

        public Task<int> GetLopendeSollicitatiesCountAsync(int userId) => Task.FromResult(LopendeSollicitaties);

        public Task<int> GetGesprekkenGeplandCountAsync(int userId) => Task.FromResult(GesprekkenGepland);

        public Task<int> GetAfgewezenCountAsync(int userId) => Task.FromResult(Afgewezen);

        public Task<int> GetAanbiedingenCountAsync(int userId) => Task.FromResult(Aanbiedingen);

        public Task<IEnumerable<Domain.Entities.Application>> GetAllLopendeSollicitatiesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InterviewEntity>> GetAllIntervieuwApplicationsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
