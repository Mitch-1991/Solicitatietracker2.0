using SolicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolicitatieTracker.Infrastructure.Data.Repos
{
    public interface IApplicationNoteRepository
    {
        Task<ApplicationNote> AddApplicationNoteAsync(ApplicationNote applicationNote);
    }
}
