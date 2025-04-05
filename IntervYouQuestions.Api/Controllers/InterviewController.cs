using IntervYouQuestions.Api.Contracts.Requests;
using IntervYouQuestions.Api.Contracts.Responses;
using IntervYouQuestions.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntervYouQuestions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;
        private readonly IInterviewExpirationService _interviewExpirationService;

        public InterviewController(
            IInterviewService interviewService,
            IInterviewExpirationService interviewExpirationService)
        {
            _interviewService = interviewService;
            _interviewExpirationService = interviewExpirationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InterviewResponse>>> GetAllInterviews()
        {
            var interviews = await _interviewService.GetAllInterviewsAsync();
            return Ok(interviews);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<InterviewResponse>>> GetUserInterviews(string userId)
        {
            var interviews = await _interviewService.GetUserInterviewsAsync(userId);
            return Ok(interviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InterviewResponse>> GetInterview(int id)
        {
            var interview = await _interviewService.GetInterviewByIdAsync(id);
            return Ok(interview);
        }

        [HttpPost]
        public async Task<ActionResult<InterviewResponse>> CreateInterview(CreateInterviewRequest createInterviewRequest)
        {
            var interview = await _interviewService.CreateInterviewAsync(createInterviewRequest);
            return CreatedAtAction(nameof(GetInterview), new { id = interview.InterviewId }, interview);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InterviewResponse>> UpdateInterview(int id, UpdateInterviewRequest updateInterviewRequest)
        {
            var interview = await _interviewService.UpdateInterviewAsync(id, updateInterviewRequest);
            return Ok(interview);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterview(int id)
        {
            await _interviewService.DeleteInterviewAsync(id);
            return NoContent();
        }

        [HttpPost("start")]
        public async Task<ActionResult<InterviewResponse>> StartInterview(StartInterviewRequest startInterviewRequest)
        {
            var interview = await _interviewService.StartInterviewAsync(startInterviewRequest);
            return Ok(interview);
        }

        [HttpPost("start/user/{userId}")]
        public async Task<ActionResult<InterviewResponse>> StartInterviewForUser(
            string userId,
            [FromQuery] int numberOfQuestions = 10,
            [FromQuery] int timeLimitInMinutes = 30)
        {
            var interview = await _interviewService.StartInterviewForUserAsync(userId, numberOfQuestions, timeLimitInMinutes);
            return Ok(interview);
        }

        [HttpGet("{id}/expiration")]
        public async Task<ActionResult<DateTime>> GetInterviewExpirationDate(int id)
        {
            var expirationDate = await _interviewExpirationService.GetInterviewExpirationDateAsync(id);
            return Ok(expirationDate);
        }

        [HttpGet("{id}/is-expired")]
        public async Task<ActionResult<bool>> IsInterviewExpired(int id)
        {
            var isExpired = await _interviewExpirationService.IsInterviewExpiredAsync(id);
            return Ok(isExpired);
        }

        [HttpPost("check-expired")]
        public async Task<IActionResult> CheckExpiredInterviews()
        {
            await _interviewExpirationService.CheckAndUpdateExpiredInterviewsAsync();
            return Ok();
        }
    }
}