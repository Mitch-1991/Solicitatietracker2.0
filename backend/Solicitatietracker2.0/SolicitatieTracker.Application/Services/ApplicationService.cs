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

        public async Task<ApplicationDto> CreateAsync(CreateApplicationDto dto, int userId)
        {
            if (string.IsNullOrWhiteSpace(dto.companyName)){
                throw new ArgumentException("Bedrijf is verplicht");
            }
            if(string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                throw new ArgumentException("JobTitle is verplicht");
            }
            Company? company = await FindMatchingCompanyAsync(dto.companyName, userId);

            if (company == null)
            {
                company = new Company
                {
                    Name = dto.companyName.Trim(),
                    Location = dto.Location,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                };
                company = await _companyRepository.AddAsync(company);
            }
            var application = new Application
            {
                CompanyId = company.Id,
                UserId = userId,
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

            ApplyInterviewData(dto.Status, dto.Interview, application);

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
            var newApplication = await _applicationRepository.GetApplicationByIdWithDetailsAsync(createdApplication.Id, userId);

            if (newApplication == null)
            {
                throw new InvalidOperationException("De aangemaakte sollicitatie kon niet opnieuw opgehaald worden.");
            }

            return MapToDto(newApplication);
        }

        public async Task<ApplicationDto?> FindByIdAsync(int id, int userId)
        {
            var application = await _applicationRepository.GetApplicationByIdWithDetailsAsync(id, userId);

            if (application == null)
            {
                return null;
            }

            return MapToDto(application);
        }

        private async Task<Company?> FindMatchingCompanyAsync(string companyName, int userId)
        {
            var companies = await _companyRepository.GetAllCompaniesAsync(userId);
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

        public async Task<ApplicationDto?> UpdateAsync(int id, UpdateApplicationDto dto, int userId)
        {
            if (string.IsNullOrWhiteSpace(dto.CompanyName))
            {
                throw new ArgumentException("Bedrijf is verplicht");
            }

            if (string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                throw new ArgumentException("JobTitle is verplicht");
            }

            var application = await _applicationRepository.GetApplicationByIdWithDetailsAsync(id, userId);

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

            application.Company.Name = dto.CompanyName.Trim();
            application.Company.Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim();

            ApplyInterviewData(dto.Status, dto.Interview, application);

            var existingNote = application.ApplicationNotes?
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(dto.Notes))
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
                        NoteText = dto.Notes.Trim(),
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existingNote.NoteText = dto.Notes.Trim();
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
            var interview = application.Interviews?
                .OrderBy(i => i.ScheduledStart)
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
                Interview = interview == null ? null : MapInterviewToDto(interview),
                CreatedAt = application.CreatedAt,
                UpdatedAt = application.UpdatedAt
            };
        }

        private static void ApplyInterviewData(Status status, InterviewDto? dto, Application application)
        {
            if (status != Status.Gesprek)
            {
                return;
            }

            ValidateInterview(dto);

            var existingInterview = application.Interviews
                .OrderBy(i => i.Id)
                .ThenBy(i => i.CreatedAt)
                .FirstOrDefault();

            if (existingInterview == null)
            {
                existingInterview = new Interview
                {
                    ApplicationId = application.Id,
                    CreatedAt = DateTime.UtcNow
                };
                application.Interviews.Add(existingInterview);
            }

            UpdateInterviewEntity(existingInterview, dto!);
        }

        private static void UpdateInterviewEntity(Interview interview, InterviewDto dto)
        {
            var interviewType = dto.InterviewType.Trim();
            var isOnline = IsOnlineInterview(interviewType);

            interview.InterviewType = interviewType;
            interview.ScheduledStart = dto.ScheduledStart;
            interview.ScheduledEnd = dto.ScheduledEnd;
            interview.Location = isOnline ? null : dto.Location?.Trim();
            interview.MeetingLink = isOnline ? dto.MeetingLink?.Trim() : null;
            interview.ContactPerson = string.IsNullOrWhiteSpace(dto.ContactPerson) ? null : dto.ContactPerson.Trim();
            interview.ContactEmail = string.IsNullOrWhiteSpace(dto.ContactEmail) ? null : dto.ContactEmail.Trim();
            interview.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
        }

        private static void ValidateInterview(InterviewDto? dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Interviewgegevens zijn verplicht wanneer de status Gesprek is.");
            }

            var interviewType = dto.InterviewType?.Trim();
            if (!IsOnlineInterview(interviewType) && !IsLocationInterview(interviewType))
            {
                throw new ArgumentException("Interviewtype moet Online of Op locatie zijn.");
            }

            if (dto.ScheduledStart == default)
            {
                throw new ArgumentException("Starttijd van het interview is verplicht.");
            }

            if (dto.ScheduledEnd.HasValue && dto.ScheduledEnd.Value < dto.ScheduledStart)
            {
                throw new ArgumentException("Eindtijd mag niet voor de starttijd liggen.");
            }

            if (IsOnlineInterview(interviewType))
            {
                if (string.IsNullOrWhiteSpace(dto.MeetingLink))
                {
                    throw new ArgumentException("Meeting link is verplicht voor een online interview.");
                }

                if (!Uri.TryCreate(dto.MeetingLink.Trim(), UriKind.Absolute, out var meetingUri) ||
                    (meetingUri.Scheme != Uri.UriSchemeHttp && meetingUri.Scheme != Uri.UriSchemeHttps))
                {
                    throw new ArgumentException("Meeting link moet een geldige URL zijn.");
                }
            }

            if (IsLocationInterview(interviewType) && string.IsNullOrWhiteSpace(dto.Location))
            {
                throw new ArgumentException("Locatie is verplicht voor een interview op locatie.");
            }

            if (!string.IsNullOrWhiteSpace(dto.ContactEmail) && !IsValidEmail(dto.ContactEmail.Trim()))
            {
                throw new ArgumentException("Contact e-mail is ongeldig.");
            }
        }

        private static InterviewDto MapInterviewToDto(Interview interview)
        {
            return new InterviewDto
            {
                InterviewType = interview.InterviewType,
                ScheduledStart = interview.ScheduledStart,
                ScheduledEnd = interview.ScheduledEnd,
                Location = interview.Location,
                MeetingLink = interview.MeetingLink,
                ContactPerson = interview.ContactPerson,
                ContactEmail = interview.ContactEmail,
                Notes = interview.Notes
            };
        }

        private static bool IsOnlineInterview(string? interviewType)
        {
            return string.Equals(interviewType, "Online", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsLocationInterview(string? interviewType)
        {
            return string.Equals(interviewType, "Op locatie", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsValidEmail(string email)
        {
            return email.Contains('@') && email.Contains('.');
        }
    }

}




