using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.Infrastructure.Data.Repos
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly SollicitatietrackerDbContext _context;

        public CompanyRepository(SollicitatietrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Company> AddAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
           return await _context.Companies.ToListAsync();
        }
    }
}
