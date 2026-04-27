using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.App.Services;
using SollicitatieTracker.Infrastructure.Data.Repos;
using ApplicationEntity = SollicitatieTracker.Domain.Entities.Application;
using ApplicationNoteEntity = SollicitatieTracker.Domain.Entities.ApplicationNote;
using CompanyEntity = SollicitatieTracker.Domain.Entities.Company;
using InterviewEntity = SollicitatieTracker.Domain.Entities.Interview;
using TaskSystem = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Tests;

public class ApplicationServiceTests
{
    [Fact]
    public async TaskSystem CreateAsync_ThrowsWhenBedrijfIsMissing()
    {
        var service = CreateService();

        var dto = new CreateApplicationDto
        {
            companyName = " ",
            JobTitle = "Backend Developer",
            Status = Status.Verzonden
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto, userId: 1));

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
            companyName = " OpenAl ",
            JobTitle = "Frontend Developer",
            Status = Status.Verzonden,
            AppliedDate = new DateOnly(2026, 3, 31),
            NextStep = "Technisch gesprek"
        };

        var result = await service.CreateAsync(dto, userId: 1);

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
            companyName = "  Acme  ",
            JobTitle = "QA Engineer",
            JobUrl = "https://example.com/jobs/qa",
            Location = "Antwerpen",
            Status = Status.Aanbieding,
            Priority = "hoog",
            AppliedDate = appliedDate,
            NextStep = "Contract bespreken",
            SalaryMin = 3500,
            SalaryMax = 4200,
            Notes = "  Eerste intake afgerond.  "
        };

        var result = await service.CreateAsync(dto, userId: 7);

        var addedCompany = Assert.Single(companyRepository.AddedCompanies);
        Assert.Equal("Acme", addedCompany.Name);
        Assert.Equal(7, addedCompany.UserId);
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
        Assert.Equal(7, result.UserId);
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

        var result = await service.FindByIdAsync(8, userId: 4);

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

    [Fact]
    public async TaskSystem CreateAsync_CreatesInterviewWhenStatusIsGesprek()
    {
        var applicationRepository = new FakeApplicationRepository();
        var service = CreateService(applicationRepository);
        var interviewStart = new DateTime(2026, 4, 12, 14, 30, 0);

        var result = await service.CreateAsync(new CreateApplicationDto
        {
            companyName = "Acme",
            JobTitle = "Backend Developer",
            Status = Status.Gesprek,
            Interview = ValidOnlineInterview(interviewStart)
        }, userId: 3);

        var addedApplication = Assert.Single(applicationRepository.AddedApplications);
        var addedInterview = Assert.Single(addedApplication.Interviews);
        Assert.Equal("Online", addedInterview.InterviewType);
        Assert.Equal(interviewStart, addedInterview.ScheduledStart);
        Assert.Equal("https://meet.example.com/interview", addedInterview.MeetingLink);
        Assert.NotNull(result.Interview);
        Assert.Equal("Online", result.Interview!.InterviewType);
    }

    [Fact]
    public async TaskSystem CreateAsync_ThrowsWhenGesprekHasNoInterview()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new CreateApplicationDto
        {
            companyName = "Acme",
            JobTitle = "Backend Developer",
            Status = Status.Gesprek
        }, userId: 3));

        Assert.Equal("Interviewgegevens zijn verplicht wanneer de status Gesprek is.", exception.Message);
    }

    [Fact]
    public async TaskSystem CreateAsync_IgnoresInterviewWhenStatusIsNotGesprek()
    {
        var applicationRepository = new FakeApplicationRepository();
        var service = CreateService(applicationRepository);

        await service.CreateAsync(new CreateApplicationDto
        {
            companyName = "Acme",
            JobTitle = "Backend Developer",
            Status = Status.Verzonden,
            Interview = ValidOnlineInterview(new DateTime(2026, 4, 12, 14, 30, 0))
        }, userId: 3);

        Assert.Empty(applicationRepository.AddedApplications.Single().Interviews);
    }

    [Fact]
    public async TaskSystem UpdateAsync_CreatesInterviewWhenStatusBecomesGesprek()
    {
        var applicationRepository = new FakeApplicationRepository
        {
            ApplicationById = ExistingApplication(status: Status.Verzonden)
        };
        var service = CreateService(applicationRepository);

        var result = await service.UpdateAsync(8, new UpdateApplicationDto
        {
            CompanyName = "Acme",
            JobTitle = "Backend Developer",
            Status = Status.Gesprek,
            AppliedDate = new DateOnly(2026, 4, 1),
            Interview = ValidLocationInterview(new DateTime(2026, 4, 12, 10, 0, 0))
        }, userId: 3);

        Assert.NotNull(result!.Interview);
        Assert.Equal("Op locatie", result.Interview!.InterviewType);
        Assert.Equal("Antwerpen", result.Interview.Location);
        Assert.Single(applicationRepository.ApplicationById!.Interviews);
    }

    [Fact]
    public async TaskSystem UpdateAsync_UpdatesExistingInterviewWhenStatusStaysGesprek()
    {
        var existingInterview = new InterviewEntity
        {
            Id = 5,
            ApplicationId = 8,
            InterviewType = "Online",
            ScheduledStart = new DateTime(2026, 4, 10, 9, 0, 0),
            MeetingLink = "https://meet.example.com/old",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        var applicationRepository = new FakeApplicationRepository
        {
            ApplicationById = ExistingApplication(status: Status.Gesprek, existingInterview)
        };
        var service = CreateService(applicationRepository);
        var newStart = new DateTime(2026, 4, 12, 11, 0, 0);

        var result = await service.UpdateAsync(8, new UpdateApplicationDto
        {
            CompanyName = "Acme",
            JobTitle = "Backend Developer",
            Status = Status.Gesprek,
            AppliedDate = new DateOnly(2026, 4, 1),
            Interview = ValidOnlineInterview(newStart)
        }, userId: 3);

        Assert.Single(applicationRepository.ApplicationById!.Interviews);
        Assert.Equal(newStart, existingInterview.ScheduledStart);
        Assert.Equal("https://meet.example.com/interview", existingInterview.MeetingLink);
        Assert.Equal(newStart, result!.Interview!.ScheduledStart);
    }

    [Fact]
    public async TaskSystem CreateAsync_ThrowsWhenOnlineInterviewHasInvalidMeetingLink()
    {
        var service = CreateService();
        var interview = ValidOnlineInterview(new DateTime(2026, 4, 12, 14, 30, 0));
        interview.MeetingLink = "geen-link";

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new CreateApplicationDto
        {
            companyName = "Acme",
            JobTitle = "Backend Developer",
            Status = Status.Gesprek,
            Interview = interview
        }, userId: 3));

        Assert.Equal("Meeting link moet een geldige URL zijn.", exception.Message);
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

    private static InterviewDto ValidOnlineInterview(DateTime scheduledStart)
    {
        return new InterviewDto
        {
            InterviewType = "Online",
            ScheduledStart = scheduledStart,
            ScheduledEnd = scheduledStart.AddHours(1),
            MeetingLink = "https://meet.example.com/interview",
            ContactPerson = "Sofie Janssens",
            ContactEmail = "sofie@example.com",
            Notes = "Technische intake"
        };
    }

    private static InterviewDto ValidLocationInterview(DateTime scheduledStart)
    {
        return new InterviewDto
        {
            InterviewType = "Op locatie",
            ScheduledStart = scheduledStart,
            ScheduledEnd = scheduledStart.AddHours(1),
            Location = "Antwerpen",
            ContactPerson = "Sofie Janssens",
            ContactEmail = "sofie@example.com",
            Notes = "Gesprek op kantoor"
        };
    }

    private static ApplicationEntity ExistingApplication(Status status, params InterviewEntity[] interviews)
    {
        return new ApplicationEntity
        {
            Id = 8,
            CompanyId = 11,
            UserId = 3,
            Company = new CompanyEntity
            {
                Id = 11,
                Name = "Acme",
                UserId = 3,
                CreatedAt = DateTime.UtcNow
            },
            JobTitle = "Backend Developer",
            Status = status,
            AppliedDate = new DateOnly(2026, 4, 1),
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
            Interviews = interviews.ToList()
        };
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

        public Task<ApplicationEntity> GetApplicationByIdAsync(int id, int userId)
        {
            return Task.FromResult(ApplicationById!);
        }

        public Task<ApplicationEntity?> GetApplicationByIdWithDetailsAsync(int id, int userId)
        {
            return Task.FromResult(ApplicationById ?? AddedApplications.FirstOrDefault(application => application.Id == id));
        }

        public Task<ApplicationEntity> UpdateApplicationAsync(ApplicationEntity application)
        {
            return Task.FromResult(application);
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

        public Task<List<CompanyEntity>> GetAllCompaniesAsync(int userId)
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
