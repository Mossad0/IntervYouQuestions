using IntervYouQuestions.Api.Contracts.Requests;
using IntervYouQuestions.Api.Contracts.Responses;
using IntervYouQuestions.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntervYouQuestions.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewQuestionsController : ControllerBase
    {
        private readonly IInterviewService _interviewService;
        private readonly IUserAnswerService _userAnswerService;

        public InterviewQuestionsController(
            IInterviewService interviewService,
            IUserAnswerService userAnswerService)
        {
            _interviewService = interviewService;
            _userAnswerService = userAnswerService;
        }

        // Get all questions for a specific interview
        [HttpGet("{interviewId}/questions")]
        public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetInterviewQuestions(int interviewId)
        {
            try
            {
                // Call the NEW service method
                var questions = await _interviewService.GetQuestionsForInterviewAsync(interviewId);

                // The service method should handle the "not found" case,
                // or you can check for null/empty list here if it returns that.
                // if (questions == null || !questions.Any())
                // {
                //     return NotFound($"Questions for interview ID {interviewId} not found or interview doesn't exist.");
                // }

                return Ok(questions);
            }
            catch (NotFoundException ex) // Catch specific exceptions from the service
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(500, "Internal server error retrieving interview questions.");
            }
        }

        // Submit an answer for a question
        [HttpPost("answer")]
        public async Task<ActionResult> SubmitAnswer([FromBody] SubmitAnswerRequest request)
        {
            try
            {
                await _userAnswerService.SaveUserAnswerAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Submit the entire interview to calculate score
        [HttpPost("{interviewId}/submit")]
        public async Task<ActionResult<InterviewResultResponse>> SubmitInterview(int interviewId)
        {
            try
            {
                var result = await _userAnswerService.CalculateInterviewScoreAsync(interviewId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}