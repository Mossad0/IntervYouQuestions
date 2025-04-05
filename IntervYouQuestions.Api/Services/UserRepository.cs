using IntervYouQuestions.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntervYouQuestions.Api.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly    InterviewModuleContext _context;

        public UserRepository(InterviewModuleContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            return await _context.Set<UserProfile>()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<UserProfile> CreateUserProfileAsync(UserProfile profile)
        {
            _context.Set<UserProfile>().Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> UpdateUserProfileAsync(UserProfile profile)
        {
            _context.Set<UserProfile>().Update(profile);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Interview>> GetUserInterviewsAsync(string userId)
        {
            return await _context.Interviews
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.StartTime)
                .ToListAsync();
        }
    }
}



