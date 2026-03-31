using SolicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemTask = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Data.Repos
{
    public interface IApplicationRepository
    {
        Task<Application> AddApplicationAsync(Application application);
        Task<Application> GetApplicationByIdAsync(int id);
    }
}
