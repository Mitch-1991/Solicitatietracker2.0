using SollicitatieTracker.App.DTOs;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.App.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IApplicationNoteRepository _applicationNoteRepository;

        public ApplicationService(IApplicationRepository applicationRepository, ICompanyRepository companyRepository, IApplicationNoteRepository applicationNoteRepository)
        {
            _applicationRepository = applicationRepository;
            _companyRepository = companyRepository;
            _applicationNoteRepository = applicationNoteRepository;
        }

        public async Task<ApplicationDto> CreateAsync(CreateApplicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.companyName)){
                throw new ArgumentException("Bedrijf is verplicht");
            }
            if(string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                throw new ArgumentException("JobTitle is verplicht");
            }
            Company? company = await FindMatchingCompanyAsync(dto.companyName);

            if (company == null)
            {
                company = new Company
                {
                    Name = dto.companyName.Trim(),
                    Location = dto.Location,
                    CreatedAt = DateTime.UtcNow,
                    UserId = dto.UserId
                };
                company = await _companyRepository.AddAsync(company);
            }
            var application = new Application
            {
                CompanyId = company.Id,
                UserId = dto.UserId,
                JobTitle = dto.JobTitle,
                JobUrl = dto.JobUrl,
                Status = dto.Status,
                Priority = dto.Priority,
                AppliedDate = dto.AppliedDate,
                NextStep = dto.NextStep,
                SalaryMin = dto.SalaryMin,
                SalaryMax = dto.SalaryMax,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdApplication = await _applicationRepository.AddApplicationAsync(application);

            ApplicationNote? createdNote = null;
            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                var note = new ApplicationNote
                {
                    ApplicationId = createdApplication.Id,
                    NoteText = dto.Notes.Trim(),
                    CreatedAt = DateTime.UtcNow
                };
                createdNote = await _applicationNoteRepository.AddApplicationNoteAsync(note);
            }
            var newApplication = await _applicationRepository.GetApplicationByIdWithDetailsAsync(createdApplication.Id);

            if (newApplication == null)
            {
                throw new InvalidOperationException("De aangemaakte sollicitatie kon niet opnieuw opgehaald worden.");
            }

            return MapToDto(newApplication);
        }

        public async Task<ApplicationDto?> FindByIdAsync(int id)
        {
            var application = await _applicationRepository.GetApplicationByIdWithDetailsAsync(id);

            if (application == null)
            {
                return null;
            }

            return MapToDto(application);
        }

        private async Task<Company?> FindMatchingCompanyAsync(string companyName)
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();
            var normalizedInput = companyName.Trim().ToLower();

            foreach (var company in companies)
            {
                var normalizedCompanyName = company.Name.Trim().ToLower();
                if (normalizedCompanyName == normalizedInput)
                {
                    return company;
                }
                int distance = LevenshteinDistance(normalizedInput, normalizedCompanyName);
                if (distance <= 2) 
                {
                    return company;
                }
            }
            return null;
        }
        private static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
                return t.Length;

            if (string.IsNullOrEmpty(t))
                return s.Length;

            var d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++)
                d[i, 0] = i;

            for (int j = 0; j <= t.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            return d[s.Length, t.Length];
        }

        public async Task<ApplicationDto?> UpdateAsync(int id, UpdateApplicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Bedrijf))
            {
                throw new ArgumentException("Bedrijf is verplicht");
            }

            if (string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                throw new ArgumentException("JobTitle is verplicht");
            }

            var application = await _applicationRepository.GetApplicationByIdWithDetailsAsync(id);

            if (application == null)
            {
                return null;
            }
            if (application.Company == null)
            {
                throw new InvalidOperationException("De gekoppelde company werd niet gevonden.");
            }

            application.JobTitle = dto.JobTitle.Trim();
            application.JobUrl = string.IsNullOrWhiteSpace(dto.JobUrl) ? null : dto.JobUrl.Trim();
            application.Status = dto.Status;
            application.Priority = string.IsNullOrWhiteSpace(dto.Priority) ? null : dto.Priority.Trim();
            application.AppliedDate = dto.AppliedDate;
            application.NextStep = string.IsNullOrWhiteSpace(dto.NextStep) ? null : dto.NextStep.Trim();
            application.SalaryMin = dto.SalaryMin;
            application.SalaryMax = dto.SalaryMax;
            application.UpdatedAt = DateTime.UtcNow;

            application.Company.Name = dto.Bedrijf.Trim();
            application.Company.Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim();

            var existingNote = application.ApplicationNotes?
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(dto.Omschrijving))
            {
                if (existingNote != null)
                {
                    existingNote.NoteText = string.Empty;
                }
            }
            else
            {
                if (existingNote == null)
                {
                    application.ApplicationNotes.Add(new ApplicationNote
                    {
                        ApplicationId = application.Id,
                        NoteText = dto.Omschrijving.Trim(),
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existingNote.NoteText = dto.Omschrijving.Trim();
                }
            }

            var updatedApplication = await _applicationRepository.UpdateApplicationAsync(application);

           return MapToDto(updatedApplication);
        }
        private static ApplicationDto MapToDto(Application application)
        {
            var latestNote = application.ApplicationNotes?
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefault();

            return new ApplicationDto
            {
                Id = application.Id,
                CompanyId = application.CompanyId,
                UserId = application.UserId,
                CompanyName = application.Company?.Name ?? string.Empty,
                JobTitle = application.JobTitle,
                JobUrl = application.JobUrl,
                Location = application.Company?.Location,
                Status = application.Status.ToString(),
                Priority = application.Priority,
                AppliedDate = application.AppliedDate,
                NextStep = application.NextStep,
                SalaryMin = application.SalaryMin,
                SalaryMax = application.SalaryMax,
                Notes = latestNote?.NoteText,
                CreatedAt = application.CreatedAt,
                UpdatedAt = application.UpdatedAt
            };
        }
    }

}




