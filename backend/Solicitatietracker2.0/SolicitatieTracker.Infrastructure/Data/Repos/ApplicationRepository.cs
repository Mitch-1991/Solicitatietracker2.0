using SolicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemTask = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Data.Repos
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly SollicitatietrackerDbContext _context;

        public ApplicationRepository(SollicitatietrackerDbContext context)
        {
            _context = context;
        }

        public async Task<Application> AddApplicationAsync(Application application)
        {
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<Application> GetApplicationByIdAsync(int id) =>
            await _context.Applications.FindAsync(id);


    }
}
