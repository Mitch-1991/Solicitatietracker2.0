using SolicitatieTracker.App.DTOs;
using SolicitatieTracker.Infrastructure.Data.Repos;

namespace SolicitatieTracker.Tests;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetKPIAsync_MapsRepositoryCountsToDto()
    {
        var repository = new FakeDashboardRepository
        {
            LopendeSollicitaties = 5,
            GesprekkenGepland = 2,
            Afgewezen = 1,
            Aanbiedingen = 3
        };

        var service = new DashboardService(repository);

        var result = await service.GetKPIAsync();

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

        public Task<int> GetLopendeSollicitatiesCountAsync() => Task.FromResult(LopendeSollicitaties);

        public Task<int> GetGesprekkenGeplandCountAsync() => Task.FromResult(GesprekkenGepland);

        public Task<int> GetAfgewezenCountAsync() => Task.FromResult(Afgewezen);

        public Task<int> GetAanbiedingenCountAsync() => Task.FromResult(Aanbiedingen);

        public Task<IEnumerable<Domain.Entities.Application>> GetAllLopendeSollicitatiesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
