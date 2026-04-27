using Microsoft.EntityFrameworkCore;
using SollicitatieTracker.Domain.Entities;
using SollicitatieTracker.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSystem = System.Threading.Tasks.Task;

namespace SolicitatieTracker.Infrastructure.Data.Repos.Auth
{
    public class UserRepository : IUserRepository
    {

        private readonly SollicitatietrackerDbContext _context;

        public UserRepository(SollicitatietrackerDbContext context)
        {
            _context = context;
        }
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByResetTokenHashAsync(string resetTokenHash)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetTokenHash == resetTokenHash);
        }

        public async TaskSystem UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
