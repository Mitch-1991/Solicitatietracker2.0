using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SollicitatieTracker.Infrastructure.Data.Repos
{
    public class ApplicationNoteRepository :IApplicationNoteRepository
    {
        private readonly SollicitatietrackerDbContext _context;

        public ApplicationNoteRepository(SollicitatietrackerDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationNote> AddApplicationNoteAsync(ApplicationNote applicationNote)
        {
           _context.ApplicationNotes.Add(applicationNote);
            await _context.SaveChangesAsync();
            return applicationNote;
        }
    }
}
