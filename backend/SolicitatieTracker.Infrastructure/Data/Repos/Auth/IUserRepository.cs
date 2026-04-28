using SollicitatieTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Data.Repos.Auth
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByResetTokenHashAsync(string resetTokenHash);
        Task<User> AddAsync(User user);
        Task <bool> EmailExistsAsync(string email);
        TaskSystem UpdateAsync(User user);
    }
}
