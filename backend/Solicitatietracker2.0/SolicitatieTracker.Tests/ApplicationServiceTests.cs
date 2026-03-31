using SolicitatieTracker.App.DTOs;
using SolicitatieTracker.App.Services;
using SolicitatieTracker.Infrastructure.Data.Repos;
using ApplicationEntity = SolicitatieTracker.Domain.Entities.Application;
using ApplicationNoteEntity = SolicitatieTracker.Domain.Entities.ApplicationNote;
using CompanyEntity = SolicitatieTracker.Domain.Entities.Company;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Tests;

public class ApplicationServiceTests
{
    [Fact]
    public async TaskSystem CreateAsync_ThrowsWhenBedrijfIsMissing()
    {
        var service = CreateService();

        var dto = new CreateApplicationDto
        {
            Bedrijf = " ",
            JobTitle = "Backend Developer",
            Status = Status.Verzonden
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));

        Assert.Equal("Bedrijf is verplicht", exception.Message);
    }

    [Fact]
    public async TaskSystem CreateAsync_ReusesExistingCompanyOnCloseMatch()
    {
        var existingCompany = new CompanyEntity
        {
            Id = 42,
            Name = "OpenAI",
            UserId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var companyRepository = new FakeCompanyRepository(existingCompany);
        var applicationRepository = new FakeApplicationRepository();
        var noteRepository = new FakeApplicationNoteRepository();
        var service = CreateService(applicationRepository, companyRepository, noteRepository);

        var dto = new CreateApplicationDto
        {
            UserId = 1,
            Bedrijf = " OpenAl ",
            JobTitle = "Frontend Developer",
            Status = Status.Gesprek,
            AppliedDate = new DateOnly(2026, 3, 31),
            NextStep = "Technisch gesprek"
        };

        var result = await service.CreateAsync(dto);

        Assert.Equal(existingCompany.Id, result.CompanyId);
        Assert.Empty(companyRepository.AddedCompanies);
        Assert.Equal(existingCompany.Id, applicationRepository.AddedApplications.Single().CompanyId);
        Assert.Null(noteRepository.LastAddedNote);
    }

    [Fact]
    public async TaskSystem CreateAsync_CreatesCompanyAndNoteAndMapsResult()
    {
        var companyRepository = new FakeCompanyRepository();
        var applicationRepository = new FakeApplicationRepository();
        var noteRepository = new FakeApplicationNoteRepository();
        var service = CreateService(applicationRepository, companyRepository, noteRepository);
        var appliedDate = new DateOnly(2026, 3, 30);

        var dto = new CreateApplicationDto
        {
            UserId = 7,
            Bedrijf = "  Acme  ",
            JobTitle = "QA Engineer",
            JobUrl = "https://example.com/jobs/qa",
            Location = "Antwerpen",
            Status = Status.Aanbieding,
            Priority = "hoog",
            AppliedDate = appliedDate,
            NextStep = "Contract bespreken",
            SalaryMin = 3500,
            SalaryMax = 4200,
            Omschrijving = "  Eerste intake afgerond.  "
        };

        var result = await service.CreateAsync(dto);

        var addedCompany = Assert.Single(companyRepository.AddedCompanies);
        Assert.Equal("Acme", addedCompany.Name);
        Assert.Equal(dto.UserId, addedCompany.UserId);
        Assert.Equal(dto.Location, addedCompany.Location);

        var addedApplication = Assert.Single(applicationRepository.AddedApplications);
        Assert.Equal(addedCompany.Id, addedApplication.CompanyId);
        Assert.Equal(dto.JobTitle, addedApplication.JobTitle);
        Assert.Equal(dto.Status, addedApplication.Status);
        Assert.Equal(appliedDate, addedApplication.AppliedDate);

        Assert.NotNull(noteRepository.LastAddedNote);
        Assert.Equal(addedApplication.Id, noteRepository.LastAddedNote!.ApplicationId);
        Assert.Equal("Eerste intake afgerond.", noteRepository.LastAddedNote.NoteText);

        Assert.Equal(101, result.Id);
        Assert.Equal(addedCompany.Id, result.CompanyId);
        Assert.Equal(dto.UserId, result.UserId);
        Assert.Equal(dto.JobTitle, result.JobTitle);
        Assert.Equal(nameof(Status.Aanbieding), result.Status);
        Assert.Equal(dto.Priority, result.Priority);
        Assert.Equal(appliedDate, result.AppliedDate);
        Assert.Equal(dto.NextStep, result.NextStep);
    }

    [Fact]
    public async TaskSystem FindByIdAsync_ReturnsMappedDtoWhenApplicationExists()
    {
        var createdAt = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var updatedAt = createdAt.AddHours(2);
        var applicationRepository = new FakeApplicationRepository
        {
            ApplicationById = new ApplicationEntity
            {
                Id = 8,
                CompanyId = 11,
                UserId = 4,
                JobTitle = "Platform Engineer",
                Status = Status.Verzonden,
                Priority = "laag",
                AppliedDate = new DateOnly(2026, 2, 28),
                NextStep = "Afwachten",
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            }
        };

        var service = CreateService(applicationRepository);

        var result = await service.FindByIdAsync(8);

        Assert.NotNull(result);
        Assert.Equal(8, result!.Id);
        Assert.Equal(11, result.CompanyId);
        Assert.Equal(4, result.UserId);
        Assert.Equal("Platform Engineer", result.JobTitle);
        Assert.Equal(nameof(Status.Verzonden), result.Status);
        Assert.Equal("laag", result.Priority);
        Assert.Equal(new DateOnly(2026, 2, 28), result.AppliedDate);
        Assert.Equal("Afwachten", result.NextStep);
        Assert.Equal(createdAt, result.CreatedAt);
        Assert.Equal(updatedAt, result.UpdatedAt);
    }

    private static ApplicationService CreateService(
        FakeApplicationRepository? applicationRepository = null,
        FakeCompanyRepository? companyRepository = null,
        FakeApplicationNoteRepository? noteRepository = null)
    {
        return new ApplicationService(
            applicationRepository ?? new FakeApplicationRepository(),
            companyRepository ?? new FakeCompanyRepository(),
            noteRepository ?? new FakeApplicationNoteRepository());
    }

    private sealed class FakeApplicationRepository : IApplicationRepository
    {
        public List<ApplicationEntity> AddedApplications { get; } = [];
        public ApplicationEntity? ApplicationById { get; init; }

        public Task<ApplicationEntity> AddApplicationAsync(ApplicationEntity application)
        {
            application.Id = 101 + AddedApplications.Count;
            AddedApplications.Add(application);
            return Task.FromResult(application);
        }

        public Task<ApplicationEntity> GetApplicationByIdAsync(int id)
        {
            return Task.FromResult(ApplicationById!);
        }
    }

    private sealed class FakeCompanyRepository : ICompanyRepository
    {
        private readonly List<CompanyEntity> _companies;

        public FakeCompanyRepository(params CompanyEntity[] companies)
        {
            _companies = companies.ToList();
        }

        public List<CompanyEntity> AddedCompanies { get; } = [];

        public Task<List<CompanyEntity>> GetAllCompaniesAsync()
        {
            return Task.FromResult(_companies.ToList());
        }

        public Task<CompanyEntity> AddAsync(CompanyEntity company)
        {
            company.Id = _companies.Count + 1;
            _companies.Add(company);
            AddedCompanies.Add(company);
            return Task.FromResult(company);
        }
    }

    private sealed class FakeApplicationNoteRepository : IApplicationNoteRepository
    {
        public ApplicationNoteEntity? LastAddedNote { get; private set; }

        public Task<ApplicationNoteEntity> AddApplicationNoteAsync(ApplicationNoteEntity applicationNote)
        {
            applicationNote.Id = 500;
            LastAddedNote = applicationNote;
            return Task.FromResult(applicationNote);
        }
    }
}
