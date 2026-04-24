using SolicitatieTracker.App.DTOs;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.App.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }


        public async Task<List<CompanyDto>> GetAllCompaniesAsync(int userId)
        {
            var companies = await _companyRepository.GetAllCompaniesAsync(userId);

            var dtos = new List<CompanyDto>();

            foreach (var company in companies)
            {
                dtos.Add(new CompanyDto
                {
                    Id = company.Id,
                    CompanyName = company.Name,
                    WebsiteURL = company.Website,
                    Location = company.Location,
                    Notes = company.Notes
                });
            }

            return dtos;
        }
    }
}
