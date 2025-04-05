using IntervYouQuestions.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using IntervYouQuestions.Api.Exceptions;
namespace IntervYouQuestions.Api.Services
{
    public class InterviewExpirationService : IInterviewExpirationService
    {
        private readonly InterviewModuleContext _context;
        private readonly ILogger<InterviewExpirationService> _logger;

        public InterviewExpirationService(
            InterviewModuleContext context,
            ILogger<InterviewExpirationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CheckAndUpdateExpiredInterviewsAsync()
        {
            var now = DateTime.UtcNow;
            var expiredInterviews = await _context.Interviews
                .Where(i => i.ExpirationDate <= now && i.Status != InterviewStatus.Expired)
                .ToListAsync();

            foreach (var interview in expiredInterviews)
            {
                interview.Status = InterviewStatus.Expired;
                _logger.LogInformation("Interview {InterviewId} has expired and been marked as expired", interview.Id);
            }

            if (expiredInterviews.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsInterviewExpiredAsync(int interviewId)
        {
            var interview = await _context.Interviews.FindAsync(interviewId);
            if (interview == null)
                throw new NotFoundException($"Interview with ID {interviewId} not found");

            return interview.ExpirationDate <= DateTime.UtcNow;
        }

        public async Task<DateTime> GetInterviewExpirationDateAsync(int interviewId)
        {
            var interview = await _context.Interviews.FindAsync(interviewId);
            if (interview == null)
                throw new NotFoundException($"Interview with ID {interviewId} not found");

            return interview.ExpirationDate;
        }
    }
}
