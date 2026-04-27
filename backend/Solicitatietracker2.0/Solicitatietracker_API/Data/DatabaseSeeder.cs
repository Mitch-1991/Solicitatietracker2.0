using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data;
using TaskSystem = System.Threading.Tasks.Task;

namespace Sollicitatietracker_API.Data;

public static class DatabaseSeeder
{
    public static async TaskSystem SeedAsync(SollicitatietrackerDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        await EnsureAuthColumnsAsync(context);

        const string seedEmail = "dummy.user@sollicitatietracker.local";

        var existingUser = await context.Users
            .SingleOrDefaultAsync(u => u.Email == seedEmail);

        if (existingUser is not null)
        {
            var existingApplicationCount = await context.Applications
                .CountAsync(a => a.UserId == existingUser.Id);

            if (existingApplicationCount >= 10)
            {
                return;
            }
        }

        if (existingUser is null)
        {
            existingUser = new User
            {
                FirstName = "Demo",
                LastName = "Gebruiker",
                Email = seedEmail,
                PasswordHash = "seeded-dummy-password-hash",
                CreatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(existingUser);
            await context.SaveChangesAsync();
        }

        var companies = new[]
        {
            new Company { UserId = existingUser.Id, Name = "TechNova", Website = "https://technova.example", Location = "Brussel", Industry = "Software", Notes = "Focus op SaaS-oplossingen.", CreatedAt = DateTime.UtcNow.AddDays(-45) },
            new Company { UserId = existingUser.Id, Name = "DataPulse", Website = "https://datapulse.example", Location = "Antwerpen", Industry = "Data & AI", Notes = "Zoekt full stack profielen.", CreatedAt = DateTime.UtcNow.AddDays(-42) },
            new Company { UserId = existingUser.Id, Name = "CloudPeak", Website = "https://cloudpeak.example", Location = "Gent", Industry = "Cloud", Notes = "Sterke Azure-omgeving.", CreatedAt = DateTime.UtcNow.AddDays(-40) },
            new Company { UserId = existingUser.Id, Name = "PixelForge", Website = "https://pixelforge.example", Location = "Leuven", Industry = "Product Design", Notes = "Frontend heavy team.", CreatedAt = DateTime.UtcNow.AddDays(-38) },
            new Company { UserId = existingUser.Id, Name = "SecureLayer", Website = "https://securelayer.example", Location = "Mechelen", Industry = "Cybersecurity", Notes = "Backend rol met security-focus.", CreatedAt = DateTime.UtcNow.AddDays(-35) },
            new Company { UserId = existingUser.Id, Name = "GreenGrid", Website = "https://greengrid.example", Location = "Hasselt", Industry = "EnergyTech", Notes = "Werkt met IoT-platformen.", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new Company { UserId = existingUser.Id, Name = "FinCore", Website = "https://fincore.example", Location = "Brugge", Industry = "FinTech", Notes = "C# en React stack.", CreatedAt = DateTime.UtcNow.AddDays(-28) },
            new Company { UserId = existingUser.Id, Name = "HealthBridge", Website = "https://healthbridge.example", Location = "Kortrijk", Industry = "HealthTech", Notes = "Zoekt API-ervaring.", CreatedAt = DateTime.UtcNow.AddDays(-24) },
            new Company { UserId = existingUser.Id, Name = "LogiFlow", Website = "https://logiflow.example", Location = "Genk", Industry = "Logistics", Notes = "Scrum team, hybride.", CreatedAt = DateTime.UtcNow.AddDays(-21) },
            new Company { UserId = existingUser.Id, Name = "BrightApps", Website = "https://brightapps.example", Location = "Namen", Industry = "Consulting", Notes = "Projectconsultancy.", CreatedAt = DateTime.UtcNow.AddDays(-18) }
        };

        var existingCompanies = await context.Companies
            .Where(c => c.UserId == existingUser.Id)
            .Select(c => c.Name)
            .ToListAsync();

        var missingCompanies = companies
            .Where(company => !existingCompanies.Contains(company.Name))
            .ToList();

        if (missingCompanies.Count > 0)
        {
            await context.Companies.AddRangeAsync(missingCompanies);
            await context.SaveChangesAsync();
        }

        var companyByName = await context.Companies
            .Where(c => c.UserId == existingUser.Id)
            .ToDictionaryAsync(c => c.Name, c => c);

        var now = DateTime.UtcNow;

        var applications = new[]
        {
            new Application { UserId = existingUser.Id, CompanyId = companyByName["TechNova"].Id, JobTitle = "Junior .NET Developer", JobUrl = "https://technova.example/jobs/junior-dotnet", Status = Status.Verzonden, Priority = "High", AppliedDate = DateOnly.FromDateTime(now.AddDays(-30)), NextStep = "Wachten op eerste feedback", SalaryMin = 2800m, SalaryMax = 3400m, Source = "LinkedIn", CreatedAt = now.AddDays(-30), UpdatedAt = now.AddDays(-30) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["DataPulse"].Id, JobTitle = "Full Stack Developer", JobUrl = "https://datapulse.example/jobs/full-stack", Status = Status.Gesprek, Priority = "High", AppliedDate = DateOnly.FromDateTime(now.AddDays(-27)), NextStep = "Technisch interview", SalaryMin = 3200m, SalaryMax = 3800m, Source = "VDAB", CreatedAt = now.AddDays(-27), UpdatedAt = now.AddDays(-20) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["CloudPeak"].Id, JobTitle = "Cloud Engineer", JobUrl = "https://cloudpeak.example/jobs/cloud-engineer", Status = Status.Afgewezen, Priority = "Medium", AppliedDate = DateOnly.FromDateTime(now.AddDays(-24)), NextStep = "Afgesloten", SalaryMin = 3400m, SalaryMax = 4100m, Source = "Indeed", CreatedAt = now.AddDays(-24), UpdatedAt = now.AddDays(-18) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["PixelForge"].Id, JobTitle = "React Frontend Developer", JobUrl = "https://pixelforge.example/jobs/react-frontend", Status = Status.Verzonden, Priority = "Low", AppliedDate = DateOnly.FromDateTime(now.AddDays(-21)), NextStep = "Portfolio opvolgen", SalaryMin = 2900m, SalaryMax = 3500m, Source = "LinkedIn", CreatedAt = now.AddDays(-21), UpdatedAt = now.AddDays(-21) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["SecureLayer"].Id, JobTitle = "Backend Developer", JobUrl = "https://securelayer.example/jobs/backend", Status = Status.Gesprek, Priority = "High", AppliedDate = DateOnly.FromDateTime(now.AddDays(-18)), NextStep = "Case voorbereiden", SalaryMin = 3300m, SalaryMax = 4000m, Source = "StepStone", CreatedAt = now.AddDays(-18), UpdatedAt = now.AddDays(-12) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["GreenGrid"].Id, JobTitle = "Software Engineer", JobUrl = "https://greengrid.example/jobs/software-engineer", Status = Status.Verzonden, Priority = "Medium", AppliedDate = DateOnly.FromDateTime(now.AddDays(-15)), NextStep = "Recruiter mail afwachten", SalaryMin = 3000m, SalaryMax = 3600m, Source = "Company Site", CreatedAt = now.AddDays(-15), UpdatedAt = now.AddDays(-15) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["FinCore"].Id, JobTitle = "C# Developer", JobUrl = "https://fincore.example/jobs/csharp", Status = Status.Aanbieding, Priority = "High", AppliedDate = DateOnly.FromDateTime(now.AddDays(-12)), NextStep = "Aanbod evalueren", SalaryMin = 3600m, SalaryMax = 4300m, Source = "Referral", CreatedAt = now.AddDays(-12), UpdatedAt = now.AddDays(-4) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["HealthBridge"].Id, JobTitle = "API Developer", JobUrl = "https://healthbridge.example/jobs/api-developer", Status = Status.Afgewezen, Priority = "Medium", AppliedDate = DateOnly.FromDateTime(now.AddDays(-9)), NextStep = "Afgesloten", SalaryMin = 3100m, SalaryMax = 3700m, Source = "LinkedIn", CreatedAt = now.AddDays(-9), UpdatedAt = now.AddDays(-3) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["LogiFlow"].Id, JobTitle = "Medior Software Developer", JobUrl = "https://logiflow.example/jobs/medior-software", Status = Status.Verzonden, Priority = "Medium", AppliedDate = DateOnly.FromDateTime(now.AddDays(-6)), NextStep = "Telefonische screening", SalaryMin = 3200m, SalaryMax = 3900m, Source = "Indeed", CreatedAt = now.AddDays(-6), UpdatedAt = now.AddDays(-6) },
            new Application { UserId = existingUser.Id, CompanyId = companyByName["BrightApps"].Id, JobTitle = "Consultant Developer", JobUrl = "https://brightapps.example/jobs/consultant-developer", Status = Status.Gesprek, Priority = "Low", AppliedDate = DateOnly.FromDateTime(now.AddDays(-3)), NextStep = "Tweede gesprek", SalaryMin = 3000m, SalaryMax = 3650m, Source = "StepStone", CreatedAt = now.AddDays(-3), UpdatedAt = now.AddDays(-1) }
        };

        var existingApplicationTitles = await context.Applications
            .Where(a => a.UserId == existingUser.Id)
            .Select(a => new { a.CompanyId, a.JobTitle })
            .ToListAsync();

        var missingApplications = applications
            .Where(application => !existingApplicationTitles.Any(existing =>
                existing.CompanyId == application.CompanyId &&
                existing.JobTitle == application.JobTitle))
            .ToList();

        if (missingApplications.Count > 0)
        {
            await context.Applications.AddRangeAsync(missingApplications);
            await context.SaveChangesAsync();
        }

        var applicationByTitle = await context.Applications
            .Where(a => a.UserId == existingUser.Id)
            .ToDictionaryAsync(a => a.JobTitle, a => a);

        var interviews = new[]
        {
            new Interview { ApplicationId = applicationByTitle["Full Stack Developer"].Id, InterviewType = "HR", ScheduledStart = now.AddDays(2).Date.AddHours(10), ScheduledEnd = now.AddDays(2).Date.AddHours(11), Location = "Antwerpen", ContactPerson = "Sofie Janssens", ContactEmail = "sofie@datapulse.example", Notes = "Introgesprek met HR", CreatedAt = now.AddDays(-1) },
            new Interview { ApplicationId = applicationByTitle["Backend Developer"].Id, InterviewType = "Technisch", ScheduledStart = now.AddDays(4).Date.AddHours(14), ScheduledEnd = now.AddDays(4).Date.AddHours(15), MeetingLink = "https://meet.securelayer.example/backend", ContactPerson = "Tom Peeters", ContactEmail = "tom@securelayer.example", Notes = "Technische case bespreken", CreatedAt = now.AddHours(-12) },
            new Interview { ApplicationId = applicationByTitle["Consultant Developer"].Id, InterviewType = "Team Fit", ScheduledStart = now.AddDays(7).Date.AddHours(9), ScheduledEnd = now.AddDays(7).Date.AddHours(10), MeetingLink = "https://meet.brightapps.example/teamfit", ContactPerson = "Annelies Martin", ContactEmail = "annelies@brightapps.example", Notes = "Gesprek met delivery manager", CreatedAt = now.AddHours(-6) }
        };

        var existingInterviewIds = await context.Interviews
            .Where(i => interviews.Select(seedInterview => seedInterview.ApplicationId).Contains(i.ApplicationId))
            .Select(i => i.ApplicationId)
            .ToListAsync();

        var missingInterviews = interviews
            .Where(interview => !existingInterviewIds.Contains(interview.ApplicationId))
            .ToList();

        if (missingInterviews.Count > 0)
        {
            await context.Interviews.AddRangeAsync(missingInterviews);
            await context.SaveChangesAsync();
        }
    }

    private static async TaskSystem EnsureAuthColumnsAsync(SollicitatietrackerDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            IF COL_LENGTH('users', 'password_reset_token_hash') IS NULL
            BEGIN
                ALTER TABLE users ADD password_reset_token_hash nvarchar(500) NULL;
            END

            IF COL_LENGTH('users', 'password_reset_token_expires_at') IS NULL
            BEGIN
                ALTER TABLE users ADD password_reset_token_expires_at datetime2 NULL;
            END
            """);
    }
}
