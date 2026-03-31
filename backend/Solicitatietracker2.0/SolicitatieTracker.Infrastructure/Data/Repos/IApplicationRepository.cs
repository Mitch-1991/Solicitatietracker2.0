using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemTask = System.Threading.Tasks.Task;

namespace SollicitatieTracker.Infrastructure.Data.Repos
{
    public interface IApplicationRepository
    {
        Task<Application> AddApplicationAsync(Application application);
        Task<Application> GetApplicationByIdAsync(int id);
    }
}
